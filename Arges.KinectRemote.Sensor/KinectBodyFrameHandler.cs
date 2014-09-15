using System;
using System.Linq;
using Arges.KinectRemote.Data;
using Microsoft.Kinect;

namespace Arges.KinectRemote.Sensor
{
    /// <summary>
    /// Defines a simple runtime class that acts as a wrapper for the KinectSDK body interface
    /// </summary>
    public class KinectBodyFrameHandler : AFrameHandler
    {
        /// <summary>
        /// Body frame reader
        /// </summary>
        public BodyFrameReader FrameReader { get; private set; }

        public Body[] Bodies { get; private set; }

        public KinectBodyFrameHandler(KinectSensorManager manager) : base(manager)
        {
            Bodies = new Body[6];
        }

        /// <summary>
        /// Handler called whenever one of the sensors has a frame ready.
        /// </summary>
        public event EventHandler<KinectItemListEventArgs<KinectBody>> FrameReady;

        internal override void OnStart()
        {
            FrameReader = Manager.Sensor.BodyFrameSource.OpenReader();
            FrameReader.FrameArrived += OnFrameArrived;
        }

        internal override void OnStop()
        {
            FrameReader.Dispose();
            FrameReader = null;
        }


        /// <summary>
        /// Handles a new body frame by creating a list of mapped bodies and sending it over the wire
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void OnFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            if (FrameReady == null)
            {
                return;
            }

            var frame = e.FrameReference.AcquireFrame();
            if (frame == null)
            {
                return;
            }

            using (frame)
            {
                frame.GetAndRefreshBodyData(Bodies);

                var resultingBodies = Bodies.Where(b => b.IsTracked)
                    .Select(body => MapBody(body, Manager.SensorId))
                    .ToList();

                FrameReady(this, new KinectItemListEventArgs<KinectBody>(Manager.SensorId, resultingBodies));
            }
        }


        /// <summary>
        /// Maps the information received for a Body from the Kinect to a
        /// KinectBody we can serialize and send over the wire.
        /// </summary>
        /// <param name="body">Body to map</param>
        /// <param name="sensorId">Sensor ID, or any other value being used as the device connection Id</param>
        /// <returns>Mapped KinectBody containing the body information, 
        /// an identifier, and othe processed data</returns>
        private static KinectBody MapBody(Body body, string sensorId)
        {
            var d = new KinectBody(sensorId, body.TrackingId);

            // All six bodies are fully tracked. Wee!
            var jointCount = Enum.GetNames(typeof (KinectJointType)).Length;
            d.Joints = new KinectJoint[jointCount];

            for (var i = 0; i < jointCount; i++)
            {
                var nativeJoint = body.Joints[(JointType) i];
                var orientation = body.JointOrientations[(JointType) i].Orientation;

                var joint = new KinectJoint
                {
                    TrackingState = ((KinectTrackingState) (int) nativeJoint.TrackingState),
                    Position = new KinectVector3
                    {
                        X = nativeJoint.Position.X,
                        Y = nativeJoint.Position.Y,
                        Z = nativeJoint.Position.Z
                    },
                    JointType = ((KinectJointType) (int) nativeJoint.JointType),
                    Orientation = new KinectVector4
                    {
                        W = orientation.W,
                        X = orientation.X,
                        Y = orientation.Y,
                        Z = orientation.Z
                    }
                };
                d.Joints[i] = joint;
            }
            d.Lean = new KinectPoint {X = body.Lean.X, Y = body.Lean.Y};
            d.LeanTrackingState = (KinectTrackingState) (int) body.LeanTrackingState;

            // Record hand states
            d.HandLeftState = (KinectHandState) (int) body.HandLeftState;
            d.HandRightState = (KinectHandState) (int) body.HandRightState;

            // Record hand confidence.  Initially we'll just convert the enum to an int,
            // but we could do some exponential smoothing between their {0,1} values.
            d.HandLeftConfidence = (int) body.HandLeftConfidence;
            d.HandRightConfidence = (int) body.HandRightConfidence;

            return d;
        }
    }
}