using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Kinect;
using KineticMath.SubControls;

namespace KineticMath.Kinect.Gestures
{
    public class JumpGesture : IGesture
    {
        public event EventHandler UserJumped;

        public void ProcessSkeleton(Skeleton skeleton)
        {
            Joint handLeft = skeleton.Joints[JointType.HandLeft].ScaleTo(640, 480, KinectSkeleton.k_xMaxJointScale, KinectSkeleton.k_yMaxJointScale);
            // NOTE: Example - not real :O
            System.Console.WriteLine("Handleft.x = " + handLeft.Position.Y);
            if ( handLeft.Position.Y < 90)
            {
                if (UserJumped != null)
                {
                    UserJumped(this, EventArgs.Empty);
                }
           }
        }
    }
}
