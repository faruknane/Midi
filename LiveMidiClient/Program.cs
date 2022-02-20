using BaseCom;
using Midi;
using System;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;

namespace LiveMidiClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter ip adress: ");

            byte[] StartWord = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] EndWord = new byte[8] { 8, 7, 6, 5, 4, 3, 2, 1 };
            Client c = new Client(new TcpClient(Console.ReadLine(), 11000), StartWord, EndWord);
            
            string usrname;

            while (true)
            {
                Console.Write("Sisteme kayıt olmak için bir kullanıcı adı giriniz: ");
                usrname = Console.ReadLine();
                Message m = "registeruser";
                m["username"] = usrname;
                c.SendMessage(m);
                Message r = c.ReceiveMessage();
                Console.WriteLine(r);
                if (r == "Success")
                    break;
                Console.WriteLine("Muhtemelen bu nickname alınmış!");
            }

            Console.Write("İşlem Giriniz (Dinle, Dinlet): ");
            string type = Console.ReadLine();
            if(type == "Dinle")
            {
                Console.WriteLine("Gelen Notalar Dinleniyor...");
                OutStream midiout = new OutStream();
                Console.WriteLine("Output Midi Device Count: " + OutStream.DeviceCount);
                Console.Write("Hangi Device: ");
                int devid = int.Parse(Console.ReadLine());
                Console.WriteLine(midiout.Open(devid));
                while (true)
                {
                    Message m = c.ReceiveMessage();
                    if(m == "playmidi")
                    {
                        ShortMessage s = (ShortMessage)JsonSerializer.Deserialize(m["body"], typeof(ShortMessage));
                        midiout.SendMessage(s);
                        Console.WriteLine("Message Received: " + s);
                    }
                }
                midiout.Close();
            }
            else if(type == "Dinlet")
            {
                Console.Write("Kime Dinletmek istersin: ");
                string to = Console.ReadLine();

                InStream midiin = new InStream();
                midiin.MessageReceived += (MidiMessage mm) =>
                {
                    if (mm.Param1.Command == 144 || mm.Param1.Command == 128)
                    {
                        Message m = "sendmidi";
                        m["to"] = to;
                        m["body"] = JsonSerializer.Serialize(mm.Param1);
                        c.SendMessage(m);
                        Console.WriteLine("Sent message: " + mm.Param1);
                    }
                };

                Console.WriteLine("Input Midi Device Count: " + InStream.DeviceCount);
                Console.Write("Hangi Device: ");
                int devid = int.Parse(Console.ReadLine());
                Console.WriteLine(midiin.Open(devid));
                while (true)
                {
                    Thread.Sleep(1000);
                }
                midiin.Close();
            }
        }
    }
}
