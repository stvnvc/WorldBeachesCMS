using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.IO;

namespace WorldBeachesCMS.Models
{
    [Serializable]
    public class Beach : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public double Rating { get; set; }

        public string ImagePath { get; set; }

        public string RtfFilePath { get; set; }

        public DateTime CreationDate { get; set; }

        [XmlIgnore]
        public string FullImagePath
        {
            get
            {
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.ImagePath);
            }
        }

        public Beach() { }

        [NonSerialized]
        private bool _isSelected;

        [XmlIgnore] //promenjiva za DataGrid CheckBox
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        

    }
}
