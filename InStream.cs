using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Midi.NativeMethods;

namespace Midi
{
    public class InStream
    {
        public event EventHelper.MessageReceived MessageReceived;
        private NativeMethods.MidiInProc midiInProc { get; set; }
        private IntPtr Handle { get; set; }
        public int DeviceId { get; private set; }

        public InStream()
        {
            midiInProc = new NativeMethods.MidiInProc(MidiProc);
            Handle = IntPtr.Zero;
        }

        public bool Close()
        {
            bool result = Stop();
            result = result && NativeMethods.midiInClose(Handle)
                == MMRESULT.MMSYSERR_NOERROR;
            Handle = IntPtr.Zero;
            return result;
        }

        public bool Open(int id)
        {
            DeviceId = id;
            IntPtr h;

            MMRESULT res = NativeMethods.midiInOpen(
                out h,
                id,
                midiInProc,
                IntPtr.Zero,
                NativeMethods.CALLBACK_FUNCTION);
            Handle = h;
            bool ret = res == MMRESULT.MMSYSERR_NOERROR && Start();

            return ret;
        }

        private bool Start()
        {
            return NativeMethods.midiInStart(Handle)
                == MMRESULT.MMSYSERR_NOERROR;
        }

        private bool Stop()
        {
            return NativeMethods.midiInStop(Handle)
                == MMRESULT.MMSYSERR_NOERROR;
        }

        private void MidiProc(IntPtr hMidiIn,
            int wMsg,
            IntPtr dwInstance,
            ShortMessage dwParam1,
            ShortMessage dwParam2)
        {
            MessageReceived?.Invoke(new MidiMessage(hMidiIn, wMsg, dwInstance, dwParam1, dwParam2));
        }

        public static int DeviceCount
        {
            get { return NativeMethods.midiInGetNumDevs(); }
        }


    }

}
