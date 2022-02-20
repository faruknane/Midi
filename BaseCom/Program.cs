using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace BaseCom
{
    class Program
    {
        static void Main(string[] args)
        {
            Server s = new Server(10000);
            s.Start();
            new Thread(() =>
            {
                Client c = new Client(new TcpClient("localhost", 11000), s.StartWord, s.EndWord);
                Message m2 = new Message();
                m2["deneme"] = "deneme1234";
                for (int i = 0; i < 100; i++)
                {
                    m2["deneme"] += i;
                    c.SendMessage(m2);
                }
                c.Close();
            }).Start();
            Client c = s.AcceptClient();

            while (true)
            {
                Message m = c.ReceiveMessage();
                Console.WriteLine(m["deneme"]);
            }
            s.Stop();
        }
    }
}
