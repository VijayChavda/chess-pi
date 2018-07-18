using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Device_Simulator
{
    public sealed partial class Simulator : UserControl
    {
        public bool[] Status { get; set; }

        public event EventHandler<bool[]> StatusChanged;

        public Simulator()
        {
            this.InitializeComponent();
        }

        private void APinChanged(object sender, EventArgs e)
        {
            List<bool> status = new List<bool>();

            foreach (var pinView in Board.Children.Cast<Square>())
            {
                status.Add(pinView.PinStatus);
            }

            Status = status.ToArray();
            StatusChanged?.Invoke(this, Status);
        }
    }
}
