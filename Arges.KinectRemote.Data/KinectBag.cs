using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Arges.KinectRemote.Data
{
    /// <summary>
    /// Used to collect a list of items (bodies or gesture results) for sending over the wire
    /// </summary>
    [Serializable, ProtoContract]
    public class KinectBag<T>
    {
        /// <summary>
        /// Unique Kinect Sensor Id
        /// </summary>
        [ProtoMember(1)]
        public string SensorId;

        /// <summary>
        /// List of items being sent over.
        /// </summary>
        [ProtoMember(2)]
        public List<T> Items;
    }
}
