using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace Arges.KinectRemote.Data
{
    /// <summary>
    /// Used to collect a list of bodies for sending over the wire
    /// </summary>
    [Serializable, ProtoContract]
    public class KinectBodyBag
    {
        /// <summary>
        /// Unique Kinect Sensor Id
        /// </summary>
        [ProtoMember(1)]
        public string DeviceConnectionId;

        /// <summary>
        /// List of tracked skeletons
        /// </summary>
        [ProtoMember(2)]
        public List<KinectBodyData> Bodies;

    }

    /// <summary>
    /// Encapsulates data for a single body
    /// </summary>
    [Serializable, ProtoContract]
    public class KinectBodyData
    {

        /// <summary>
        /// Current Skeleton Id
        /// </summary>
        [ProtoMember(1)]
        public string BodyId;

        /// <summary>
        /// Collection of Joints
        /// </summary>
        [ProtoMember(2)]
        public KinectJoint[] Joints;

        /// <summary>
        /// Determines how ambiguous is the data
        /// </summary>
        [ProtoMember(3), ProtoEnum]
        public BodyAmbiguity Ambiguity = BodyAmbiguity.Clear;

        [ProtoMember(4)]
        /// <summary>
        /// Left hand state from the list of possible Kinect hand states
        /// </summary>
        public KinectHandState HandLeftState;

        [ProtoMember(5)]
        /// <summary>
        /// Right hand state from the list of possible Kinect hand states
        /// </summary>
        public KinectHandState HandRightState;

        [ProtoMember(6)]
        /// <summary>
        /// Confidence for the left hand state
        /// </summary>
        public float HandLeftConfidence;

        [ProtoMember(7)]
        /// <summary>
        /// Confidence for the right hand state
        /// </summary>
        public float HandRightConfidence;


        /// <summary>
        /// Offsets joints to a particular distance.
        /// </summary>
        /// <param name="x">X offset</param>
        /// <param name="y">Y offset</param>
        /// <param name="z">Z offset</param>
        public void ApplyOffset(float x, float y, float z)
        {
            if (Joints == null) { return; }
            foreach(var joint in Joints)
            {
                joint.X += x;
                joint.Y += y;
                joint.Z += z;
            }
        }
    }


    [ProtoContract]
    public enum KinectHandState
    {
        Unknown = 0,
        NotTracked = 1,
        Open = 2,
        Closed = 3,
        Lasso = 4,
    }
   

    [ProtoContract]
    public enum KinectJointType
    {
        SpineBase = 0,
        SpineMid = 1,
        Neck = 2,
        Head = 3,
        ShoulderLeft = 4,
        ElbowLeft = 5,
        WristLeft = 6,
        HandLeft = 7,
        ShoulderRight = 8,
        ElbowRight = 9,
        WristRight = 10,
        HandRight = 11,
        HipLeft = 12,
        KneeLeft = 13,
        AnkleLeft = 14,
        FootLeft = 15,
        HipRight = 16,
        KneeRight = 17,
        AnkleRight = 18,
        FootRight = 19,
        SpineShoulder = 20,
        HandTipLeft = 21,
        ThumbLeft = 22,
        HandTipRight = 23,
        ThumbRight = 24
    }

    /// <summary>
    /// Body ambiguity flag. Not used on the public version yet.
    /// </summary>
    [ProtoContract, Flags]
    public enum BodyAmbiguity
    {
        Clear = 0,
        Obscured = 1 << 0,
        Sitting = 1 << 1,
        MissingLeftArm = 1 << 2,
        MissingRightArm = 1 << 3,
        ShadowOutOfRange = 1 << 4,
        ShadowLost = 1 << 5,
    }
}
