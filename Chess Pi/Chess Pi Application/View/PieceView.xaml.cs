using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Chess_Pi_Application.View
{
    public sealed partial class PieceView : UserControl, INotifyPropertyChanged
    {
        public int Row { get { return Grid.GetRow(this); } }
        public int Column { get { return Grid.GetColumn(this); } }
        public int Position { get { return Row * 8 + Column; } }

        public bool HasMoved
        {
            get { return hasMoved; }
            set
            {
                hasMoved = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasMoved"));
            }
        }
        private bool hasMoved;

        public Players Player
        {
            get { return player; }
            set
            {
                player = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Player"));
            }
        }
        private Players player;

        public ImageSource Image
        {
            get { return image; }
            set
            {
                image = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Image"));
            }
        }
        private ImageSource image;

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

        public Pieces PieceType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public PieceView()
        {
            this.InitializeComponent();

            DataContext = this;
            State = PieceStates.Down;
        }

        public override string ToString()
        {
            return string.Format("Square #{0} ({1}, {2})", Position, Row, Column);
        }
    }
}
