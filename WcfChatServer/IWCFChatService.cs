using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfChatServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(SessionMode= SessionMode.Required, CallbackContract = typeof(IWcfChatClient))]
    public interface IWCFChatService
    {
        [OperationContract(IsOneWay = false)]
        string Connect(string username);

        [OperationContract(IsOneWay = false)]
        bool Disconnect(string id);

        [OperationContract(IsOneWay = false)]
        void SendMessage(string id, string message);

        [OperationContract(IsOneWay = false)]
        bool ChangeName(string id, string newname);


    }
}
