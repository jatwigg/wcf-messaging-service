using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace WcfChatServer
{
    interface IWcfChatClient
    {
        [OperationContract(IsOneWay = true)]
        void onMessageReceived(string username, string message);

        [OperationContract(IsOneWay = true)]
        void onServerInfoReceived(int statusCode, string message);
    }
}
