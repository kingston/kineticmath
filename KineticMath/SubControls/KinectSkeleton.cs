using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using KineticMath.Kinect;

namespace KineticMath.SubControls
{
    public class KinectSkeletion : UserControl
    {
        protected IKinectService kinectService;
        //Scaling constants
        public static float k_xMaxJointScale = 1.5f;
        public static float k_yMaxJointScale = 1.5f;
    }
}
