using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KineticMath.Kinect.Gestures
{
    /// <summary>
    /// Gesture that keeps track of the history of movements
    /// </summary>
    public abstract class HistoryGestureBase : BaseGesture
    {
        protected List<SkeletonTimeEntry> skeletalHistory = new List<SkeletonTimeEntry>();
        protected int maxTimeToLive = 1000; // number of milliseconds to keep a history for

        public override void ProcessSkeleton(Skeleton skeleton)
        {
            // Clean up old entries
            if (skeletalHistory.Count > 0)
            {
                while (DateTime.Now.Subtract(skeletalHistory.First().Time).Milliseconds > maxTimeToLive)
                {
                    skeletalHistory.RemoveAt(0);
                }
            }
            // TODO3: Reset skeletal history when a new skeleton is used
            skeletalHistory.Add(new SkeletonTimeEntry() { Time = DateTime.Now, Skeleton = skeleton });
            base.ProcessSkeleton(skeleton);
        }

        protected struct SkeletonTimeEntry
        {
            public DateTime Time;
            public Skeleton Skeleton;
        }
    }
}
