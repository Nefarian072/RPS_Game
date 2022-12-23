using ChatClient.Models;
using ChatClient.ServiceChat;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChatClient.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, IServiceChatCallback
    {
        Random random;

        private ObservableCollection<User> users;
        ServiceChatClient client;
        private double canvasHeight = 200;

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
        private double canvasWeight = 200;

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



        public ObservableCollection<User> Users
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
        private User me;

        public User Me
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
            Me = new User();
            Users = new ObservableCollection<User>();
            Users.CollectionChanged += Users_CollectionChanged;


            Users.Add(new User()
            {
                Name = "Nikita",
                Id = 0,
            });
            Users.Add(new User()
            {
                Name = "Evgen",
                Id = 1,
            }); 
            Users.Add(new User()
            {
                Name = "Vlad",
                Id = 2,
            });

        }

        #region Commands
        private RelayCommand startGame;
        private RelayCommand endGame;
        private RelayCommand rockSelect;

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
                      //client.;

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
                      Users.Add(new User()
                      {
                          Name = "Nikita",
                          Id = random.Next(),
                      });
                  }/*(obj)=>!Me.InGame*/));
            }
        }
       

        #endregion


        void Users_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int playerCount = Users.Count;
            var points = CalculatePositions.Calculate(Users.Count, (int)CanvasHeight / 2, (int)canvasWeight / 2, (int)canvasHeight / 2);
            string pt = "";
            for (int i = 0; i < playerCount; i++)
            {
                Users[i].X = (int)points[i].X-25;
                Users[i].Y = (int)points[i].Y-25;
                Users[i].Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)255, (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)));


                pt += $"| {i} | X = {(int)points[i].X} | Y = {(int)points[i].Y} |\n";
            }

            //MessageBox.Show(pt);
        }
        public void MsgCallback(string msg)
        {
            MessageBox.Show(msg);
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


