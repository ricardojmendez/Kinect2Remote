using System;
using System.Collections.Generic;
using Microsoft.Kinect;

namespace Arges.KinectRemote.Sensor
{
    /// <summary>
    /// Manages a Kinect sensor opening/closing. Operates over the default sensor
    /// for now.
    /// </summary>
    public class KinectSensorManager
    {
        private List<AFrameHandler> _frameHandlers = new List<AFrameHandler>();

        /// <summary>
        /// Unique sensor ID generated at runtime, so that remotes can tell 
        /// which sensor the data is coming from
        /// </summary>
        /// <remarks>
        /// This was meant as a temporary fix while Microsoft implements 
        /// a sensor id, but I have configured it to be user-settable so
        /// that we can refer to sensors by a human-readable value.
        /// </remarks>
        public string SensorId { get; set; }

        /// <summary>
        /// Indicates if this sensor manager is running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Sensor being managed
        /// </summary>
        public KinectSensor Sensor { get; private set; }


        public KinectSensorManager(string sensorId = null)
        {
            SensorId = sensorId ?? Guid.NewGuid().ToString("d");
        }

        /// <summary>
        /// Enables tracking and starts the sensor, if there is one attached
        /// </summary>
        public void OpenSensor()
        {
            Sensor = KinectSensor.GetDefault();
            Console.WriteLine("- Opening sensor: {0}", SensorId);
            Sensor.Open();

            foreach (var handler in _frameHandlers)
            {
                handler.OnStart();
            }
            IsRunning = true;
        }

        public void CloseSensor()
        {
            foreach (var handler in _frameHandlers)
            {
                handler.OnStop();
            }

            // Sensor might be null if Open hasn't been called
            if (Sensor != null)
            {
                Console.WriteLine("Closing sensor");
                Sensor.Close();
                Console.WriteLine("Closed sensor");
            }
            IsRunning = false;
        }

        /// <summary>
        /// Adds a new frame handler to the manager
        /// </summary>
        /// <param name="handler">Frame handler to add</param>
        /// <exception cref="InvalidOperationException">Will raise an InvalidOperationException if the sensor is already open</exception>
        public void AddFrameHandler(AFrameHandler handler)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Cannot add a frame handler while the sensor is open");
            }
            _frameHandlers.Add(handler);
        }

        /// <summary>
        /// Removes a frame handler from the sensor manager and stops it
        /// </summary>
        /// <param name="handler">Handler to remove</param>
        public void RemoveFrameHandler(AFrameHandler handler)
        {
            if (_frameHandlers.Remove(handler))
            {
                handler.OnStop();
            }
        }
    }
}