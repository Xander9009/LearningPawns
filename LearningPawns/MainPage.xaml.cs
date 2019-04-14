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
        public List<Polygon> Highlights = new List<Polygon>();
        private bool PlayersTurn = true;

        private double H;
        private double W;

        public Piece A1;
        public Piece A2;
        public Piece A3;
        public Piece B1;
        public Piece B2;
        public Piece B3;
        public Piece mA1;
        public Piece mA2;
        public Piece mA3;
        public Piece mB1;
        public Piece mB2;
        public Piece mB3;

        public MainPage()
        {
            this.InitializeComponent();

            A1 = new Piece() { Circle = eA1 };
            A2 = new Piece() { Circle = eA2 };
            A3 = new Piece() { Circle = eA3 };
            B1 = new Piece() { Circle = eB1, IsHuman = true };
            B2 = new Piece() { Circle = eB2, IsHuman = true };
            B3 = new Piece() { Circle = eB3, IsHuman = true };
            mA1 = new Piece() { Circle = emA1 };
            mA2 = new Piece() { Circle = emA2 };
            mA3 = new Piece() { Circle = emA3 };
            mB1 = new Piece() { Circle = emB1, IsHuman = true };
            mB2 = new Piece() { Circle = emB2, IsHuman = true };
            mB3 = new Piece() { Circle = emB3, IsHuman = true };
        }

        private void SetBoardCoordinates()
        {
            DoOnce = true;

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

            foreach (var Row in BoardState)
            {
                foreach (var C in Row)
                {
                    if (C.Piece != null)
                    {
                        C.Piece.Margin = new Thickness(C.X, C.Y, 0, 0);
                    }
                }
            }
        }

        private void SetMatchboxCoordinates()
        {
            mDoOnce = true;

            MatchboxState = new List<List<Coord>>() { new List<Coord>() { new Coord() { C = 1, R = 1 }, new Coord() { C = 2, R = 1 }, new Coord() { C = 3, R = 1 } },
                                                      new List<Coord>() { new Coord() { C = 1, R = 2 }, new Coord() { C = 2, R = 2 }, new Coord() { C = 3, R = 2 } },
                                                      new List<Coord>() { new Coord() { C = 1, R = 3 }, new Coord() { C = 2, R = 3 }, new Coord() { C = 3, R = 3 } }
            };

            mA1.Coordinates = MatchboxState[0][0];
            mA2.Coordinates = MatchboxState[0][1];
            mA3.Coordinates = MatchboxState[0][2];
            mB1.Coordinates = MatchboxState[2][0];
            mB2.Coordinates = MatchboxState[2][1];
            mB3.Coordinates = MatchboxState[2][2];
        }

        private void UpdateBoardCoordinates()
        {
            if (!DoOnce)
            {
                ResetBoard();
            }

            /*
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

            BoardState[0][0].X = (int)(((W / 3) - PieceSize) / 2);
            BoardState[0][0].Y = (int)(((H / 3) - PieceSize) / 2);
            BoardState[0][1].X = (int)((W / 3) + (((W / 3) - PieceSize) / 2));
            BoardState[0][1].Y = (int)(((H / 3) - PieceSize) / 2);
            BoardState[0][2].X = (int)(((W / 3) * 2) + (((W / 3) - PieceSize) / 2));
            BoardState[0][2].Y = (int)(((H / 3) - PieceSize) / 2);
            BoardState[1][0].X = (int)(((W / 3) - PieceSize) / 2);
            BoardState[1][0].Y = (int)((H / 3) + (((H / 3) - PieceSize) / 2));
            BoardState[1][1].X = (int)((W / 3) + (((W / 3) - PieceSize) / 2));
            BoardState[1][1].Y = (int)((H / 3) + (((H / 3) - PieceSize) / 2));
            BoardState[1][2].X = (int)(((W / 3) * 2) + (((W / 3) - PieceSize) / 2));
            BoardState[1][2].Y = (int)((H / 3) + (((H / 3) - PieceSize) / 2));
            BoardState[2][0].X = (int)(((W / 3) - PieceSize) / 2);
            BoardState[2][0].Y = (int)(((H / 3) * 2) + (((H / 3) - PieceSize) / 2));
            BoardState[2][1].X = (int)((W / 3) + (((W / 3) - PieceSize) / 2));
            BoardState[2][1].Y = (int)(((H / 3) * 2) + (((H / 3) - PieceSize) / 2));
            BoardState[2][2].X = (int)(((W / 3) * 2) + (((W / 3) - PieceSize) / 2));
            BoardState[2][2].Y = (int)(((H / 3) * 2) + (((H / 3) - PieceSize) / 2));
            */
            
        }

        private void UpdateMatchboxCoordinates()
        {
            if (!mDoOnce)
            {
                ResetMatchbox();
            }
            var W = PageGrid.ColumnDefinitions[0].Width.Value;
            var H = W;

            int PieceSize = (int)(W / 6);
            mA1.Height = PieceSize;
            mA1.Width = PieceSize;
            mA2.Height = PieceSize;
            mA2.Width = PieceSize;
            mA3.Height = PieceSize;
            mA3.Width = PieceSize;
            mB1.Height = PieceSize;
            mB1.Width = PieceSize;
            mB2.Height = PieceSize;
            mB2.Width = PieceSize;
            mB3.Height = PieceSize;
            mB3.Width = PieceSize;

            MatchboxState[0][0].X = (int)(((W / 3) - PieceSize) / 2);
            MatchboxState[0][0].Y = (int)(((H / 3) - PieceSize) / 2);
            MatchboxState[0][1].X = (int)((W / 3) + (((W / 3) - PieceSize) / 2));
            MatchboxState[0][1].Y = (int)(((H / 3) - PieceSize) / 2);
            MatchboxState[0][2].X = (int)(((W / 3) * 2) + (((W / 3) - PieceSize) / 2));
            MatchboxState[0][2].Y = (int)(((H / 3) - PieceSize) / 2);
            MatchboxState[1][0].X = (int)(((W / 3) - PieceSize) / 2);
            MatchboxState[1][0].Y = (int)((H / 3) + (((H / 3) - PieceSize) / 2));
            MatchboxState[1][1].X = (int)((W / 3) + (((W / 3) - PieceSize) / 2));
            MatchboxState[1][1].Y = (int)((H / 3) + (((H / 3) - PieceSize) / 2));
            MatchboxState[1][2].X = (int)(((W / 3) * 2) + (((W / 3) - PieceSize) / 2));
            MatchboxState[1][2].Y = (int)((H / 3) + (((H / 3) - PieceSize) / 2));
            MatchboxState[2][0].X = (int)(((W / 3) - PieceSize) / 2);
            MatchboxState[2][0].Y = (int)(((H / 3) * 2) + (((H / 3) - PieceSize) / 2));
            MatchboxState[2][1].X = (int)((W / 3) + (((W / 3) - PieceSize) / 2));
            MatchboxState[2][1].Y = (int)(((H / 3) * 2) + (((H / 3) - PieceSize) / 2));
            MatchboxState[2][2].X = (int)(((W / 3) * 2) + (((W / 3) - PieceSize) / 2));
            MatchboxState[2][2].Y = (int)(((H / 3) * 2) + (((H / 3) - PieceSize) / 2));

            foreach (var Row in MatchboxState)
            {
                foreach (var C in Row)
                {
                    if (C.Piece != null)
                    {
                        C.Piece.Margin = new Thickness(C.X, C.Y, 0, 0);
                    }
                }
            }
            FindMoves();
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

        private void ResetMatchbox()
        {
            SetMatchboxCoordinates();

            GetMCoord(1, 1).SetPiece(mA1);
            GetMCoord(2, 1).SetPiece(mA2);
            GetMCoord(3, 1).SetPiece(mA3);
            GetMCoord(1, 3).SetPiece(mB1);
            GetMCoord(2, 3).SetPiece(mB2);
            GetMCoord(3, 3).SetPiece(mB3);

            UpdateMatchboxCoordinates();
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
            Status = "Starting";
            MovePlayer();
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
                        }
                    }
                }
            }
        }

        public class Coord
        {
            public int C;
            public int R;
            public int X;
            public int Y;
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
                Piece.Circle.Margin = new Thickness(X, Y, 0, 0);
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

        }

        public class Move
        {
            public Coord Start;
            public Coord End;
            public Polygon Arrow;

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
                Piece.Circle.Margin = new Thickness(Piece.Coordinates.X, Piece.Coordinates.Y, 0, 0);
                End.Piece = Piece;
                Start.Piece = null;
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetBoard();
            ResetMatchbox();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var A = (this.Parent as Frame).Parent as Border;
            if (!DoOnce)
            {
                UpdateBoardCoordinates();
                UpdateMatchboxCoordinates();
                W = A.Width - PageGrid.ColumnDefinitions[0].Width.Value;
                H = A.Height;
            }
            else
            {
                var NW = A.Width - PageGrid.ColumnDefinitions[0].Width.Value;
                var NH = A.Height;
                var SW = NW / W;
                var SH = NH / H;
                float Scale = SW <= SH ? (float)SW : (float)SH;
                Piece[] Pieces = new Piece[] { A1, A2, A3, B1, B2, B3 };
                foreach (var P in Pieces)
                {
                    P.Circle.Scale = new System.Numerics.Vector3(Scale, Scale, 0);
                }
                foreach (Move M in Moves)
                {
                    M.Arrow.Scale = new System.Numerics.Vector3(Scale, Scale, 0);
                }
            }
        }

        private void Grid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Status = Window.Current.CoreWindow.PointerPosition.X + ":" + Window.Current.CoreWindow.PointerPosition.Y;
        }

        private void Move_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PlayersTurn = !PlayersTurn;
            Status = (sender as Polygon).Name;
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
