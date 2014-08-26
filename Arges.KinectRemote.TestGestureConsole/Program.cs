#define NoWait
using System;
using System.Configuration;
using Arges.KinectRemote.Data;
using Arges.KinectRemote.Transport;

namespace Arges.KinectRemote.TestGestureConsole
{
    /// <summary>
    /// Logs any gesture messages it receives, while not connecting to any
    /// other message queues such as body updates.
    /// </summary>
    static class Program
    {
        private static string _exchange;
        private static string _ipAddress;
        private static string _gestureBindingKey;

        private static void ReadConfigSettings()
        {
            _exchange = ConfigurationManager.AppSettings["exchange"].Trim();
            _ipAddress = ConfigurationManager.AppSettings["ipAddress"].Trim();
            _gestureBindingKey = ConfigurationManager.AppSettings["gestureBindingKey"].Trim();
            if (string.IsNullOrEmpty(_exchange))
            {
                throw new ArgumentException("Exchange is not specified in the app.config.");
            }
            if (string.IsNullOrEmpty(_ipAddress))
            {
                Console.WriteLine("IP Address not specified. Connecting to local host.");
                _ipAddress = "127.0.0.1";
            }
            if (string.IsNullOrEmpty(_gestureBindingKey))
            {
                Console.WriteLine("Gesture key not specified. Binding to all remotes for gesture information.");
                _gestureBindingKey = "*.gesture";
            }
            Console.WriteLine("Exchange is: {0}", _exchange);
            Console.WriteLine("IP address is: {0}", _ipAddress);
            Console.WriteLine("Listening for gestures to: {0}", _gestureBindingKey);
        }

        private static void Main()
        {
            ReadConfigSettings();

            try
            {
                using (var receiver = new KinectBagReceiver<KinectGesture>(_ipAddress, _exchange, _gestureBindingKey))
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
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.WriteLine(ex.StackTrace);
                Console.ReadLine();
            }
        }

        static void LogKinectData(KinectBag<KinectGesture> bundle)
        {
            Console.WriteLine("{0} New bundle from {1} ", DateTime.UtcNow.ToFileTimeUtc(), bundle.SensorId);
            if (bundle.Items != null)
            {
                foreach (var item in bundle.Items)
                {
                    Console.WriteLine("- {0}", item);
                }
            }
            else
            {
                Console.WriteLine("Empty bundle.");
            }
            Console.WriteLine();
        }
    }
}
