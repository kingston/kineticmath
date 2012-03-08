using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;
using KineticMath.Kinect.Gestures;
using KineticMath.Views;

namespace KineticMath.Kinect
{
    public class GestureController
    {
        private List<IGesture> gestures = new List<IGesture>();
        private List<IGesture> removedGestures = new List<IGesture>();
        private List<IGesture> addedGestures = new List<IGesture>();
        private Dictionary<IView, List<IGesture>> gestureMap = new Dictionary<IView, List<IGesture>>();
        private IKinectService kinect;

        // An event to allow PointControllers to pre-process the skeleton prior to gesture processing
        public event EventHandler<SkeletonPreProcessedEventArgs> SkeletonPreProcessed;

        public GestureController(IKinectService kinect)
        {
            this.kinect = kinect;
            this.kinect.SkeletonUpdated += new EventHandler<SkeletonEventArgs>(kinect_SkeletonUpdated);
        }

        void kinect_SkeletonUpdated(object sender, SkeletonEventArgs e)
        {
            Skeleton skel = (from s in e.Skeletons
                                     where s.TrackingState == SkeletonTrackingState.Tracked
                                     select s).FirstOrDefault();
            foreach (var gesture in removedGestures)
            {
                gestures.Remove(gesture);
            }

            foreach (var gesture in addedGestures)
            {
                gestures.Add(gesture);
            }
            removedGestures.Clear();
            addedGestures.Clear();
            if (skel != null)
            {
                if (SkeletonPreProcessed != null)
                {
                    SkeletonPreProcessed(this, new SkeletonPreProcessedEventArgs() { Skeleton = skel });
                }
                foreach (var gesture in gestures)
                {
                    gesture.ProcessSkeleton(skel);
                }
            }
        }

        /// <summary>
        /// Adds a gesture to the gesture recognizer's set of gestures to recognize
        /// </summary>
        /// <param name="currentView">The current view to associate with the gesture</param>
        /// <param name="gesture">The gesture to add</param>
        public void AddGesture(IView currentView, IGesture gesture)
        {
            if (currentView != null)
            {
                if (!gestureMap.ContainsKey(currentView)) gestureMap.Add(currentView, new List<IGesture>());
                gestureMap[currentView].Add(gesture);
            }
            addedGestures.Add(gesture);
        }

        public void RemoveGesture(IGesture gesture)
        {
            removedGestures.Add(gesture);
            //gestures.Remove(gesture);
        }

        public void ClearViewGestures(IView view)
        {
            if (gestureMap.ContainsKey(view))
            {
                foreach (var gesture in gestureMap[view])
                {
                    this.RemoveGesture(gesture);
                }
                gestureMap[view].Clear();
            }
        }

    }

    public class SkeletonPreProcessedEventArgs : EventArgs
    {
        public Skeleton Skeleton { get; set; }
    }
}
