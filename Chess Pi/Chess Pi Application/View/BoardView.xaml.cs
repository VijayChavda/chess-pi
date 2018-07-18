using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Chess_Pi_Application.View
{
    public sealed partial class BoardView : UserControl
    {
        public List<SquareView> Squares { get; set; }
        public ObservableCollection<PieceView> Pieces { get; set; }
        public ObservableCollection<PieceView> CapturedPiecesWhite { get; set; }
        public ObservableCollection<PieceView> CapturedPiecesBlack { get; set; }

        public BoardView()
        {
            this.InitializeComponent();

            Squares = new List<SquareView>();
            Pieces = new ObservableCollection<PieceView>();
            CapturedPiecesWhite = new ObservableCollection<PieceView>();
            CapturedPiecesBlack = new ObservableCollection<PieceView>();

            Loaded += BoardView_Loaded;

            Pieces.CollectionChanged += Pieces_CollectionChanged;
        }

        private void Pieces_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var piece in e.OldItems.Cast<PieceView>())
                {
                    BoardGrid.Children.Remove(piece);
                    (piece.Player == Players.White ? CapturedPiecesWhite : CapturedPiecesBlack).Add(piece);
                }
            }
        }

        private void BoardView_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var square in BoardGrid.Children.Where(x => x.GetType() == typeof(SquareView)))
            {
                Squares.Add(square as SquareView);
            }

            foreach (var piece in BoardGrid.Children.Where(x => x.GetType() == typeof(PieceView)))
            {
                Pieces.Add(piece as PieceView);
            }
        }
    }
}
