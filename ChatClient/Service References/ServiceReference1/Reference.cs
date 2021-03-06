﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChatClient.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.IWCFChatService", CallbackContract=typeof(ChatClient.ServiceReference1.IWCFChatServiceCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface IWCFChatService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFChatService/Connect", ReplyAction="http://tempuri.org/IWCFChatService/ConnectResponse")]
        string Connect(string username);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFChatService/Connect", ReplyAction="http://tempuri.org/IWCFChatService/ConnectResponse")]
        System.Threading.Tasks.Task<string> ConnectAsync(string username);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFChatService/Disconnect", ReplyAction="http://tempuri.org/IWCFChatService/DisconnectResponse")]
        bool Disconnect(string id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFChatService/Disconnect", ReplyAction="http://tempuri.org/IWCFChatService/DisconnectResponse")]
        System.Threading.Tasks.Task<bool> DisconnectAsync(string id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFChatService/SendMessage", ReplyAction="http://tempuri.org/IWCFChatService/SendMessageResponse")]
        void SendMessage(string id, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFChatService/SendMessage", ReplyAction="http://tempuri.org/IWCFChatService/SendMessageResponse")]
        System.Threading.Tasks.Task SendMessageAsync(string id, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFChatService/ChangeName", ReplyAction="http://tempuri.org/IWCFChatService/ChangeNameResponse")]
        bool ChangeName(string id, string newname);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFChatService/ChangeName", ReplyAction="http://tempuri.org/IWCFChatService/ChangeNameResponse")]
        System.Threading.Tasks.Task<bool> ChangeNameAsync(string id, string newname);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFChatService/GetUsersOnline", ReplyAction="http://tempuri.org/IWCFChatService/GetUsersOnlineResponse")]
        string[] GetUsersOnline(string id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFChatService/GetUsersOnline", ReplyAction="http://tempuri.org/IWCFChatService/GetUsersOnlineResponse")]
        System.Threading.Tasks.Task<string[]> GetUsersOnlineAsync(string id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFChatService/Seen", ReplyAction="http://tempuri.org/IWCFChatService/SeenResponse")]
        System.DateTime Seen(string id, string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWCFChatService/Seen", ReplyAction="http://tempuri.org/IWCFChatService/SeenResponse")]
        System.Threading.Tasks.Task<System.DateTime> SeenAsync(string id, string name);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IWCFChatServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IWCFChatService/onMessageReceived")]
        void onMessageReceived(string username, string message);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IWCFChatService/onServerInfoReceived")]
        void onServerInfoReceived(int statusCode, string message);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IWCFChatServiceChannel : ChatClient.ServiceReference1.IWCFChatService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WCFChatServiceClient : System.ServiceModel.DuplexClientBase<ChatClient.ServiceReference1.IWCFChatService>, ChatClient.ServiceReference1.IWCFChatService {
        
        public WCFChatServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public WCFChatServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public WCFChatServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public WCFChatServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public WCFChatServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public string Connect(string username) {
            return base.Channel.Connect(username);
        }
        
        public System.Threading.Tasks.Task<string> ConnectAsync(string username) {
            return base.Channel.ConnectAsync(username);
        }
        
        public bool Disconnect(string id) {
            return base.Channel.Disconnect(id);
        }
        
        public System.Threading.Tasks.Task<bool> DisconnectAsync(string id) {
            return base.Channel.DisconnectAsync(id);
        }
        
        public void SendMessage(string id, string message) {
            base.Channel.SendMessage(id, message);
        }
        
        public System.Threading.Tasks.Task SendMessageAsync(string id, string message) {
            return base.Channel.SendMessageAsync(id, message);
        }
        
        public bool ChangeName(string id, string newname) {
            return base.Channel.ChangeName(id, newname);
        }
        
        public System.Threading.Tasks.Task<bool> ChangeNameAsync(string id, string newname) {
            return base.Channel.ChangeNameAsync(id, newname);
        }
        
        public string[] GetUsersOnline(string id) {
            return base.Channel.GetUsersOnline(id);
        }
        
        public System.Threading.Tasks.Task<string[]> GetUsersOnlineAsync(string id) {
            return base.Channel.GetUsersOnlineAsync(id);
        }
        
        public System.DateTime Seen(string id, string name) {
            return base.Channel.Seen(id, name);
        }
        
        public System.Threading.Tasks.Task<System.DateTime> SeenAsync(string id, string name) {
            return base.Channel.SeenAsync(id, name);
        }
    }
}
