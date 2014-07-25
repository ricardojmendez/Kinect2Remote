using System;
using ProtoBuf;

namespace Arges.KinectRemote.Data
{
    /// <summary>
    /// Encapsulates information about a joint
    /// </summary>
    [Serializable, ProtoContract]
    public class KinectJoint
    {
        /// <summary>
        /// Joint position
        /// </summary>
        [ProtoMember(1)] 
        public KinectVector3 Position;

        /// <summary>
        /// The tracking state of this joint
        /// </summary>
        [ProtoMember(2)]
        public KinectJointTrackingState TrackingState;

        /// <summary>
        /// Type of the joint
        /// </summary>
        [ProtoMember(3)]
        public KinectJointType JointType;

        /// <summary>
        /// Hierarchical Orientation of the joint
        /// </summary>
        [ProtoMember(4)]
        public KinectVector4 Orientation;

        public static bool IsJointMirrorable(int jointIndex)
        {
            return jointIndex != GetSkeletonMirroredJoint(jointIndex);
        }

        /// <summary>
        /// Returns the equivalent mirrored joint for a joint index (eg., left hand for right hand)
        /// </summary>
        /// <param name="jointIndex">Joint to return the mirror for</param>
        /// <returns>Mirrored joint index, or the same index if it's not mirrorable</returns>
        public static int GetSkeletonMirroredJoint(int jointIndex)
        {
            switch (jointIndex)
            {
                case (int)KinectJointType.ShoulderLeft:
                    return (int)KinectJointType.ShoulderRight;
                case (int)KinectJointType.ElbowLeft:
                    return (int)KinectJointType.ElbowRight;
                case (int)KinectJointType.WristLeft:
                    return (int)KinectJointType.WristRight;
                case (int)KinectJointType.HandLeft:
                    return (int)KinectJointType.HandRight;
                case (int)KinectJointType.HandTipLeft:
                    return (int)KinectJointType.HandTipRight;
                case (int)KinectJointType.ThumbLeft:
                    return (int)KinectJointType.ThumbRight;
                case (int)KinectJointType.ShoulderRight:
                    return (int)KinectJointType.ShoulderLeft;
                case (int)KinectJointType.ElbowRight:
                    return (int)KinectJointType.ElbowLeft;
                case (int)KinectJointType.WristRight:
                    return (int)KinectJointType.WristLeft;
                case (int)KinectJointType.HandRight:
                    return (int)KinectJointType.HandLeft;
                case (int)KinectJointType.HandTipRight:
                    return (int)KinectJointType.HandTipLeft;
                case (int)KinectJointType.ThumbRight:
                    return (int)KinectJointType.ThumbLeft;
                case (int)KinectJointType.HipLeft:
                    return (int)KinectJointType.HipRight;
                case (int)KinectJointType.KneeLeft:
                    return (int)KinectJointType.KneeRight;
                case (int)KinectJointType.AnkleLeft:
                    return (int)KinectJointType.AnkleRight;
                case (int)KinectJointType.FootLeft:
                    return (int)KinectJointType.FootRight;
                case (int)KinectJointType.HipRight:
                    return (int)KinectJointType.HipLeft;
                case (int)KinectJointType.KneeRight:
                    return (int)KinectJointType.KneeLeft;
                case (int)KinectJointType.AnkleRight:
                    return (int)KinectJointType.AnkleLeft;
                case (int)KinectJointType.FootRight:
                    return (int)KinectJointType.FootLeft;
            }

            return jointIndex;
        }

        public override string ToString()
        {
            return string.Format("KinectJoint {0} {1} {2}", JointType, TrackingState, Position);
        }

    }

    /// <summary>
    /// Joint tracking state
    /// </summary>
    public enum KinectJointTrackingState{
        NotTracked,
        Inferred,
        Tracked,
    }


}
