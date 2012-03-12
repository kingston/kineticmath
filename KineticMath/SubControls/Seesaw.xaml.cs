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

using KineticMath.Messaging;
using KineticMath.Kinect.Gestures;
using KineticMath.Controllers;
using System.Windows.Media.Animation;


namespace KineticMath.SubControls
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    /// 

    public partial class Seesaw : UserControl
    {
        private const int MAX_ROTATION_ANGLE = 30;
        private BalanceGame game;
        private double originalOffset = 0;
        private double currentBottom = 0;
        private const double rightBallPanelLeft = 192;
        private double rightBallPanelLeftAdjustment = 0.0;
        List<Storyboard> runningAnimations;

        public Seesaw()
        {
            InitializeComponent();
            runningAnimations = new List<Storyboard>();
        }

        public void RegisterGame(BalanceGame game)
        {
            game.LevelReset += new EventHandler(game_LevelReset);
            game.LeftBalanceBalls.CollectionChanged += new NotifyCollectionChangedEventHandler(BalanceBalls_CollectionChanged);
            game.RightBalanceBalls.CollectionChanged += new NotifyCollectionChangedEventHandler(BalanceBalls_CollectionChanged);
            this.game = game;
        }

        public void animateRightBlocks()
        {
            rightBallPanelLeftAdjustment += 5.0;
            moveRightPanelTo(rightBallPanelLeft + rightBallPanelLeftAdjustment, new Duration(TimeSpan.FromSeconds(1.0)));
        }

        public void resetRightBallPanel()
        {
            rightBallPanel.RenderTransform = new RotateTransform();
        }

        public void animateRotateBlocks() {
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = 90;
            da.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            RotateTransform rt = new RotateTransform(0, 70, Canvas.GetTop(rightBallPanel) + rightBallPanel.Height);
            rightBallPanel.RenderTransform = rt;
            rt.BeginAnimation(RotateTransform.AngleProperty, da);
            
        }

        void moveRightPanelTo(double newPosition, Duration duration)
        {
            DoubleAnimation animation = new DoubleAnimation(newPosition, duration);
            animation.BeginTime = TimeSpan.FromSeconds(0);
            Storyboard.SetTarget(animation, rightBallPanel);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
            Storyboard sb = new Storyboard();
            sb.Children.Add(animation);
            runningAnimations.Add(sb);
            sb.Completed += delegate
            {
                runningAnimations.Remove(sb);
            };
            sb.Begin();
        }

        void game_LevelReset(object sender, EventArgs e)
        {
            originalOffset = 0;
            currentBottom = 0;
            // TODO2: Terminate all animations
            runningAnimations.Clear();
            // Reset rightBallPanel position
            rightBallPanelLeftAdjustment = 0.0;
            moveRightPanelTo(rightBallPanelLeft, new Duration(TimeSpan.FromSeconds(0.0)));
        }

        void BalanceBalls_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool isLeft = (sender == game.LeftBalanceBalls);
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (SeesawObject obj in e.NewItems)
                {
                    AddObject(obj, isLeft);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Ball obj in e.OldItems)
                {
                    RemoveObject(obj, isLeft);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Panel panel = isLeft ? (Panel) leftBallPanel : (Panel) rightBallPanel;
                panel.Children.Clear();
            }
            int offset = (int) game.GetBalanceOffset();
            if (offset != originalOffset)
            {
                // TODO2: Trigger balance animation
                originalOffset = offset;
                RenderWeights();
                // After animation is complete...
                game.VerifySolution();
            }
        }

        public void AddObject(SeesawObject obj, bool isLeft = true)
        {
            if (isLeft)
            {
                obj.OnLeftSeesaw = true;
                leftBallPanel.Children.Add(obj);
            }
            else
            {
                rightBallPanel.Children.Add(obj);
                Canvas.SetBottom(obj, currentBottom);
                currentBottom += obj.Height;
            }
        }


        public void RemoveObject(SeesawObject obj, bool isLeft = true)
        {
            if (isLeft)
            {
                obj.OnLeftSeesaw = false;
                leftBallPanel.Children.Remove(obj);
            }
            else
            {
                rightBallPanel.Children.Remove(obj);
            }
            RenderWeights();
        }

        private void RenderWeights()
        {
            // Work out rotation
            double angle = game.GetBalanceOffset() / game.GetMaximumValue() * MAX_ROTATION_ANGLE;
            uxBalanceCanvas.RenderTransform = new RotateTransform(angle);

            DoubleAnimationUsingKeyFrames rotateAnimation = FindResource("doubleAnimation") as DoubleAnimationUsingKeyFrames;

            Storyboard.SetTarget(rotateAnimation, uxBalanceCanvas);
            Storyboard.SetTargetProperty(rotateAnimation,  new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)"));
            Storyboard ballMove = new Storyboard();
            ballMove.Children.Add(rotateAnimation);
        }
    }
}
