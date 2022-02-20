using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Midi
{
    public static class EventHelper
    {
        public delegate void MessageReceived(MidiMessage message);  
    }

    //https://www.pinvoke.net/default.aspx/winmm.midiOutClose
    internal static class NativeMethods
    {
        internal enum MMRESULT : uint
        {
            MMSYSERR_NOERROR = 0,
            MMSYSERR_ERROR = 1,
            MMSYSERR_BADDEVICEID = 2,
            MMSYSERR_NOTENABLED = 3,
            MMSYSERR_ALLOCATED = 4,
            MMSYSERR_INVALHANDLE = 5,
            MMSYSERR_NODRIVER = 6,
            MMSYSERR_NOMEM = 7,
            MMSYSERR_NOTSUPPORTED = 8,
            MMSYSERR_BADERRNUM = 9,
            MMSYSERR_INVALFLAG = 10,
            MMSYSERR_INVALPARAM = 11,
            MMSYSERR_HANDLEBUSY = 12,
            MMSYSERR_INVALIDALIAS = 13,
            MMSYSERR_BADDB = 14,
            MMSYSERR_KEYNOTFOUND = 15,
            MMSYSERR_READERROR = 16,
            MMSYSERR_WRITEERROR = 17,
            MMSYSERR_DELETEERROR = 18,
            MMSYSERR_VALNOTFOUND = 19,
            MMSYSERR_NODRIVERCB = 20,
            WAVERR_BADFORMAT = 32,
            WAVERR_STILLPLAYING = 33,
            WAVERR_UNPREPARED = 34
        }

        internal const int CALLBACK_FUNCTION = 0x00030000;
        internal const int CALLBACK_NULL = 0x00000000;

        //int stream
        [StructLayout(LayoutKind.Sequential)]
        internal struct MIDIINCAPS
        {
            public ushort wMid;
            public ushort wPid;
            public uint vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public uint dwSupport;
        }

        internal delegate void MidiInProc(
            IntPtr hMidiIn,
            int wMsg,
            IntPtr dwInstance,
            ShortMessage dwParam1,
            ShortMessage dwParam2);

        [DllImport("winmm.dll", SetLastError = true)]
        internal static extern MMRESULT midiInGetDevCaps(int uDeviceID, ref MIDIINCAPS caps, uint cbMidiInCaps);
        internal static MMRESULT midiInGetDevCaps(int uDeviceID, ref MIDIINCAPS caps)
        {
            return midiInGetDevCaps(uDeviceID, ref caps, (uint)Marshal.SizeOf(typeof(MIDIINCAPS)));
        }

        internal static MIDIINCAPS midiInGetDevCaps(int uDeviceID)
        {
            MIDIINCAPS res = new MIDIINCAPS();
            MMRESULT err = midiInGetDevCaps(uDeviceID, ref res, (uint)Marshal.SizeOf(typeof(MIDIINCAPS)));

            if (err != MMRESULT.MMSYSERR_NOERROR)
                throw new Exception("err: " + err.ToString());

            return res;
        }

        [DllImport("winmm.dll")]
        internal static extern int midiInGetNumDevs();

        [DllImport("winmm.dll")]
        internal static extern MMRESULT midiInClose(
            IntPtr hMidiIn);

        [DllImport("winmm.dll")]
        internal static extern MMRESULT midiInOpen(
            out IntPtr lphMidiIn,
            int uDeviceID,
            MidiInProc dwCallback,
            IntPtr dwCallbackInstance,
            int dwFlags);

        [DllImport("winmm.dll")]
        internal static extern MMRESULT midiInStart(
            IntPtr hMidiIn);

        [DllImport("winmm.dll")]
        internal static extern MMRESULT midiInStop(
            IntPtr hMidiIn);




        //out stream
        [StructLayout(LayoutKind.Sequential)]
        internal struct MIDIHDR
        {
            public IntPtr data;
            public uint bufferLength;
            public uint bytesRecorded;
            public IntPtr user;
            public uint flags;
            public IntPtr next;
            public IntPtr reserved;
            public uint offset;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public IntPtr[] reservedArray;
        }


        [StructLayout(LayoutKind.Sequential)]
        internal struct MIDIOUTCAPS
        {
            public ushort wMid;
            public ushort wPid;
            public uint vDriverVersion;     //MMVERSION
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public ushort wTechnology;
            public ushort wVoices;
            public ushort wNotes;
            public ushort wChannelMask;
            public uint dwSupport;
        }

        // values for wTechnology field of MIDIOUTCAPS structure
        private const ushort MOD_MIDIPORT = 1;     // output port
        private const ushort MOD_SYNTH = 2;        // generic internal synth
        private const ushort MOD_SQSYNTH = 3;      // square wave internal synth
        private const ushort MOD_FMSYNTH = 4;      // FM internal synth
        private const ushort MOD_MAPPER = 5;       // MIDI mapper
        private const ushort MOD_WAVETABLE = 6;    // hardware wavetable synth
        private const ushort MOD_SWSYNTH = 7;      // software synth

        // flags for dwSupport field of MIDIOUTCAPS structure
        private const uint MIDICAPS_VOLUME = 1;      // supports volume control
        private const uint MIDICAPS_LRVOLUME = 2;    // separate left-right volume control
        private const uint MIDICAPS_CACHE = 4;
        private const uint MIDICAPS_STREAM = 8;      // driver supports midiStreamOut directly

        [DllImport("winmm.dll", SetLastError = true)]
        internal static extern MMRESULT midiOutGetDevCaps(int uDeviceID, ref MIDIOUTCAPS caps, uint cbMidiInCaps);
        internal static MMRESULT midiOutGetDevCaps(int uDeviceID, ref MIDIOUTCAPS caps)
        {
            return midiOutGetDevCaps(uDeviceID, ref caps, (uint)Marshal.SizeOf(typeof(MIDIOUTCAPS)));
        }

        internal static MIDIOUTCAPS midiOutGetDevCaps(int uDeviceID)
        {
            MIDIOUTCAPS res = new MIDIOUTCAPS();
            MMRESULT err = midiOutGetDevCaps(uDeviceID, ref res, (uint)Marshal.SizeOf(typeof(MIDIOUTCAPS)));

            if (err != MMRESULT.MMSYSERR_NOERROR)
                throw new Exception("err: " + err.ToString());

            return res;
        }

        [DllImport("winmm.dll")]
        internal static extern int midiOutGetNumDevs();

        [DllImport("winmm.dll")]
        internal static extern MMRESULT midiOutClose(IntPtr hMidiOut);

        [DllImport("winmm.dll")]
        internal static extern MMRESULT midiOutOpen(out IntPtr lphMidiOut, uint uDeviceID, IntPtr dwCallback, IntPtr dwInstance, uint dwFlags);

        [DllImport("winmm.dll")]
        internal static extern MMRESULT midiOutShortMsg(IntPtr hMidiOut, ShortMessage dwMsg);

    }
}
