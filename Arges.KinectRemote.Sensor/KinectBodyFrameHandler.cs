using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// List of known body frame readers. Will for now contain only
        /// one value, since the Kinect2 SDK does not yet support multiple
        /// sensors.
        /// </summary>
        public List<BodyFrameReader> BodyFrameReaders { get; private set; }

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
            BodyFrameReaders = new List<BodyFrameReader>();
            Bodies = new Body[6];
            SensorId = Guid.NewGuid().ToString("d");
        }

        /// <summary>
        /// Handler called whenever one of the sensors has a frame ready.
        /// </summary>
        public event EventHandler<BodyFrameReadyEventArgs> BodyFrameReady;

        /// <summary>
        /// Enables tracking and starts all sensors
        /// </summary>
        public void StartSensor()
        {
            var sensor = KinectSensor.GetDefault();
            Console.WriteLine("- Opening sensor: {0}", SensorId);

            sensor.Open();

            var reader = sensor.BodyFrameSource.OpenReader();
            reader.FrameArrived += new EventHandler<BodyFrameArrivedEventArgs>(OnFrameArrived);
            BodyFrameReaders.Add(reader);
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

                List<KinectBodyData> resultingBodies = new List<KinectBodyData>();

                foreach (Body body in Bodies.Where(b => b.IsTracked))
                {
                    resultingBodies.Add(MapBody(body, SensorId));
                }

                BodyFrameReady(this, new BodyFrameReadyEventArgs(resultingBodies, SensorId));
            }
        }

        /// <summary>
        /// Stops all sensors
        /// </summary>
        public void StopAllSensors()
        {
            foreach (var reader in BodyFrameReaders)
            {
                reader.Dispose();
            }
            BodyFrameReaders.Clear();

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
            KinectBodyData d = new KinectBodyData();

            var spine = body.Joints[JointType.SpineBase];

            // This is to keep the skeleton entity unique across all devices.
            d.BodyId = string.Format("{0}.{1}", sensorId, body.TrackingId);

            // All six bodies are fully tracked. Wee!
            int jointsCount = Enum.GetNames(typeof(KinectJointType)).Length;
            d.Joints = new KinectJoint[jointsCount];

            for (int i = 0; i < jointsCount; i++)
            {
                var nativeJoint = body.Joints[(JointType)i];
                var orientation = body.JointOrientations[(JointType)i].Orientation;

                KinectJoint joint = new KinectJoint
                {
                    TrackingState = ((KinectJointTrackingState)(int)nativeJoint.TrackingState),
                    X = nativeJoint.Position.X,
                    Y = nativeJoint.Position.Y,
                    Z = nativeJoint.Position.Z,
                    JointType = ((KinectJointType)(int)nativeJoint.JointType),
                    Rotation = new KinectVector4
                    {
                        W = orientation.W,
                        X = orientation.X,
                        Y = orientation.Y,
                        Z = orientation.Z
                    }
                };
                d.Joints[i] = joint;
            }

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
