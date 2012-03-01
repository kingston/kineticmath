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

        private BalanceGame game;
        private BodyRelativePointConverter bodyConverter;
       
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
            game.LevelCompleted += new EventHandler(game_LevelCompleted);
            game.NewGame();
        }

        void game_LevelCompleted(object sender, EventArgs e)
        {
            // TODO: Say something cool!
        }

        void HeldBalls_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Update the held balls accordingly
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // Add balls to screen
            } else if (e.Action == NotifyCollectionChangedAction.Replace) 
            {
                // Remove ball from screen as it would be replaced with null
            }
        }

        private void HandleKeyDownEvent(object sender, KeyEventArgs e)
        {
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
            bodyConverter = new BodyRelativePointConverter(
                new Rect(Canvas.GetLeft(uxPersonRectangle), Canvas.GetTop(uxPersonRectangle), uxPersonRectangle.ActualWidth, uxPersonRectangle.ActualHeight),
                this._sharedData.GestureController);

            JointMoveGestures handGestures = new JointMoveGestures(JointType.HandLeft, JointType.HandRight);
            handGestures.JointMoved += new EventHandler<JointMovedEventArgs>(handGesture_JointMoved);
            _sharedData.GestureController.AddGesture(this, handGestures);

            HandPushGesture handPushGesture = new HandPushGesture();
            handPushGesture.HandPushed += new EventHandler<HandPushedEventArgs>(handPushGesture_HandPushed);
            _sharedData.GestureController.AddGesture(this, handPushGesture);
        }

        void handPushGesture_HandPushed(object sender, HandPushedEventArgs e)
        {
            SkeletonPoint pt = bodyConverter.ConvertPoint(e.Position);
            Ball pushedBall = null;
            // TODO: Identify which ball was pushed
            if (pushedBall != null)
            {
                game.PushBall(pushedBall);
                // TODO: Trigger animation for ball and after animation is triggered
                game.AddBallToBalance(pushedBall, true); // push ball to left side
            }
        }

        void handGesture_JointMoved(object sender, JointMovedEventArgs e)
        {
            // Show the movement on the screen
            SkeletonPoint pt = bodyConverter.ConvertPoint(e.NewPosition);
            // TODO: Reflect move action on screen
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
