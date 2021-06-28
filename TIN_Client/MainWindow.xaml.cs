using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TIN_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<ChatMessage> chatHistory;
        ObservableCollection<string> rooms;

        public MainWindow()
        {
            InitializeComponent();

            ((App)Application.Current).service.ChatMessageReceived += Service_ChatMessageReceived;
            ((App)Application.Current).service.RoomListReceived += Service_RoomListReceived;
            ((App)Application.Current).service.IdentitySet += Service_IdentitySet;


            chatHistory = new();
            rooms = new();
            DataGridChat.ItemsSource = chatHistory;
            ComboBoxJoinRoom.ItemsSource = rooms;

            //DataGridChat.AutoGenerateColumns = false;

            // Disable all controls until name is set
            setStage(0);
        }

        private void Service_IdentitySet()
        {
            // Enable chatroom selection, disable name changing
            setStage(1);
        }

        private void ButtonSetName_Click(object sender, RoutedEventArgs e)
        {
            App thisApp = ((App)Application.Current);
            string name = TextBoxSetName.Text;
            if (name.Equals(string.Empty))
            {
                // Todo add some error
            }
            thisApp.service.SetIdentity(name);
            thisApp.userName = name;
        }

        private void Service_RoomListReceived(String obj)
        {
            string s_obj = (string)obj;
            if (!rooms.Contains(s_obj))
                rooms.Add(s_obj);
        }

        private async void ButtonGetRooms_Click(object sender, RoutedEventArgs e)
        {
            App thisApp = ((App)Application.Current);

            await thisApp.service.ListRooms();
        }

        private void ButtonJoinRoom_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxJoinRoom.SelectedIndex < 0)
                return;

            App thisApp = ((App)Application.Current);

            thisApp.service.JoinRoom((string)ComboBoxJoinRoom.SelectedItem);

            // Enable chatroom selection, disable name changing
            setStage(2);
        }

        private void ButtonCreateRoom_Click(object sender, RoutedEventArgs e)
        {
            string newRoom = TextBoxCreateRoom.Text;

            if (newRoom.Length <= 0)
            {
                return;
            }

            App thisApp = ((App)Application.Current);

            thisApp.service.JoinRoom(newRoom);

            // Enable chatroom selection, disable name changing
            setStage(2);
        }

        private void ButtonLeave_Click(object sender, RoutedEventArgs e)
        {
            App thisApp = ((App)Application.Current);

            thisApp.service.LeaveRoom();

            // Enable chatroom selection, disable name changing
            setStage(1);
        }

        private void Service_ChatMessageReceived(string message)
        {
            ChatMessage msg = new ChatMessage(DateTime.Now, message);
            chatHistory.Add(msg);

            TextBoxChat.Text += "\n" + message;
        }

        private async void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            App thisApp = ((App)Application.Current);
            string msg = TextBoxMessage.Text;

            TextBoxMessage.Text = string.Empty;

            await thisApp.service.SendChatMessage(msg);
        }

        private async void ButtonSendPriv_Click(object sender, RoutedEventArgs e)
        {
            App thisApp = ((App)Application.Current);
            string conId = TextBoxPrivID.Text;
            string msg = TextBoxPrivMsg.Text;

            TextBoxPrivMsg.Text = string.Empty;

            await thisApp.service.SendPrivMessage(conId, msg);
        }

        private void setStage(int stage)
        {
            switch(stage)
            {
                case 0: // Just launched app
                    // Disable all controls until name is set
                    TextBoxSetName.IsEnabled = true;
                    ButtonSetName.IsEnabled = true;

                    TextBoxCreateRoom.IsEnabled = false;
                    ButtonCreateRoom.IsEnabled = false;
                    ButtonGetRooms.IsEnabled = false;
                    ComboBoxJoinRoom.IsEnabled = false;
                    ButtonJoinRoom.IsEnabled = false;

                    TextBoxMessage.IsEnabled = false;
                    ButtonSend.IsEnabled = false;

                    ButtonLeave.IsEnabled = false;
                    break;
                case 1: // Just changed name or left chatroom
                    // Enable chatroom selection, disable name changing
                    TextBoxSetName.IsEnabled = false;
                    ButtonSetName.IsEnabled = false;

                    ButtonGetRooms.IsEnabled = true;
                    TextBoxCreateRoom.IsEnabled = true;
                    ButtonCreateRoom.IsEnabled = true;
                    ComboBoxJoinRoom.IsEnabled = true;
                    ButtonJoinRoom.IsEnabled = true;

                    TextBoxMessage.IsEnabled = false;
                    ButtonSend.IsEnabled = false;

                    ButtonLeave.IsEnabled = false;
                    break;
                case 2: // Just connected to chatroom
                    // Enable chatroom selection, disable name changing
                    TextBoxSetName.IsEnabled = false;
                    ButtonSetName.IsEnabled = false;

                    ButtonGetRooms.IsEnabled = false;
                    TextBoxCreateRoom.IsEnabled = false;
                    ButtonCreateRoom.IsEnabled = false;
                    ComboBoxJoinRoom.IsEnabled = false;
                    ButtonJoinRoom.IsEnabled = false;

                    TextBoxMessage.IsEnabled = true;
                    ButtonSend.IsEnabled = true;

                    ButtonLeave.IsEnabled = true;
                    break;
            }
        }
    }
}
