using System;
using System.Linq;
using Microsoft.Kinect;
using Arges.KinectRemote.Data;

namespace Arges.KinectRemote.Sensor
{
    /// <summary>
    /// Defines a simple runtime class that acts as a wrapper for the KinectSDK body interface
    /// </summary>
    public class KinectBodyFrameHandler
    {

        /// <summary>
        /// Body frame reader
        /// </summary>
        /// <remarks>
        /// The Kinect2 SDK only supports one sensor at a time
        /// </remarks>
        public BodyFrameReader FrameReader { get; private set; }

        public Body[] Bodies { get; private set; }

        /// <summary>
        /// Unique sensor ID generated at runtime, so that remotes can tell 
        /// which sensor the data is coming from
        /// </summary>
        /// <remarks>
        /// This is likely to be a temporary fix while Microsoft implements 
        /// a sensor id. If they do not, we can expand it to be a value that
        /// can be set by the developer.
        /// </remarks>
        public string SensorId { get; private set; }

        public KinectBodyFrameHandler()
        {
            Bodies = new Body[6];
            SensorId = Guid.NewGuid().ToString("d");
        }

        /// <summary>
        /// Handler called whenever one of the sensors has a frame ready.
        /// </summary>
        public event EventHandler<BodyFrameReadyEventArgs> BodyFrameReady;

        /// <summary>
        /// Enables tracking and starts the sensor, if there is one attached
        /// </summary>
        public void OpenSensor()
        {
            var sensor = KinectSensor.GetDefault();
            Console.WriteLine("- Opening sensor: {0}", SensorId);

            sensor.Open();

            FrameReader = sensor.BodyFrameSource.OpenReader();
            FrameReader.FrameArrived += OnFrameArrived;            
        }

        /// <summary>
        /// Handles a new body frame by creating a list of mapped bodies and sending it over the wire
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void OnFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            if (BodyFrameReady == null)
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
                    .Select(body => MapBody(body, SensorId))
                    .ToList();

                BodyFrameReady(this, new BodyFrameReadyEventArgs(SensorId, resultingBodies));
            }
        }

        /// <summary>
        /// Closes the current sensor and disposes the body frame reader
        /// </summary>
        public void CloseSensor()
        {
            FrameReader.Dispose();
            FrameReader = null;

            Console.WriteLine("Closing sensor");
            KinectSensor.GetDefault().Close();
            Console.WriteLine("Closed sensor");
        }

        /// <summary>
        /// Maps the information received for a Body from the Kinect to a
        /// KinectBodyData we can serialize and send over the wire.
        /// </summary>
        /// <param name="body">Body to map</param>
        /// <param name="sensorId">Sensor ID, or any other value being used as the device connection Id</param>
        /// <returns>Mapped KinectBodyData containing the body information, 
        /// an identifier, and othe processed data</returns>
        private static KinectBodyData MapBody(Body body, string sensorId)
        {
            // Add an identifier using the sensor ID to keep the skeleton entity unique across all devices.
            var d = new KinectBodyData { BodyId = string.Format("{0}.{1}", sensorId, body.TrackingId) };


            // All six bodies are fully tracked. Wee!
            var jointCount = Enum.GetNames(typeof(KinectJointType)).Length;
            d.Joints = new KinectJoint[jointCount];

            for (var i = 0; i < jointCount; i++)
            {
                var nativeJoint = body.Joints[(JointType) i];
                var orientation = body.JointOrientations[(JointType) i].Orientation;

                var joint = new KinectJoint
                {
                    TrackingState = ((KinectTrackingState)(int)nativeJoint.TrackingState),
                    Position = new KinectVector3
                    {
                        X = nativeJoint.Position.X, 
                        Y = nativeJoint.Position.Y, 
                        Z = nativeJoint.Position.Z
                    },
                    JointType = ((KinectJointType)(int)nativeJoint.JointType),
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
            d.Lean = new KinectPoint { X = body.Lean.X, Y = body.Lean.Y };
            d.LeanTrackingState = (KinectTrackingState) (int) body.LeanTrackingState;

            // Record hand states
            d.HandLeftState = (KinectHandState)(int)body.HandLeftState;
            d.HandRightState = (KinectHandState)(int)body.HandRightState;
            
            // Record hand confidence.  Initially we'll just convert the enum to an int,
            // but we could do some exponential smoothing between their {0,1} values.
            d.HandLeftConfidence = (int)body.HandLeftConfidence;
            d.HandRightConfidence = (int)body.HandRightConfidence;

            return d;
        }
    }
}
