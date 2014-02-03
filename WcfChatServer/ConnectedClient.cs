using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfChatServer
{
    public class ConnectedClient
    {
        public ConnectedClient(string username)
        {
            id = Guid.NewGuid().ToString();
            name = username;
            lastMessageTime = DateTime.Now;
        }

        public string name { get; set; }
        public string id { get; private set; }
        public DateTime lastMessageTime { get; set; }
    }
}