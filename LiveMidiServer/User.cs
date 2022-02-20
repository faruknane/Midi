using BaseCom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveMidiServer
{
    public class User
    {
        public Client Client;
        public ServerContext Context;

        public Message Success, Unsuccess;
        public string UserName;

        public User(ServerContext s, Client c)
        {
            this.Client = c;
            this.Context = s;
            this.Success = "Success";
            this.Unsuccess = "Unsuccess";
        }

        public void DoWork()
        {
            Message m = Client.ReceiveMessage();
            if(m == null)
                return;
            if(UserName == null)
            {
                if (m != "registeruser" || !m.SubMessages.ContainsKey("username"))
                {
                    Client.SendMessage(Unsuccess);
                    return;
                }
                string usrname = m["username"];

                lock (Context.UserLock)
                {
                    if(Context.UsersByUserName.ContainsKey(usrname))
                    {
                        Client.SendMessage(Unsuccess);
                        return;
                    }
                    else
                    {
                        Context.UsersByUserName[usrname] = this;
                        this.UserName = usrname;
                        Client.SendMessage(Success);
                        Console.WriteLine("User registered: " + usrname);
                        return;
                    }
                }
            }
            if (m == "sendmidi")
            {
                string to = m["to"];
                string body = m["body"];
                lock (Context.UserLock)
                {
                    if (Context.UsersByUserName.ContainsKey(to))
                    {
                        Message s = "playmidi";
                        s["body"] = body;
                        Context.UsersByUserName[to].Client.SendMessage(s);
                    }
                }
            }

        }
    }
}
