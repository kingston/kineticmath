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
        public static Color SELECTED_COLOR = Colors.Orange;
        public static Color DESELECTED_COLOR = Colors.Yellow; //Color.FromRgb(0xE2, 0x51, 0x51);
       
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
            _sharedData.GestureController.AddGesture(mover);

            //SwipeGesture swipe = new SwipeGesture();
            //swipe.SwipeGestureMade += new EventHandler(swipe_SwipeGestureMade);
            //_sharedData.GestureController.AddGesture(swipe);

            HoldGesture holder = new HoldGesture();
            holder.UserHolded += new EventHandler(Holder_hold);
            _sharedData.GestureController.AddGesture(holder);
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

        void selectItem()
        {
            if (ResetSelected)
            {
                ClearBalls();
                SetupBalls();
            }
            else
            {
                Ball b = fallingGroup.RemoveSelected();
                if (b != null)
                    seesaw1.AddBall(b);

                if (seesaw1.checkAnswer())
                {
                    ClearBalls();
                    Setup();
                    uxWinLabel.BeginAnimation(UIElement.OpacityProperty, null); // reset animation
                    uxWinLabel.Opacity = 1;

                    // Hide it when we're done
                    DoubleAnimation labelAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(1000)));
                    labelAnimation.BeginTime = TimeSpan.FromSeconds(1);
                    Storyboard.SetTarget(labelAnimation, uxWinLabel);
                    Storyboard.SetTargetProperty(labelAnimation, new PropertyPath(UIElement.OpacityProperty));
                    Storyboard labelSb = new Storyboard();
                    labelSb.Children.Add(labelAnimation);
                    labelSb.Begin();
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
                case Key.Left:
                    fallingGroup.ChoosePrevious();
                    break;
                case Key.Right:

                    fallingGroup.ChooseNext();
                    break;
            }
        }

        private int[] weightsArray;
        private int answer;

        private void Setup()
        {
            weightsArray = new int[NUM_WEIGHTS];
            answer = generateAnswer(weightsArray);
            SetupBalls();
        }

        private void SetupBalls()
        {
            seesaw1.AddBall(new Ball(answer.ToString(), answer), false);
            fallingGroup.addBall(weightsArray);
        }

        private int generateAnswer(int[] weightsArray)
        {
            Random random = new Random();
            int[] tmp  = new int[NUM_WEIGHTS];
            for (int i = 0; i < NUM_WEIGHTS; i++)
                tmp[i] = weightsArray[i] = random.Next(1, 8);

            int numOfDiscards = random.Next(1, 4);

            for (int i = 0; i < numOfDiscards; i++)
            {
                int r = random.Next(0, 4);
                tmp[r] = 0;
            }

            int sum = 0;
            for (int i = 0; i < NUM_WEIGHTS; i++)
                sum += tmp[i];

            return sum;
        }
    }
}
