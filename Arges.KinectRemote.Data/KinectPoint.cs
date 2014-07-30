using System;
using ProtoBuf;

namespace Arges.KinectRemote.Data
{
    /// <summary>
    /// Represents a 2d point. Currently used for the Lean indication.
    /// </summary>
    [Serializable, ProtoContract]
    public struct KinectPoint
    {
        [ProtoMember(1)]
        public float X;
        [ProtoMember(2)]
        public float Y;

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }

    }
}
