using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LearningPawns
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static public List<List<Coord>> BoardState = new List<List<Coord>>();
        static public List<List<Coord>> MatchboxState = new List<List<Coord>>();
        static public List<Move> Moves = new List<Move>();
        static bool DoOnce = false;
        static bool mDoOnce = false;
        public int Wins = 0;
        public int Losses = 0;
        private string s;
        public string Status
        {
            get { return s; }
            set {
                s = value;
                StatusText.Text = s;
            }
        }
        public int ArrowHeadSideLength = 50;
        private bool PlayersTurn = true;
        public Grid GameBoard;
        public bool GameActive = false;

        private static double InitialHeight = 120;
        private static double InitialWidth= 163;

        public Piece A1;
        public Piece A2;
        public Piece A3;
        public Piece B1;
        public Piece B2;
        public Piece B3;

        public MainPage()
        {
            this.InitializeComponent();

            A1 = new Piece() { Circle = eA1 };
            A2 = new Piece() { Circle = eA2 };
            A3 = new Piece() { Circle = eA3 };
            B1 = new Piece() { Circle = eB1, IsHuman = true };
            B2 = new Piece() { Circle = eB2, IsHuman = true };
            B3 = new Piece() { Circle = eB3, IsHuman = true };
        }

        private void SetBoardCoordinates()
        {
            BoardState = new List<List<Coord>>() { new List<Coord>() { new Coord() { C = 1, R = 1 }, new Coord() { C = 2, R = 1 }, new Coord() { C = 3, R = 1 } },
                                                   new List<Coord>() { new Coord() { C = 1, R = 2 }, new Coord() { C = 2, R = 2 }, new Coord() { C = 3, R = 2 } },
                                                   new List<Coord>() { new Coord() { C = 1, R = 3 }, new Coord() { C = 2, R = 3 }, new Coord() { C = 3, R = 3 } }
            };

            A1.Coordinates = BoardState[0][0];
            A2.Coordinates = BoardState[0][1];
            A3.Coordinates = BoardState[0][2];
            B1.Coordinates = BoardState[2][0];
            B2.Coordinates = BoardState[2][1];
            B3.Coordinates = BoardState[2][2];

            Moves.Add(new Move() { Arrow = Move1112, Start = BoardState[0][0], End = BoardState[1][0] });
            Moves.Add(new Move() { Arrow = Move1122, Start = BoardState[0][0], End = BoardState[1][1] });
            Moves.Add(new Move() { Arrow = Move2112, Start = BoardState[0][1], End = BoardState[1][0] });
            Moves.Add(new Move() { Arrow = Move2122, Start = BoardState[0][1], End = BoardState[1][1] });
            Moves.Add(new Move() { Arrow = Move2132, Start = BoardState[0][1], End = BoardState[1][2] });
            Moves.Add(new Move() { Arrow = Move3122, Start = BoardState[0][2], End = BoardState[1][1] });
            Moves.Add(new Move() { Arrow = Move3132, Start = BoardState[0][2], End = BoardState[1][2] });
            Moves.Add(new Move() { Arrow = Move1211, Start = BoardState[1][0], End = BoardState[0][0] });
            Moves.Add(new Move() { Arrow = Move1221, Start = BoardState[1][0], End = BoardState[0][1] });
            Moves.Add(new Move() { Arrow = Move2211, Start = BoardState[1][1], End = BoardState[0][0] });
            Moves.Add(new Move() { Arrow = Move2221, Start = BoardState[1][1], End = BoardState[0][1] });
            Moves.Add(new Move() { Arrow = Move2231, Start = BoardState[1][1], End = BoardState[0][2] });
            Moves.Add(new Move() { Arrow = Move3221, Start = BoardState[1][2], End = BoardState[0][1] });
            Moves.Add(new Move() { Arrow = Move3231, Start = BoardState[1][2], End = BoardState[0][2] });
            Moves.Add(new Move() { Arrow = Move1213, Start = BoardState[1][2], End = BoardState[2][0] });
            Moves.Add(new Move() { Arrow = Move1223, Start = BoardState[1][2], End = BoardState[2][1] });
            Moves.Add(new Move() { Arrow = Move2213, Start = BoardState[1][1], End = BoardState[2][0] });
            Moves.Add(new Move() { Arrow = Move2223, Start = BoardState[1][1], End = BoardState[2][1] });
            Moves.Add(new Move() { Arrow = Move2233, Start = BoardState[1][1], End = BoardState[2][2] });
            Moves.Add(new Move() { Arrow = Move3223, Start = BoardState[1][2], End = BoardState[2][1] });
            Moves.Add(new Move() { Arrow = Move3233, Start = BoardState[1][2], End = BoardState[2][2] });
            Moves.Add(new Move() { Arrow = Move1312, Start = BoardState[2][0], End = BoardState[1][0] });
            Moves.Add(new Move() { Arrow = Move1322, Start = BoardState[2][0], End = BoardState[1][1] });
            Moves.Add(new Move() { Arrow = Move2312, Start = BoardState[2][1], End = BoardState[1][0] });
            Moves.Add(new Move() { Arrow = Move2322, Start = BoardState[2][1], End = BoardState[1][1] });
            Moves.Add(new Move() { Arrow = Move2332, Start = BoardState[2][1], End = BoardState[1][2] });
            Moves.Add(new Move() { Arrow = Move3322, Start = BoardState[2][2], End = BoardState[1][1] });
            Moves.Add(new Move() { Arrow = Move3332, Start = BoardState[2][2], End = BoardState[1][2] });

            foreach (var M in Moves)
            {
                M.InitialPoints = M.Arrow.Points;
            }
        }

        private void UpdateBoardCoordinates()
        {
            if (!DoOnce)
            {
                ResetBoard();
            }

            var A = (this.Parent as Frame).Parent as Border;
            var W = A.Width - PageGrid.ColumnDefinitions[0].Width.Value;
            var H = A.Height;

            int PieceSize = W < H ? (int)(W / 6) : (int)(H / 6);
            A1.Height = PieceSize;
            A1.Width = PieceSize;
            A2.Height = PieceSize;
            A2.Width = PieceSize;
            A3.Height = PieceSize;
            A3.Width = PieceSize;
            B1.Height = PieceSize;
            B1.Width = PieceSize;
            B2.Height = PieceSize;
            B2.Width = PieceSize;
            B3.Height = PieceSize;
            B3.Width = PieceSize;
            
        }

        private void ResetBoard()
        {
            SetBoardCoordinates();

            GetCoord(1, 1).SetPiece(A1);
            GetCoord(2, 1).SetPiece(A2);
            GetCoord(3, 1).SetPiece(A3);
            GetCoord(1, 3).SetPiece(B1);
            GetCoord(2, 3).SetPiece(B2);
            GetCoord(3, 3).SetPiece(B3);
        }

        public Coord GetCoord(int C, int R)
        {
            if (C >= 1 && R >= 1 && C <= 3 && R <= 3)
            {
                return BoardState[R - 1][C - 1];
            }
            return null;
        }

        public Coord GetMCoord(int C, int R)
        {
            if (C >= 1 && R >= 1 && C <= 3 && R <= 3)
            {
                return MatchboxState[R - 1][C - 1];
            }
            return null;
        }

        private void Begin_Click(object sender, RoutedEventArgs e)
        {
            if (!GameActive)
            {
                ResetBoard();
                GameActive = true;
                MovePlayer();
            }
        }

        private void MovePlayer()
        {
            //PlayersTurn = true;
            FindMoves();
        }

        private void MoveComputer()
        {
            //PlayersTurn = false;
            FindMoves();
        }

        private void FindMoves()
        {
            Piece[] Pieces;
            if (PlayersTurn)
            {
                Pieces = new Piece[] { A1, A2, A3 };
            }
            else
            {
                Pieces = new Piece[] { B1, B2, B3 };
            }
            foreach (var item in Moves)
            {
                item.Activate(false);
            }
            bool HasValidMoves = false;
            foreach (var P in Pieces)
            {
                if (P.Circle.Visibility != Visibility.Visible) { continue; }
                int R = P.R - 1;
                if (!P.IsHuman)
                {
                    R = P.R + 1;
                }

                var End = GetCoord(P.C - 1, R);
                if (End != null && (End.Piece != null && End.Piece.IsHuman != P.IsHuman))
                {
                    foreach (var item in Moves)
                    {
                        if (item.Start == P.Coordinates && item.End == End)
                        {
                            item.Activate();
                            HasValidMoves = true;
                        }
                    }
                }
                End = GetCoord(P.C, R);
                if (End != null && End.Piece == null)
                {
                    foreach (var item in Moves)
                    {
                        if (item.Start == P.Coordinates && item.End == End)
                        {
                            item.Activate();
                            HasValidMoves = true;
                        }
                    }
                }
                End = GetCoord(P.C + 1, R);
                if (End != null && (End.Piece != null && End.Piece.IsHuman != P.IsHuman))
                {
                    foreach (var item in Moves)
                    {
                        if (item.Start == P.Coordinates && item.End == End)
                        {
                            item.Activate();
                            HasValidMoves = true;
                        }
                    }
                }
            }
            if (!HasValidMoves && GameActive)
            {
                if (PlayersTurn)
                {
                    Losses++;
                }
                else
                {
                    Wins++;
                }
                Status = "P: " + Wins + " -- C: " + Losses;
                GameActive = false;
            }
        }

        public class Coord
        {
            public int C;
            public int R;
            public Piece Piece;

            public void SetPiece(Piece P)
            {
                foreach (var Row in BoardState)
                {
                    foreach (var C in Row)
                    {
                        if (C.Piece == P)
                        {
                            C.Piece = null;
                        }
                    }
                }
                Piece = P;
                Grid.SetColumn(Piece.Circle, C - 1);
                Grid.SetRow(Piece.Circle, R - 1);
                Piece.Circle.Visibility = Visibility.Visible;
            }
        }

        public class Piece
        {
            public Ellipse Circle;
            public Coord Coordinates;
            public bool IsHuman;

            public double Height
            {
                get { return Circle.Height; }
                set { Circle.Height = value; }
            }
            public double Width
            {
                get { return Circle.Width; }
                set { Circle.Width = value; }
            }
            public Thickness Margin
            {
                get { return Circle.Margin; }
                set { Circle.Margin = value; }
            }
            public int C
            {
                get { return Coordinates.C; }
                set { Coordinates.C = value; }
            }
            public int R
            {
                get { return Coordinates.R; }
                set { Coordinates.R = value; }
            }

            public void Refresh(double Scale)
            {
                Circle.Width = Scale;
                Circle.Height = Scale;
            }
        }

        public class Move
        {
            public Coord Start;
            public Coord End;
            public Polygon Arrow;
            public PointCollection InitialPoints;

            public void Activate(bool Active = true)
            {
                if (Active)
                {
                    Arrow.Visibility = Visibility.Visible;
                }
                else
                {
                    Arrow.Visibility = Visibility.Collapsed;
                }
            }

            public void Perform()
            {
                Piece Piece = Start.Piece;
                Piece.Coordinates = End;
                Grid.SetColumn(Piece.Circle, End.C - 1);
                Grid.SetRow(Piece.Circle, End.R - 1);
                if (End.Piece != null)
                {
                    End.Piece.Circle.Visibility = Visibility.Collapsed;
                }
                End.Piece = Piece;
                Start.Piece = null;
            }

            public void Refresh(double CurrentWidth, double CurrentHeight)
            {
                Arrow.Points = new PointCollection() { new Point((InitialPoints[0].X * CurrentWidth) / InitialWidth, (InitialPoints[0].Y * CurrentHeight) / InitialHeight),
                                                       new Point((InitialPoints[1].X * CurrentWidth) / InitialWidth, (InitialPoints[1].Y * CurrentHeight) / InitialHeight),
                                                       new Point((InitialPoints[2].X * CurrentWidth) / InitialWidth, (InitialPoints[2].Y * CurrentHeight) / InitialHeight) };
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Wins = 0;
            Losses = 0;
            Status = "P: 0 -- C: 0";
            //ClearKnowledge();
            ResetBoard();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var A = (this.Parent as Frame).Parent as Border;
            if (!DoOnce)
            {
                UpdateBoardCoordinates();
                GameBoard = Board;
                DoOnce = true;
            }
            else
            {
                var CurrentWidth = (A.Width - PageGrid.ColumnDefinitions[0].Width.Value) / 6;
                var CurrentHeight = A.Height / 6;
                float Scale = CurrentWidth / InitialWidth <= CurrentHeight / InitialHeight ? (float)(CurrentWidth / InitialWidth) : (float)(CurrentHeight / InitialHeight);
                foreach (var P in new Piece[] { A1, A2, A3, B1, B2, B3 })
                {
                    P.Refresh(Scale * InitialHeight);
                }
                foreach (Move M in Moves)
                {
                    M.Refresh(CurrentWidth, CurrentHeight);
                }
            }
        }

        private void Grid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            //Status = Window.Current.CoreWindow.PointerPosition.X + ":" + Window.Current.CoreWindow.PointerPosition.Y;
        }

        private void Move_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PlayersTurn = !PlayersTurn;
            string N = (sender as Polygon).Name.Substring(4);
            var Start = GetCoord(int.Parse(N.Substring(0, 1)), int.Parse(N.Substring(1, 1)));
            var End = GetCoord(int.Parse(N.Substring(2, 1)), int.Parse(N.Substring(3, 1)));
            foreach (var Move in Moves)
            {
                if (Move.Start == Start && Move.End == End)
                {
                    Move.Perform();
                    MoveComputer();
                    break;
                }
            }
        }
    }
}
