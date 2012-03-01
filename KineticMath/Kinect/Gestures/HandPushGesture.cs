using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;

using KineticMath.Helpers;

namespace KineticMath.Kinect.Gestures
{
    /// <summary>
    /// TODO2: Explain how this works
    /// </summary>
    public class HandPushGesture : HistoryGestureBase
    {
        public event EventHandler<HandPushedEventArgs> HandPushed;
        private const double MAX_XY_DISTANCE = 0.05; // The maximum distance a gesture can happen outside the XY plane
        private const double Z_THRESHOLD = 0.10; // The depth threshold that will trigger the gesture

        public override void OnProcessSkeleton(Skeleton skeleton)
        {
            JointType[] activeJointTypes = new JointType[] { JointType.HandLeft, JointType.HandRight };
            foreach (var jointType in activeJointTypes) {
                // Do processing based off history
                var curPos = skeleton.Joints[jointType].Position;
                string debugMessage = "SKELETAL HISTORY\n";
                bool cancel = false;
                for (int i = this.skeletalHistory.Count - 1; i >= 0; i--)
                {
                    var frame = this.skeletalHistory[i];
                    var pos = frame.Skeleton.Joints[jointType].Position;
                    debugMessage += pos.ToReadableString() + "\n";
                    if (!IsWithin(pos.X, curPos.X, MAX_XY_DISTANCE) || !IsWithin(pos.Y, curPos.Y, MAX_XY_DISTANCE)) cancel = true;
                    if (pos.Z - curPos.Z > Z_THRESHOLD && !cancel)
                    {
                        skeletalHistory.Clear();
                        if (HandPushed != null) HandPushed(this, new HandPushedEventArgs() { Joint = jointType, Position = skeleton.Joints[jointType].Position });
                        return;
                    }
                }
                DebugHelper.GetInstance().LogMessage(debugMessage);
            }
        }

        private bool IsWithin(double val, double targetVal, double roughness)
        {
            return (targetVal - roughness < val) && (val < targetVal + roughness);
        }
    }

    public class HandPushedEventArgs : EventArgs {
        public JointType Joint { get; set; }
        public SkeletonPoint Position { get; set; }
    }
}
