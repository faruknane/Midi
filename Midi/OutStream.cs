using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midi
{
    public class OutStream
    {
        private IntPtr Handle { get; set; }
        public int DeviceId { get; private set; }

        public OutStream()
        {

        }

        public bool Open(int id)
        {
            DeviceId = id;
            IntPtr h;
            bool ret = NativeMethods.midiOutOpen(out h, (uint)id, IntPtr.Zero, IntPtr.Zero, NativeMethods.CALLBACK_NULL) == NativeMethods.MMRESULT.MMSYSERR_NOERROR;
            Handle = h;
            return ret;
        }

        public bool Close()
        {
            return NativeMethods.midiOutClose(Handle) == NativeMethods.MMRESULT.MMSYSERR_NOERROR;
        }

        public bool SendMessage(ShortMessage message)
        {
            return NativeMethods.midiOutShortMsg(this.Handle, message) == NativeMethods.MMRESULT.MMSYSERR_NOERROR;
        }

        public static int DeviceCount
        {
            get { return NativeMethods.midiOutGetNumDevs(); }
        }
    }
}
