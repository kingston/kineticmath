using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Kinect;
using KineticMath.SubControls;

namespace KineticMath.Kinect.Gestures
{
    class AlwaysGesture : IGesture
    {
        private DateTime lastAction = DateTime.Now;
        private float timelength = 1;
        float heightThreshould = 20;
        float frontThreshould = 50;
        public event EventHandler<AlwaysEventArgs> handler;

        public void ProcessSkeleton(Skeleton skeleton)
        {
            Joint handLeft = skeleton.Joints[JointType.HandLeft].ScaleTo(640, 480, KinectSkeleton.k_xMaxJointScale, KinectSkeleton.k_yMaxJointScale);
            Joint handRight = skeleton.Joints[JointType.HandRight].ScaleTo(640, 480, KinectSkeleton.k_xMaxJointScale, KinectSkeleton.k_yMaxJointScale);
            Joint footLeft = skeleton.Joints[JointType.FootLeft].ScaleTo(640, 480, KinectSkeleton.k_xMaxJointScale, KinectSkeleton.k_yMaxJointScale);
            Joint footRight = skeleton.Joints[JointType.FootRight].ScaleTo(640, 480, KinectSkeleton.k_xMaxJointScale, KinectSkeleton.k_yMaxJointScale);
            
            if (handler != null)
            {
                handler(this, new AlwaysEventArgs() { _handLeft = handLeft,
                                                        _handRight= handRight,
                                                          _footLeft= footLeft,
                                                        _footRight= footRight
                                                    });
            }
        }

        public class AlwaysEventArgs : EventArgs
        {
            public Joint _handLeft { get; set; }
            public Joint _handRight { get; set; }
            public Joint _footLeft { get; set; }
            public Joint _footRight { get; set; }
        }
    }
}
