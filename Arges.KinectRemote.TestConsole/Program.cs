#define NoWait
#define OnlyLast
using System;
using System.Configuration;
using Arges.KinectRemote.Data;
using Arges.KinectRemote.Transport;


namespace Arges.KinectRemote.TestConsole
{
    class Program
    {
        static string _exchange;
        static string _ipAddress;
        static string _bindingKey;

        private static void ReadConfigSettings()
        {
            _exchange = ConfigurationManager.AppSettings["exchange"].Trim();
            _ipAddress = ConfigurationManager.AppSettings["ipAddress"].Trim();
            _bindingKey = ConfigurationManager.AppSettings["bindingKey"].Trim();
            if(string.IsNullOrEmpty(_exchange))
            {
                throw new ArgumentException("Exchange is not specified in the app.config.");
            }
            if (string.IsNullOrEmpty(_ipAddress))
            {
                Console.WriteLine("IP Address not specified. Connecting to local host.");
                _ipAddress = "127.0.0.1";
            }
            if (string.IsNullOrEmpty(_bindingKey))
            {
                Console.WriteLine("Binding key not specified. Binding to all remotes for body information.");
                _bindingKey = "*.body";
            }
            Console.WriteLine("Exchange is: {0}", _exchange);
            Console.WriteLine("IP address is: {0}", _ipAddress);
            Console.WriteLine("Listening to: {0}", _bindingKey);
        }

        private static void Main(string[] args)
        {
            ReadConfigSettings();
            
            try
            {
#if OnlyLast
                using (var receiver = new KinectBodyReceiverLastOnly(_ipAddress, _exchange, _bindingKey))
#else
                using (var receiver = new KinectBodyReceiver(_ipAddress, _exchange))
#endif
                {
                    while (true)
                    {
#if NoWait
                        // Non-blocking, will return null if there isn't any data available
                        var kinectData = receiver.DequeueNoWait();
                        if (kinectData != null)
                        {
                            LogKinectData(kinectData);
                        }
                        System.Threading.Thread.Sleep(1);
#else
                        // Blocking call
                        var kinectData = receiver.Dequeue();
                        LogKinectData(kinectData);
#endif
                    }
                }
            }
            catch(Exception ex)
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
            Console.WriteLine("{0} Logging data bundle for {1} ", DateTime.UtcNow.ToFileTimeUtc(), bundle.SensorId);
            if (bundle.Bodies != null)
            {
                Console.WriteLine("Bundle contains {0} bodies", bundle.Bodies.Count);
                foreach (var body in bundle.Bodies)
                {
                    Console.WriteLine("- Body {0}", body.BodyId);
                    Console.WriteLine("- Hand States. Left {0} (Conf: {1}) Right {2} (Conf: {3})", body.HandLeftState, body.HandLeftConfidence, body.HandRightState, body.HandRightConfidence);
                    Console.WriteLine("- Joints");
                    foreach (var joint in body.Joints)
                    {
                        Console.WriteLine("  - {0}", joint);
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
