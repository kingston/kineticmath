using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;

namespace KineticMath.Kinect.Gestures
{
    public class JumpGesture : IGesture
    {
        public event EventHandler UserJumped;

        public void ProcessSkeleton(Skeleton skeleton)
        {
            // NOTE: Example - not real :O
            if (skeleton.Joints[JointType.FootRight].Position.Z > 0.5)
            {
                if (UserJumped != null)
                {
                    UserJumped(this, EventArgs.Empty);
                }
            }
        }
    }
}
