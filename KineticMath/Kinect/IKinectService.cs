using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;

namespace KineticMath.Kinect
{
    /// <summary>
    /// Concept borrowed from http://kindohm.com/2011/07/20/KinectMvvm.html
    /// </summary>
    public interface IKinectService
    {
        event EventHandler<SkeletonEventArgs> SkeletonUpdated;
        event EventHandler<ColorImageFrameReadyEventArgs> ImageFrameReady;
        bool Initialize();
        void Uninitialize();
    }
}
