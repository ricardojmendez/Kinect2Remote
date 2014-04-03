using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arges.KinectRemote.Data;

namespace Arges.KinectRemote.Sensor
{
    public class BodyFrameReadyEventArgs : EventArgs
    {

        private readonly List<KinectBodyData> _bodies;
        private readonly string _deviceConnectionId;

        public BodyFrameReadyEventArgs(List<KinectBodyData> bodies, string deviceConnectionId)
        {
            if (string.IsNullOrEmpty(deviceConnectionId))
            {
                throw new ArgumentException("deviceConnectionId");
            }
            _bodies = bodies;
            _deviceConnectionId = deviceConnectionId;
        }

        public List<KinectBodyData> Bodies
        {
            get
            {
                return _bodies;
            }
        }

        public string DeviceConnectionId
        {
            get
            {
                return _deviceConnectionId;
            }
        }
    }
}
