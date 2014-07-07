using System;
using ProtoBuf;

namespace Arges.KinectRemote.Data
{
    [Serializable, ProtoContract]
    public struct KinectVector3
    {
        [ProtoMember(1)]
        public float X;
        [ProtoMember(2)]
        public float Y;
        [ProtoMember(3)]
        public float Z;
    }
}
