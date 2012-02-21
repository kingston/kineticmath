using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;

namespace KineticMath.Kinect
{
    public class SkeletonEventArgs : EventArgs
    {
        public SkeletonEventArgs(Skeleton[] skeletons)
        {
            this.Skeletons = skeletons;
        }

        public Skeleton[] Skeletons { get; private set; }
    }
}
