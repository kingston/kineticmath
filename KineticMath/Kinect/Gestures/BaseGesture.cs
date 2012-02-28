using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using KineticMath.SubControls;

namespace KineticMath.Kinect.Gestures
{
    public abstract class BaseGesture : IGesture
    {
        public virtual void ProcessSkeleton(Skeleton skeleton)
        {
            OnProcessSkeleton(skeleton);
        }

        public abstract void OnProcessSkeleton(Skeleton skeleton);

        /// <summary>
        /// Scales a joint to a fixed 640 by 480 plane
        /// </summary>
        /// <param name="skel"></param>
        /// <param name="joint"></param>
        /// <returns></returns>
        private Joint GetScaledJoint(Skeleton skel, JointType joint)
        {
            return skel.Joints[JointType.HandLeft].ScaleTo(640, 480, KinectSkeleton.k_xMaxJointScale, KinectSkeleton.k_yMaxJointScale);
        }

        private Joint GetJointScaledToScreen(Skeleton skel, JointType joint)
        {
            // TODO: Don't hard code values
            return skel.Joints[JointType.HandLeft].ScaleTo(1200, 675, KinectSkeleton.k_xMaxJointScale, KinectSkeleton.k_yMaxJointScale);
        }
    }
}
