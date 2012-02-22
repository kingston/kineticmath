using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Kinect;
using KineticMath.SubControls;

namespace KineticMath.Kinect.Gestures
{
    class EnergizeGesture : IGesture
    {
        private DateTime lastAction = DateTime.Now;
        private float timelength = 1;
        float heightThreshould = -30;
        bool clutch = false;
        float initDistance = 0;

        private float preHandLeftY = 0;
        private float preHandRightY = 0;
        private float currentHandLeftY = 0;
        private float currentHandRightY = 0;

        float dyThreshould = -15;

        public delegate void EnergyEventHandler(object sender, EnergyEventArgs m);
        public event EnergyEventHandler UserEnergized;

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

            currentHandLeftY = handLeft.Position.Y;
            currentHandRightY = handRight.Position.Y;

            if (DateTime.Now.Subtract(lastAction) >= TimeSpan.FromSeconds(timelength))
            {
                //System.Console.WriteLine("heightLeft = " + heightLeft + " heightRight = " + heightRight);

                if (!clutch && heightLeft < heightThreshould && heightRight < heightThreshould)
                {
                    if (UserEnergized != null)
                    {
                        System.Console.WriteLine("UserEnergized");
                        clutch = true;
                        initDistance = Math.Abs(handLeft.Position.X - handRight.Position.X);
                    }
                }

                if (clutch && (heightLeft >= heightThreshould || heightRight >= heightThreshould)) {
                    clutch = false;
                    UserEnergized(this, new EnergyEventArgs(0));
                }

                if (clutch) {
                    float dist = Math.Abs(handLeft.Position.X - handRight.Position.X);
                    //System.Console.WriteLine("dist = " + dist);
                    if(dist <= 80)
                        UserEnergized(this, new EnergyEventArgs(1));
                    else if( dist <= 120)
                        UserEnergized(this, new EnergyEventArgs(2));
                    else if( dist <= 160)
                        UserEnergized(this, new EnergyEventArgs(3));
                    else if( dist <= 200)
                        UserEnergized(this, new EnergyEventArgs(4));
                    else if( dist <= 240)
                        UserEnergized(this, new EnergyEventArgs(5));

                    float leftDy = currentHandLeftY - preHandLeftY;
                    float rightDy = currentHandRightY - preHandRightY;

                    if ((leftDy < dyThreshould) || (rightDy < dyThreshould))
                    {
                        System.Console.WriteLine("leftDy = " + leftDy + "  rightDy = " + rightDy);
                        UserEnergized(this, new EnergyEventArgs(-1));
                        clutch = false;
                    }
                }

                preHandLeftY = currentHandLeftY;
                preHandRightY = currentHandRightY;
                lastAction = DateTime.Now;
            }
        }
    }
}
