using System;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Chess_Pi_Application.View
{
    public sealed partial class GameView : UserControl, INotifyPropertyChanged
    {
        public GameStates GameState
        {
            get { return gameState; }
            set
            {
                previousGameState = gameState;
                gameState = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GameState"));
            }
        }
        private GameStates gameState;
        private GameStates previousGameState;

        public SquareView LastChangeSquare
        {
            get { return lastChangeSquare; }
            set
            {
                lastChangeSquare = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LastChangeSquare"));
            }
        }
        private SquareView lastChangeSquare;

        public PieceView CurrentMovingPiece { get; set; }

        public Players Turn { get; set; }

        public bool[] CurrentState { get; set; }
        public bool[] PreviousState { get; set; }
        public bool[] InitialState = new bool[]
            {
                true, true, true, true, true, true, true, true,
                true, true, true, true, true, true, true, true,
                false,false,false,false,false,false,false,false,
                false,false,false,false,false,false,false,false,
                false,false,false,false,false,false,false,false,
                false,false,false,false,false,false,false,false,
                true, true, true, true, true, true, true, true,
                true, true, true, true, true, true, true, true,
            };

        private bool IsCheckingCheck { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public GameView()
        {
            this.InitializeComponent();

            DataContext = this;

            GameState = GameStates.NotStarted;
            previousGameState = GameStates.NotStarted;
            LastChangeSquare = null;
            CurrentMovingPiece = null;
            Turn = Players.White;
            CurrentState = InitialState;
            PreviousState = null;
            IsCheckingCheck = false;
        }

        internal void StateChanged(bool[] state)
        {
            CurrentState = state;

            //When this method is called for the first time
            if (PreviousState == null)
            {
                PreviousState = CurrentState;
                return;
            }

            //Count the number of state 'changes'
            //If only one change occured, remember the 'changePosition'
            int changePosition = 0, changes = 0;
            for (int position = 0; position < 64; position++)
            {
                if (PreviousState[position] != CurrentState[position])
                {
                    changes++;
                    changePosition = position;
                }
            }

            //Indicating Piece-up and Piece-down using Opacity.
            foreach (var piece in CurrentBoard.Pieces)
            {
                piece.State = CurrentState[piece.Position] ? PieceStates.Down : PieceStates.Up;
            }

            //When the game state was invalid
            if (GameState == GameStates.Invalid)
            {
                //Has the board state been restored?
                if (CurrentState.SequenceEqual(PreviousState))
                {
                    GameState = previousGameState;
                }
                else
                {
                    return;
                }
            }

            //Don't bother going ahead if nothing has changed on the board
            if (changes == 0)
            {
                return;
            }

            //If we've reached this far, only one state change has happened on the board
            //Determine if that change was a Piece-up or Piece-down
            bool isPieceUp = PreviousState[changePosition] == true && CurrentState[changePosition] == false;
            bool isPieceDown = PreviousState[changePosition] == false && CurrentState[changePosition] == true;

            //Get the Square at the position where state changed.
            SquareView changedSquare = CurrentBoard.Squares[changePosition];

            if (GameState == GameStates.Waiting)
            {
                if (isPieceUp)
                {
                    var pickedUpPiece = CurrentBoard.Pieces.Single(x => x.Position == changedSquare.Position);

                    if (pickedUpPiece.Player == Turn)
                    {
                        GameState = GameStates.Moving;
                        changedSquare.State = SquareStates.PieceAboveIsMoving;
                        CurrentMovingPiece = pickedUpPiece;
                        ShowLegalMoves(pickedUpPiece);
                    }
                    else
                    {
                        GameState = GameStates.WrongTurn;
                        //LastChangeSquare = changedSquare;
                        //return;
                    }
                }
                else
                {
                    GameState = GameStates.Invalid;
                    return;
                }
            }
            else if (GameState == GameStates.Moving)
            {
                if (isPieceDown)
                {
                    bool isMoveCanceled = changedSquare.State == SquareStates.PieceAboveIsMoving;
                    bool isLegalMove =
                        changedSquare.State == SquareStates.ShowingLegalMove ||
                        changedSquare.State == SquareStates.ShowingCastling ||
                        changedSquare.State == SquareStates.ShowingPromotion ||
                        changedSquare.State == SquareStates.ShowingEnPassant ||
                        changedSquare.State == SquareStates.ShowingPieceThreatened;

                    if (isMoveCanceled)
                    {
                        GameState = GameStates.Waiting;

                        ClearAllSquares();
                        CheckCheck(Turn);
                    }
                    else if (isLegalMove)
                    {
                        GameState = GameStates.Waiting;

                        MovePiece(CurrentMovingPiece, changedSquare);
                        CheckCheck(Turn);
                    }
                    else
                    {
                        GameState = GameStates.Illegal;

                        changedSquare.State = SquareStates.ShowingIllegalMove;
                    }
                }
                else
                {
                    //Check if player had picked up the piece to capture it
                    bool isCapturingMove = changedSquare.State == SquareStates.ShowingPieceThreatened;

                    if (isCapturingMove)
                    {
                        //Capture the piece
                        PieceView capturedPiece = CurrentBoard.Pieces.Single(x => x.Position == changedSquare.Position);
                        CurrentBoard.Pieces.Remove(capturedPiece);

                        //Now the capturing piece HAS to take place of the captured piece.
                        ClearAllSquares();
                        changedSquare.State = SquareStates.ShowingPieceThreatened;
                    }
                    else
                    {
                        GameState = GameStates.Invalid;
                        return;
                    }
                }
            }
            else if (GameState == GameStates.Illegal)
            {
                if (isPieceUp)
                {
                    //If the misplaced piece is picked up, change game state back to 'Moving'.
                    if (LastChangeSquare == changedSquare)
                    {
                        GameState = GameStates.Moving;
                        changedSquare.State = SquareStates.Normal;
                    }
                    else
                    {
                        GameState = GameStates.Invalid;
                        return;
                    }
                }
                else
                {
                    GameState = GameStates.Invalid;
                    return;
                }
            }
            else if (GameState == GameStates.WrongTurn)
            {
                if (isPieceDown)
                {
                    //If the mispicked piece is put back down, change game state back to 'Waiting'
                    if (LastChangeSquare == changedSquare)
                    {
                        GameState = GameStates.Waiting;
                    }
                    else
                    {
                        GameState = GameStates.Invalid;
                        return;
                    }
                }
                else
                {
                    GameState = GameStates.Invalid;
                    return;
                }
            }

            LastChangeSquare = changedSquare;
            PreviousState = CurrentState;

            //Reindicating Piece-up and Piece-down using Opacity.
            foreach (var piece in CurrentBoard.Pieces)
            {
                piece.State = CurrentState[piece.Position] ? PieceStates.Down : PieceStates.Up;
            }
        }

        private void ClearAllSquares()
        {
            CurrentBoard.Squares.ForEach(x => x.State = SquareStates.Normal);
        }

        private void MovePiece(PieceView piece, SquareView square)
        {
            //Move the piece
            Grid.SetRow(piece, square.Row);
            Grid.SetColumn(piece, square.Column);

            //Remember that this piece has moved
            //PieceView piece = CurrentBoard.Pieces.Single(x => x.Position == square.Position);
            piece.HasMoved = true;

            //Clear squares showing legal moves
            ClearAllSquares();

            //Change player turn
            Turn = Turn == Players.White ? Players.Black : Players.White;

            //TODO:
            //Check game over
        }

        private void MovePieceVirtually(PieceView piece, SquareView square)
        {
            //Move the piece
            Grid.SetRow(piece, square.Row);
            Grid.SetColumn(piece, square.Column);
        }

        private void ShowLegalMoves(PieceView piece)
        {
            switch (piece.PieceType)
            {
                case Pieces.King:
                    ShowLegalMoveHelper(piece, -1, -1);
                    ShowLegalMoveHelper(piece, 0, -1);
                    ShowLegalMoveHelper(piece, 1, -1);

                    ShowLegalMoveHelper(piece, -1, 0);
                    ShowLegalMoveHelper(piece, 1, 0);

                    ShowLegalMoveHelper(piece, -1, 1);
                    ShowLegalMoveHelper(piece, 0, 1);
                    ShowLegalMoveHelper(piece, 1, 1);
                    break;
                case Pieces.Queen:
                    ShowLegalMovesHelper(piece, -1, -1);
                    ShowLegalMovesHelper(piece, 0, -1);
                    ShowLegalMovesHelper(piece, 1, -1);

                    ShowLegalMovesHelper(piece, -1, 0);
                    ShowLegalMovesHelper(piece, 1, 0);

                    ShowLegalMovesHelper(piece, -1, 1);
                    ShowLegalMovesHelper(piece, 0, 1);
                    ShowLegalMovesHelper(piece, 1, 1);
                    break;
                case Pieces.Rook:
                    ShowLegalMovesHelper(piece, 0, -1);
                    ShowLegalMovesHelper(piece, 0, 1);

                    ShowLegalMovesHelper(piece, -1, 0);
                    ShowLegalMovesHelper(piece, 1, 0);
                    break;
                case Pieces.Bishop:
                    ShowLegalMovesHelper(piece, -1, -1);
                    ShowLegalMovesHelper(piece, 1, -1);

                    ShowLegalMovesHelper(piece, -1, 1);
                    ShowLegalMovesHelper(piece, 1, 1);
                    break;
                case Pieces.Knight:
                    ShowLegalMoveHelper(piece, -1, -2);

                    ShowLegalMoveHelper(piece, 2, 1);
                    ShowLegalMoveHelper(piece, 2, -1);
                    ShowLegalMoveHelper(piece, -2, 1);
                    ShowLegalMoveHelper(piece, -2, -1);

                    ShowLegalMoveHelper(piece, 1, 2);
                    ShowLegalMoveHelper(piece, -1, 2);
                    ShowLegalMoveHelper(piece, 1, -2);
                    ShowLegalMoveHelper(piece, -1, -2);
                    break;
                case Pieces.Pawn:
                    if (piece.Player == Players.White)
                    {
                        if (piece.HasMoved == false)
                        {
                            ShowLegalMoveHelper(piece, 0, -2, true);
                        }
                        ShowLegalMoveHelper(piece, 0, -1, true);
                        ShowLegalMoveHelper(piece, -1, -1);
                        ShowLegalMoveHelper(piece, 1, -1);
                    }
                    else
                    {
                        if (piece.HasMoved == false)
                        {
                            ShowLegalMoveHelper(piece, 0, 2, true);
                        }
                        ShowLegalMoveHelper(piece, 0, 1, true);
                        ShowLegalMoveHelper(piece, -1, 1);
                        ShowLegalMoveHelper(piece, 1, 1);
                    }
                    break;
            }
        }

        private void ShowLegalMoveHelper(PieceView piece, int x, int y, bool friendly = false)
        {
            ShowLegalMovesHelper(piece, x, y, friendly, true);
        }

        private void ShowLegalMovesHelper(PieceView piece, int x, int y, bool friendly = false, bool singleStep = false)
        {
            int far = singleStep ? 2 : 8;
            for (int i = 1; i < far; i++)
            {
                int row = piece.Row + i * y;
                int column = piece.Column + i * x;

                if (row < 0 || column < 0 || row > 7 || column > 7)
                {
                    break;
                }

                bool occupied = CurrentBoard.Pieces.Count(p => p.Row == row && p.Column == column) == 1;
                SquareView square = CurrentBoard.Squares.Single(p => p.Row == row && p.Column == column);

                if (occupied && !friendly)
                {
                    PieceView foreignPiece = CurrentBoard.Pieces.Single(p => p.Position == square.Position);
                    if (foreignPiece.Player != piece.Player)
                    {
                        if (foreignPiece.PieceType == Pieces.King)
                        {
                            square.State = SquareStates.ShowingKingInCheck;
                        }
                        else if (!IsCheckingCheck)
                        {
                            int originalPosition = piece.Position;
                            MovePieceVirtually(piece, square);
                            if (CheckCheck(piece.Player) == false)
                            {
                                square.State = SquareStates.ShowingPieceThreatened;
                            }
                            MovePieceVirtually(piece, CurrentBoard.Squares.Single(p => p.Position == originalPosition));
                        }
                    }
                    return;
                }
                else if (!occupied)
                {
                    if (piece.PieceType != Pieces.Pawn || piece.PieceType == Pieces.Pawn && friendly)
                    {
                        if (!IsCheckingCheck)
                        {
                            int originalPosition = piece.Position;
                            MovePieceVirtually(piece, square);
                            if (CheckCheck(piece.Player) == false)
                            {
                                square.State = SquareStates.ShowingLegalMove;
                            }
                            else
                            {
                                square.State = SquareStates.ShowingNotPossibleMoves;
                            }
                            MovePieceVirtually(piece, CurrentBoard.Squares.Single(p => p.Position == originalPosition));
                        }
                    }
                }
            }
        }

        private bool CheckCheck(Players player)
        {
            PieceView king = CurrentBoard.Pieces.Where(x => x.PieceType == Pieces.King).Single(x => x.Player == player);
            SquareView kingSquare = CurrentBoard.Squares.Single(x => x.Position == king.Position);
            kingSquare.State = SquareStates.Normal;
            foreach (var enemy in CurrentBoard.Pieces.Where(x => x.Player != player))
            {
                if (CurrentBoard.Pieces.Count(x => x.Position == enemy.Position) == 1)
                {
                    IsCheckingCheck = true;
                    ShowLegalMoves(enemy);
                    IsCheckingCheck = false;
                }

                if (kingSquare.State == SquareStates.ShowingKingInCheck)
                {
                    return true;
                }
            }

            return false;
        }

        private void NewGameClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (CurrentState.SequenceEqual(InitialState) || true)
            {
                GameState = GameStates.Waiting;
                LastChangeSquare = null;
                CurrentState = InitialState;
                PreviousState = InitialState;
                Turn = Players.White;
            }
        }
    }
}
