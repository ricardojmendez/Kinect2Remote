using System;
using System.Configuration;
using Arges.KinectRemote.Data;
using Arges.KinectRemote.Transport;


namespace Arges.KinectRemote.TestConsole
{
    class Program
    {
        private static string _exchange;
        private static string _ipAddress;

        private static void ReadConfigSettings()
        {
            _exchange = ConfigurationManager.AppSettings["exchange"].Trim();
            _ipAddress = ConfigurationManager.AppSettings["ipAddress"].Trim();
            if(string.IsNullOrEmpty(_exchange))
            {
                throw new System.Exception("Exchange is not specified in the app.config.");
            }
            if (string.IsNullOrEmpty(_ipAddress))
            {
                throw new System.Exception("IP Address is not specified in the app.config.");
            }
            Console.WriteLine(string.Format("Exchange is: {0}", _exchange));
            Console.WriteLine(string.Format("IP address is: {0}", _ipAddress));
        }

        static void Main(string[] args)
        {
            ReadConfigSettings();
            
            try
            {
                using (var receiver = new KinectBodyReceiver(_ipAddress, _exchange))
                {
                    while (true)
                    {
                        var kinectData = receiver.Dequeue();
                        LogKinectData(kinectData);
                    }

                }
            }
            catch(System.Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.WriteLine(ex.StackTrace);
                Console.ReadLine();
            }
        }

        static void LogKinectData(KinectBodyBag bundle)
        {
            Console.WriteLine("{0} Logging data bundle for {1} ", DateTime.UtcNow.ToFileTimeUtc(), bundle.DeviceConnectionId);
            if (bundle.Bodies != null)
            {
                Console.WriteLine("Bundle contains {0} bodies. Create time: {1}", bundle.Bodies.Count, bundle.CreatedUTCTime);
                foreach (var body in bundle.Bodies)
                {
                    Console.WriteLine("- Body {0}", body.BodyId);
                    Console.WriteLine("- Joints");
                    foreach (var joint in body.Joints)
                    {
                        Console.WriteLine("  - {0}", joint.ToString());
                    }
                }
            }
            else
            {
                Console.WriteLine("Empty bundle.");
            }
            

        }
    }
}
