using BaseCom;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace LiveMidiServer
{
    class Program
    {
        public static ServerContext s = new ServerContext();

        static void Main(string[] args)
        {
            Server s = new Server(11000);
            s.Start();

            while (true)
            {
                Client c = s.AcceptClient();
                new Thread(ProcessClient).Start(c);
            }
            s.Stop();
        }

        public static void ProcessClient(object aaa)
        {
            Console.WriteLine("Client Geldi!");
            User u = new User(s, aaa as Client);
            while (u.Client.TcpClient.Connected)
                u.DoWork();

            lock(s.UserLock)
            {
                if(u.UserName != null)
                    s.UsersByUserName.Remove(u.UserName);
            }

            Console.WriteLine("Client Gitti!");
        }
    }
}
