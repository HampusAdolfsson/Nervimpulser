using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ArmWrestling
{
    public class Server
    {
        public static event DataHandler ReceivedData;

        public const int BufferSize = 1024;

        private static Socket listener;
        private static Socket handler;

        public static void Start()
        {

            var address = IPAddress.Parse("127.0.0.1");
            IPEndPoint endpoint = new IPEndPoint(address, 30001);

            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(endpoint);

            new Thread(readStream).Start();
        }

        public static void Send(string data)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(data);
            if (handler != null && handler.Connected) handler.Send(bytes);
        }

        private static void readStream()
        {
            try
            {
                string data;
                byte[] buffer = new byte[BufferSize];

                listener.Listen(10);

                while (true)
                {
                    handler = listener.Accept();
                    while (true)
                    {
                        buffer = new byte[BufferSize];
                        int bytesRec = handler.Receive(buffer);
                        data = Encoding.ASCII.GetString(buffer, 0, bytesRec);
                        if (data.IndexOf("\n") > -1)
                        {
                            ReceivedData(data.Trim());
                        }
                        Thread.Sleep(5);
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }

        public delegate void DataHandler(string data);

    }
}
