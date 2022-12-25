using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace wcf_chat
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceChat : IServiceChat
    {
        List<ServerUser> users = new List<ServerUser>();
        Dictionary<int, GameAction> actionUsers = new Dictionary<int, GameAction>();
        int nextId = 1;
        public int tm = 0;
        public ServiceChat()
        {
            Timer();
        }

        private async void Timer()
        {
            while (true)
            {
                if (users.Count < 2)
                {
                    await Task.Delay(100);
                    tm = 0;
                    continue;
                }
                if (tm > 9)
                {

                    tm = 0;
                    GameResult result = GameLogic();
                    if (!CheckCountResultPlayer())
                    {
                        ResultFlow();
                    }
                    GetResult(result);

                }
                await Task.Delay(1000);
                tm += 1;
                GetTime(tm);
            }
        }

        public int Connect(string name)
        {

            ServerUser user = new ServerUser()
            {
                ID = nextId,
                Name = name,
                operationContext = OperationContext.Current,
                operationContextTime = OperationContext.Current,
            };
            nextId++;
            GetUsers();
            users.Add(user);
            return user.ID;
        }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(i => i.ID == id);
            if (user != null)
            {
                users.Remove(user);
            }
            GetUsers();
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
                item.operationContext.GetCallbackChannel<IServerChatCallback>().MsgCallback(answer);
            }
        }
        public void GetUsers()
        {
            var answer = users.Select(u => new ChatClient.Models.User
            {
                Id = u.ID,
                Name = u.Name,
            }).ToList();
            foreach (var item in users)
            {
                item.operationContext.GetCallbackChannel<IServerChatCallback>().CallbackPlayers(answer);
            }

        }

        public void SendChoice(int id, GameAction action)
        {
            actionUsers.Add(id, action);
        }

        public void GetTime(int time)
        {
            foreach (var item in users)
            {
                item.operationContextTime.GetCallbackChannel<IServerChatCallback>().CallbackTime(time);
            }
        }

        public enum GameAction
        {
            None = 0,
            Rock = 1,
            Paper = 2,
            Scissors = 3
        }

        public enum GameResult
        {
            None,
            WinRock,
            WinPaper,
            WinScissors,
            Draw
        }

        public bool CheckCountResultPlayer()
        {
            return users.Count == actionUsers.Count;
        }
        public void ResultFlow()
        {
            foreach (var item in users)
            {
                if (!actionUsers.ContainsKey(item.ID))
                    actionUsers.Add(item.ID, (GameAction)(new Random()).Next(1, 3));
            }
        }
        public GameResult GameLogic()
        {

            var values = actionUsers.Values.ToList();
            if (values.Contains(GameAction.Rock) && values.Contains(GameAction.Paper) && values.Contains(GameAction.Scissors))
            {
                return GameResult.Draw;
            }

            GameResult result = GameResult.None;

            int RCount = values.Count(a => a == GameAction.Rock);
            int SCount = values.Count(a => a == GameAction.Scissors);
            int PCount = values.Count(a => a == GameAction.Paper);
            if (RCount > 0 && SCount > 0)
            {
                result = GameResult.WinRock;
            }
            else
            if (RCount > 0 && SCount == 0 && PCount == 0)
            {
                result = GameResult.WinRock;

            }
            else
            if (SCount > 0 && PCount > 0)
            {
                result = GameResult.WinScissors;

            }
            else
            if (SCount > 0 && PCount == 0 && RCount == 0)
            {
                result = GameResult.WinScissors;

            }
            else
            if (PCount > 0 && RCount > 0)
            {
                result = GameResult.WinPaper;

            }
            else
            if (PCount > 0 && SCount == 0 && RCount == 0)
            {
                result = GameResult.WinPaper;
            }

            actionUsers = new Dictionary<int, GameAction>();
            return result;
        }

        public void GetResult(GameResult result)
        {
            foreach (var item in users)
            {
                item.operationContext.GetCallbackChannel<IServerChatCallback>().CallbackResult(result);
            }
        }
    }
}
