using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;

namespace KineticMath.Kinect.Gestures
{
    public class HandPushGesture : HistoryGestureBase
    {
        public event EventHandler<HandPushedEventArgs> HandPushed;

        public override void OnProcessSkeleton(Skeleton skeleton)
        {
            JointType[] activeJointTypes = new JointType[] { JointType.HandLeft, JointType.HandRight };
            foreach (var jointType in activeJointTypes) {
                // Do processing based off history
                foreach (var frame in this.skeletalHistory)
                {
                    // Do stuff
                }
                if (true)
                {
                    if (HandPushed != null) HandPushed(this, new HandPushedEventArgs() { Joint = jointType, Position = skeleton.Joints[jointType].Position });
                }
            }
        }
    }

    public class HandPushedEventArgs : EventArgs {
        public JointType Joint { get; set; }
        public SkeletonPoint Position { get; set; }
    }
}
