using System;
using ProtoBuf;
using System.Collections.Generic;

namespace Arges.KinectRemote.Data
{
    /// <summary>
    /// Encapsulates information about a joint
    /// </summary>
    [Serializable, ProtoContract]
    public class KinectJoint
    {
        /// <summary>
        /// Dictionary storing (child, parent) joint relationships as (key, value) pairs
        /// </summary>
        public static Dictionary<KinectJointType, KinectJointType> JointParent = new Dictionary<KinectJointType, KinectJointType>()
        {
            { KinectJointType.FootLeft, KinectJointType.AnkleLeft },
            { KinectJointType.AnkleLeft, KinectJointType.KneeLeft },
            { KinectJointType.KneeLeft, KinectJointType.HipLeft },
            { KinectJointType.HipLeft, KinectJointType.SpineBase },
        
            { KinectJointType.FootRight, KinectJointType.AnkleRight },
            { KinectJointType.AnkleRight, KinectJointType.KneeRight },
            { KinectJointType.KneeRight, KinectJointType.HipRight },
            { KinectJointType.HipRight, KinectJointType.SpineBase },
        
            { KinectJointType.HandTipLeft, KinectJointType.HandLeft },
            { KinectJointType.ThumbLeft, KinectJointType.HandLeft },
            { KinectJointType.HandLeft, KinectJointType.WristLeft },
            { KinectJointType.WristLeft, KinectJointType.ElbowLeft },
            { KinectJointType.ElbowLeft, KinectJointType.ShoulderLeft },
            { KinectJointType.ShoulderLeft, KinectJointType.SpineShoulder },
        
            { KinectJointType.HandTipRight, KinectJointType.HandRight },
            { KinectJointType.ThumbRight, KinectJointType.HandRight },
            { KinectJointType.HandRight, KinectJointType.WristRight },
            { KinectJointType.WristRight, KinectJointType.ElbowRight },
            { KinectJointType.ElbowRight, KinectJointType.ShoulderRight },
            { KinectJointType.ShoulderRight, KinectJointType.SpineShoulder },
        
            { KinectJointType.SpineMid, KinectJointType.SpineBase },
            { KinectJointType.SpineShoulder, KinectJointType.SpineMid },
            { KinectJointType.Neck, KinectJointType.SpineShoulder },
            { KinectJointType.Head, KinectJointType.Neck },

            { KinectJointType.SpineBase, KinectJointType.SpineBase },
        };


        /// <summary>
        /// Joint position
        /// </summary>
        [ProtoMember(1)] 
        public KinectVector3 Position;

        /// <summary>
        /// The tracking state of this joint
        /// </summary>
        [ProtoMember(2)]
        public KinectTrackingState TrackingState;

        /// <summary>
        /// Type of the joint
        /// </summary>
        [ProtoMember(3)]
        public KinectJointType JointType;

        /// <summary>
        /// Hierarchical Orientation of the joint
        /// </summary>
        /// <remarks>
        /// This is *not* the joint's rotation, it actually describes
        /// a look vector from the parent joint to this one. For example:
        /// 
        /// leftShoulder.Orientation.ToQuaternion() * Vector3.up
        /// 
        /// will equal a normalized
        /// 
        /// leftShoulder.Position.ToVector3() - spineShoulder.Position.ToVector3();
        /// </remarks>
        [ProtoMember(4)]
        public KinectVector4 Orientation;

        public static bool IsJointMirrorable(int jointIndex)
        {
            return jointIndex != GetMirroredJoint(jointIndex);
        }

        /// <summary>
        /// Returns the equivalent mirrored joint for a joint index (eg., left hand for right hand)
        /// </summary>
        /// <param name="jointIndex">Joint to return the mirror for</param>
        /// <returns>Mirrored joint index, or the same index if it's not mirrorable</returns>
        public static int GetMirroredJoint(int jointIndex)
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
    /// Tracking state, used for both joints and lean
    /// </summary>
    public enum KinectTrackingState{
        NotTracked = 0,
        Inferred = 1,
        Tracked = 2,
    }


}
