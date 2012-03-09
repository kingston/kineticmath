using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Kinect;
using KineticMath.SubControls;
using System.Windows;
using KineticMath.Kinect.PointConverters;
using KineticMath.Helpers;

namespace KineticMath.Kinect.Gestures
{
    public class HitGesture : HistoryGestureBase
    {
        // TODO: Find a way to remove HitRectangles from detection once they've been chosen
        private JointType[] joints;
        private IPointConverter pointConverter;

        public List<Rect> HitRectangles { get; private set; }

        public event EventHandler<RectHitEventArgs> RectHit;

        public HitGesture(List<Rect> hitRectangles, IPointConverter pointConverter, params JointType[] joints) {
            if (hitRectangles == null) throw new ArgumentNullException("hitRectangles");
            if (pointConverter == null) throw new ArgumentNullException("pointConverter");
            this.maxTimeToLive = 200;
            this.joints = joints;
            this.HitRectangles = hitRectangles.ToList();
            this.pointConverter = pointConverter;
        }

        public override void OnProcessSkeleton(Skeleton skeleton)
        {
            // Check if joints hit anything
            foreach (JointType type in joints)
            {
                SkeletonPoint pt = pointConverter.ConvertPoint(skeleton.Joints[type].Position);
                if (type == JointType.HandLeft)
                {
                    //DebugHelper.GetInstance().LogMessage(this.HitRectangles[1].ToString() + " - " + pt.ToReadableString());
                }
                int rectIdx = HitTestRects(pt);
                if (rectIdx > -1)
                {
                    Vector velocity = ComputeJointVelocity(type);
                    // Max velocity
                    if (Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y) > 10)
                    {
                        if (RectHit != null)
                        {
                            RectHit(this, new RectHitEventArgs() { Joint = type, HitVelocity = velocity, RectIdx = rectIdx });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Computes the velocity of a joint given the joint history
        /// </summary>
        /// <param name="type">The type of joint to compute the velocity of</param>
        /// <returns>A vector containing the velocity of the joint</returns>
        private Vector ComputeJointVelocity(JointType type)
        {
            SkeletonPoint oldPt = pointConverter.ConvertPoint(this.skeletalHistory.First().Skeleton.Joints[type].Position);
            SkeletonPoint newPt = pointConverter.ConvertPoint(this.skeletalHistory.Last().Skeleton.Joints[type].Position);
            return Point.Subtract(newPt.To2DPoint(), oldPt.To2DPoint()) / this.maxTimeToLive * 100; // Want the velocity for the past 50ms
        }

        /// <summary>
        /// Checks whether the joint hit a rectangle and if so, return the index of the rect it hit
        /// </summary>
        /// <param name="skelPt">The skeleton point to check</param>
        /// <returns>The index of the rectangle hit if found; otherwise -1</returns>
        private int HitTestRects(SkeletonPoint skelPt)
        {
            Point pt = skelPt.To2DPoint();
            for (int i = 0; i < HitRectangles.Count; i++)
            {
                if (HitRectangles[i].Contains(pt))
                {
                    return i;
                }
            }
            return -1;
        }
    }

    public class RectHitEventArgs : EventArgs
    {
        public JointType Joint { get; set; }
        public int RectIdx { get; set; }
        public Vector HitVelocity { get; set; }
    }
}
