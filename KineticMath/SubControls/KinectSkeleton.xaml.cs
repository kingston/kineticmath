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

using KineticMath.Kinect;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf; 

namespace KineticMath.SubControls
{
    /// <summary>
    /// Interaction logic for KinectSkeleton.xaml
    /// </summary>
    public partial class KinectSkeleton : UserControl
    {
        private IKinectService kinectService;

        public KinectSkeleton()
        {
            InitializeComponent();
            //Force video to the background
            Canvas.SetZIndex(uxKinectImage, -10000);
        }

        public void InitializeSkeleton(IKinectService kinectService)
        {
            kinectService.ImageFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(kinectService_ImageFrameReady);
            kinectService.SkeletonUpdated += new EventHandler<SkeletonEventArgs>(kinectService_SkeletonUpdated);
            this.kinectService = kinectService;
        }

        void kinectService_SkeletonUpdated(object sender, SkeletonEventArgs e)
        {
            //get the first tracked skeleton
            Skeleton skeleton = (from s in e.Skeletons
                                     where s.TrackingState == SkeletonTrackingState.Tracked
                                     select s).FirstOrDefault();


            if (skeleton != null)
            {
                //set positions on our joints of interest (already defined as Ellipse objects in the xaml)
                SetEllipsePosition(headEllipse, skeleton.Joints[JointType.Head]);
                SetEllipsePosition(leftEllipse, skeleton.Joints[JointType.HandLeft]);
                SetEllipsePosition(rightEllipse, skeleton.Joints[JointType.HandRight]);
                SetEllipsePosition(shoulderCenter, skeleton.Joints[JointType.ShoulderCenter]);
                SetEllipsePosition(shoulderRight, skeleton.Joints[JointType.ShoulderRight]);
                SetEllipsePosition(shoulderLeft, skeleton.Joints[JointType.ShoulderLeft]);
                SetEllipsePosition(ankleRight, skeleton.Joints[JointType.AnkleRight]);
                SetEllipsePosition(ankleLeft, skeleton.Joints[JointType.AnkleLeft]);
                SetEllipsePosition(footLeft, skeleton.Joints[JointType.FootLeft]);
                SetEllipsePosition(footRight, skeleton.Joints[JointType.FootRight]);
                SetEllipsePosition(wristLeft, skeleton.Joints[JointType.WristLeft]);
                SetEllipsePosition(wristRight, skeleton.Joints[JointType.WristRight]);
                SetEllipsePosition(elbowLeft, skeleton.Joints[JointType.ElbowLeft]);
                SetEllipsePosition(elbowRight, skeleton.Joints[JointType.ElbowRight]);
                SetEllipsePosition(ankleLeft, skeleton.Joints[JointType.AnkleLeft]);
                SetEllipsePosition(footLeft, skeleton.Joints[JointType.FootLeft]);
                SetEllipsePosition(footRight, skeleton.Joints[JointType.FootRight]);
                SetEllipsePosition(wristLeft, skeleton.Joints[JointType.WristLeft]);
                SetEllipsePosition(wristRight, skeleton.Joints[JointType.WristRight]);
                SetEllipsePosition(kneeLeft, skeleton.Joints[JointType.KneeLeft]);
                SetEllipsePosition(kneeRight, skeleton.Joints[JointType.KneeRight]);
                SetEllipsePosition(hipCenter, skeleton.Joints[JointType.HipCenter]);
            }
        }

        //Scaling constants
        private float k_xMaxJointScale = 1.5f;
        private float k_yMaxJointScale = 1.5f;

        private void SetEllipsePosition(Ellipse ellipse, Joint joint)
        {
            var scaledJoint = joint.ScaleTo(640, 480, k_xMaxJointScale, k_yMaxJointScale);

            Canvas.SetLeft(ellipse, scaledJoint.Position.X - (double)ellipse.GetValue(Canvas.WidthProperty) / 2);
            Canvas.SetTop(ellipse, scaledJoint.Position.Y - (double)ellipse.GetValue(Canvas.WidthProperty) / 2);
            Canvas.SetZIndex(ellipse, (int)-Math.Floor(scaledJoint.Position.Z * 100));
            if (joint.JointType == JointType.HandLeft || joint.JointType == JointType.HandRight)
            {
                byte val = (byte)(Math.Floor((joint.Position.Z - 0.8) * 255 / 2));
                ellipse.Fill = new SolidColorBrush(Color.FromRgb(val, val, val));
            }
        }

        void kinectService_ImageFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            //Automagically create BitmapSource for Video
            uxKinectImage.Source = e.OpenColorImageFrame().ToBitmapSource();
        }
    }
}
