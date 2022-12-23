using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChatClient.Models
{
    public class User : INotifyPropertyChanged
    {
        private int id;
        private string name;
        private string choice;
        private bool inGame;

        private int x;

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
                OnPropertyChanged();
            }
        }

        private int y;

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
                OnPropertyChanged();
            }
        }

        public User()
        {
            InGame = false;
            Name = "";
            Choice = "";
            X = 0;
            Y = 0;
            id = -1;
        }
        public bool InGame
        {
            get
            {
                return inGame;
            }
            set
            {
                inGame = value;
                OnPropertyChanged();
            }
        }


        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }


        public string Choice
        {
            get
            {
                return choice;
            }
            set
            {
                choice = value;
                OnPropertyChanged();
            }
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

