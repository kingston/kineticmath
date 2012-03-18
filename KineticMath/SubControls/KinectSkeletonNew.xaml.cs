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
using KineticMath.Helpers;

using KineticMath.Kinect.PointConverters;

namespace KineticMath.SubControls
{
    /// <summary>
    /// Interaction logic for KinectSkeleton.xaml
    /// </summary>
    public partial class KinectSkeletonNew : UserControl
    {
        //Scaling constants
        public static float k_xMaxJointScale = 1.5f;
        public static float k_yMaxJointScale = 1.5f;
        private IPointConverter pointConverter;

        public KinectSkeletonNew()
        {
            InitializeComponent();
        }

        public void InitializeSkeleton(GestureController gestureController, IPointConverter pointConverter)
        {
            gestureController.SkeletonPreProcessed += new EventHandler<SkeletonPreProcessedEventArgs>(gestureController_SkeletonPreProcessed);
            this.pointConverter = pointConverter;
        }

        void gestureController_SkeletonPreProcessed(object sender, SkeletonPreProcessedEventArgs e)
        {
            Skeleton skeleton = e.Skeleton;
            // Seralize
            List<JointType[]> jointChains = new List<JointType[]>();
            jointChains.Add(new JointType[] { JointType.ShoulderLeft, JointType.ElbowLeft, JointType.HandLeft });
            jointChains.Add(new JointType[] { JointType.ShoulderRight, JointType.ElbowRight, JointType.HandRight });
            jointChains.Add(new JointType[] { JointType.HipLeft, JointType.KneeLeft, JointType.FootLeft });
            jointChains.Add(new JointType[] { JointType.HipRight, JointType.KneeRight, JointType.FootRight });
            foreach (var jointChain in jointChains)
            {
                for (int i = 0; i < jointChain.Length - 1; i++)
                {
                    positionLimb(skeleton, jointChain[i], jointChain[i + 1]);
                }
            }
            // Position elements
            PositionBody(uxMainBody, skeleton);
            PositionElement(uxHeadPart, skeleton.Joints[JointType.Head], true);
            PositionElement(uxLeftHand, skeleton.Joints[JointType.HandLeft], true);
            PositionElement(uxRightHand, skeleton.Joints[JointType.HandRight], true);
        }

        private void PositionBody(Path body, Skeleton skel)
        {
            JointType[] jointChain = new JointType[] { JointType.ShoulderLeft, JointType.Head, JointType.ShoulderRight, JointType.HipRight, JointType.HipLeft };
            PathGeometry geometry = new PathGeometry();
            String path = "";
            bool firstPoint = true;
            for (int i = 0; i < jointChain.Length; i++)
            {
                var pt = GetPoint(skel, jointChain[i]);
                // Hard code neck
                if (jointChain[i] == JointType.Head)
                {
                    // Average with shoulder
                    var shoulderPt = GetPoint(skel, JointType.ShoulderCenter);
                    pt.X = pt.X * 0.1f + shoulderPt.X * 0.9f;
                    pt.Y = pt.Y * 0.1f + shoulderPt.Y * 0.9f;
                }
                if (firstPoint)
                {
                    path += "M " + pt.X.ToString() + "," + pt.Y.ToString() + " ";
                    firstPoint = false;
                }
                else
                {
                    path += "L " + pt.X.ToString() + "," + pt.Y.ToString();
                }
            }
            // Close the path
            path += " z";
            Canvas.SetLeft(uxMainBody, 0);
            Canvas.SetTop(uxMainBody, 0);
            uxMainBody.Data = Geometry.Parse(path);
        }

        private SkeletonPoint GetPoint(Skeleton skel, JointType type)
        {
            return pointConverter.ConvertPoint(skel.Joints[type].Position);
        }

        private void PositionElement(FrameworkElement element, Joint joint, bool center)
        {
            SkeletonPoint pt = pointConverter.ConvertPoint(joint.Position);
            double dx = 0;
            double dy = 0;
            if (center)
            {
                dx = element.ActualWidth / 2;
                dy = element.ActualHeight / 2;
            }
            Canvas.SetLeft(element, pt.X - dx);
            Canvas.SetTop(element, pt.Y - dy);
        }

        private Dictionary<String, Line> LimbToLineDict = new Dictionary<string, Line>();

        void positionLimb(Skeleton skeleton, JointType jointType1, JointType jointType2)
        {
            SkeletonPoint pt1 = pointConverter.ConvertPoint(skeleton.Joints[jointType1].Position);
            SkeletonPoint pt2 = pointConverter.ConvertPoint(skeleton.Joints[jointType2].Position);
            String limbKey = jointType1.ToString() + "-" + jointType2.ToString();
            if (!LimbToLineDict.ContainsKey(limbKey))
            {
                Line newLine = new Line();
                newLine.Stroke = Brushes.Black;
                byte gray = 55;
                newLine.Stroke = new SolidColorBrush(Color.FromRgb(gray, gray, gray));
                newLine.StrokeThickness = 26;
                newLine.StrokeStartLineCap = PenLineCap.Round;
                newLine.StrokeEndLineCap = PenLineCap.Round;
                Canvas.SetTop(newLine, 0);
                Canvas.SetLeft(newLine, 0);
                uxMainCanvas.Children.Add(newLine);
                LimbToLineDict.Add(limbKey, newLine);
            }
            Line line = LimbToLineDict[limbKey];
            line.X1 = pt1.X;
            line.X2 = pt2.X;
            line.Y1 = pt1.Y;
            line.Y2 = pt2.Y;
        }      

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
    }
}
