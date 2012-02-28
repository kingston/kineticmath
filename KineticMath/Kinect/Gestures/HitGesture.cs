using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Kinect;
using KineticMath.SubControls;

namespace KineticMath.Kinect.Gestures
{
    class HitGesture : IGesture
    {
        private DateTime lastAction = DateTime.Now;
        private float timelength = 1;
        float heightThreshould = 20;
        float frontThreshould = 50;

        public event EventHandler handler;

        public void ProcessSkeleton(Skeleton skeleton)
        {
            Joint handLeft = skeleton.Joints[JointType.HandLeft].ScaleTo(640, 480, KinectSkeleton.k_xMaxJointScale, KinectSkeleton.k_yMaxJointScale);
            Joint handRight = skeleton.Joints[JointType.HandRight].ScaleTo(640, 480, KinectSkeleton.k_xMaxJointScale, KinectSkeleton.k_yMaxJointScale);

            Joint elbowLeft = skeleton.Joints[JointType.ElbowLeft].ScaleTo(640, 480, KinectSkeleton.k_xMaxJointScale, KinectSkeleton.k_yMaxJointScale);
            Joint elbowRight = skeleton.Joints[JointType.ElbowRight].ScaleTo(640, 480, KinectSkeleton.k_xMaxJointScale, KinectSkeleton.k_yMaxJointScale);

            Joint shoulderLeft = skeleton.Joints[JointType.ShoulderLeft].ScaleTo(640, 480, KinectSkeleton.k_xMaxJointScale, KinectSkeleton.k_yMaxJointScale);
            Joint shoulderRight = skeleton.Joints[JointType.ShoulderRight].ScaleTo(640, 480, KinectSkeleton.k_xMaxJointScale, KinectSkeleton.k_yMaxJointScale);

            float heightLeft = (handLeft.Position.Y - elbowLeft.Position.Y);
            float heightRight = (handRight.Position.Y - elbowRight.Position.Y);
            float frontLeft = handLeft.Position.X - shoulderLeft.Position.X;
            float frontRight = handRight.Position.X - shoulderRight.Position.X;

            
            if (DateTime.Now.Subtract(lastAction) >= TimeSpan.FromMilliseconds(1000))
            {
                System.Console.WriteLine("handleft.X : " + handLeft.Position.X);
                System.Console.WriteLine("handleft.Y : " + handLeft.Position.Y);

                if (Math.Abs(heightLeft) < heightThreshould && Math.Abs(heightRight) < heightThreshould
                    && Math.Abs(frontLeft) < frontThreshould && Math.Abs(frontRight) < frontThreshould)
                {
                    if (handler != null)
                    {
                        System.Console.WriteLine("Pick up!");
                        handler(this, EventArgs.Empty);
                    }
                }
                lastAction = DateTime.Now;
            }
        }
    }
}
