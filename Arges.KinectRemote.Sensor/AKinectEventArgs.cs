using System;

namespace Arges.KinectRemote.Sensor
{
    /// <summary>
    /// Base class for Kinect events
    /// </summary>
    public abstract class AKinectEventArgs: EventArgs
    {
        public string SensorId { get; private set; }

        protected AKinectEventArgs(string sensorId)
        {
            if (string.IsNullOrEmpty(sensorId))
            {
                throw new ArgumentException("sensorId");
            }
            SensorId = sensorId;            
        }
    }
}
