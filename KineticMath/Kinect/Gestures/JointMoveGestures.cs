using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;

namespace KineticMath.Kinect.Gestures
{
    public class JointMoveGestures : BaseGesture
    {
        public JointType JointType { get; private set; }
        public event EventHandler<JointMovedEventArgs> JointMoved;

        public JointMoveGestures(JointType type)
        {
            this.JointType = type;
        }

        public override void OnProcessSkeleton(Skeleton skeleton)
        {
            Joint joint = skeleton.Joints[this.JointType];
            if (joint.TrackingState != JointTrackingState.NotTracked)
            {
                SkeletonPoint point = joint.Position;
                if (JointMoved != null)
                {
                    JointMoved(this, new JointMovedEventArgs() { JointType = this.JointType, NewPosition = point });
                }
            }
        }
    }

    public class JointMovedEventArgs : EventArgs
    {
        public JointType JointType { get; set; }
        public SkeletonPoint NewPosition { get; set; }
    }
}
