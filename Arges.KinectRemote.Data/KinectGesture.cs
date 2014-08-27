using System;
using ProtoBuf;

namespace Arges.KinectRemote.Data
{
    /// <summary>
    /// Encapsulates the data for Kinect gestures so it can be sent over the wire.
    /// 
    /// Unlike the Kinect SDK, we will use the same struct for both continuous 
    /// and discrete gestures.  See the properties Value, IsContinuous.
    /// 
    /// We will not send any information for non-detected gestures, so 
    /// DiscreteGestureResult's Detected is not relevant.
    /// </summary>
    [Serializable, ProtoContract]
    public struct KinectGesture
    {
        /// <summary>
        /// Gesture name
        /// </summary>
        [ProtoMember(1)] public string Name;

        /// <summary>
        /// Tracking Id for the user for which we received the gesture
        /// </summary>
        [ProtoMember(2)] public ulong TrackingId;

        /// <summary>
        /// Indicates if the gesture is continuous or discrete.
        /// </summary>
        [ProtoMember(3)] public bool IsContinuous;

        /// <summary>
        /// Used as the confidence on discrete gestures, and 
        /// the progress on continuous gestures.
        /// </summary>
        [ProtoMember(4)] public float Value;

        public override string ToString()
        {
            return string.Format("Gesture {0} for {1}: {2} ({3})", Name, TrackingId, Value, IsContinuous ? "Continuous" : "Discrete");
        }
    }
}
