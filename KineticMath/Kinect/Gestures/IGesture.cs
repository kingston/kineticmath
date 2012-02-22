using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows;


namespace KineticMath.Kinect.Gestures
{
    
    public interface IGesture
    {
        void ProcessSkeleton(Skeleton skeleton);
    }
}
