using System;
using System.Configuration;

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
