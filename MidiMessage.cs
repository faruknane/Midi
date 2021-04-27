using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Midi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ShortMessage
    {
        [JsonPropertyName("1")]
        public byte Command { get; set; }
        [JsonPropertyName("2")]
        public byte Note { get; set; }
        [JsonPropertyName("3")]
        public byte Velocity { get; set; }
        [JsonPropertyName("4")]
        public byte Ignored { get; set; }


        public ShortMessage(byte c, byte n, byte v)
        {
            this.Command = c;
            this.Note = n;
            this.Velocity = v;
            this.Ignored = 0;
        }

        public override string ToString()
        {
            return this.GetType().Name + " {"
            + "Cmnd: " + this.Command + ", "
            + "Note: " + this.Note + ", "
            + "Velocity: " + this.Velocity
            + "}";
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MidiMessage
    {
        [JsonPropertyName("1")]
        public IntPtr Handle { get; set; }
        [JsonPropertyName("2")]
        public int Message { get; set; }
        [JsonPropertyName("3")]
        public IntPtr Instance { get; set; }
        [JsonPropertyName("4")]
        public ShortMessage Param1 { get; set; }
        [JsonPropertyName("5")]
        public ShortMessage Param2 { get; set; }

        public MidiMessage(IntPtr h, int m, IntPtr i, ShortMessage p1, ShortMessage p2)
        {
            this.Handle = h;
            this.Message = m;
            this.Instance = i;
            this.Param1 = p1;
            this.Param2 = p2;
        }


        public override string ToString()
        {
            return this.GetType().Name + " {"
            + "Msg: " + this.Message + ", "
            + "Cmnd: " + this.Param1.Command + ", "
            + "Note: " + this.Param1.Note + ", "
            + "Velocity: " + this.Param1.Velocity 
            + "}";
        }


    }
}
