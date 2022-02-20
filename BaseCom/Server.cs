using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BaseCom
{
    public class Server 
    {
        public byte[] StartWord { get; private set; }
        public byte[] EndWord { get; private set; }

        public TcpListener TcpListener { get; private set; }

        public Server(int port) 
        {
            TcpListener = new TcpListener(IPAddress.Any, port);
            StartWord = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
            EndWord = new byte[8] { 8, 7, 6, 5, 4, 3, 2, 1 };
        }

        public Client AcceptClient()
        {
            TcpClient c = this.TcpListener.AcceptTcpClient();
            return new Client(c, StartWord, EndWord);
        }

        public void Stop()
        {
            TcpListener.Stop();
        }

        public void Start()
        {
            TcpListener.Start();
        }
    }
}
