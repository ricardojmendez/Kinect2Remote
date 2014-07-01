using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Arges.KinectRemote.Data;

namespace Arges.KinectRemote.Sensor
{
    /// <summary>
    /// Defines a simple runtime class that acts as a wrapper for the KinectSDK interface
    /// </summary>
    public class KinectBodyFrameHandler
    {

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
        public string RemoteId { get; private set; }

        public KinectBodyFrameHandler()
        {
            BodyFrameReaders = new List<BodyFrameReader>();
            Bodies = new Body[6];
            RemoteId = Guid.NewGuid().ToString("d");
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
            Console.WriteLine("- Default sensor: {0}", RemoteId);
            Console.WriteLine("- Opening sensor: {0}", RemoteId);

            sensor.Open();

            var reader = sensor.BodyFrameSource.OpenReader();
            reader.FrameArrived += new EventHandler<BodyFrameArrivedEventArgs>(OnFrameArrived);
            BodyFrameReaders.Add(reader);
        }

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
                var sensor = frame.BodyFrameSource.KinectSensor;

                foreach (Body body in Bodies.Where(b => b.IsTracked))
                {
                    resultingBodies.Add(MapBody(body, RemoteId));
                }

                BodyFrameReady(this, new BodyFrameReadyEventArgs(resultingBodies, RemoteId));
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

#if KINECT1
        void OnSensorFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            //no events
            if (SkeletonFrameReady == null)
                return;

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                //if no data available, nothing to do
                if (skeletonFrame == null)
                    return;

                //extract frame data
                List<KinectSkeletonData> resultingSkeletons = new List<KinectSkeletonData>();
                Skeleton[] skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                skeletonFrame.CopySkeletonDataTo(skeletons);

                foreach (Skeleton skel in skeletons)
                {
                    resultingSkeletons.Add(MapSkeleton(skel, ((KinectSensor)sender).DeviceConnectionId));
                }

                //dispatch data
                SkeletonFrameReady(this, new SkeletonReadyEventArgs(resultingSkeletons, ((KinectSensor)sender).DeviceConnectionId));           
            }
        }
#endif

        private static KinectBodyData MapBody(Body body, string deviceConnectionId)
        {
            KinectBodyData d = new KinectBodyData();

            var spine = body.Joints[JointType.SpineBase];

            // This is to keep the skeleton entity unique across all devices.
            d.BodyId = string.Format("{0}.{1}", deviceConnectionId, body.TrackingId.ToString());

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
