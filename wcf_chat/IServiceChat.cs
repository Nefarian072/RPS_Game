using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using static wcf_chat.ServiceChat;
using ChatClient.Models;

namespace wcf_chat
{
    [ServiceContract(CallbackContract = typeof(IServerChatCallback))]
    public interface IServiceChat
    {
        [OperationContract]
        int Connect(string name);

        [OperationContract]
        void Disconnect(int id);

        [OperationContract(IsOneWay = true)]
        void SendMsg(string msg, int id);

        [OperationContract(IsOneWay = true)]
        void GetUsers();

        [OperationContract(IsOneWay = true)]
        void SendChoice(int id, GameAction action);

        [OperationContract(IsOneWay = true)]
        void GetTime(int time);

        [OperationContract(IsOneWay = true)]
        void GetResult(GameResult result);

    }

    public interface IServerChatCallback
    {
        [OperationContract(IsOneWay = true)]
        void MsgCallback(string msg);
        [OperationContract(IsOneWay = true)]
        void CallbackTime(int time);

        [OperationContract(IsOneWay = true)]
        void CallbackPlayers(List<User> users);

        [OperationContract(IsOneWay = true)]
        void CallbackResult(GameResult result);
    }
}
