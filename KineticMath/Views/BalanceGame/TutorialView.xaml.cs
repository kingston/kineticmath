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

using KineticMath.Helpers;
using KineticMath.Views;
using KineticMath.Kinect;
using KineticMath.Messaging;
using KineticMath.Kinect.PointConverters;
using KineticMath.Kinect.Gestures;
using Microsoft.Kinect;

namespace KineticMath.Views
{
    /// <summary>
    /// Interaction logic for TutorialScreen.xaml
    /// </summary>
    public partial class TutorialView : BaseView
    {
        private BodyRelativePointConverter bodyConverter;

        enum State { HITME, RULES, HITMEAGAIN };
        State currentState;

        public TutorialView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(TutorialView_Loaded);
        }

        private void TutorialView_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterGestures();
            currentState = State.HITME;
        }

        private void BaseView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.SendMessage(new ChangeViewMessage(typeof(MainView)));
        }

        private List<Rect> _hitZones = new List<Rect>(); // Zones for the hit gesture
        private HitGesture hitGesture;
        private JointMoveGestures handGestures;
        private DateTime lastAction = DateTime.Now;

        private void RegisterGestures()
        {
            bodyConverter = new BodyRelativePointConverter(uxPersonRectangle.GetBoundaryRect(), this._sharedData.GestureController);

            handGestures = new JointMoveGestures(JointType.HandLeft, JointType.HandRight, JointType.Head);
            handGestures.JointMoved += new EventHandler<JointMovedEventArgs>(handGesture_JointMoved);
            _sharedData.GestureController.AddGesture(this, handGestures);
            setHitZone();
            if (hitGesture == null)
            {
                hitGesture = new HitGesture(_hitZones, bodyConverter, JointType.HandRight, JointType.HandLeft);
                hitGesture.RectHit += new EventHandler<RectHitEventArgs>(hitGesture_RectHit);
            }
            _sharedData.GestureController.AddGesture(this, hitGesture);

        }

        private const int HIT_ROUGHNESS = 10; // The amount of rough distance they can hit in between to make it easier to hit

        void setHitZone()
        {
            _hitZones.Clear();
            Rect boundaryRect = hitRect.GetBoundaryRect();
            boundaryRect.Inflate(HIT_ROUGHNESS, HIT_ROUGHNESS);
            Console.WriteLine(boundaryRect.Height);
            _hitZones.Add(boundaryRect);
        }

        void hitGesture_RectHit(object sender, RectHitEventArgs e)
        {
            if (DateTime.Now.Subtract(lastAction) > TimeSpan.FromSeconds(1))
            {
                HitBall(e.RectIdx, e.HitVelocity);
                lastAction = DateTime.Now;
            }
        }

        void handGesture_JointMoved(object sender, JointMovedEventArgs e)
        {
            // Show the movement on the screen
            SkeletonPoint pt = bodyConverter.ConvertPoint(e.NewPosition);
            // TODO2: Make pretty way to reflect hand movements
            if (e.JointType == JointType.HandLeft) SetCanvasLocationCentered(uxLeftHand, pt);
            else if (e.JointType == JointType.HandRight) SetCanvasLocationCentered(uxRightHand, pt);
        }
        private void HitBall(int index, Vector velocity)
        {
            //this.SendMessage(new ChangeViewMessage(typeof(MainView)));
            switch (currentState)
            {
                case State.HITME:
                    instructionBlock.Text = "You're doing so well! \nHit the block again to play the game.";
                    currentState = State.RULES;
                    break;
                case State.RULES:
                    this.SendMessage(new ChangeViewMessage(typeof(MainView)));
                    break;
                    instructionBlock.Text = "The game is simple, you just need to hit the bird.\n" + 
                                            "Choose the bird with number equal to sum of numbers on righthand side of the sea-saw.\n"+
                                            "Ready? Hit me again to play the game!";
                    currentState = State.HITMEAGAIN;
                    break;
                case State.HITMEAGAIN:
                    //_sharedData.GestureController.RemoveGesture(handGestures);
                    //_sharedData.GestureController.RemoveGesture(hitGesture);
                    this.SendMessage(new ChangeViewMessage(typeof(MainView)));
                    break;
            }
            setHitZone();

        }
        private void SetCanvasLocationCentered(FrameworkElement element, SkeletonPoint pt)
        {
            Canvas.SetLeft(element, pt.X - element.ActualWidth / 2);
            Canvas.SetTop(element, pt.Y - element.ActualHeight / 2);
        }

    }
}