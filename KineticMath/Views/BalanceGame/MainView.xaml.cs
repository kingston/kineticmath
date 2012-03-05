using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Kinect;
using KineticMath.Messaging;
using KineticMath.Kinect.Gestures;
using KineticMath.SubControls;
using System.Windows.Media.Animation;

using KineticMath.Kinect.PointConverters;
using KineticMath.Controllers;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace KineticMath.Views
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainView : BaseView, IView
    {
        public static Color SELECTED_COLOR = Colors.Orange;
        public static Color DESELECTED_COLOR = Colors.Yellow; //Color.FromRgb(0xE2, 0x51, 0x51);

        private BalanceGame game;
        private BodyRelativePointConverter bodyConverter;
        public static int labelDisplayTime = 2000;
        private List<Storyboard> runningAnimations = new List<Storyboard>();
        private bool selectingBall = false;
        public MainView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainView_Loaded);
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeGameController();
        }

        private void InitializeGameController()
        {
            game = new BalanceGame();
            game.HeldBalls.CollectionChanged += new NotifyCollectionChangedEventHandler(HeldBalls_CollectionChanged);
            game.LevelReset += new EventHandler(game_LevelReset);
            game.LevelCompleted += new EventHandler(game_LevelCompleted);
            game.LevelLost += new EventHandler(game_LevelLost);
            game.UpdateView += new EventHandler(timerCallback);
            seesaw.RegisterGame(game);
            modeLabel.Content = "Challenge Mode";
            game.NewGame();
        }

        void timerCallback(object sender, EventArgs e)
        {
            BalanceGame bg = (BalanceGame) sender;
            scoreText.Content = bg.Score;
            timeText.Content = 60- bg.Counter;
            
            if (60 - bg.Counter == 0)
            {
                modeLabel.Content = "Time's up!";
                statusLabel.Content = "You scored " + bg.Score + " points!";

                finalScore.Content = "Score: " + bg.Score;
                playAgain.BeginAnimation(UIElement.OpacityProperty, null);
                finalScore.BeginAnimation(UIElement.OpacityProperty, null);
                playAgain.Opacity = 1;
                finalScore.Opacity = 1;
                // Hide it when we're done
                DoubleAnimation labelAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(4)));
                labelAnimation.BeginTime = TimeSpan.FromSeconds(0);
                DoubleAnimation imageAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(4)));
                imageAnimation.BeginTime = TimeSpan.FromSeconds(0);
                Storyboard.SetTarget(labelAnimation, playAgain);
                Storyboard.SetTargetProperty(labelAnimation, new PropertyPath(UIElement.OpacityProperty));
                Storyboard.SetTarget(imageAnimation, finalScore);
                Storyboard.SetTargetProperty(imageAnimation, new PropertyPath(UIElement.OpacityProperty));
                Storyboard labelSb = new Storyboard();
                labelSb.Children.Add(labelAnimation);
                labelSb.Children.Add(imageAnimation);
                

                labelSb.Completed += delegate {
                    modeLabel.Content = "Challenge Mode";
                    game.NewGame();
                };
                labelSb.Begin();
            }
        }


        void game_LevelLost(object sender, EventArgs e)
        {
            uxLoseLabel.BeginAnimation(UIElement.OpacityProperty, null); // reset animation
            uxLoseLabel.Opacity = 1;
            // Hide it when we're done
            DoubleAnimation labelAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(labelDisplayTime)));
            labelAnimation.BeginTime = TimeSpan.FromSeconds(1);
            Storyboard.SetTarget(labelAnimation, uxLoseLabel);
            Storyboard.SetTargetProperty(labelAnimation, new PropertyPath(UIElement.OpacityProperty));
            Storyboard labelSb = new Storyboard();
            labelSb.Children.Add(labelAnimation);
            labelSb.Completed += delegate 
            {
                game.Reset();
                game.LoadCurrentLevel(); 
            };
            labelSb.Begin();
        }

        void game_LevelReset(object sender, EventArgs e)
        {
            foreach (var animation in runningAnimations)
            {
                animation.Stop();
            }
            runningAnimations.Clear();
        }

        void game_LevelCompleted(object sender, EventArgs e)
        {
            uxWinLabel.BeginAnimation(UIElement.OpacityProperty, null); // reset animation
            uxWinLabel.Opacity = 1;

            // Hide it when we're done
            DoubleAnimation labelAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(labelDisplayTime)));
            labelAnimation.BeginTime = TimeSpan.FromSeconds(1.75);
            Storyboard.SetTarget(labelAnimation, uxWinLabel);
            Storyboard.SetTargetProperty(labelAnimation, new PropertyPath(UIElement.OpacityProperty));
            Storyboard labelSb = new Storyboard();
            // TODO2: Start level once animation is over
            labelSb.Children.Add(labelAnimation);
            labelSb.Completed += delegate
            {
                game.LoadCurrentLevel();
            };
            labelSb.Begin();
        }

        private void HeldBalls_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<Ball> col = (ObservableCollection<Ball>)sender;
            SetupBallHolders(col.Count);
            for (int i = 0; i < col.Count; i++)
            {
                if (col[i] == null) {
                    BallHolders[i].Children.Clear();
                } else if (BallHolders[i].Children.Count == 0) {
                    BallHolders[i].Children.Add(col[i]);
                    Canvas.SetLeft(col[i], 0);
                    Canvas.SetTop(col[i], 0);
                }
            }
        }

        // TODO2: Use an actual ball holder UI control, not a canvas
        private Canvas[] BallHolders;

        private void SetupBallHolders(int numHolders)
        {
            if (BallHolders == null || BallHolders.Length != numHolders)
            {
                // Remove all the ball holders
                if (BallHolders != null)
                {
                    foreach (var holder in BallHolders)
                    {
                        holder.Children.Clear();
                        uxMainCanvas.Children.Remove(holder);
                    }
                }
                BallHolders = new Canvas[numHolders];
                // Points relative to the uxPersonCanvas space
                Point[] holderPositions = new Point[] {
                    new Point(0.1, 0.3),
                    new Point(0.3, 0.2),
                    new Point(0.7, 0.2),
                    new Point(0.9, 0.3)
                };
                if (numHolders > holderPositions.Length) throw new InvalidOperationException("You must define the locations of all holders");
                for (int i = 0; i < numHolders; i++)
                {
                    Canvas canvas = new Canvas();
                    canvas.Width = 50;
                    canvas.Height = 50;
                    uxMainCanvas.Children.Add(canvas);
                    // -25 to center it
                    Canvas.SetLeft(canvas, holderPositions[i].X * uxPersonRectangle.ActualWidth + Canvas.GetLeft(uxPersonRectangle) - 25);
                    Canvas.SetTop(canvas, holderPositions[i].Y * uxPersonRectangle.ActualHeight + Canvas.GetTop(uxPersonRectangle) - 25);
                    BallHolders[i] = canvas;
                }
            }
        }

        private void HandleKeyDownEvent(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D1:
                    modeLabel.Content = "Challenge Mode";
                    ChallengeModeGUI.Visibility = System.Windows.Visibility.Visible;
                    game.setMode(BalanceGame.Mode.Challenge);
                    game.NewGame();
                    break;
                case Key.D2:
                    modeLabel.Content = "Practice Mode";
                    ChallengeModeGUI.Visibility = System.Windows.Visibility.Hidden;
                    statusLabel.Content = "";
                    game.setMode(BalanceGame.Mode.Practice);
                    game.NewGame();
                    break;
            }
            //Console.Out.WriteLine("Keydown");
            //switch (e.Key)
            //{
            //    case Key.T:
            //        seesaw.AddObject(new SubControls.Ball());
            //        //Console.Out.WriteLine("ball add");

            //        break;
            //    case Key.S:
            //        break;
            //    case Key.R:
            //        Reset();
            //        break;
            //    case Key.Left:
            //        fallingGroup.ChoosePrevious();
            //        break;
            //    case Key.Right:
            //        fallingGroup.ChooseNext();
            //        break;
            //}
        }

        public override void OnViewActivated()
        {
            base.OnViewActivated();
            RegisterGestures();
            ParentWindow.AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandleKeyDownEvent);
        }

        public override void OnViewDeactivated()
        {
            base.OnViewDeactivated();
            ParentWindow.RemoveHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandleKeyDownEvent);
        }

        private void RegisterGestures()
        {
            bodyConverter = new BodyRelativePointConverter(GetBoundingRectangle(uxPersonRectangle), this._sharedData.GestureController);

            JointMoveGestures handGestures = new JointMoveGestures(JointType.HandLeft, JointType.HandRight, JointType.Head);
            handGestures.JointMoved += new EventHandler<JointMovedEventArgs>(handGesture_JointMoved);
            _sharedData.GestureController.AddGesture(this, handGestures);

            HandPushGesture handPushGesture = new HandPushGesture();
            handPushGesture.HandPushed += new EventHandler<HandPushedEventArgs>(handPushGesture_HandPushed);
            _sharedData.GestureController.AddGesture(this, handPushGesture);
        }

        void handGesture_JointMoved(object sender, JointMovedEventArgs e)
        {
            // Show the movement on the screen
            SkeletonPoint pt = bodyConverter.ConvertPoint(e.NewPosition);
            // TODO2: Make pretty way to reflect hand movements
            if (e.JointType == JointType.HandLeft) SetCanvasLocationCentered(uxLeftHand, pt);
            else if (e.JointType == JointType.HandRight) SetCanvasLocationCentered(uxRightHand, pt);
            else if (e.JointType == JointType.Head) SetCanvasLocationCentered(uxTester, pt);
        }

        void handPushGesture_HandPushed(object sender, HandPushedEventArgs e)
        {
            HandlePushEvent(bodyConverter.ConvertPoint(e.Position));
        }

        private void HandlePushEvent(SkeletonPoint pt)
        {
            if (selectingBall) return;
            Ball pushedBall = null;
            foreach (var holder in BallHolders)
            {
                Rect rect = GetBoundingRectangle(holder);
                if (rect.Contains(ConvertSkeletonPointTo2DPoint(pt)))
                {
                    if (holder.Children.Count > 0)
                    {
                        pushedBall = (Ball)holder.Children[0];
                    }
                }
            }
            if (pushedBall != null)
            {
                int index = game.HeldBalls.IndexOf(pushedBall);
                if (game.PushBall(pushedBall))
                {
                    this.BallHolders[index].Children.Add(pushedBall);
                    // TODO2: Trigger animation for ball and after animation is triggered

                    DoubleAnimationUsingPath ballAnimationX = new DoubleAnimationUsingPath();
                    DoubleAnimationUsingPath ballAnimationY = new DoubleAnimationUsingPath();

                    PathGeometry animationPath = new PathGeometry();
                    PathFigure pFigure = new PathFigure();
                    pFigure.StartPoint = new Point(10, 100);
                    //PathFigureCollection pfc = FindResource("RectanglePathFigureCollection") as PathFigureCollection;

                    pFigure.Segments.Add(getCurve(index));
                    animationPath.Figures.Add(pFigure);
                    // Freeze the PathGeometry for performance benefits.
                    animationPath.Freeze();

                    /*
                    animationPath.Figures = pfc;*/
                    ballAnimationX.PathGeometry = animationPath;
                    ballAnimationX.BeginTime = TimeSpan.FromSeconds(0);
                    ballAnimationX.AutoReverse = false;
                    ballAnimationY.PathGeometry = animationPath;
                    ballAnimationY.AutoReverse = false;
                    ballAnimationY.BeginTime = TimeSpan.FromSeconds(0);
                    Storyboard.SetTarget(ballAnimationX, pushedBall);
                    Storyboard.SetTarget(ballAnimationY, pushedBall);
                    Storyboard.SetTargetProperty(ballAnimationX, new PropertyPath("(Canvas.Left)"));
                    Storyboard.SetTargetProperty(ballAnimationY, new PropertyPath("(Canvas.Top)"));
                    Storyboard ballMove = new Storyboard();
                    ballMove.Children.Add(ballAnimationX);
                    ballMove.Children.Add(ballAnimationY);

                    ballMove.Completed += delegate
                    {
                        selectingBall = false;
                        this.BallHolders[index].Children.Remove(pushedBall);
                        runningAnimations.Remove(ballMove);
                        game.AddBallToBalance(pushedBall, true); // push ball to left side
                    };
                    selectingBall = true;
                    ballMove.Begin();
                    // TODO2: Trigger animation for ball and after animation is triggered
                }
            }
        }

        private PolyBezierSegment getCurve(int index) {
            PolyBezierSegment pBezierSegment = new PolyBezierSegment();
            switch (index)
            {
                case 0:
                    pBezierSegment.Points.Add(new Point(15, 0));
                    pBezierSegment.Points.Add(new Point(105, 0));
                    pBezierSegment.Points.Add(new Point(130, 100));
                    pBezierSegment.Points.Add(new Point(150, 190));
                    pBezierSegment.Points.Add(new Point(225, 200));
                    pBezierSegment.Points.Add(new Point(260, 100));
                    break;
                case 1:
                    pBezierSegment.Points.Add(new Point(15, 0));
                    pBezierSegment.Points.Add(new Point(105, 0));
                    pBezierSegment.Points.Add(new Point(130, 100));
                    pBezierSegment.Points.Add(new Point(150, 190));
                    pBezierSegment.Points.Add(new Point(225, 200));
                    pBezierSegment.Points.Add(new Point(260, 100));
                    break;
                case 2:
                    pBezierSegment.Points.Add(new Point(15, 0));
                    pBezierSegment.Points.Add(new Point(105, 0));
                    pBezierSegment.Points.Add(new Point(130, 100));
                    pBezierSegment.Points.Add(new Point(150, 190));
                    pBezierSegment.Points.Add(new Point(180, 180));
                    pBezierSegment.Points.Add(new Point(200, 180));
                    break;
                case 3:
                    pBezierSegment.Points.Add(new Point(15, 0));
                    pBezierSegment.Points.Add(new Point(105, 0));
                    pBezierSegment.Points.Add(new Point(130, 100));
                    pBezierSegment.Points.Add(new Point(150, 190));
                    pBezierSegment.Points.Add(new Point(160, 160));
                    pBezierSegment.Points.Add(new Point(200, 180));
                    break;
                default:
                    System.Console.Write("4");
                     pBezierSegment.Points.Add(new Point(15, 0));
                    pBezierSegment.Points.Add(new Point(105, 0));
                    pBezierSegment.Points.Add(new Point(130, 100));
                    pBezierSegment.Points.Add(new Point(130, 130));
                   pBezierSegment.Points.Add(new Point(130, 130));
                    break;
            }

            return pBezierSegment;
        }

        private void SetCanvasLocationCentered(FrameworkElement element, SkeletonPoint pt)
        {
            Canvas.SetLeft(element, pt.X - element.ActualWidth / 2);
            Canvas.SetTop(element, pt.Y - element.ActualHeight / 2);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Rect rect = GetBoundingRectangle(uxPersonRectangle);
            Point pt = e.GetPosition(uxMainCanvas);
            if (rect.Contains(e.GetPosition(uxMainCanvas))) {
                SkeletonPoint skelPt = new SkeletonPoint() { X = (float) pt.X, Y = (float) pt.Y, Z = 0 };
                HandlePushEvent(skelPt);
            }
        }

        // TODO3: Extract out to extension methods
        private Point ConvertSkeletonPointTo2DPoint(SkeletonPoint pt)
        {
            return new Point(pt.X, pt.Y);
        }

        /// <summary>
        /// Gets the bounding rectangle of an element given the canvas
        /// 
        /// TODO3: Extract out somewhere else
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private Rect GetBoundingRectangle(FrameworkElement element)
        {
            return new Rect(Canvas.GetLeft(element), Canvas.GetTop(element), element.Width, element.Height);
        }

        /*** EVERYTHING BELOW HERE IS OLD CODE ***/


       

        //void selectItem()
        //{
        //    Ball b = fallingGroup.RemoveSelected();
        //    if(b != null)
        //        seesaw.AddObject(b);

        //    if (seesaw.checkAnswer())
        //    {
        //        RoundComplete();
        //    }
        //    else {
        //        PromptIfGetWrong();
        //    }
        //}

        //void RoundComplete()
        //{
        //    uxWinLabel.BeginAnimation(UIElement.OpacityProperty, null); // reset animation
        //    uxWinLabel.Opacity = 1;

        //    levelsCompleted++;
        //    //difficulty = levelsCompleted / 3 + 1;

        //    // Hide it when we're done
        //    DoubleAnimation labelAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(1000)));
        //    labelAnimation.BeginTime = TimeSpan.FromSeconds(1);
        //    Storyboard.SetTarget(labelAnimation, uxWinLabel);
        //    Storyboard.SetTargetProperty(labelAnimation, new PropertyPath(UIElement.OpacityProperty));
        //    Storyboard labelSb = new Storyboard();
        //    labelSb.Children.Add(labelAnimation);
        //    labelAnimation.Completed += new EventHandler(NewRound);
        //    labelSb.Begin();
        //}

        //void PromptIfGetWrong()
        //{
        //    uxLoseLabel.BeginAnimation(UIElement.OpacityProperty, null); // reset animation
        //    uxLoseLabel.Opacity = 1;
        //    // Hide it when we're done
        //    DoubleAnimation labelAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(1000)));
        //    labelAnimation.BeginTime = TimeSpan.FromSeconds(0);
        //    Storyboard.SetTarget(labelAnimation, uxLoseLabel);
        //    Storyboard.SetTargetProperty(labelAnimation, new PropertyPath(UIElement.OpacityProperty));
        //    Storyboard labelSb = new Storyboard();
        //    labelSb.Children.Add(labelAnimation);
        //    labelAnimation.Completed += new EventHandler(ResetWrong);
        //    labelSb.Begin();
        //}

        //void ClearBalls()
        //{
        //    fallingGroup.RemoveAllBalls();
        //    seesaw.RemoveAllObjects();
        //}

        //void Mover_move(object sender, MoveEventArgs m)
        //{
        //    if (m.GetDirection() == 1)
        //    {
        //        System.Console.WriteLine("Move Right");
        //        fallingGroup.ChooseNext();
        //    }
        //    else
        //    {
        //        System.Console.WriteLine("Move Left");
        //        fallingGroup.ChoosePrevious();
        //    }
        //}


        //private List<int> lhs, rhs;
        //private int answer;

        //private void Setup()
        //{
        //    lhs = new List<int>();
        //    rhs = new List<int>();
        //    answer = GenerateQuestion(lhs, rhs);
        //    SetupBalls();
        //}

        //private void SetupBalls()
        //{
        //    for (int i = 0; i < rhs.Count; i++) {
        //        seesaw.AddObject(new Brick(rhs[i].ToString(), rhs[i]), false);
        //    }
        //    fallingGroup.addBall(lhs);
        //}

        //private int GenerateQuestion(List<int> lhsArray, List<int> rhsArray)
        //{
        //    Random rand = new Random();
        //    // Generate the answer options
        //    while (lhsArray.Count < NUM_WEIGHTS)
        //    {
        //        int candidate = rand.Next(3, difficulty * 5 + 7);
        //        if (!lhsArray.Contains(candidate))
        //        {
        //            lhsArray.Add(candidate);
        //        }
        //    }
        //    // Pick one to be the correct answer
        //    int answer = lhsArray[rand.Next(0, NUM_WEIGHTS - 1)];
        //    // Generate the question
        //    int sum = 0;
        //    int maxParts = Math.Min(difficulty + 1, MAX_NUMBERS_TO_ADD);
        //    while (sum < answer && rhsArray.Count < maxParts - 1)
        //    {
        //        int part = rand.Next(1, answer - sum);
        //        rhsArray.Add(part);
        //        sum += part;
        //    }
        //    if (sum < answer)
        //    {
        //        rhsArray.Add(answer - sum);
        //    }
        //    return answer;
        //}
    }
}
