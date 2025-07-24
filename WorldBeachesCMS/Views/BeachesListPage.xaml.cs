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
using WorldBeachesCMS.Helpers;
using WorldBeachesCMS.Models;
using System.IO;

namespace WorldBeachesCMS.Views
{
    /// <summary>
    /// Interaction logic for BeachesListPage.xaml
    /// </summary>
    public partial class BeachesListPage : Page
    {
        private MainWindow _parentWindow;
        private DataIO _dataIO;
        public ObservableCollection<Beach> Beaches { get; set; }

        public BeachesListPage()
        {
            InitializeComponent();

            _dataIO = new DataIO();

            this.DataContext = this; 
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _parentWindow = Window.GetWindow(this) as MainWindow;
            AdjustUiForUserRole();

            string beachesFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/Beaches.xml");
            Beaches = _dataIO.DeserializeObject<ObservableCollection<Beach>>(beachesFilePath);
            if (Beaches == null) {
                Beaches = new ObservableCollection<Beach>();
            }

            BeachesDataGrid.ItemsSource = Beaches;

            AdjustUiForUserRole();

            UpdateDeleteButtonState();
        }

        private void AdjustUiForUserRole()
        {
            if(_parentWindow.LoggedInUser.Role == UserRole.Visitor)
            {
                AddButton.Visibility = Visibility.Collapsed;
                DeleteButton.Visibility = Visibility.Collapsed;
                SelectAllCheckBox.Visibility = Visibility.Collapsed;

                BeachesDataGrid.Columns[0].Visibility = Visibility.Collapsed;
            }


        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AddEditBeachPage(null));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            List<Beach> selectedBeaches = Beaches.Where(b=> b.IsSelected).ToList();

            var result = MessageBox.Show($"Are you sure you want to delete {selectedBeaches.Count} beach(es)?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var beach in selectedBeaches)
                {
                    Beaches.Remove(beach);
                }

                _dataIO.SerializeObject(Beaches, "Data/Beaches.xml");

                _parentWindow.ShowToastNotification("Success", "Successfully deleted selected beaches", Notification.Wpf.NotificationType.Success);


            }

        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _parentWindow.LoggedInUser = null;
            this.NavigationService.Navigate(new LoginPage());
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as CheckBox).IsChecked ?? false;
            foreach(var beach in Beaches)
            {
                beach.IsSelected = isChecked;
            }
            UpdateDeleteButtonState();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Hyperlink)?.DataContext is Beach selectedBeach)
            {
                if (_parentWindow.LoggedInUser.Role == UserRole.Admin)
                {
                    this.NavigationService.Navigate(new AddEditBeachPage(selectedBeach));
                }
                else
                {
                    this.NavigationService.Navigate(new BeachDetailsPage(selectedBeach));
                }
            }
        }

        private void UpdateDeleteButtonState()
        {
            DeleteButton.IsEnabled = Beaches.Any(b => b.IsSelected);
        } 

        private void OnBeackSelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateDeleteButtonState();
        }

    }
}
