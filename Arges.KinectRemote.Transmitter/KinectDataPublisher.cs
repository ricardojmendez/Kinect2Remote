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

        public KinectDataPublisher(string ipAddress, string exchangeName)
        {
            _messagePublisher = new RabbitMqMessagePublisher(ipAddress, exchangeName);

            Console.WriteLine("Starting all sensors");
            _kinectRuntime.StartAllSensors();
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
                Bodies = bodyData            };

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
