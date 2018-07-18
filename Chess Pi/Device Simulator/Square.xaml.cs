using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Device_Simulator
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
                PinStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private bool pinStatus;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler PinStatusChanged;

        public Square()
        {
            this.InitializeComponent();
        }

        private void PinClicked(object sender, RoutedEventArgs e)
        {
            PinStatus = !PinStatus;
        }
    }
}
