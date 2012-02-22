using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KineticMath.Kinect.Gestures
{
    class SwipeGesture : IGesture
    {
        public event EventHandler SwipeGestureMade;

        public SwipeGesture()
        {
            this.Active = false;
        }

        public void ProcessSkeleton(Skeleton skeleton)
        {
            if (Active) {
                if (IsOut(skeleton))
                {
                    Active = false;
                }
                else if (IsComplete(skeleton))
                {
                    if (SwipeGestureMade != null) SwipeGestureMade(this, EventArgs.Empty);
                    Active = false;
                }
            }
            else {
                if (GetTriggerScore(skeleton) > 0)
                {
                    Active = true;
                }
            }
        }

        public bool Active { get; set; }

        protected bool IsInBoundingBox(SkeletonPoint pt, SkeletonPoint referencePoint, SkeletonPoint relativeCenter, SkeletonPoint size)
        {

            SkeletonPoint boxCenter = referencePoint.Add(relativeCenter);
            return (boxCenter.X - size.X / 2 < pt.X && pt.X < boxCenter.X + size.X / 2 &&
                    boxCenter.Y - size.Y / 2 < pt.Y && pt.Y < boxCenter.Y + size.Y / 2 &&
                    boxCenter.Z - size.Z / 2 < pt.Z && pt.Z < boxCenter.Z + size.Z / 2);
        }

        protected static SkeletonPoint CreateSkeletonPoint(float x, float y, float z)
        {
            return GestureExtensions.CreateSkeletonPoint(x, y, z);
        }

        public double GetTriggerScore(Skeleton skeleton)
        {
            Joint handRight = skeleton.Joints[JointType.HandRight];
            Joint elbowRight = skeleton.Joints[JointType.ElbowRight];
            Joint central = skeleton.Joints[JointType.Spine];

            SkeletonPoint boundingBox = CreateSkeletonPoint(0.2f, 0.3f, 10.0f);
            SkeletonPoint relativeToCentral = CreateSkeletonPoint(-0.3f, 0.5f, 0.3f);

            if (IsInBoundingBox(handRight.Position, central.Position, relativeToCentral, boundingBox))
            {
                return 1.0;
            }
            return 0.0;
        }

        public bool IsOut(Skeleton skeleton)
        {
            if (!Active) return false;
            Joint handRight = skeleton.Joints[JointType.HandRight];
            Joint elbowRight = skeleton.Joints[JointType.ElbowRight];
            Joint central = skeleton.Joints[JointType.Spine];

            SkeletonPoint boundingBox = CreateSkeletonPoint(2.0f, 0.3f, 10.0f);
            SkeletonPoint relativeToCentral = CreateSkeletonPoint(0f, 0.5f, 0.3f);
            return (!IsInBoundingBox(handRight.Position, central.Position, relativeToCentral, boundingBox));
        }

        public bool IsComplete(Skeleton skeleton)
        {
            if (!Active) return false;
            Joint handRight = skeleton.Joints[JointType.HandRight];
            Joint elbowRight = skeleton.Joints[JointType.ElbowRight];
            Joint central = skeleton.Joints[JointType.Spine];

            SkeletonPoint boundingBox = CreateSkeletonPoint(0.2f, 0.3f, 10.0f);
            SkeletonPoint relativeToCentral = CreateSkeletonPoint(0.3f, 0.5f, 0.3f);

            return IsInBoundingBox(handRight.Position, central.Position, relativeToCentral, boundingBox);
        }
    }

    public static class GestureExtensions
    {
        public static SkeletonPoint CreateSkeletonPoint(float x, float y, float z)
        {
            return new SkeletonPoint() { X = x, Y = y, Z = z };
        }

        public static SkeletonPoint Add(this SkeletonPoint SkeletonPoint1, SkeletonPoint SkeletonPoint2)
        {
            return CreateSkeletonPoint(SkeletonPoint1.X + SkeletonPoint2.X, SkeletonPoint1.Y + SkeletonPoint2.Y, SkeletonPoint1.Z + SkeletonPoint1.Z);
        }

        public static SkeletonPoint Subtract(this SkeletonPoint SkeletonPoint1, SkeletonPoint SkeletonPoint2)
        {
            return CreateSkeletonPoint(SkeletonPoint1.X - SkeletonPoint2.X, SkeletonPoint1.Y - SkeletonPoint2.Y, SkeletonPoint1.Z - SkeletonPoint1.Z);
        }

        public static string ToReadableString(this SkeletonPoint SkeletonPoint)
        {

            return "X: " + SkeletonPoint.X + " | Y: " + SkeletonPoint.Y + " | Z: " + SkeletonPoint.Z;
        }
    }
}
