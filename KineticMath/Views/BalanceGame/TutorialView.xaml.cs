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

        enum TutorialState { HITME, HITMEAGAIN };
        TutorialState currentState;

        public TutorialView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(TutorialView_Loaded);
        }

        private void TutorialView_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterGestures();
            currentState = TutorialState.HITME;
        }

        private void BaseView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.SendMessage(new ChangeViewMessage(typeof(MainView)));
        }

        private List<Rect> _hitZones = new List<Rect>(); // Zones for the hit gesture
        private HitGesture hitGesture;
        private DateTime lastAction = DateTime.Now;

        private void RegisterGestures()
        {
            bodyConverter = new BodyRelativePointConverter(uxPersonRectangle.GetBoundaryRect(), this._sharedData.PlayerOneController);

            setHitZone();
            if (hitGesture == null)
            {
                hitGesture = new HitGesture(_hitZones, bodyConverter, JointType.HandRight, JointType.HandLeft);
                hitGesture.RectHit += new EventHandler<RectHitEventArgs>(hitGesture_RectHit);
            }
            _sharedData.PlayerOneController.AddGesture(this, hitGesture);
            uxPlayerSkeleton.InitializeSkeleton(_sharedData.PlayerOneController, bodyConverter);
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

        private void HitBall(int index, Vector velocity)
        {
            switch (currentState)
            {
                case TutorialState.HITME:
                    if (index == 0)
                    {
                        instructionBlock.Text = "Great!\nHit the block to start!";
                        bird1.Opacity = 0;
                        bird2.Opacity = 1;
                        currentState = TutorialState.HITMEAGAIN;
                    }
                    break;
                case TutorialState.HITMEAGAIN:
                    if (index == 1)
                    {
                        startBrickAnimation();
                    }
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
    }
}