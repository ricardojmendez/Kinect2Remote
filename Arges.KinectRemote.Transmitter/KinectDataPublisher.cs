using System;
using System.Collections.Generic;
using Arges.KinectRemote.Data;
using Arges.KinectRemote.Transport;
using Arges.KinectRemote.Sensor;

namespace Arges.KinectRemote.Transmitter
{
    /// <summary>
    /// Connects to the available Kinect sensors, encondes the body data for publishing
    /// and sends it over the wire serialized.
    /// </summary>
    public class KinectDataPublisher
    {
        MessagePublisherBase _messagePublisher;
        KinectBodyFrameHandler _kinectRuntime = new KinectBodyFrameHandler();

        public bool BroadcastEnabled { set; get; }

        /// <summary>
        /// Initializes a Kinect Data Publisher
        /// </summary>
        /// <param name="ipAddress">IP Address for the RabbitMQ server</param>
        /// <param name="exchangeName">Exchange to publish information to</param>
        /// <param name="senderID">Sender ID, used as the first part of the topic</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public KinectDataPublisher(string ipAddress, string exchangeName, string senderID, string username = "guest", string password = "guest")
        {
            _messagePublisher = new RabbitMqMessagePublisher(ipAddress, exchangeName, senderID, username, password);

            Console.WriteLine("Starting all sensors");
            _kinectRuntime.StartSensor();
            _kinectRuntime.BodyFrameReady += new EventHandler<BodyFrameReadyEventArgs>(OnBodyFrameReady);
            Console.WriteLine("All Kinect Sensors are started.");

            BroadcastEnabled = true;

        }

        ~KinectDataPublisher()
        {
            _kinectRuntime.StopAllSensors();
        }

        void OnBodyFrameReady(object sender, BodyFrameReadyEventArgs e)
        {
            BodyDataReady(e.DeviceConnectionId, e.Bodies);
        }


        /// <summary>
        /// Creates a body bag with the body data received
        /// </summary>
        /// <param name="bodyData">List of KinectBodyData items to add to the bag</param>
        /// <param name="deviceID">Kinect Sensor ID that they were received from.</param>
        static KinectBodyBag StuffBodyBag(string deviceID, List<KinectBodyData> bodyData)
        {
            KinectBodyBag bundle = null;
            bundle = new KinectBodyBag
            {
                DeviceConnectionId = deviceID,
                Bodies = bodyData            
            };

            return bundle;
        }

        /// <summary>
        /// Called when data is ready, creates a bundle, serializes it and broadcasts it
        /// </summary>
        /// <param name="bodies">List of bodies to send</param>
        /// <param name="deviceConnectionId">Device ID for the Kinect sensor</param>
        public void BodyDataReady(string deviceConnectionId, List<KinectBodyData> bodies)
        {
            if (!BroadcastEnabled || bodies.Count == 0) 
            { 
                return; 
            }

            KinectBodyBag bundle = StuffBodyBag(deviceConnectionId, bodies);
            _messagePublisher.SerializeAndSendObject<KinectBodyBag>(bundle);
        }

    }
}
