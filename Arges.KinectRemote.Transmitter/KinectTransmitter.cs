using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arges.KinectRemote.Transmitter
{
    class KinectTransmitter
    {
        static void Main(string[] args)
        {
            var exchange = ConfigurationManager.AppSettings["exchange"];
            var ipAddress = ConfigurationManager.AppSettings["ipAddress"];


            Console.WriteLine("Initializing Kinect data transmitter...");
            var publisher = new KinectDataPublisher(ipAddress, exchange);

            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
            
        }
    }
}
