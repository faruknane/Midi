using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Midi
{
    class Program
    {
        static void instreamexample()
        {
            Console.WriteLine(InStream.DeviceCount);
            Console.WriteLine(NativeMethods.midiInGetDevCaps(0).szPname);
            InStream a = new InStream();
            a.MessageReceived += (MidiMessage message) => {
                Console.WriteLine(message);
            };

            if (a.Open(0))
            {
                Console.WriteLine("success");
            }
            else
            {
                Console.WriteLine("not success");
            }

            Thread.Sleep(10000);

            a.Close();
        }

        static void Main(string[] args)
        {
            Console.WriteLine(InStream.DeviceCount);
            Console.WriteLine(OutStream.DeviceCount);

            InStream b = new InStream();
            OutStream a = new OutStream();

            b.MessageReceived += (MidiMessage m) => {
                if(m.Param1.Command == 144 || m.Param1.Command == 128)
                {
                    a.SendMessage(new ShortMessage(m.Param1.Command, (byte)(m.Param1.Note+12), (byte)(Math.Min(m.Param1.Velocity/2*3, 127))));
                }
            };

            Console.WriteLine(b.Open(0));
            Console.WriteLine(a.Open(0));

            //a.SendMessage(new ShortMessage(144, 60, 127));
            Thread.Sleep(10000000);

            Console.WriteLine(a.Close());
            Console.WriteLine(b.Close());
        }
    }
}
