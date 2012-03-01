using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Kinect;
using KineticMath.SubControls;

namespace KineticMath.Kinect.Gestures
{
    public class JointMoveGestures : BaseGesture
    {
        public JointType[] JointTypes { get; private set; }
        public event EventHandler<JointMovedEventArgs> JointMoved;

        public JointMoveGestures(params JointType[] types)
        {
            this.JointTypes = types;
        }

        public override void OnProcessSkeleton(Skeleton skeleton)
        {
            foreach (var type in this.JointTypes)
            {
                Joint joint = skeleton.Joints[type];
                if (joint.TrackingState != JointTrackingState.NotTracked)
                {
                    if (JointMoved != null)
                    {
                        JointMoved(this, new JointMovedEventArgs() { JointType = type, NewPosition = joint.Position });
                    }
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
