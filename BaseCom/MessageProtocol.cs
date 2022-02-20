using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BaseCom
{
    public class MessageProtocol
    {
        public NetworkStream Stream { get; private set; }
        public byte[] StartWord { get; private set; }
        public byte[] EndWord { get; private set; }

        public List<byte> Buffer { get; private set; }

        private bool StartWordRead;

        public Queue<Message> Messages;

        private byte[] TempBuffer;

        public MessageProtocol(NetworkStream s, byte[] sword, byte[] eword)
        {
            this.StartWord = sword;
            this.EndWord = eword;
            this.Buffer = new List<byte>();
            this.StartWordRead = false;
            this.Messages = new Queue<Message>();
            this.Stream = s;
            this.TempBuffer = new byte[512];
        }

        public Message ReceiveMessage()
        {
            while (this.Messages.Count == 0)
            {
                int read = Stream.Read(TempBuffer, 0, TempBuffer.Length);
                this.AddData(TempBuffer, read);
            }
            return this.Messages.Dequeue();
        }

        public void AddData(byte[] data, int length)
        {
            //case 1: find the start word
            //case 2: find the end word and add a message to the queue

            AddtoBuffer(data, length);

            ParseMessage();
        }

        public void ParseMessage()
        {
            if (!StartWordRead) //case 1
            {
                int sindex = FindWord(0, Buffer.Count, StartWord);
                StartWordRead = sindex != -1;
                if (StartWordRead && sindex != 0)
                    Buffer.RemoveRange(0, sindex);
            }
            if (StartWordRead) //case 2
            {
                int eindex = FindWord(0, Buffer.Count, EndWord);
                if (eindex != -1)
                {
                    var arr = Encoding.UTF8.GetString(Buffer.GetRange(StartWord.Length, eindex - StartWord.Length).ToArray());
                    Message m = (Message)JsonSerializer.Deserialize(arr, typeof(Message));
                    Messages.Enqueue(m);
                    Buffer.RemoveRange(0, eindex + EndWord.Length);
                    StartWordRead = false;
                    ParseMessage();
                }
            }
        }

        public int FindWord(int start, int end, byte[] word)
        {
            if (Buffer.Count < word.Length)
                return -1;

            for (int i = start; i < end; i++)
            {
                bool equal = true;
                for (int j = 0; j < word.Length; j++)
                    if (i + j >= Buffer.Count || Buffer[i + j] != word[j])
                    {
                        equal = false;
                        break;
                    }
                if (equal)
                    return i;
            }

            return -1;
        }

        public void AddtoBuffer(byte[] data, int length)
        {
            this.Buffer.AddRange(data[0..length]);
        }

        public bool SendMessage(Message m)
        {
            try
            {
                string a = JsonSerializer.Serialize(m);
                var data = Encoding.UTF8.GetBytes(a);
                Stream.Write(StartWord, 0, StartWord.Length);
                Stream.Write(data, 0, data.Length);
                Stream.Write(EndWord, 0, EndWord.Length);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

    }
}
