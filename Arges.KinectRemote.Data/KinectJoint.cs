using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace Arges.KinectRemote.Data
{
    [Serializable, ProtoContract]
    public class KinectJoint{
        /// <summary>
        /// Joint Position X
        /// </summary>
        [ProtoMember(1)]
        public float X;

        /// <summary>
        /// Joint Position Y
        /// </summary>
        [ProtoMember(2)]
        public float Y;
        /// <summary>
        /// Joint Position Z
        /// </summary>
        [ProtoMember(3)]
        public float Z;

        /// <summary>
        /// The tracking state of this joint
        /// </summary>
        [ProtoMember(4)]
        public KinectJointTrackingState TrackingState;

        /// <summary>
        /// Type of the joint
        /// </summary>
        [ProtoMember(5)]
        public KinectJointType JointType;

        /// <summary>
        /// Hierarchical Rotation of the joint
        /// </summary>
        [ProtoMember(6)]
        public KinectVector4 Rotation;

        public static bool IsJointMirrorable(int jointIndex)
        {
            return jointIndex != GetSkeletonMirroredJoint(jointIndex);
        }

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
            return string.Format("KinectJoint {0} {1} ({2},{3},{4})", JointType, TrackingState, X, Y, Z);
        }

    }


    public enum KinectJointTrackingState{
        NotTracked,
        Inferred,
        Tracked,
    }


}
