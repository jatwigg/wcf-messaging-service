using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfChatServer
{
    public class WCFChatService : IWCFChatService
    {
        private const int MESSAGE_TYPE_SERVER = 1, MESSAGE_TYPE_USER = 2;
        delegate void MessageEventHandler(object sender, MessageArgs e);
        // each connect will subscribe a new instance of MessageEventHandler delagate to this event, and disconnect will remove it.
        private static event MessageEventHandler MessageEvent;
        private static List<ConnectedClient> _connections = new List<ConnectedClient>(); //TODO: thread safe this

        private MessageEventHandler _handler = null; // retain this so we can unsubscribe from the event on disconnect
        private IWcfChatClient _callback = null;

        /// <summary>
        /// Connect and receive a unique identifier which you may use to perform actions until you disconnect.
        /// </summary>
        /// <param name="username">A chosen username to be known by, does not need to be unqiue.</param>
        /// <returns>a unique string that a client may use to perform actions, or null if there is a problem.</returns>
        public string Connect(string username)
        {
            ConnectedClient client = new ConnectedClient(username);
            _connections.Add(client);

            try
            {
                _callback = OperationContext.Current.GetCallbackChannel<IWcfChatClient>();
                MessageEvent += (_handler = new MessageEventHandler(messageEventHandler));
                MessageEvent(this, new MessageArgs(null, string.Format("USER {0} has joined the server.", username), MESSAGE_TYPE_SERVER, 0));
            }
            catch (Exception e)
            {
                log(string.Format("Exception raised during assigning callback, perhaps callback channel is not of type {0}. Exception: {1}.", typeof(IWcfChatClient).Name, e.ToString()));
                return null;
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
            // fire the event. all subscribers (including this instance) will receive the message
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

            MessageEvent(this, new MessageArgs(null, string.Format("USER {0} is now known as {1}.", client.name, client.name = newname), MESSAGE_TYPE_SERVER, 0));
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
                log(string.Format("Unknown message type {0} for message {1}.", e.type, e.message));
            }
        }

        /// <summary>
        /// Get the current username of a user by their ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The username or "unknown".</returns>
        private string getUserName(string id)
        {
            ConnectedClient client = _connections.Find(s => s.id == id);
            return (client==null? "unknown" : client.name);
        }

        /// <summary>
        /// Write to log.
        /// </summary>
        /// <param name="text">The text to log/</param>
        private void log(string text)
        {
            //TODO: output to log file
        }
    }

    /// <summary>
    /// The arguement passed into the MessageEvent. Represents messages that are pushed to the clients.
    /// </summary>
    public class MessageArgs : EventArgs
    {
        public int type;
        public int code;
        public string userid;
        public string message;

        public MessageArgs(string id, string message, int type, int code)
        {
            this.userid = id;
            this.message = message;
            this.type = type;
            this.code = code;
        }
    }
}
