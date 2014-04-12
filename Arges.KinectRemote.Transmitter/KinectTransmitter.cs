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
            var senderID = ConfigurationManager.AppSettings["senderID"];
            var username = ConfigurationManager.AppSettings["username"];
            var password = ConfigurationManager.AppSettings["password"];


            Console.WriteLine("Initializing Kinect data transmitter...");
            var publisher = new KinectDataPublisher(ipAddress, exchange, senderID, username, password);

            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
            
        }
    }
}
