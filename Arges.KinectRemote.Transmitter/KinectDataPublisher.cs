using System;
using System.Collections.Generic;
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

        public bool BroadcastEnabled { set; get; }

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

        }

        ~KinectDataPublisher()
        {
            _kinectRuntime.StopAllSensors();
        }

        void OnBodyFrameReady(object sender, BodyFrameReadyEventArgs e)
        {
            ProcessBodies(e.SensorId, e.Bodies);
        }

        /// <summary>
        /// Called when data is ready, creates a bundle, serializes it and broadcasts it
        /// </summary>
        /// <param name="bodies">List of bodies to send</param>
        /// <param name="sensorId">Device ID for the Kinect sensor</param>
        void ProcessBodies(string sensorId, List<KinectBodyData> bodies)
        {
            if (!BroadcastEnabled || bodies.Count == 0) 
            { 
                return; 
            }

            var stuffedBodyBag = new KinectBodyBag
            {
                SensorId = sensorId,
                Bodies = bodies
            };

            // We may want to add some object pre-processing here, or 
            // just let the receivers take care of that.

            _messagePublisher.SerializeAndSendObject(stuffedBodyBag);
        }
    }
}
