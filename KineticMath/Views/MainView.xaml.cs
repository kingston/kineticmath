using System;
using System.Collections.Generic;
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

namespace KineticMath.Views
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainView : BaseView, IView
    {
        private static int NUM_WEIGHTS = 5;
        private static int MAX_NUMBERS_TO_ADD = 4;
        public static Color SELECTED_COLOR = Colors.Orange;
        public static Color DESELECTED_COLOR = Colors.Yellow; //Color.FromRgb(0xE2, 0x51, 0x51);

        private int difficulty = 1;
        private int levelsCompleted = 0;

        private BalanceGame game;
       
        public MainView()
        {
            InitializeComponent();
            Setup();
            Loaded += new RoutedEventHandler(MainView_Loaded);
        }

        private void InitializeGame()
        {
            bodyConverter = new BodyRelativePointConverter(new Rect(100, 100, 1000, 1000));
            game = new BalanceGame();
            DependencyPropertyDescriptor.FromProperty(BalanceGame.HeldBallsProperty, game.GetType()).AddValueChanged(game, new EventHandler(BallsChanged));
            game.NewGame();
            // TODO: Actually provide valid coordinates
            
        }

        public void BallsChanged(object sender, EventArgs e)
        {
            // Implement code when the ball collection in the game object has changed, e.g. show/hide balls in holder
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeGame();
        }

        private BodyRelativePointConverter bodyConverter;

        private void RegisterGestures()
        {
            JointMoveGestures leftHandGesture = new JointMoveGestures(JointType.HandLeft);
            JointMoveGestures rightHandGesture = new JointMoveGestures(JointType.HandLeft);

            leftHandGesture.JointMoved += new EventHandler<JointMovedEventArgs>(handGesture_JointMoved);
            rightHandGesture.JointMoved += new EventHandler<JointMovedEventArgs>(handGesture_JointMoved);

            _sharedData.GestureController.AddGesture(this, leftHandGesture);
            _sharedData.GestureController.AddGesture(this, rightHandGesture);
        }

        void handGesture_JointMoved(object sender, JointMovedEventArgs e)
        {
            // Show the movement on the screen
            SkeletonPoint pt = bodyConverter.ConvertPoint(e.NewPosition);
            // e.g. move circle to pt
        }

        void Reset()
        {
            ClearBalls();
            SetupBalls();
        }

        void NewRound(object sender, EventArgs args)
        {
            ClearBalls();
            Setup();
        }

        void RoundComplete()
        {
            uxWinLabel.BeginAnimation(UIElement.OpacityProperty, null); // reset animation
            uxWinLabel.Opacity = 1;

            levelsCompleted++;
            difficulty = levelsCompleted / 3 + 1;

            // Hide it when we're done
            DoubleAnimation labelAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(1000)));
            labelAnimation.BeginTime = TimeSpan.FromSeconds(1);
            Storyboard.SetTarget(labelAnimation, uxWinLabel);
            Storyboard.SetTargetProperty(labelAnimation, new PropertyPath(UIElement.OpacityProperty));
            Storyboard labelSb = new Storyboard();
            labelSb.Children.Add(labelAnimation);
            labelAnimation.Completed += new EventHandler(NewRound);
            labelSb.Begin();
        }

        void ClearBalls()
        {
            fallingGroup.RemoveAllBalls();
            seesaw1.RemoveAllBalls();
        }

        void Mover_move(object sender, MoveEventArgs m)
        {
            if (m.GetDirection() == 1)
            {
                System.Console.WriteLine("Move Right");
                fallingGroup.ChooseNext();
            }
            else
            {
                System.Console.WriteLine("Move Left");
                fallingGroup.ChoosePrevious();
            }
        }

        public override void OnViewActivated()
        {
             base.OnViewActivated();
             RegisterGestures();
             ParentWindow.AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandleKeyDownEvent);
        }

        public override void  OnViewDeactivated()
        {
 	         base.OnViewDeactivated();
             ParentWindow.RemoveHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandleKeyDownEvent);
        }

        private void HandleKeyDownEvent(object sender, KeyEventArgs e)
        {
            //Console.Out.WriteLine("Keydown");
            switch (e.Key)
            {
                case Key.T:
                    seesaw1.AddBall(new SubControls.Ball());
                        //Console.Out.WriteLine("ball add");
                    
                    break;
                case Key.S:
                    break;
                case Key.R:
                    Reset();
                    break;
                case Key.Left:
                    fallingGroup.ChoosePrevious();
                    break;
                case Key.Right:

                    fallingGroup.ChooseNext();
                    break;
            }
        }

        private List<int> lhs, rhs;
        private int answer;

        private void Setup()
        {
            lhs = new List<int>();
            rhs = new List<int>();
            answer = GenerateQuestion(lhs, rhs);
            SetupBalls();
        }

        private void SetupBalls()
        {
            for (int i = 0; i < rhs.Count; i++) {
                seesaw1.AddBall(new Ball(rhs[i].ToString(), rhs[i]), false);
            }
            fallingGroup.addBall(lhs);
        }

        private int GenerateQuestion(List<int> lhsArray, List<int> rhsArray)
        {
            Random rand = new Random();
            // Generate the answer options
            while (lhsArray.Count < NUM_WEIGHTS)
            {
                int candidate = rand.Next(3, difficulty * 5 + 7);
                if (!lhsArray.Contains(candidate))
                {
                    lhsArray.Add(candidate);
                }
            }
            // Pick one to be the correct answer
            int answer = lhsArray[rand.Next(0, NUM_WEIGHTS - 1)];
            // Generate the question
            int sum = 0;
            int maxParts = Math.Min(difficulty + 1, MAX_NUMBERS_TO_ADD);
            while (sum < answer && rhsArray.Count < maxParts - 1)
            {
                int part = rand.Next(1, answer - sum);
                rhsArray.Add(part);
                sum += part;
            }
            if (sum < answer)
            {
                rhsArray.Add(answer - sum);
            }
            return answer;
        }
    }
}
