using System;
using System.IO;
using System.Diagnostics;

class Watchdog
{
    static void Watchdoge()
    {
        // Get the folder where the watchdog executable is running from
        string watchdogFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        // Assuming the target app is in the same folder:
        string targetPath = Path.Combine(watchdogFolder, "server.exe");

        while (true)
        {
            Process[] running = Process.GetProcessesByName("server1");
            if (running.Length == 0)
            {
                Console.WriteLine("Starting target app...");
                Process.Start(targetPath);
            }
            else
            {
                Console.WriteLine("Target app running.");
            }

            System.Threading.Thread.Sleep(5000);
        }
    }
}
