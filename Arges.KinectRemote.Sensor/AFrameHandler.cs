using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace Arges.KinectRemote.Sensor
{
    /// <summary>
    /// Abstract frame handler base class
    /// </summary>
    public abstract class AFrameHandler
    {
        /// <summary>
        /// Sensor manager for this frame handler
        /// </summary>
        public KinectSensorManager Manager { get; private set; }

        public AFrameHandler(KinectSensorManager manager)
        {
            Manager = manager;
        }

        /// <summary>
        /// Called by the sensor manager when the sensor is opened
        /// </summary>
        internal abstract void OnStart();

        /// <summary>
        /// Called by the sensor manager when the sensor is closed
        /// </summary>
        internal abstract void OnStop();
    }
}
