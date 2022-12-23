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
                    continue;
                }
                if (tm >= 15)
                {

                    tm = 0;
                    GameResult result = GameLogic();
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
                operationContext = OperationContext.Current
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
                item.operationContext.GetCallbackChannel<IServerChatCallback>().CallbackTime(time);
            }
        }

        public enum GameAction
        {
            None,
            Rock,
            Paper,
            Scissors
        }

        public enum GameResult
        {
            None,
            Win,
            Lose,
            Draw
        }

        public GameResult GameLogic()
        {
            return GameResult.None;
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
