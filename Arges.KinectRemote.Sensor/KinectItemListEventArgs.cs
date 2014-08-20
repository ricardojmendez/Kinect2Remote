using System;
using System.Collections.Generic;
using Arges.KinectRemote.Data;

namespace Arges.KinectRemote.Sensor
{
    public class KinectItemListEventArgs<T>: AKinectEventArgs
    {
        private readonly List<T> _items;

        public List<T> Items
        {
            get { return _items; }
        }

        public KinectItemListEventArgs(string sensorId, List<T> items): base(sensorId)
        {
            _items = items;
        }

    }
}