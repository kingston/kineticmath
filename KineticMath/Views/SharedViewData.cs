using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KineticMath.Kinect;

namespace KineticMath.Views
{
    /// <summary>
    /// Data shared between all views
    /// </summary>
    public class SharedViewData
    {
        public IKinectService KinectService { get; set; }
        public SkeletonController SkeletonController { get; set; }
        public GestureController PlayerOneController { get; set; }
        public GestureController PlayerTwoController { get; set; }
    }
}
