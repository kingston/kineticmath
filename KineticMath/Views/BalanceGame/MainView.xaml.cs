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
        public static int labelDisplayTime = 750;
        private static TimeSpan RESET_DELAY = TimeSpan.FromSeconds(0.5);
        private List<Storyboard> runningAnimations = new List<Storyboard>();
        private bool selectingBall = false;
        private bool gameActive = false;
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
            game.GameOver += new EventHandler(game_GameOver);
            game.TimerTicked += new EventHandler(timerCallback);
            seesaw.RegisterGame(game);
            startNewMode(BalanceGame.Mode.Classic);
        }

        void timerCallback(object sender, EventArgs e)
        {
            if (!gameActive) return;
            BalanceGame bg = (BalanceGame) sender;
            scoreText.TextContent = bg.Score;
            timeText.Content = bg.TimeLeft;

            if (bg.TimeLeft == 0)
            {
                gameActive = false;
                notime.Stop();
                notime.Play();
            }

            if (game.CurrentMode == BalanceGame.Mode.Challenge)
            {
                if (bg.TimeLeft < 10)
                {
                    ding.Stop();
                    ding.Play();
                }
            }
            else if (game.CurrentMode == BalanceGame.Mode.Classic)
            {
                if (bg.TimeLeft > 0)
                {
                    seesaw.animateRightBlocks();
                }
                else {
                    seesaw.animateRotateBlocks();
                }
                if (bg.TimeLeft <= 3)
                {
                    ding.Stop();
                    ding.Play();
                }
            }
        }

        void game_GameOver(object sender, EventArgs e)
        {
            gameActive = false;
            BalanceGame bg = (BalanceGame)sender;

            if (game.CurrentMode == BalanceGame.Mode.Classic)
                lifeCanvas.Children.Clear();

            modeLabel.Content = "Game Over!";

            finalScore.Content = "Score: " + bg.Score;
            playLabelAnimation(finalScore, delegate
            {
                finalScore.Opacity = 0;
            });

            playAgain.BeginAnimation(UIElement.OpacityProperty, null);
            playAgain.Opacity = 1;
        }

        void game_LevelLost(object sender, EventArgs e)
        {
            gameActive = false;
            LevelLostEventArgs args = (LevelLostEventArgs) e;
         
            if (game.CurrentMode == BalanceGame.Mode.Classic && lifeCanvas.Children.Count != 0)
                lifeCanvas.Children.RemoveAt(0);

            if (args.reason == LevelLostEventArgs.Reason.WrongAnswer)
            {
                showStatusLabel("Try again!", Brushes.Orange, delegate
                {
                    game.Reset();
                    if (game.LivesLeft > 0) { gameActive = true; }
                });
            }
            else
            {
                showStatusLabel("Time's up!\nTry a new round!", Brushes.Blue, delegate
                {
                    seesaw.resetRightBallPanel();
                    game.LoadCurrentLevel();
                    if (game.LivesLeft > 0) gameActive = true;
                });
            }
        }

        void game_LevelReset(object sender, EventArgs e)
        {
            foreach (var animation in runningAnimations)
            {
                animation.SkipToFill();
                animation.Stop();
            }
            runningAnimations.Clear();
        }

        private void showStatusLabel(String labelString, Brush foreground, EventHandler onComplete)
        {
            uxStatusLabel.Content = labelString;
            uxStatusLabel.Foreground = foreground;
            playLabelAnimation(uxStatusLabel, onComplete);
        }

        void playLabelAnimation(Label label, EventHandler onComplete)
        {
            DoubleAnimation appearAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(0)));
            appearAnimation.BeginTime = TimeSpan.FromSeconds(0.0);
            Storyboard.SetTarget(appearAnimation, label);
            Storyboard.SetTargetProperty(appearAnimation, new PropertyPath(UIElement.OpacityProperty));

            // Hide it when we're done
            DoubleAnimation labelAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(labelDisplayTime)));
            labelAnimation.BeginTime = TimeSpan.FromSeconds(0.75);
            Storyboard.SetTarget(labelAnimation, label);
            Storyboard.SetTargetProperty(labelAnimation, new PropertyPath(UIElement.OpacityProperty));
            Storyboard labelSb = new Storyboard();
            labelSb.Children.Add(appearAnimation);
            labelSb.Children.Add(labelAnimation);
            labelSb.Completed += delegate
            {
                runningAnimations.Remove(labelSb);
                seesaw.IsHappy = false;
            };
            if (onComplete != null)
            {
                labelSb.Completed += onComplete;
                
            }
            runningAnimations.Add(labelSb);
            labelSb.Begin();
            
           
        }

        void game_LevelCompleted(object sender, EventArgs e)
        {
            soundEffect.Play();
            seesaw.IsHappy = true;
            showStatusLabel("Correct!", Brushes.Green, delegate
            {
                soundEffect.Stop();
                game.LoadCurrentLevel();
                gameActive = true;
            });
        }

        private void HeldBalls_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<SeesawObject> col = (ObservableCollection<SeesawObject>)sender;
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
                    new Point(0.15, 0.3),
                    new Point(0.35, 0.05),
                    new Point(0.65, 0.05),
                    new Point(0.85, 0.3)
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
                        holderPositions[i].X * uxPersonRectangle.ActualWidth + Canvas.GetLeft(uxPersonRectangle) - canvas.Width / 2,
                        holderPositions[i].Y * uxPersonRectangle.ActualHeight + Canvas.GetTop(uxPersonRectangle) - canvas.Height / 2)
                    );
                    BallHolders[i] = canvas;
                    Rect boundaryRect = canvas.GetBoundaryRect();
                    boundaryRect.Inflate(HIT_ROUGHNESS, HIT_ROUGHNESS);
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
                    startNewMode(BalanceGame.Mode.Classic);
                    break;
                case Key.D2:
                    startNewMode(BalanceGame.Mode.Challenge);
                    break;
                case Key.D3:
                    startNewMode(BalanceGame.Mode.Practice);
                    break;
            }
        }

        private void startNewMode(BalanceGame.Mode mode) {
            playAgain.Opacity = 0;
            switch (mode) {
                case BalanceGame.Mode.Classic:
                    lifeCanvas.Opacity = 1;
                    modeLabel.Content = "Classic Mode";
                    lifeCanvas.Children.RemoveRange(0, 3);
                    for (int i = 0; i < 3; i++)
                    {
                        Image heart = new Image();
                        heart.Source = new BitmapImage(new Uri("/KineticMath;component/Images/heart.png", UriKind.Relative));
                        lifeCanvas.Children.Add(heart);
                        Canvas.SetLeft(heart, 50 * i);
                    }
                    break;
                case BalanceGame.Mode.Challenge:
                    lifeCanvas.Opacity = 0;
                    modeLabel.Content = "Challenge Mode";
                    ChallengeModeGUI.Visibility = System.Windows.Visibility.Visible;
                    break;
                case BalanceGame.Mode.Practice:
                    lifeCanvas.Opacity = 0;
                    modeLabel.Content = "Practice Mode";
                    ChallengeModeGUI.Visibility = System.Windows.Visibility.Hidden;
                    break;
            }
            game.CurrentMode = mode;
            game.NewGame();
            gameActive = true;
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
            uxPlayerSkeleton.InitializeSkeleton(_sharedData.GestureController, bodyConverter);
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
            //if (e.JointType == JointType.HandLeft) SetCanvasLocationCentered(uxLeftHand, pt);
            //else if (e.JointType == JointType.HandRight) SetCanvasLocationCentered(uxRightHand, pt);
            //else if (e.JointType == JointType.Head) SetCanvasLocationCentered(uxTester, pt);
        }

        void handPushGesture_HandPushed(object sender, HandPushedEventArgs e)
        {
            HandlePushEvent(bodyConverter.ConvertPoint(e.Position));
        }

        public static PolyBezierSegment ComputeCurve(Point startPoint, Point endPoint, Vector velocity)
        {
            PolyBezierSegment pBezierSegment = new PolyBezierSegment();
            //pBezierSegment.Points.Add(new Point(startPoint.X + (endPoint.X - startPoint.X) * 2 / 3, startPoint.Y));
            //pBezierSegment.Points.Add(new Point(endPoint.X, endPoint.Y - (endPoint.Y - startPoint.Y) * 2 / 3));
            //pBezierSegment.Points.Add(endPoint);
            // Compute path
            int divisions = 10; // The granularity of the path
            double[] xPoints = ComputeAcceleratePoints(velocity.X, endPoint.X - startPoint.X, divisions);
            double[] yPoints = ComputeAcceleratePoints(velocity.Y, endPoint.Y - startPoint.Y, divisions);

            for (int i = 0; i < divisions; i++)
            {
                pBezierSegment.Points.Add(new Point(startPoint.X + xPoints[i], startPoint.Y + yPoints[i]));
            }
            return pBezierSegment;
        }

        public static double[] ComputeAcceleratePoints(double initialSpeed, double distance, int totalTime)
        {
            // Using SUVAT equation s = ut + 1/2 at^2
            double acceleration = 2 * (distance - initialSpeed * totalTime) / (totalTime * totalTime);
            double[] points = new double[totalTime];
            for (int i = 1; i <= totalTime; i++)
            {
                points[i - 1] = initialSpeed * i + 0.5 * acceleration * (i * i);
            }
            return points;
        }

        private void HitBall(int index, Vector velocity)
        {
            var pushedBall = game.HeldBalls[index];
            // Check if any running animations to avoid conflicts
            if (this.gameActive && runningAnimations.Count == 0 && game.PushBall(pushedBall))
            {
                gameActive = false;
                var ballHolder = this.BallHolders[index];
                uxMainCanvas.Children.Add(pushedBall);

                // TODO2: Trigger animation for ball and after animation is triggered

                PointAnimationUsingPath ballAnimation = new PointAnimationUsingPath();
                PathGeometry animationPath = new PathGeometry();
                PathFigure pFigure = new PathFigure();
                pFigure.StartPoint = PointCanvas.GetTopLeft(ballHolder);
                //PathFigureCollection pfc = FindResource("RectanglePathFigureCollection") as PathFigureCollection;
                Point endPoint = new Point(
                    Canvas.GetLeft(seesaw) + Canvas.GetLeft(seesaw.uxBalanceCanvas) + Canvas.GetLeft(seesaw.leftBallPanel),
                    Canvas.GetTop(seesaw) + Canvas.GetTop(seesaw.uxBalanceCanvas) + Canvas.GetTop(seesaw.leftBallPanel)
                );
                pFigure.Segments.Add(ComputeCurve(pFigure.StartPoint, endPoint, velocity));

                animationPath.Figures.Add(pFigure);
                // Freeze the PathGeometry for performance benefits.
                animationPath.Freeze();

                ballAnimation.PathGeometry = animationPath;
                ballAnimation.BeginTime = TimeSpan.FromSeconds(0);
                ballAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.75));
                ballAnimation.AutoReverse = false;

                Storyboard.SetTarget(ballAnimation, pushedBall);
                Storyboard.SetTargetProperty(ballAnimation, new PropertyPath("(TopLeft)"));
                Storyboard ballMove = new Storyboard();
                ballMove.Children.Add(ballAnimation);
                runningAnimations.Add(ballMove);

                ballMove.Completed += delegate
                {
                    selectingBall = false;
                    uxMainCanvas.Children.Remove(pushedBall);
                    runningAnimations.Remove(ballMove);
                    game.AddBallToBalance(pushedBall, true); // push ball to left side
                };
                selectingBall = true;
                ballMove.Begin();
                // TODO2: Trigger animation for ball and after animation is triggered

                /*Debug*
                Path myPath = new Path();
                myPath.Stroke = Brushes.Black;
                myPath.StrokeThickness = 1;
                myPath.Data = animationPath;
                uxMainCanvas.Children.Add(myPath);
                /**/
            }
        }

        private void HandlePushEvent(SkeletonPoint pt)
        {
            if (selectingBall) return;
            SeesawObject pushedBall = null;
            PointCanvas ballHolder = null;
            foreach (var holder in BallHolders)
            {
                Rect rect = holder.GetBoundaryRect();
                if (rect.Contains(pt.To2DPoint()))
                {
                    if (holder.Children.Count > 0)
                    {
                        pushedBall = (SeesawObject) holder.Children[0];
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

        private void Background_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
