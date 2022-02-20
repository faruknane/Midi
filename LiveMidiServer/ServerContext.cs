using BaseCom;
using System.Collections.Generic;

namespace LiveMidiServer
{
    public class ServerContext
    {
        public Dictionary<string, User> UsersByUserName = new Dictionary<string, User>();
        public readonly object UserLock = new object();
    }
}