//#define LOG_SITTING
using System;
using System.Linq;
using Arges.KinectRemote.Data;

namespace Arges.KinectRemote.BodyProcessor
{
    /// <summary>
    /// Sample body processor. This case would right now be much better
    /// handled by a gesture recognizer.
    /// </summary>
    public class SittingProcessor: ABodyProcessor
    {
        /// <summary>
        /// We calculate the hip-to-head and knee-to-head ratios, and
        /// if the difference is lower than this value, then we consider
        /// the user to be sitting down.
        /// </summary>
        public float MinProportion { get; private set; }

        /// <summary>
        /// Height that the sensor is placed at, so we can displace
        /// the calculations
        /// </summary>
        public float SensorHeight { get; private set; }

        public SittingProcessor(float sensorHeight = 2, float minProportion = 0.1f)
        {
            MinProportion = minProportion;
            SensorHeight = sensorHeight;
        }

        /// <summary>
        /// Applies a heuristic to the hips and knees to determine if the
        /// user is sitting or not
        /// </summary>
        /// <param name="body">Body to evaluate</param>
        /// <returns>True if the body is sitting, false if otherwise</returns>
        protected override bool ProcessBody(KinectBody body)
        {
            var leftHip = body.Joints.FirstOrDefault(j => j.JointType == KinectJointType.HipLeft);
            var rightHip = body.Joints.FirstOrDefault(j => j.JointType == KinectJointType.HipRight);

            var leftKnee = body.Joints.FirstOrDefault(j => j.JointType == KinectJointType.KneeLeft);
            var rightKnee = body.Joints.FirstOrDefault(j => j.JointType == KinectJointType.KneeRight);

            var head = body.Joints.FirstOrDefault(j => j.JointType == KinectJointType.Head);

            if (leftHip == null || rightHip == null || leftKnee == null || rightKnee == null || head == null)
            {
                return false;
            }

            var headHeight = head.Position.Y + SensorHeight;
            var averageHip = (leftHip.Position.Y + rightHip.Position.Y + SensorHeight*2)/2;
            var hipTohead = averageHip/headHeight;
            var leftToHead = (leftKnee.Position.Y + SensorHeight)/headHeight;
            var rightToHead = (rightKnee.Position.Y + SensorHeight)/headHeight;

            var isSitting = Math.Abs(leftToHead - hipTohead) < MinProportion &&
                            Math.Abs(rightToHead - hipTohead) < MinProportion;

            if (isSitting)
            {
                body.Tags.Add("Sitting");
            }

#if LOG_SITTING
            if (isSitting)
            {
                Console.WriteLine("HEAD {3} Left {4} headToHip {0} leftToHead {1} rightToHead {2}", hipTohead, leftToHead, rightToHead, headHeight, leftKnee.Position);
            }
#endif

            return isSitting;
        }
    }
}
