using System.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace Device_Status_Viewer
{
    public sealed partial class Square : UserControl, INotifyPropertyChanged
    {
        public bool PinStatus
        {
            get
            {
                return pinStatus;
            }
            set
            {
                pinStatus = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PinStatus"));
            }
        }
        private bool pinStatus;

        public event PropertyChangedEventHandler PropertyChanged;

        public Square()
        {
            this.InitializeComponent();
        }
    }
}
