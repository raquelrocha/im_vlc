using System;
using Microsoft.Kinect;
using Paelife.KinectFramework.Gestures;

namespace VLC_KinectControl
{
    /// <summary>
    /// An implementation of a <see cref="GestureDetector"/>
    /// that detects swipes motions.
    /// </summary>
    public class UpDownGestures : GestureDetector
    {
        /// <summary>
        /// The minimal lenght the swipe gesture should have.
        /// </summary>
        public float SwipeMinimalLength {get;set;}

        /// <summary>
        /// The maximal lenght the swipe gesture should have.
        /// </summary>
        public float SwipeMaximalHeight {get;set;}

        /// <summary>
        /// The minimal duration the swipe gesture should have.
        /// </summary>
        public int SwipeMininalDuration {get;set;}

        /// <summary>
        /// The maximal duration the swipe gesture should have.
        /// </summary>
        public int SwipeMaximalDuration {get;set;}


        /// <summary>
        /// Default constructor that receives the joint that should be tracked and the
        /// number of recorded positions that are considered when detecting gestures.
        /// </summary>
        public UpDownGestures(JointType trackedJoint, int windowSize = 20)
            : base(trackedJoint, windowSize)
        {
            SwipeMinimalLength = 0.4f;
            SwipeMaximalHeight = 0.2f;
            SwipeMininalDuration = 250;
            SwipeMaximalDuration = 1500;
            MinimalPeriodBetweenGestures = 1000;
        }

        //! @cond
        protected bool ScanPositions(Func<Vector3, Vector3, bool> heightFunction, Func<Vector3, Vector3, bool> directionFunction, 
            Func<Vector3, Vector3, bool> lengthFunction, int minTime, int maxTime)
        {
            int start = 0;

            for (int index = 1; index < Entries.Count - 1; index++)
            {
                if (!heightFunction(Entries[0].Position, Entries[index].Position) || !directionFunction(Entries[index].Position, Entries[index + 1].Position))
                {
                    start = index;
                }

                if (lengthFunction(Entries[index].Position, Entries[start].Position))
                {
                    double totalMilliseconds = (Entries[index].Time - Entries[start].Time).TotalMilliseconds;
                    if (totalMilliseconds >= minTime && totalMilliseconds <= maxTime)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        //! @endcond

        /// <summary>
        /// The implementation of the method that will actualy look for swipes.
        /// </summary>
        protected override void LookForGesture()
        {
            // from up to down
            if (ScanPositions((p1, p2) => Math.Abs(p2.X - p1.X) < 0.20f,
              (p1, p2) => p2.Y - p1.Y < 0.01f, 
              (p1, p2) => Math.Abs(p2.Y - p1.Y) > 0.2f, 250, 2500))
            {
                RaiseGestureDetected("SwipeDown");
                return;
            }

            // from down to up
            if (ScanPositions((p1, p2) => Math.Abs(p2.X - p1.X) < 0.20f,
              (p1, p2) => p2.Y - p1.Y > -0.01f,
              (p1, p2) => Math.Abs(p2.Y - p1.Y) > 0.2f, 250, 2500))
            {
                RaiseGestureDetected("SwipeUp");
                return;
            }

        }
    }
}