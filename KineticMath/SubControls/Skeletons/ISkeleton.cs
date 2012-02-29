using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KineticMath.Kinect;
using Microsoft.Kinect;

namespace KineticMath.SubControls.Skeletons
{
    interface ISkeletonControl
    {
        void SkeletonUpdated(object sender, SkeletonEventArgs e);
        void ImageFrameReady(object sender, ColorImageFrameReadyEventArgs e);
    }
}
