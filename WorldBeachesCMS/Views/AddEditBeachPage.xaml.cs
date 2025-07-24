using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Printing;
using System.Reflection;
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

namespace WorldBeachesCMS.Views
{
    /// <summary>
    /// Interaction logic for AddEditBeachPage.xaml
    /// </summary>
    public partial class AddEditBeachPage : Page
    {
        private Beach _beach;
        private bool _isEditMode;
        private MainWindow _parentWindow;
        private DataIO _dataIO = new DataIO();

        //Add mode
        public AddEditBeachPage()
        {
            InitializeComponent();
            _isEditMode = false;
            _beach = new Beach();

            InitializeComboBoxes();
        }

        //Edit mode
        public AddEditBeachPage(Beach beachToEdit)
        {
            InitializeComponent();
            InitializeComboBoxes();
            if(beachToEdit != null)
            {
                _isEditMode = true;
                _beach = beachToEdit;
            }
            else
            {
                _isEditMode = false;
                _beach = new Beach();

            }
        }

        private void InitializeComboBoxes()
        {
            FontsComboBox.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);

            FontSizeComboBox.ItemsSource = new List<string>() { "8", "9", "10", "11", "12", "14", "16", "18", "20", "24", "28", "32", "48", "72" };

            FontColorComboBox.ItemsSource = typeof(Colors).GetProperties();
        }

        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true) { 
                string sourceFile = openFileDialog.FileName;
                string uniqueFileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(sourceFile);

                string relativePath = System.IO.Path.Combine("Data", "Images", uniqueFileName);
                string destinationFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destinationFile));

                File.Copy(sourceFile, destinationFile, true);

                _beach.ImagePath = relativePath;

                ImagePreview.Source = new BitmapImage(new Uri(destinationFile));

            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _parentWindow = Window.GetWindow(this) as MainWindow;

            if (!ValidateForm())
            {
                _parentWindow.ShowToastNotification("Error", "Form fields are not correctly filled!", Notification.Wpf.NotificationType.Error);
                return;
            }
            try
            {
                string _name = NameTextBox.Text;
                _beach.Rating = double.Parse(RatingTextBox.Text);


                //Sacuvamo RTF
                string rtfDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "RtfContent");

                Directory.CreateDirectory(rtfDirectory);

                string rtfFileName = _isEditMode ? System.IO.Path.GetFileName(_beach.RtfFilePath) : $"{Guid.NewGuid()}.rtf";
                string rtfFilePath = System.IO.Path.Combine(rtfDirectory, rtfFileName);

                TextRange range = new TextRange(DescriptionRichTextBox.Document.ContentStart, DescriptionRichTextBox.Document.ContentEnd);

                using (FileStream fs = new FileStream(rtfFilePath, FileMode.Create))
                {
                    range.Save(fs, DataFormats.Rtf);
                }

                _beach.RtfFilePath = System.IO.Path.Combine("Data", "RtfContent", rtfFileName);

                //Ucitamo postojece plaze, dodajemo/menjamo, cuvamo u XML
                string beachesFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/Beaches.xml");
                var beaches = _dataIO.DeserializeObject<ObservableCollection<Beach>>(beachesFilePath) ?? new ObservableCollection<Beach>();

                if (_isEditMode)
                {
                    var existingBeach = beaches.FirstOrDefault(b => b.Name == _beach.Name);
                    if (existingBeach != null)
                    {
                        _beach.Name = _name;
                        int index = beaches.IndexOf(existingBeach);
                        beaches[index] = _beach;
                    }
                }
                else
                {
                    _beach.Name = _name;
                    _beach.CreationDate = DateTime.Now;
                    beaches.Add(_beach);
                }

                _dataIO.SerializeObject(beaches, beachesFilePath);

                _parentWindow.ShowToastNotification("Success", $"Beach has been successfully added {(_isEditMode ? "updated" : "added")}.", Notification.Wpf.NotificationType.Success);

                this.NavigationService.GoBack();

            }
            catch (Exception ex) {
                _parentWindow.ShowToastNotification("Error", $"An error occurred while saving: {ex.Message}", Notification.Wpf.NotificationType.Error);
            }


        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            NameErrorLabel.Content = "";
            RatingErrorLabel.Content = "";
            ImageErrorLabel.Content = "";
            DescriptionErrorLabel.Content = "";

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                NameErrorLabel.Content = "Beach name cannot be empty.";
                isValid= false;
            }

            if(!double.TryParse(RatingTextBox.Text, out double rating) || rating < 0 || rating > 5)
            {
                RatingErrorLabel.Content = "Rating must be a number between 0,0 and 5,0.";
                isValid = false;
            }
            if (ImagePreview.Source == null)
            {
                ImageErrorLabel.Content = "An image must be selected.";
                isValid = false;
            }

            string description = new TextRange(DescriptionRichTextBox.Document.ContentStart, DescriptionRichTextBox.Document.ContentEnd).Text;
            if (string.IsNullOrWhiteSpace(description))
            {
                DescriptionErrorLabel.Content = "Description cannot be empty.";
                isValid = false;
            }

            return isValid;

        }

        private void FontsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(FontsComboBox.SelectedItem != null)
            {
                DescriptionRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, FontsComboBox.SelectedItem);
            }
            DescriptionRichTextBox.Focus();

        }

        private void FontSizeComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(double.TryParse(FontSizeComboBox.Text, out double newSize))
            {
                DescriptionRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, newSize);
            }
            DescriptionRichTextBox.Focus();
        }
        
        private void FontColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(FontColorComboBox.SelectedItem is PropertyInfo selectedColorInfo)
            {
                var color = (Color)selectedColorInfo.GetValue(null, null);
                var brush = new SolidColorBrush(color);

                DescriptionRichTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            }
            DescriptionRichTextBox.Focus();
        }

        private void DescriptionRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //Bold
            object fontWeight = DescriptionRichTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            BoldToggleButton.IsChecked = (fontWeight != DependencyProperty.UnsetValue) && (fontWeight.Equals(FontWeights.Bold));

            //Italic
            object fontStyle = DescriptionRichTextBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            ItalicToggleButton.IsChecked = (fontStyle != DependencyProperty.UnsetValue) && (fontStyle.Equals(FontStyles.Italic));

            //Underline
            object textDecoration = DescriptionRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineToggleButton.IsChecked = (textDecoration != DependencyProperty.UnsetValue) && (textDecoration.Equals(TextDecorations.Underline));

            object fontFamily = DescriptionRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            FontsComboBox.SelectedItem = fontFamily;

            object fontSize = DescriptionRichTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty);
            FontSizeComboBox.Text = fontSize.ToString();

            object fontColor = DescriptionRichTextBox.Selection.GetPropertyValue(TextElement.ForegroundProperty);
            if (fontColor is SolidColorBrush colorBrush)
            {
                var selectedColor = typeof(Colors).GetProperties().FirstOrDefault(p => (Color)p.GetValue(null, null) == colorBrush.Color);
                if (selectedColor != null)
                {
                    FontColorComboBox.SelectedItem = selectedColor;
                }
            }

        }

        private void DescriptionRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = new TextRange(DescriptionRichTextBox.Document.ContentStart, DescriptionRichTextBox.Document.ContentEnd).Text;

            char[] delimiters = new char[] { ' ', '\r', '\n'};
            int wordCount = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;

            WordCountTextBlock.Text = $"Words: {wordCount}";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _parentWindow = Window.GetWindow(this) as MainWindow;
            if(_isEditMode)
            {
                LoadBeachData();
            }
        }

        private void LoadBeachData() 
        {
            TitleTextBlock.Text = "Edit Beach";
            NameTextBox.Text = _beach.Name;
            RatingTextBox.Text = _beach.Rating.ToString();

            if (!string.IsNullOrEmpty(_beach.ImagePath)) { 
                ImagePreview.Source = new BitmapImage(new Uri(_beach.FullImagePath));
            
            }

            string rtfFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _beach.RtfFilePath);

            if (File.Exists(rtfFilePath)) { 
                TextRange range = new TextRange(DescriptionRichTextBox.Document.ContentStart, DescriptionRichTextBox.Document.ContentEnd);
                using (FileStream fs = new FileStream(rtfFilePath, FileMode.Open))
                {
                    range.Load(fs, DataFormats.Rtf);
                }
            
            }
        
        
        
        }

    }
}
