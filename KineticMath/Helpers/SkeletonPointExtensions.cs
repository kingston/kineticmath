using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Kinect;

namespace KineticMath.Helpers
{
    public static class SkeletonPointExtensions
    {
        public static Point To2DPoint(this SkeletonPoint pt)
        {
            return new Point(pt.X, pt.Y);
        }
    }
}
