using ChatClient.Models;
using ChatClient.ServiceChat;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChatClient.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, IServiceChatCallback
    {
        Random random;

        private ObservableCollection<Models.User> users;
        ServiceChatClient client;

        private int time = 0;
        private string result = "";

        public int Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
                OnPropertyChanged();
            }
        }

        public string Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
                OnPropertyChanged();
            }
        }
        private double canvasHeight = 250;

        public double CanvasHeight
        {
            get
            { 
                return canvasHeight; 
            }
            set 
            {
                canvasHeight = value;
                OnPropertyChanged();
            }
        }
        private double canvasWeight = 250;

        public double CanvasWeight
        {
            get
            { 
                return canvasWeight; 
            }
            set 
            {
                canvasWeight = value;
                OnPropertyChanged();
            }
        }



        public ObservableCollection<Models.User> Users
        {
            get
            {
                return users;
            }
            set
            {
                users = value;
                OnPropertyChanged();
            }
        }
        private Models.User me;
        public Models.User Me
        {
            get
            {
                return me;
            }
            set
            {
                me = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel()
        {
            random = new Random();
            Me = new Models.User();
            Users = new ObservableCollection<Models.User>();
            Users.CollectionChanged += Users_CollectionChanged;

        }

        #region Commands
        private RelayCommand startGame;
        private RelayCommand endGame;
        private RelayCommand rockSelect;
        private RelayCommand paperSelect;
        private RelayCommand scissorsSelect;

        public RelayCommand StartGame
        {
            get
            {
                return startGame ??
                  (startGame = new RelayCommand(obj =>
                  {

                      Me.InGame = true;

                      client = new ServiceChatClient(new InstanceContext(this));
                      Me.Id = client.Connect(Me.Name);
                      client.GetUsers();

                  }, (obj) => !Me.InGame && Me.Name.Length > 0));
            }
        }
        public RelayCommand EndGame
        {
            get
            {
                return endGame ??
                  (endGame = new RelayCommand(obj =>
                  {

                      Me.InGame = false;

                      client.Disconnect(Me.Id);
                      client = null;

                  }, (obj) => Me.InGame));
            }
        }

        public RelayCommand RockSelect
        {
            get
            {
                return rockSelect ??
                  (rockSelect = new RelayCommand(obj =>
                  {
                      client.SendChoice(Me.Id, ServiceChatGameAction.Rock);
                  },(obj)=>Me.InGame));
            }
        }
        public RelayCommand PaperSelect
        {
            get
            {
                return paperSelect ??
                  (paperSelect = new RelayCommand(obj =>
                  {
                      client.SendChoice(Me.Id, ServiceChatGameAction.Paper);
                  },(obj)=>Me.InGame));
            }
        }
        public RelayCommand ScissorsSelect
        {
            get
            {
                return scissorsSelect ??
                  (scissorsSelect = new RelayCommand(obj =>
                  {
                      client.SendChoice(Me.Id, ServiceChatGameAction.Scissors);
                  },(obj)=>Me.InGame));
            }
        }
       

        #endregion
        void Users_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int playerCount = Users.Count;
            var points = CalculatePositions.Calculate(Users.Count, (int)CanvasHeight / 2, (int)canvasWeight / 2, (int)canvasHeight / 2);
            for (int i = 0; i < playerCount; i++)
            {
                Users[i].X = (int)points[i].X-25;
                Users[i].Y = (int)points[i].Y-25;
            }
        }
        public void MsgCallback(string msg)
        {
            MessageBox.Show(msg);
        }

        public void CallbackTime(int time)
        {
            Time = time;
        }

        public void CallbackPlayers(ServiceChat.User[] users)
        {
            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(
                    new Models.User()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        InGame = true
                    });
            }
        }

        public void CallbackResult(ServiceChatGameResult result)
        {
            switch(result) 
            {
                case ServiceChatGameResult.WinRock:
                    Result = "Камень победил!";
                    break;
                case ServiceChatGameResult.WinScissors:
                    Result = "Ножницы победили!";
                    break;
                case ServiceChatGameResult.WinPaper:
                    Result = "Бумага победила!";
                    break;
                case ServiceChatGameResult.Draw:
                    Result = "Ничья!";
                    break;
                default:
                    break;
            }
            MessageBox.Show(Result);
            Result = "";
        }
        #region MVVM
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }




        #endregion
        
    }
}


