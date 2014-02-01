using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfChatServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class WCFChatService : IWCFChatService
    {
        const int MESSAGE_TYPE_SERVER = 1, MESSAGE_TYPE_USER = 2;

        delegate void MessageEventHandler(object sender, MessageArgs e);
        event MessageEventHandler MessageEvent;
        MessageEventHandler _handler = null; // retain this so we can remove on disconnect

        List<ConnectedClient> _connections = new List<ConnectedClient>();
        IWcfChatClient _callback = null;

        /// <summary>
        /// Connect and receive a unique identifier which you may use to perform actions until you disconnect.
        /// </summary>
        /// <param name="username">A chosen username to be known by, does not need to be unqiue.</param>
        /// <returns></returns>
        public string Connect(string username)
        {
            ConnectedClient client = new ConnectedClient(username);
            _connections.Add(client);

            try
            {
                _callback = OperationContext.Current.GetCallbackChannel<IWcfChatClient>();
                MessageEvent += (_handler = new MessageEventHandler(messageEventHandler));
            }
            catch (Exception e)
            {
                // TODO: log
                // TODO: return null 
            }

            return client.id;
        }

        /// <summary>
        /// No longer receive or send messages.
        /// </summary>
        /// <param name="id">Your unique id that the server assigned you. This will be revoked.</param>
        /// <returns>True if cleanly disconnected, else false (perhaps you were already disconnected).</returns>
        public bool Disconnect(string id)
        {
            MessageEvent -= _handler;
            return (_connections.RemoveAll(c => c.id == id)  > 0);
        }

        /// <summary>
        /// Send a message to everyone connected.
        /// </summary>
        /// <param name="id">The unique ID the server granted you/</param>
        /// <param name="message">The string message to distribute.</param>
        public void SendMessage(string id, string message)
        {
            MessageEvent(this, new MessageArgs(id, message, MESSAGE_TYPE_USER, 0));
        }

        /// <summary>
        /// Change your handle on the server.
        /// </summary>
        /// <param name="id">Your unique ID the server granted you upon connection.</param>
        /// <param name="newname">Your new name.</param>
        /// <returns>True if success, ele false.</returns>
        public bool ChangeName(string id, string newname)
        {
            ConnectedClient client = _connections.Find(c => c.id == id);
            if (client == null) return false;

            MessageEvent(this, new MessageArgs(null, string.Format("USER {0} his now known as {1}.", client.name, client.name = newname), MESSAGE_TYPE_SERVER, 0));
            return true;
        }


        public void messageEventHandler(object sender, MessageArgs e)
        {
            if (e.type == MESSAGE_TYPE_SERVER)
            {
                _callback.onServerInfoReceived(e.code, e.message);
            }
            else if (e.type == MESSAGE_TYPE_USER)
            {
                _callback.onMessageReceived(getUserName(e.userid), e.message);
            }
            else
            {
                // TODO: log unknown message type
            }
        }

        private string getUserName(string id)
        {
            ConnectedClient client = _connections.Find(s => s.id == id);
            return (client==null? "unknown" : client.name);
        }
    }

    public class MessageArgs : EventArgs
    {
        public int type;
        public int code;
        public string userid;
        public string message;

        public MessageArgs(string id, string message, int type, int code)
        {
            // TODO: Complete member initialization
            this.userid = id;
            this.message = message;
            this.type = type;
            this.code = code;
        }
    }
}
