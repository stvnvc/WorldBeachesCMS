using System;
using System.Collections.Generic;
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
using WorldBeachesCMS.Models;
using System.IO;

namespace WorldBeachesCMS.Views
{
    /// <summary>
    /// Interaction logic for BeachDetailsPage.xaml
    /// </summary>
    public partial class BeachDetailsPage : Page
    {
        private readonly Beach _beach;

        public BeachDetailsPage(Beach beachToShow)
        {
            InitializeComponent();

            _beach = beachToShow;

            this.DataContext = _beach;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRichTextContent();
        }

        private void LoadRichTextContent()
        {
            if(_beach != null && !string.IsNullOrEmpty(_beach.RtfFilePath))
            {
                string rtfFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _beach.RtfFilePath);
                if (File.Exists(rtfFilePath))
                {
                    TextRange range = new TextRange(DescriptionRichTextBox.Document.ContentStart, DescriptionRichTextBox.Document.ContentEnd);
                    using (FileStream fs = new FileStream(rtfFilePath, FileMode.Open))
                    {
                        range.Load(fs, DataFormats.Rtf);
                    }
                
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack) { 
                this.NavigationService.GoBack();
            }
        }
    }
}
