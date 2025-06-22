using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Management;
using System.Text;

namespace Ring3Audit
{
    public class EfiAuditor
    {
        public static bool IsSecureBootEnabled()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("root\\Microsoft\\Windows\\HardwareManagement", "SELECT SecureBootEnabled FROM Win32_SecureBoot"))
                {
                    foreach (var obj in searcher.Get())
                        return Convert.ToBoolean(obj["SecureBootEnabled"]);
                }
            }
            catch { }
            return false;
        }

        public static string GetFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] data = File.ReadAllBytes(filePath);
                byte[] hash = sha256.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        public static string FindEfiPartition()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.DriveFormat == "FAT32" && drive.IsReady && drive.TotalSize < 300 * 1024 * 1024)
                {
                    string efiPath = Path.Combine(drive.RootDirectory.FullName, "EFI");
                    if (Directory.Exists(efiPath))
                        return efiPath;
                }
            }
            return null;
        }

        public static List<Tuple<string, string>> ScanBootFiles(string efiRoot)
        {
            var results = new List<Tuple<string, string>>();

            if (!Directory.Exists(efiRoot))
                return results;

            foreach (var file in Directory.GetFiles(efiRoot, "*.efi", SearchOption.AllDirectories))
            {
                string hash = GetFileHash(file);
                results.Add(new Tuple<string, string>(file, hash));
            }

            return results;
        }

        private static readonly HashSet<string> KnownVulnerableHashes = new HashSet<string>
        {
            // Sample known bad hashes — replace with real ones from DBX
            "abc123deadbeef...",
            "def456cafe001..."
        };

        public static bool IsFileVulnerable(string hash)
        {
            return KnownVulnerableHashes.Contains(hash);
        }

        public static void SaveResults(List<Tuple<string, string>> data, string file)
        {
            using (var sw = new StreamWriter(file))
            {
                foreach (var item in data)
                {
                    string status = IsFileVulnerable(item.Item2) ? "VULNERABLE" : "OK";
                    sw.WriteLine($"{item.Item1} | {item.Item2} | {status}");
                }
            }
        }

        public static void EncryptLog(string path)
        {
            if (!File.Exists(path)) return;

            string content = File.ReadAllText(path);
            byte[] data = Encoding.UTF8.GetBytes(content);
            for (int i = 0; i < data.Length; i++) data[i] ^= 0x5A;
            File.WriteAllBytes(path, data);
        }

        public static void RunFullAudit()
        {
            Console.WriteLine("Secure Boot Enabled: " + IsSecureBootEnabled());

            string efiPath = FindEfiPartition();
            if (efiPath == null)
            {
                Console.WriteLine("EFI partition not found.");
                return;
            }

            Console.WriteLine("Scanning: " + efiPath);
            var results = ScanBootFiles(efiPath);

            foreach (var item in results)
            {
                string status = IsFileVulnerable(item.Item2) ? "VULNERABLE" : "OK";
                Console.WriteLine($"{item.Item1} | {item.Item2} | {status}");
            }

            string reportPath = Path.Combine(Path.GetTempPath(), "efi_audit_report.txt");
            SaveResults(results, reportPath);
            EncryptLog(reportPath);

            Console.WriteLine("Audit complete. Encrypted report saved to: " + reportPath);
        }
    }
}
