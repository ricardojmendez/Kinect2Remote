using System;
using System.Configuration;
using Arges.KinectRemote.BodyProcessor;

namespace Arges.KinectRemote.Transmitter
{
    static class KinectTransmitter
    {
        static void Main(string[] args)
        {
            var exchange = ConfigurationManager.AppSettings["exchange"];
            var ipAddress = ConfigurationManager.AppSettings["ipAddress"];
            var senderId = ConfigurationManager.AppSettings["senderID"];
            var username = ConfigurationManager.AppSettings["username"];
            var password = ConfigurationManager.AppSettings["password"];


            Console.WriteLine("Initializing Kinect data transmitter...");
            var publisher = new KinectDataPublisher(ipAddress, exchange, senderId, username, password);
            publisher.BodyProcessors.Add(new LeftArmAbiguityProcessor());
            publisher.BodyProcessors.Add(new RightArmAbiguityProcessor());
            publisher.BodyProcessors.Add(new SittingProcessor());

            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
            
        }
    }
}
