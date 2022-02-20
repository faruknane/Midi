using System.Collections.Generic;
using System.Text.Json;

namespace BaseCom
{
    public class Message
    {
        public string Value { get; set; }

        public Dictionary<string, Message> SubMessages { get; set; }

        public Message()
        {
            this.SubMessages = new Dictionary<string, Message>();
            Value = "";
        }

        public Message(string val)
        {
            this.SubMessages = new Dictionary<string, Message>();
            Value = val;
        }

        public Message this[string key]
        {
            get
            {
                return SubMessages[key];
            }
            set
            {
                SubMessages[key] = value;
            }
        }

        public static implicit operator string(Message s) => s.Value;
        public static implicit operator Message(string s) => new Message(s);
    }
}