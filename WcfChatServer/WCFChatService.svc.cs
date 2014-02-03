using System;
using System.Collections.Concurrent;
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
        private static List<ConnectedClient> _connections = new List<ConnectedClient>();
        private static object _listLock = new object();

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
            lock (_listLock)
            {
                _connections.Add(client);
            }
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
            lock (_listLock)
            {
                return (_connections.RemoveAll(c => c.id == id) > 0);
            }
        }

        /// <summary>
        /// Send a message to everyone connected.
        /// </summary>
        /// <param name="id">The unique ID the server granted you/</param>
        /// <param name="message">The string message to distribute.</param>
        public void SendMessage(string id, string message)
        {
            if (isNotPermitted(id)) return;
            // fire the event. all subscribers (including this instance) will receive the message
            ConnectedClient client;
            lock (_listLock)
            {
                client = _connections.Find(c => c.id == id);
                client.lastMessageTime = DateTime.Now;
            }
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
            if (isNotPermitted(id)) return false;
            ConnectedClient client;
            lock (_listLock)
            {
                client = _connections.Find(c => c.id == id);
            }
            MessageEvent(this, new MessageArgs(null, string.Format("USER {0} is now known as {1}.", client.name, client.name = newname), MESSAGE_TYPE_SERVER, 0));
            return true;
        }

        /// <summary>
        /// Function that handles this instance recuevubg a message event.
        /// </summary>
        /// <param name="sender">The sender instance.</param>
        /// <param name="e">The message args.</param>
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
        /// Retrieve an array of username that are currently connected to the server.
        /// </summary>
        /// <param name="id">The id issued to you by the server.</param>
        /// <returns>An array of username that are currently connected to the server.</returns>
        public string[] GetUsersOnline(string id)
        {
            if (isNotPermitted(id)) return null;
            List<string> names = new List<string>();
            lock (_listLock)
            {
                _connections.ForEach(c => names.Add(c.name));
            }
            return names.ToArray();
        }

        /// <summary>
        /// Ask the server when it last saw someone with a given username. Note that usernames are not tied to one client so this 
        /// information may be misrepresented and this is only to check if connected clients are still active.
        /// </summary>
        /// <param name="id">The unique ID issued to you by the server.</param>
        /// <param name="name">The name to search for.</param>
        /// <returns>A DateTime instance with the last seen DateTime for the client or DateTime.Min if a client using that username has not been seen.</returns>
        public DateTime Seen(string id, string name)
        {
            ConnectedClient client;
            lock (_listLock)
            {
                client = _connections.Find(s => s.name == name);
            }
            if (client == null) return DateTime.MinValue;
            lock (_listLock)
            {
                return client.lastMessageTime;
            }
        }

        /// <summary>
        /// Get the current username of a user by their ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The username or "unknown".</returns>
        private string getUserName(string id)
        {
            ConnectedClient client;
            lock (_listLock)
            {
                return ((client = _connections.Find(s => s.id == id)) == null ? "unknown" : client.name);
            }
        }

        /// <summary>
        /// Check the ID belongs to a valid connected client.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool isNotPermitted(string id)
        {
            lock (_listLock)
            {
                return (_connections.Find(c => c.id == id) == null);
            }
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
