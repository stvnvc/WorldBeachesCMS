using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorldBeachesCMS.Models;
using Notification.Wpf;

namespace WorldBeachesCMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public User LoggedInUser { get; set; }
        private readonly NotificationManager _notificationManager = new NotificationManager();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void ShowToastNotification(string title, string message, NotificationType type)
        {
            _notificationManager.Show(title, message, type, "WindowNotificationArea");
        }

    }
}