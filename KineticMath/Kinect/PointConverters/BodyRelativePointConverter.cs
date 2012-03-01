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
    /// 
    /// NOTE: Requires the person to be standing
    /// </summary>
    public class BodyRelativePointConverter : IPointConverter
    {
        // Using rough body proportions as such:
        //  Shoulder to top of head: 40
        //  Arm length: 80
        //  Hip to Shoulder: 50
        //  Foot to hip: 100
        // From that:
        //  Shoulder to foot: 150
        //  Foot to Extended Arm: 230
        //  Foot to Half Way Point: 115 (just above hip)
        private const int SHOULDER_TO_EXTENDED_ARM_RATIO = 230 / 150;

        // TODO: Add a meaningful scale for the scale factor
        private float scaleFactor = 1.0f; //The scale factor to scale from points in Kinect space to canvas space
        private SkeletonPoint bottomCenterPoint; // The bottom center location of the skeleton
        private int curSkeletonId = 0;

        public BodyRelativePointConverter(Rect activeRect, GestureController controller)
        {
            this.ActiveRectangle = activeRect;
            controller.SkeletonPreProcessed += new EventHandler<SkeletonPreProcessedEventArgs>(controller_SkeletonPreProcessed);
        }

        void controller_SkeletonPreProcessed(object sender, SkeletonPreProcessedEventArgs e)
        {
            // Process core
            Skeleton skel = e.Skeleton;
            SkeletonPoint leftFoot = skel.Joints[JointType.FootLeft].Position;
            SkeletonPoint rightFoot = skel.Joints[JointType.FootRight].Position;
            float minFootY = Math.Min(leftFoot.Y, rightFoot.Y);
            if (skel.TrackingId != curSkeletonId && skel.Joints[JointType.ShoulderCenter].TrackingState == JointTrackingState.Tracked)
            {
                SkeletonPoint shoulderPos = skel.Joints[JointType.ShoulderCenter].Position;

                // ## Compute scale factor for larger/smaller people based off spine to foot height ##
                double shoulderHeight = shoulderPos.Y - minFootY; // Get the spinal height and use it a rough guide
                double fullHeight = SHOULDER_TO_EXTENDED_ARM_RATIO * shoulderHeight; // Get the full height as a scale
                scaleFactor = (float) (ActiveRectangle.Height / fullHeight);

                curSkeletonId = skel.TrackingId;
            }
            bottomCenterPoint = skel.Joints[JointType.HipCenter].Position;
            bottomCenterPoint.Y = minFootY;
        }

        /// <summary>
        /// The rectangle to scale the skeleton to (it should be approximately 5:4 scaled)
        /// </summary>
        public Rect ActiveRectangle { get; set; }

        public SkeletonPoint ConvertPoint(SkeletonPoint point)
        {
            SkeletonPoint pt = new SkeletonPoint();
            pt.X = ScalePoint(pt.X, bottomCenterPoint.X) - (float) ActiveRectangle.Width / 2 + (float) ActiveRectangle.X;
            pt.Y = (float) ActiveRectangle.Height - ScalePoint(pt.X, bottomCenterPoint.Y) + (float) ActiveRectangle.Y;
            pt.Z = ScalePoint(pt.Z, bottomCenterPoint.Z);
            return point;
        }

        private float ScalePoint(float val, float center)
        {
            return (val - center) * scaleFactor;
        }
    }
}
