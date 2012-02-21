using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;

namespace KineticMath.Kinect
{
    /// <summary>
    /// Dummy kinect service that does nothing (used if Kinect is not available)
    /// </summary>
    public class DummyKinectService : IKinectService
    {
        public event EventHandler<SkeletonEventArgs> SkeletonUpdated = null;

        public event EventHandler<ColorImageFrameReadyEventArgs> ImageFrameReady = null;

        public bool Initialize()
        {
            return true; // we're always happy :)
        }

        public void Uninitialize()
        {
            // A very hacky way to avoid warnings for the two events above not being used :O
            int x = 0;
            if (x == 1)
            {
                SkeletonUpdated(null, null);
                ImageFrameReady(null, null);
            }
        }
    }
}
