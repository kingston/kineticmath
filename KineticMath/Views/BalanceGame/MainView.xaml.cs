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
using System.Windows.Threading;

using Microsoft.Kinect;
using KineticMath.Messaging;
using KineticMath.Kinect.Gestures;
using KineticMath.SubControls;
using KineticMath.Helpers;
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
        private static TimeSpan RESET_DELAY = TimeSpan.FromSeconds(0.5);
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
            game.TimerTicked += new EventHandler(timerCallback);
            seesaw.RegisterGame(game);
            modeLabel.Content = "Challenge Mode";
            game.NewGame();
        }

        void timerCallback(object sender, EventArgs e)
        {
            BalanceGame bg = (BalanceGame) sender;
            statusLabel.Content = "Score: " + bg.Score + " Remaining: " + bg.TimeLeft;
            if (bg.TimeLeft == 0)
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
            labelSb.Begin();

            // Reset
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = RESET_DELAY;
            timer.Tick += delegate
            {
                game.Reset();
                timer.Stop();
            };
            timer.Start();
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
            soundEffect.Play();
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
                soundEffect.Stop();
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
        private PointCanvas[] BallHolders;
        private const int HIT_ROUGHNESS = 10; // The amount of rough distance they can hit in between to make it easier to hit

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
                BallHolders = new PointCanvas[numHolders];
                // Points relative to the uxPersonCanvas space
                Point[] holderPositions = new Point[] {
                    new Point(0.1, 0.3),
                    new Point(0.3, 0.2),
                    new Point(0.7, 0.2),
                    new Point(0.9, 0.3)
                };
                if (numHolders > holderPositions.Length) throw new InvalidOperationException("You must define the locations of all holders");
                _hitZones.Clear();
                for (int i = 0; i < numHolders; i++)
                {
                    PointCanvas canvas = new PointCanvas();
                    canvas.Width = 80;
                    canvas.Height = 80;
                    uxMainCanvas.Children.Add(canvas);
                    PointCanvas.SetTopLeft(canvas, new Point(
                        holderPositions[i].X * uxPersonRectangle.ActualWidth + Canvas.GetLeft(uxPersonRectangle),
                        holderPositions[i].Y * uxPersonRectangle.ActualHeight + Canvas.GetTop(uxPersonRectangle))
                    );
                    BallHolders[i] = canvas;
                    Rect boundaryRect = canvas.GetBoundaryRect();
                    //boundaryRect.Inflate(HIT_ROUGHNESS, HIT_ROUGHNESS);
                    _hitZones.Add(boundaryRect);
                }
                if (hitGesture != null)
                {
                    hitGesture.HitRectangles.Clear();
                    hitGesture.HitRectangles.AddRange(_hitZones);
                }
            }
        }

        private void HandleKeyDownEvent(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D1:
                    modeLabel.Content = "Challenge Mode";
                    game.setMode(BalanceGame.Mode.Challenge);
                    game.NewGame();
                    break;
                case Key.D2:
                    modeLabel.Content = "Practice Mode";
                    statusLabel.Content = "";
                    game.setMode(BalanceGame.Mode.Practice);
                    game.NewGame();
                    break;
            }
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

        private List<Rect> _hitZones = new List<Rect>(); // Zones for the hit gesture
        private HitGesture hitGesture;

        private void RegisterGestures()
        {
            bodyConverter = new BodyRelativePointConverter(uxPersonRectangle.GetBoundaryRect(), this._sharedData.GestureController);

            JointMoveGestures handGestures = new JointMoveGestures(JointType.HandLeft, JointType.HandRight, JointType.Head);
            handGestures.JointMoved += new EventHandler<JointMovedEventArgs>(handGesture_JointMoved);
            _sharedData.GestureController.AddGesture(this, handGestures);

            // TODO: Remove dead code (hand push gesture no longer active)
            //HandPushGesture handPushGesture = new HandPushGesture();
            //handPushGesture.HandPushed += new EventHandler<HandPushedEventArgs>(handPushGesture_HandPushed);
            //_sharedData.GestureController.AddGesture(this, handPushGesture);
            if (hitGesture == null)
            {
                hitGesture = new HitGesture(_hitZones, bodyConverter, JointType.HandRight, JointType.HandLeft);
                hitGesture.RectHit += new EventHandler<RectHitEventArgs>(hitGesture_RectHit);
            }
            _sharedData.GestureController.AddGesture(this, hitGesture);
        }

        void hitGesture_RectHit(object sender, RectHitEventArgs e)
        {
            HitBall(e.RectIdx, e.HitVelocity);
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

        private void HitBall(int index, Vector velocity)
        {
            var pushedBall = game.HeldBalls[index];
            if (game.PushBall(pushedBall))
            {
                var ballHolder = this.BallHolders[index];
                ballHolder.Children.Add(pushedBall);
                // TODO2: Trigger animation for ball and after animation is triggered

                PointAnimationUsingPath ballAnimation = new PointAnimationUsingPath();

                PathGeometry animationPath = new PathGeometry();
                PathFigure pFigure = new PathFigure();
                pFigure.StartPoint = PointCanvas.GetTopLeft(ballHolder);
                //PathFigureCollection pfc = FindResource("RectanglePathFigureCollection") as PathFigureCollection;

                pFigure.Segments.Add(getCurve(index));
                animationPath.Figures.Add(pFigure);
                // Freeze the PathGeometry for performance benefits.
                animationPath.Freeze();

                /*
                animationPath.Figures = pfc;*/
                ballAnimation.PathGeometry = animationPath;
                ballAnimation.BeginTime = TimeSpan.FromSeconds(0);
                ballAnimation.AutoReverse = false;

                Storyboard.SetTarget(ballAnimation, ballHolder);
                Storyboard.SetTargetProperty(ballAnimation, new PropertyPath("(TopLeft)"));
                Storyboard ballMove = new Storyboard();
                ballMove.Children.Add(ballAnimation);

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

        private void HandlePushEvent(SkeletonPoint pt)
        {
            if (selectingBall) return;
            Ball pushedBall = null;
            PointCanvas ballHolder = null;
            foreach (var holder in BallHolders)
            {
                Rect rect = holder.GetBoundaryRect();
                if (rect.Contains(pt.To2DPoint()))
                {
                    if (holder.Children.Count > 0)
                    {
                        pushedBall = (Ball)holder.Children[0];
                        ballHolder = holder;
                    }
                }
            }
            if (pushedBall != null)
            {
                int index = game.HeldBalls.IndexOf(pushedBall);
                HitBall(index, new Vector(0, 0));
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
                    break;
                case 1:
                    pBezierSegment.Points.Add(new Point(15, 0));
                    pBezierSegment.Points.Add(new Point(105, 0));
                    pBezierSegment.Points.Add(new Point(130, 100));
                    pBezierSegment.Points.Add(new Point(150, 190));
                    pBezierSegment.Points.Add(new Point(225, 200));
                    break;
                case 2:
                    pBezierSegment.Points.Add(new Point(15, 0));
                    pBezierSegment.Points.Add(new Point(105, 0));
                    pBezierSegment.Points.Add(new Point(130, 100));
                    pBezierSegment.Points.Add(new Point(150, 190));
                    pBezierSegment.Points.Add(new Point(180, 180));
                    break;
                case 3:
                    pBezierSegment.Points.Add(new Point(15, 0));
                    pBezierSegment.Points.Add(new Point(105, 0));
                    pBezierSegment.Points.Add(new Point(130, 100));
                    pBezierSegment.Points.Add(new Point(150, 190));
                    pBezierSegment.Points.Add(new Point(160, 160));
                    break;
                default:
                    pBezierSegment.Points.Add(new Point(15, 0));
                    pBezierSegment.Points.Add(new Point(105, 0));
                    pBezierSegment.Points.Add(new Point(130, 100));
                    pBezierSegment.Points.Add(new Point(130, 130));
                    break;
            }
            pBezierSegment.Points.Add(new Point(
                Canvas.GetLeft(seesaw) + Canvas.GetLeft(seesaw.uxBalanceCanvas) + Canvas.GetLeft(seesaw.leftBallPanel),
                Canvas.GetTop(seesaw) + Canvas.GetTop(seesaw.uxBalanceCanvas) + Canvas.GetTop(seesaw.leftBallPanel))
            ); 
            return pBezierSegment;
        }

        private void SetCanvasLocationCentered(FrameworkElement element, SkeletonPoint pt)
        {
            Canvas.SetLeft(element, pt.X - element.ActualWidth / 2);
            Canvas.SetTop(element, pt.Y - element.ActualHeight / 2);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Rect rect = uxPersonRectangle.GetBoundaryRect();
            Point pt = e.GetPosition(uxMainCanvas);
            if (rect.Contains(e.GetPosition(uxMainCanvas))) {
                SkeletonPoint skelPt = new SkeletonPoint() { X = (float) pt.X, Y = (float) pt.Y, Z = 0 };
                HandlePushEvent(skelPt);
            }
        }
    }
}