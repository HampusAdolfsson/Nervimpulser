using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ArmWrestling
{
    internal class Server
    {
        public static event DataHandler ReceivedData;

        public const int BufferSize = 1024;

        private static Socket _listener;
        private static Socket _handler;

        public static void Start()
        {

            var address = IPAddress.Parse("127.0.0.1");
            var endpoint = new IPEndPoint(address, 30001);

            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(endpoint);

            new Thread(ReadStream).Start();
        }

        public static void Send(string data)
        {
            var bytes = Encoding.Unicode.GetBytes(data);
            if (_handler != null && _handler.Connected) _handler.Send(bytes);
        }

        private static void ReadStream()
        {
            try
            {
                _listener.Listen(10);

                while (true)
                {
                    _handler = _listener.Accept();
                    while (true)
                    {
                        var buffer = new byte[BufferSize];
                        var bytesRec = _handler.Receive(buffer);
                        var data = Encoding.ASCII.GetString(buffer, 0, bytesRec);
                        if (data.IndexOf("\n", StringComparison.Ordinal) > -1)
                        {
                            ReceivedData?.Invoke(data.Trim());
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
