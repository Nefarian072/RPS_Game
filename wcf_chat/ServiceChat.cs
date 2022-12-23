﻿using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;


namespace wcf_chat
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceChat : IServiceChat
    {
        List<ServerUser> users = new List<ServerUser>();
        int nextId = 1;

        public int Connect(string name)
        {

            ServerUser user = new ServerUser()
            {
                ID = nextId,
                Name = name,
                operationContext = OperationContext.Current
            };
            nextId++;

            SendMsg(": " + user.Name + " подключился к чату!", 0);
            users.Add(user);
            return user.ID;
        }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(i => i.ID == id);
            if (user != null)
            {
                users.Remove(user);
                SendMsg(": " + user.Name + " покинул чат!", 0);
            }
        }

        public void SendMsg(string msg, int id)
        {
            string answer = "";
            foreach (var us in users)
            {
                answer += $"{us.ID} | {us.Name}\n";
            }
            foreach (var item in users)
            {
                /*string answer = DateTime.Now.ToShortTimeString();

                var user = users.FirstOrDefault(i => i.ID == id);
                if (user != null)
                {
                    answer += ": " + user.Name+" ";
                }
                answer += msg;*/

                item.operationContext.GetCallbackChannel<IServerChatCallback>().MsgCallback(answer);
            }
        }
        public void GetUsers()
        {
            string answer = "";
            foreach (var us in users)
            {
                answer += $"{us.ID}|{us.Name}\n";
            }
            foreach (var item in users)
            {
                item.operationContext.GetCallbackChannel<IServerChatCallback>().MsgCallback(answer);
            }

        }

        public enum GameAction
        {
            None,
            Rock,
            Paper,
            Scissors
        }

    }
}