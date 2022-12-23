using ServiceReference1;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Windows;
using WPFTest.Models;

namespace WPFTest.ViewModels;

public class MainViewModel : INotifyPropertyChanged, IServiceChatCallback
{

    private ObservableCollection<User> users;

    private RelayCommand startGame;
    private RelayCommand endGame;

    ServiceReference1.ServiceChatClient client;

    public RelayCommand StartGame
    {
        get
        {
            return startGame ??= new RelayCommand(async obj =>
            {

                Me.InGame = true;

                client = new ServiceChatClient();
                Me.Id = await client.ConnectAsync(Me.Name);
                client.SendMsg("Бумага", Me.Id);

            }, (obj) => !Me.InGame && Me.Name.Length > 0);
        }
    }

    public RelayCommand EndGame
    {
        get
        {
            return endGame ??= new RelayCommand(async obj =>
            {

                Me.InGame = false;

                client.Disconnect(Me.Id);
                client = null;

            }, (obj) => Me.InGame);
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
        Me = new User();
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
