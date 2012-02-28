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


using KineticMath.Messaging;
using KineticMath.Kinect.Gestures;
using KineticMath.SubControls;
using System.Windows.Media.Animation;

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
       
        public MainView()
        {
            InitializeComponent();
            Setup();
            Loaded += new RoutedEventHandler(MainView_Loaded);
        }

        void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            Console.Out.WriteLine("MainView loaded");
            MoveGesture mover = new MoveGesture();
            mover.UserMoved += Mover_move;
            mover.BodyMoved += new EventHandler<BodyMoveEventArgs>(mover_BodyMoved);
            _sharedData.GestureController.AddGesture(this, mover);

            //SwipeGesture swipe = new SwipeGesture();
            //swipe.SwipeGestureMade += new EventHandler(swipe_SwipeGestureMade);
            //_sharedData.GestureController.AddGesture(swipe);

            HoldGesture holder = new HoldGesture();
            holder.UserHolded += new EventHandler(Holder_hold);
            _sharedData.GestureController.AddGesture(this, holder);
        }

        void swipe_SwipeGestureMade(object sender, EventArgs e)
        {
            MessageBox.Show("HI");
        }

        private bool ResetSelected = false;

        void mover_BodyMoved(object sender, BodyMoveEventArgs e)
        {
            float pos = e.ScreenX;
            // Check where the body is
            if (Canvas.GetLeft(fallingGroup) < pos && pos < Canvas.GetLeft(fallingGroup) + rectangle3.ActualWidth)
            {
                fallingGroup.SelectBall(pos - Canvas.GetLeft(fallingGroup));
            } else {
                fallingGroup.SelectBall(-1);
            }
            if (Canvas.GetLeft(uxResetRectangle) < pos && pos < Canvas.GetLeft(uxResetRectangle) + uxResetRectangle.ActualWidth)
            {
                uxResetRectangle.Background = new SolidColorBrush(SELECTED_COLOR);
                ResetSelected = true;
            }
            else
            {
                uxResetRectangle.Background = new SolidColorBrush(DESELECTED_COLOR);
                ResetSelected = false;
            }
        }

        void Holder_hold(object sender, EventArgs e)
        {
            System.Console.WriteLine("Holder_hold");
            selectItem();
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

        void selectItem()
        {
            if (ResetSelected)
            {
                Reset();
            }
            else
            {
                Ball b = fallingGroup.RemoveSelected();
                if (b != null)
                    seesaw1.AddBall(b);

                if (seesaw1.checkAnswer())
                {
                    RoundComplete();
                }
            }
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
                    selectItem();
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
