using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;
using KineticMath.Kinect.Gestures;

namespace KineticMath.Kinect
{
    public class GestureController
    {
        private List<IGesture> gestures;
        private IKinectService kinect;

        public GestureController(IKinectService kinect)
        {
            gestures = new List<IGesture>();
            this.kinect = kinect;
            this.kinect.SkeletonUpdated += new EventHandler<SkeletonEventArgs>(kinect_SkeletonUpdated);
        }

        void kinect_SkeletonUpdated(object sender, SkeletonEventArgs e)
        {
           
            Skeleton skel = (from s in e.Skeletons
                                     where s.TrackingState == SkeletonTrackingState.Tracked
                                     select s).FirstOrDefault();

            if (skel != null)
            {
                foreach (var gesture in gestures)
                {
                    gesture.ProcessSkeleton(skel);
                }
            }
        }

        /// <summary>
        /// Adds a gesture to the gesture recognizer's set of gestures to recognize
        /// </summary>
        /// <param name="gesture">The gesture to add</param>
        public void AddGesture(IGesture gesture)
        {
            gestures.Add(gesture);
        }

        public void RemoveGesture(IGesture gesture)
        {
            gestures.Remove(gesture);
        }
    }
}
