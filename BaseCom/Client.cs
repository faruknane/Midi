using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BaseCom
{
    public class Client
    {
        public byte[] StartWord { get; private set; }
        public byte[] EndWord { get; private set; }
        public TcpClient TcpClient { get; private set; }
        private NetworkStream Stream { get; set; }
        public MessageProtocol Protocol { get; private set; }


        public Client(TcpClient c, byte[] s, byte[] e)
        {
            this.StartWord = s;
            this.EndWord = e;
            this.TcpClient = c;
            this.Stream = c.GetStream();
            this.Protocol = new MessageProtocol(Stream, StartWord, EndWord);
        }


        public Message ReceiveMessage()
        {
            try
            {
                return this.Protocol.ReceiveMessage();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool SendMessage(Message m)
        {
            return this.Protocol.SendMessage(m);
        }

        public void Close()
        {
            this.TcpClient.Close();
        }

    }
}
