using System.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace Chess_Pi_Application.View
{
    public sealed partial class SquareView : UserControl, INotifyPropertyChanged
    {
        public int Row { get { return Grid.GetRow(this); } }
        public int Column { get { return Grid.GetColumn(this); } }
        public int Position { get { return Row * 8 + Column; } }

        public int State
        {
            get { return state; }
            set
            {
                state = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("State"));
            }
        }
        private int state;

        public event PropertyChangedEventHandler PropertyChanged;

        public SquareView()
        {
            this.InitializeComponent();

            this.State = SquareStates.Normal;
            DataContext = this;
        }

        public override string ToString()
        {
            return string.Format("Square #{0} ({1}, {2})", Position, Row, Column);
        }
    }
}
