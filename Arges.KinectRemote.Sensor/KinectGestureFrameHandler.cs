using System;
using System.Collections.Generic;
using System.Linq;
using Arges.KinectRemote.Data;
using Microsoft.Kinect.VisualGestureBuilder;

namespace Arges.KinectRemote.Sensor
{
    /// <summary>
    /// Handles a gesture frame and 
    /// 
    /// Based on Microsoft's Gesture Builder example
    /// </summary>
    public class KinectGestureFrameHandler : AFrameHandler
    {
        private VisualGestureBuilderFrameSource _frameSource;
        private VisualGestureBuilderFrameReader _frameReader;

        public string DatabasePath { get; private set; }

        /// <summary>
        /// Handler called whenever we have a new gesture frame ready
        /// </summary>
        public event EventHandler<KinectItemListEventArgs<KinectGesture>> FrameReady;


        /// <summary>
        /// Gets/sets if the frame reader is paused
        /// </summary>
        public bool IsPaused
        {
            get { return _frameReader.IsPaused; }
            set { _frameReader.IsPaused = value; }
        }

        /// <summary>
        /// Gets/sets the tracking id for the frame soruce
        /// </summary>
        public ulong TrackingId
        {
            get { return _frameSource.TrackingId; }
            set { _frameSource.TrackingId = value; }
        }


        public KinectGestureFrameHandler(KinectSensorManager manager, string databasePath) : base(manager)
        {
            DatabasePath = databasePath;
        }

        private void FrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {
            var list = new List<KinectGesture>();

            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame == null) return;
                list.AddRange(from result in frame.DiscreteGestureResults
                    let gesture = result.Key
                    let gestureResult = result.Value
                    where gestureResult.Detected
                    select
                        new KinectGesture
                        {
                            Name = gesture.Name,
                            TrackingId = TrackingId,
                            IsContinuous = false,
                            Value = gestureResult.Confidence
                        }
                    );
                list.AddRange(from result in frame.ContinuousGestureResults
                    let gesture = result.Key
                    let gestureResult = result.Value
                    where gestureResult.Progress > 0
                    select
                        new KinectGesture
                        {
                            Name = gesture.Name,
                            TrackingId = TrackingId,
                            IsContinuous = true,
                            Value = gestureResult.Progress
                        }
                    );
            }

            FrameReady(this, new KinectItemListEventArgs<KinectGesture>(Manager.SensorId, list));
        }

        internal override void OnStart()
        {
            // Create the vgb source. The associated body tracking ID will be set when a valid body frame arrives from the sensor.
            _frameSource = new VisualGestureBuilderFrameSource(Manager.Sensor, 0);

            // Open the reader for the vgb frames
            _frameReader = _frameSource.OpenReader();
            if (_frameReader != null)
            {
                _frameReader.FrameArrived += FrameArrived;
            }

            // Load all gestures
            using (var database = new VisualGestureBuilderDatabase(DatabasePath))
            {
                foreach (var gesture in database.AvailableGestures)
                {
                    Console.WriteLine("Adding gesture {0} {1}", gesture.Name, gesture.GestureType);
                    _frameSource.AddGesture(gesture);
                }
            }
        }

        internal override void OnStop()
        {
            if (_frameReader != null)
            {
                _frameReader.FrameArrived -= FrameArrived;
                _frameReader.Dispose();
                _frameReader = null;
            }

            if (_frameSource != null)
            {
                _frameSource.Dispose();
                _frameSource = null;
            }
        }
    }
}