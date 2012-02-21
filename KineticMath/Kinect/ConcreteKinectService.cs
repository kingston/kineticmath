using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect;

namespace KineticMath.Kinect
{
    public class ConcreteKinectService : IKinectService
    {
        private KinectSensor sensor;

        public bool Initialize()
        {
            if (KinectSensor.KinectSensors.Count == 0) return false;
            sensor = KinectSensor.KinectSensors[0];
            sensor.SkeletonFrameReady +=
                new EventHandler<SkeletonFrameReadyEventArgs>(runtime_SkeletonFrameReady);
            sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
            sensor.SkeletonStream.Enable();
            sensor.ColorStream.Enable();
            sensor.Start();
            return true;
        }

        public void Uninitialize()
        {
            if (sensor != null)
            {
                sensor.Stop();
            }
        }

        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            if (ImageFrameReady != null)
            {
                ImageFrameReady(this, e);
            }
        }

        void runtime_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = null;
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
                // Skeleton frame is null if processing took too long
            }
            if (skeletons != null)
            {
                if (this.SkeletonUpdated != null)
                {
                    this.SkeletonUpdated(this, new SkeletonEventArgs(skeletons));
                }
            }
        }

        public event EventHandler<SkeletonEventArgs> SkeletonUpdated;
        public event EventHandler<ColorImageFrameReadyEventArgs> ImageFrameReady;
    }
}
