using System;
using ProtoBuf;

// TODO: Now that we are adding methods to KinectVector3 we should have tests

namespace Arges.KinectRemote.Data
{
    /// <summary>
    /// Wraps a KinectVector3 and adds some useful methods
    /// </summary>
    [Serializable, ProtoContract]
    public struct KinectVector3
    {
        public static readonly KinectVector3 Zero = new KinectVector3(0, 0, 0);

        [ProtoMember(1)] public float X;
        [ProtoMember(2)] public float Y;
        [ProtoMember(3)] public float Z;

        /// <summary>
        /// Vector magnitude
        /// </summary>
        public float Magnitude
        {
            get { return (float) Math.Sqrt(SqrMagnitude); }
        }

        /// <summary>
        /// Squared Vector magnitude
        /// </summary>
        public float SqrMagnitude
        {
            get { return X * X + Y * Y + Z * Z; }
        }

        public KinectVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", X, Y, Z);
        }

        /// <summary>
        /// Distance between two vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Distance</returns>
        public static float Distance(KinectVector3 a, KinectVector3 b)
        {
            return (a - b).Magnitude;
        }

        /// <summary>
        /// Overrides vector substraction
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>a - b</returns>
        public static KinectVector3 operator -(KinectVector3 a, KinectVector3 b)
        {
            return new KinectVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        /// Overrides vector addition
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>a + b</returns>
        public static KinectVector3 operator +(KinectVector3 a, KinectVector3 b)
        {
            return new KinectVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
    }
}