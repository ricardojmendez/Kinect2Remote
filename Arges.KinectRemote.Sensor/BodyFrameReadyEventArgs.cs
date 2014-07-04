using System;
using System.Collections.Generic;
using Arges.KinectRemote.Data;

namespace Arges.KinectRemote.Sensor
{
    public class BodyFrameReadyEventArgs : EventArgs
    {

        private readonly List<KinectBodyData> _bodies;
        private readonly string _sensorId;

        public BodyFrameReadyEventArgs(List<KinectBodyData> bodies, string sensorId)
        {
            if (string.IsNullOrEmpty(sensorId))
            {
                throw new ArgumentException("sensorId");
            }
            _bodies = bodies;
            _sensorId = sensorId;
        }

        public List<KinectBodyData> Bodies
        {
            get
            {
                return _bodies;
            }
        }

        public string SensorId
        {
            get { return _sensorId; } 
        }
    }
}
