using System;
using System.Collections.Generic;
using System.Linq;
using Arges.KinectRemote.BodyProcessor;
using Arges.KinectRemote.Data;
using Arges.KinectRemote.Transport;
using Arges.KinectRemote.Sensor;

namespace Arges.KinectRemote.Transmitter
{
    /// <summary>
    /// Connects to the available Kinect sensors, encodes the body data for publishing
    /// and sends it over the wire serialized.
    /// </summary>
    public class KinectDataPublisher
    {
        readonly MessagePublisherBase _messagePublisher;
        readonly KinectBodyFrameHandler _kinectRuntime = new KinectBodyFrameHandler();

        /// <summary>
        /// Is the publisher currently allowed to broadcast?
        /// </summary>
        public bool BroadcastEnabled { set; get; }

        /// <summary>
        /// List of body processors that we should run each body through 
        /// before sending it down the wire
        /// </summary>
        public List<IBodyEvaluator> BodyEvaluators { get; private set; }

        /// <summary>
        /// Initializes a Kinect Data Publisher
        /// </summary>
        /// <param name="ipAddress">IP Address for the RabbitMQ server</param>
        /// <param name="exchangeName">Exchange to publish information to</param>
        /// <param name="senderId">Sender ID, used as the first part of the topic</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public KinectDataPublisher(string ipAddress, string exchangeName, string senderId, string username = "guest", string password = "guest")
        {
            _messagePublisher = new RabbitMqMessagePublisher(ipAddress, exchangeName, senderId, username, password);

            Console.WriteLine("Starting all sensors");
            _kinectRuntime.StartSensor();
            _kinectRuntime.BodyFrameReady += OnBodyFrameReady;
            Console.WriteLine("All Kinect Sensors are started.");

            BroadcastEnabled = true;
            BodyEvaluators = new List<IBodyEvaluator>();
        }

        ~KinectDataPublisher()
        {
            _kinectRuntime.StopAllSensors();
        }

        void OnBodyFrameReady(object sender, BodyFrameReadyEventArgs e)
        {
            if (BroadcastEnabled && e != null && e.Bodies != null && e.Bodies.Count > 0)
            {
                ProcessAndTransmit(e.SensorId, e.Bodies);
            }
        }

        /// <summary>
        /// Called when data is ready, creates a bundle, serializes it and broadcasts it
        /// </summary>
        /// <param name="bodies">List of bodies to send</param>
        /// <param name="sensorId">Device ID for the Kinect sensor</param>
        void ProcessAndTransmit(string sensorId, List<KinectBodyData> bodies)
        {
            foreach (var evaluator in BodyEvaluators)
            {
                foreach (var body in bodies.Where(body => evaluator.ShouldFlagBody(body)))
                {
                    body.Ambiguity |= evaluator.FlagToSet;
                }
            }

            var stuffedBodyBag = new KinectBodyBag
            {
                SensorId = sensorId,
                Bodies = bodies
            };
            
            _messagePublisher.SerializeAndSendObject(stuffedBodyBag);
        }
    }
}
