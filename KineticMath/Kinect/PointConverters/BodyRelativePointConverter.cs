using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Microsoft.Kinect;

namespace KineticMath.Kinect.PointConverters
{
    /// <summary>
    /// Converts points from KinectSpace to a provided rectangle fixing the center of the body as the center
    /// </summary>
    public class BodyRelativePointConverter : IPointConverter
    {
        public BodyRelativePointConverter(Rect activeRect, GestureController controller)
        {
            this.ActiveRectangle = activeRect;
            controller.SkeletonPreProcessed += new EventHandler<SkeletonPreProcessedEventArgs>(controller_SkeletonPreProcessed);
        }

        void controller_SkeletonPreProcessed(object sender, SkeletonPreProcessedEventArgs e)
        {
        }

        /// <summary>
        /// The rectangle to scale the skeleton to (it should be approximately 4:3 scaled)
        /// </summary>
        public Rect ActiveRectangle { get; set; }

        public SkeletonPoint ConvertPoint(SkeletonPoint point)
        {
            // TODO: Actually implement conversion
            return point;
        }
    }
}
