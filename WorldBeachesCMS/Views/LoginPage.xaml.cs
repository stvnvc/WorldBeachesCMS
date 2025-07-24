using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices;
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
using WorldBeachesCMS.Helpers;
using WorldBeachesCMS.Models;
using System.IO;

namespace WorldBeachesCMS.Views
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private DataIO _dataIO;
        private List<User> _users;

        public LoginPage()
        {
            InitializeComponent();
            _dataIO = new DataIO();

            _users = _dataIO.DeserializeObject<List<User>>("Data/Users.xml");
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username))
            {
                BadUsernameLabel.Content = "Username cannot be empty.";
                UsernameTextBox.BorderBrush = Brushes.Red;
                return;
            }
            else
            {
                BadUsernameLabel.Content = "";
                UsernameTextBox.BorderBrush = Brushes.Gray;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                BadPasswordLabel.Content = "Password cannot be empty";
                PasswordBox.BorderBrush = Brushes.Red;
                return;
            }
            else
            {
                BadPasswordLabel.Content = "";
                PasswordBox.BorderBrush = Brushes.Gray;
            }

                var userByUsername = _users.FirstOrDefault(u => u.Username.Equals(username));

            if (userByUsername == null)
            {
                BadUsernameLabel.Content = "User with this username does not exist.";
                UsernameTextBox.BorderBrush = Brushes.Red;

            }
            else if (userByUsername.Password.Equals(password))
            {
                UsernameTextBox.BorderBrush = Brushes.Gray;
                PasswordBox.BorderBrush = Brushes.Gray;
                MainWindow parentWindow = Window.GetWindow(this) as MainWindow;
                parentWindow.LoggedInUser = userByUsername;
                parentWindow.ShowToastNotification("Login successful", userByUsername.Role + " successfully logged in!", Notification.Wpf.NotificationType.Success);
                this.NavigationService.Navigate(new BeachesListPage());

            }
            else
            {
                BadPasswordLabel.Content = "Incorrect password.";
                PasswordBox.BorderBrush = Brushes.Red;

            }


        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
