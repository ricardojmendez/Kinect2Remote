﻿using System;
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
    /// <remarks>
    /// Currently part of the Transmitter library. I've considered moving it to
    /// the Transport library, but that would require adding a dependency on that
    /// library to Sensor, and I'd rather that remains solely where it's 
    /// necessary (since the receivers do not need to have any dependency to the
    /// sensor).
    /// </remarks>
    public class KinectDataPublisher
    {
        readonly MessagePublisherBase _messagePublisher;
        readonly KinectBodyFrameHandler _kinectRuntime = new KinectBodyFrameHandler();

        /// <summary>
        /// Last number of bodies sent
        /// </summary>
        private int _lastBodyCount = 0;

        /// <summary>
        /// Is the publisher currently allowed to broadcast?
        /// </summary>
        public bool BroadcastEnabled { set; get; }

        /// <summary>
        /// List of body processors that we should run each body through 
        /// before sending it down the wire
        /// </summary>
        public List<ABodyProcessor> BodyProcessors { get; private set; }

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
            _kinectRuntime.OpenSensor();
            _kinectRuntime.BodyFrameReady += OnBodyFrameReady;
            Console.WriteLine("All Kinect Sensors are started.");

            BroadcastEnabled = true;
            BodyProcessors = new List<ABodyProcessor>();
        }

        ~KinectDataPublisher()
        {
            _kinectRuntime.CloseSensor();
        }

        void OnBodyFrameReady(object sender, BodyFrameReadyEventArgs e)
        {
            if (BroadcastEnabled && e != null && e.Bodies != null && 
                (e.Bodies.Count > 0 || _lastBodyCount != 0))
            {
                ProcessAndTransmit(e.SensorId, e.Bodies);
                _lastBodyCount = e.Bodies.Count;
            }
        }

        /// <summary>
        /// Called when data is ready, creates a bundle, serializes it and broadcasts it
        /// </summary>
        /// <param name="bodies">List of bodies to send</param>
        /// <param name="sensorId">Device ID for the Kinect sensor</param>
        void ProcessAndTransmit(string sensorId, List<KinectBodyData> bodies)
        {
            foreach (var processor in BodyProcessors)
            {
                processor.ProcessBodies(bodies);
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
