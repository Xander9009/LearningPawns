using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
        static public List<Move> Moves = new List<Move>();
        static public List<MoveState> LosingMoves = new List<MoveState>();
        static public MoveState PreviousMoveState;
        public int PlayerWins = 0;
        public int CPUWins = 0;
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
        public bool AutoActive = false;
        public int AutoDelay = 1000;

        private static double InitialHeight = 120;
        private static double InitialWidth = 163;

        private Move LastPlayerMove;
        private Move LastCPUMove;

        static public Piece[] Pieces;
        static public Piece A1;
        static public Piece A2;
        static public Piece A3;
        static public Piece B1;
        static public Piece B2;
        static public Piece B3;

        public MainPage()
        {
            this.InitializeComponent();

            A1 = new Piece() { Circle = eA1, IsHuman = true };
            A2 = new Piece() { Circle = eA2, IsHuman = true };
            A3 = new Piece() { Circle = eA3, IsHuman = true };
            B1 = new Piece() { Circle = eB1 };
            B2 = new Piece() { Circle = eB2 };
            B3 = new Piece() { Circle = eB3 };

            Pieces = new Piece[] { A1, A2, A3, B1, B2, B3 };
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateBoardCoordinates();
            GameBoard = Board;

            var A = (this.Parent as Frame).Parent as Border;
            var CurrentWidth = (A.Width - PageGrid.ColumnDefinitions[0].Width.Value) / 6;
            var CurrentHeight = A.Height / 6;
            float Scale = CurrentWidth / InitialWidth <= CurrentHeight / InitialHeight ? (float)(CurrentWidth / InitialWidth) : (float)(CurrentHeight / InitialHeight);
            foreach (var P in new Piece[] { A1, A2, A3, B1, B2, B3 })
            {
                P.Refresh(Scale * InitialHeight);
            }

            BoardState = new List<List<Coord>>() { new List<Coord>() { new Coord() { C = 1, R = 1 }, new Coord() { C = 2, R = 1 }, new Coord() { C = 3, R = 1 } },
                                                   new List<Coord>() { new Coord() { C = 1, R = 2 }, new Coord() { C = 2, R = 2 }, new Coord() { C = 3, R = 2 } },
                                                   new List<Coord>() { new Coord() { C = 1, R = 3 }, new Coord() { C = 2, R = 3 }, new Coord() { C = 3, R = 3 } }
            };

            ResetBoard();

            foreach (var Elem in Board.Children)
            {
                if ((Elem as Polygon) != null)
                {
                    var Arrow = Elem as Polygon;
                    int[] Coords = Arrow.Name.Substring(4).ToCharArray().Select(x => int.Parse(x.ToString())).ToArray();
                    var Move = new Move() { Arrow = Arrow, Start = BoardState[Coords[1] - 1][Coords[0] - 1], End = BoardState[Coords[3] - 1][Coords[2] - 1] };
                    Moves.Add(Move);
                    Move.InitialPoints = Move.Arrow.Points;
                    Move.Refresh(CurrentWidth, CurrentHeight);
                }
            }

            GameActive = true;
            MovePlayer();
        }

        private void UpdateBoardCoordinates()
        {
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

        private void MovePlayer()
        {
            PlayersTurn = true;
            var ValidMoves = FindMoves();
            if (AutoActive && ValidMoves.Count > 0)
            {
                Move ChosenMove = ValidMoves[new Random().Next(ValidMoves.Count)];
                ChosenMove.Arrow.Fill = new SolidColorBrush(Windows.UI.Colors.Red);
                ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreateTimer((source) =>
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Dispatcher.RunAsync(CoreDispatcherPriority.High,
                        () =>
                        {
                            ChosenMove.Perform();
                            PlayersTurn = true;
                            MoveComputer();
                        });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }, TimeSpan.FromMilliseconds(AutoDelay));
            }
            else if (AutoActive)
            {
                GameActive = true;
                PlayersTurn = true;
                ResetBoard();
                ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreateTimer((source) =>
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Dispatcher.RunAsync(CoreDispatcherPriority.High,
                        () =>
                        {
                            MovePlayer();
                        });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }, TimeSpan.FromMilliseconds(AutoDelay));
            }
        }

        private void MoveComputer()
        {
            PlayersTurn = false;

            var ValidMoves = FindMoves();
            if (ValidMoves.Count > 0)
            {
                Move ChosenMove = ValidMoves[new Random().Next(ValidMoves.Count)];
                ChosenMove.Arrow.Fill = new SolidColorBrush(Windows.UI.Colors.Blue);

                ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreateTimer((source) =>
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Dispatcher.RunAsync(CoreDispatcherPriority.High,
                        () =>
                        {
                                ChosenMove.Perform();
                                PlayersTurn = true;
                                MovePlayer();
                        });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                }, TimeSpan.FromMilliseconds(AutoDelay));
            }
            else
            {
                PlayersTurn = true;
                if (AutoActive)
                {
                    GameActive = true;
                    ResetBoard();
                    ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreateTimer((source) =>
                    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        Dispatcher.RunAsync(CoreDispatcherPriority.High,
                            () =>
                            {
                                MovePlayer();
                            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    }, TimeSpan.FromMilliseconds(AutoDelay));
                }
            }
        }

        private List<Move> FindMoves()
        {
            List<Move> ValidMoves = new List<Move>();
            Piece[] lPieces;
            MoveState MS = new MoveState(Pieces);;
            if (PlayersTurn)
            {
                lPieces = new Piece[] { A1, A2, A3 };
            }
            else
            {
                lPieces = new Piece[] { B1, B2, B3 };
            }
            foreach (var Move in Moves)
            {
                Move.State = Move.States.Inactive;
            }
            if (!GameActive) { return ValidMoves; }

            foreach (var P in lPieces)
            {
                if (P.Circle.Visibility != Visibility.Visible) { continue; }
                int R = P.R + 1;
                if (!P.IsHuman)
                {
                    R = P.R - 1;
                }

                var End = GetCoord(P.C - 1, R);
                if (End != null && (End.Piece != null && End.Piece.IsHuman != P.IsHuman))
                {
                    foreach (var Move in Moves)
                    {
                        if (Move.Start == P.Coordinates && Move.End == End)
                        {
                            if (CheckMoveStateValid(MS, Move))
                            {
                                ValidMoves.Add(Move);
                                Move.State = Move.States.Active;
                            }
                        }
                    }
                }
                End = GetCoord(P.C, R);
                if (End != null && End.Piece == null)
                {
                    foreach (var Move in Moves)
                    {
                        if (Move.Start == P.Coordinates && Move.End == End)
                        {
                            if (CheckMoveStateValid(MS, Move))
                            {
                                ValidMoves.Add(Move);
                                Move.State = Move.States.Active;
                            }
                        }
                    }
                }
                End = GetCoord(P.C + 1, R);
                if (End != null && (End.Piece != null && End.Piece.IsHuman != P.IsHuman))
                {
                    foreach (var Move in Moves)
                    {
                        if (Move.Start == P.Coordinates && Move.End == End)
                        {
                            if (CheckMoveStateValid(MS, Move))
                            {
                                ValidMoves.Add(Move);
                                Move.State = Move.States.Active;
                            }
                        }
                    }
                }
            }
            if (ValidMoves.Count == 0 && GameActive)
            {
                if (PlayersTurn)
                {
                    CPUWins++;
                    //LosingMoves.Add(PreviousMoveState);
                }
                else
                {
                    PlayerWins++;
                    LosingMoves.Add(PreviousMoveState);
                }
                Status = "Player: " + PlayerWins + " -- CPU: " + CPUWins;
                GameActive = false;
            }
            return ValidMoves;
        }

        public bool CheckMoveStateValid(MoveState MS, Move M)
        {
            foreach (MoveState MoveState in LosingMoves)
            {
                if (MoveState.CompareMoveStates(MS, M))
                {
                    return false;
                }
            }
            return true;
        }

        public class Coord
        {
            public int C;
            public int R;
            public Piece Piece;

            public Coord() { }

            public Coord(Piece P)
            {
                C = P.Coordinates.C;
                R = P.Coordinates.R;
                Piece = P.Coordinates.Piece;
            }

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
                Piece.Coordinates = BoardState[R- 1][C - 1];
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
            public enum States { Inactive, Active, Chosen };
            private States state = States.Active;
            public States State
            {
                get { return state; }
                set
                {
                    if (value == States.Inactive)
                    {
                        Arrow.Fill = new SolidColorBrush(Windows.UI.Colors.Gray);
                    }
                    else if (value == States.Active)
                    {
                        Arrow.Fill = new SolidColorBrush(Windows.UI.Colors.Black);
                    }
                    else if (Start.Piece.IsHuman)
                    {
                        Arrow.Fill = new SolidColorBrush(Windows.UI.Colors.Red);
                    }
                    else
                    {
                        Arrow.Fill = new SolidColorBrush(Windows.UI.Colors.Blue);
                    }
                }
            }

            public void Perform()
            {
                //if (!Start.Piece.IsHuman)
                //{
                    PreviousMoveState = new MoveState(Pieces, this);
                //}


                State = States.Active;
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
            PlayerWins = 0;
            CPUWins = 0;
            Status = "Player: 0 -- CPU: 0";
            PreviousMoveState = null;
            LosingMoves = new List<MoveState>();
            PlayersTurn = true;
            ResetBoard();
            GameActive = true;
            MovePlayer();
        }

        private void Auto_Click(object sender, RoutedEventArgs e)
        {
            AutoActive = !AutoActive;
            if (AutoActive)
            {
                MovePlayer();
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var A = (this.Parent as Frame).Parent as Border;
            var CurrentWidth = (A.Width - PageGrid.ColumnDefinitions[0].Width.Value) / 6;
            var CurrentHeight = A.Height / 6;
            float Scale = CurrentWidth / InitialWidth <= CurrentHeight / InitialHeight ? (float)(CurrentWidth / InitialWidth) : (float)(CurrentHeight / InitialHeight);
            foreach (var P in Pieces)
            {
                P.Refresh(Scale * InitialHeight);
            }
            foreach (Move M in Moves)
            {
                M.Refresh(CurrentWidth, CurrentHeight);
            }
        }

        private void Move_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!PlayersTurn) { return; }
            string N = (sender as Polygon).Name.Substring(4);
            var Start = GetCoord(int.Parse(N.Substring(0, 1)), int.Parse(N.Substring(1, 1)));
            var End = GetCoord(int.Parse(N.Substring(2, 1)), int.Parse(N.Substring(3, 1)));
            foreach (var Move in Moves)
            {
                if (Move.Start == Start && Move.End == End)
                {
                    if (Move.State == Move.States.Inactive) { return; }
                    Move.Perform();
                    MoveComputer();
                    return;
                }
            }
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            PlayersTurn = true;
            ResetBoard();
            GameActive = true;
            MovePlayer();
        }

        public class MoveState
        {
            public Coord A1;
            public Coord A2;
            public Coord A3;
            public Coord B1;
            public Coord B2;
            public Coord B3;

            public bool A1Active;
            public bool A2Active;
            public bool A3Active;
            public bool B1Active;
            public bool B2Active;
            public bool B3Active;

            public int SC;
            public int SR;
            public int EC;
            public int ER;

            public MoveState(Piece[] Pieces, Move Move = null)
            {
                A1 = new Coord(Pieces[0]);
                A2 = new Coord(Pieces[1]);
                A3 = new Coord(Pieces[2]);
                B1 = new Coord(Pieces[3]);
                B2 = new Coord(Pieces[4]);
                B3 = new Coord(Pieces[5]);

                A1Active = Pieces[0].Circle.Visibility == Visibility.Visible;
                A2Active = Pieces[1].Circle.Visibility == Visibility.Visible;
                A3Active = Pieces[2].Circle.Visibility == Visibility.Visible;
                B1Active = Pieces[3].Circle.Visibility == Visibility.Visible;
                B2Active = Pieces[4].Circle.Visibility == Visibility.Visible;
                B3Active = Pieces[5].Circle.Visibility == Visibility.Visible;

                if (Move != null)
                {
                    SC = Move.Start.C;
                    SR = Move.Start.R;
                    EC = Move.End.C;
                    ER = Move.End.R;
                }
            }

            public bool CompareMoveStates(MoveState Move2, Move Move)
            {
                if (A1Active != Move2.A1Active || A1.C != Move2.A1.C || A1.R != Move2.A1.R)
                {
                    return false;
                }
                if (A2Active != Move2.A2Active || A2.C != Move2.A2.C || A2.R != Move2.A2.R)
                {
                    return false;
                }
                if (A3Active != Move2.A3Active || A3.C != Move2.A3.C || A3.R != Move2.A3.R)
                {
                    return false;
                }
                if (B1Active != Move2.B1Active || B1.C != Move2.B1.C || B1.R != Move2.B1.R)
                {
                    return false;
                }
                if (B2Active != Move2.B2Active || B2.C != Move2.B2.C || B2.R != Move2.B2.R)
                {
                    return false;
                }
                if (B3Active != Move2.B3Active || B3.C != Move2.B3.C || B3.R != Move2.B3.R)
                {
                    return false;
                }
                if (SC != Move.Start.C || SR != Move.Start.R)
                {
                    return false;
                }
                if (EC != Move.End.C || ER != Move.End.R)
                {
                    return false;
                }

                return true;
            }

            public bool CompareMoveStates(MoveState Move2)
            {
                if (A1Active != Move2.A1Active || A1.C != Move2.A1.C || A1.R != Move2.A1.R)
                {
                    return false;
                }
                if (A2Active != Move2.A2Active || A2.C != Move2.A2.C || A2.R != Move2.A2.R)
                {
                    return false;
                }
                if (A3Active != Move2.A3Active || A3.C != Move2.A3.C || A3.R != Move2.A3.R)
                {
                    return false;
                }
                if (B1Active != Move2.B1Active || B1.C != Move2.B1.C || B1.R != Move2.B1.R)
                {
                    return false;
                }
                if (B2Active != Move2.B2Active || B2.C != Move2.B2.C || B2.R != Move2.B2.R)
                {
                    return false;
                }
                if (B3Active != Move2.B3Active || B3.C != Move2.B3.C || B3.R != Move2.B3.R)
                {
                    return false;
                }
                if (SC != Move2.SC || SR != Move2.SR)
                {
                    return false;

                }
                if (EC != Move2.EC || ER != Move2.ER)
                {
                    return false;
                }

                return true;
            }
        }

        private void AutoDelayBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var Text = (sender as TextBox).Text;
            int NewValue;
            if (Text != "" && int.TryParse(Text, out NewValue) && NewValue >= 50)
            {
                AutoDelay = NewValue;
            }
        }
    }
}
