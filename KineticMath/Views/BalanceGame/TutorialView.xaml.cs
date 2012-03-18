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
using System.Windows.Media.Animation;

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
            uxPlayerSkeleton.InitializeSkeleton(_sharedData.GestureController, bodyConverter);
        }

        private const int HIT_ROUGHNESS = 10; // The amount of rough distance they can hit in between to make it easier to hit

        void setHitZone()
        {
            _hitZones.Clear();
            Rect boundaryRect = hitRect1.GetBoundaryRect();
            Rect boundaryRect2 = hitRect2.GetBoundaryRect();
            boundaryRect.Inflate(HIT_ROUGHNESS, HIT_ROUGHNESS);
            _hitZones.Add(boundaryRect);
            _hitZones.Add(boundaryRect2);
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
                    if (index == 0)
                    {
                        instructionBlock.Text = "You're doing so well! \nHit the block again to start the game.";
                        bird1.Opacity = 0;
                        bird2.Opacity = 1;
                        currentState = State.RULES;
                    }
                    break;
                case State.RULES:
                    if (index == 1)
                    {
                        startBrickAnimation();
                    }
                    break;
                    
                    /*instructionBlock.Text = "The game is simple, you just need to hit the bird.\n" + 
                                            "Choose the bird with number equal to sum of numbers on righthand side of the sea-saw.\n"+
                                            "Ready? Hit me again to play the game!";
                    currentState = State.HITMEAGAIN;
                    break;*/
                case State.HITMEAGAIN:
                    //_sharedData.GestureController.RemoveGesture(handGestures);
                    //_sharedData.GestureController.RemoveGesture(hitGesture);
                    this.SendMessage(new ChangeViewMessage(typeof(MainView)));
                    break;
            }
            

        }

        private void startBrickAnimation(){

            PointAnimationUsingPath ballAnimation = new PointAnimationUsingPath();
            PathGeometry animationPath = new PathGeometry();
            PathFigure pFigure = new PathFigure();
            //I don't know how to get its position....
            pFigure.StartPoint = new Point(607, 228);
            Point endPoint = new Point(200,200);
            pFigure.Segments.Add(MainView.ComputeCurve(pFigure.StartPoint, endPoint, new Vector(0,0)));

            animationPath.Figures.Add(pFigure);
            // Freeze the PathGeometry for performance benefits.
            animationPath.Freeze();

            /*
            animationPath.Figures = pfc;*/
            ballAnimation.PathGeometry = animationPath;
            ballAnimation.BeginTime = TimeSpan.FromSeconds(0);
            ballAnimation.Duration = new Duration(TimeSpan.FromSeconds(2));
            ballAnimation.AutoReverse = false;

            Storyboard.SetTarget(ballAnimation, bird2);
            Storyboard.SetTargetProperty(ballAnimation, new PropertyPath("(TopLeft)"));
            Storyboard ballMove = new Storyboard();
            ballMove.Children.Add(ballAnimation);

            ballMove.Completed += delegate
            {
                this.SendMessage(new ChangeViewMessage(typeof(MainView)));
            };
            ballMove.Begin();
        }

        private void SetCanvasLocationCentered(FrameworkElement element, SkeletonPoint pt)
        {
            Canvas.SetLeft(element, pt.X - element.ActualWidth / 2);
            Canvas.SetTop(element, pt.Y - element.ActualHeight / 2);
        }

    }
}