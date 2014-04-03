using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;
using Arges.KinectRemote.Data;
using ProtoBuf;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace Arges.KinectRemote.TestConsole
{
    class Program{

        private static string _exchange;
        private static string _ipAddress;

        private static void ReadConfigSettings(){
            _exchange = ConfigurationManager.AppSettings["exchange"].Trim();
            _ipAddress = ConfigurationManager.AppSettings["ipAddress"].Trim();
            if(string.IsNullOrEmpty(_exchange)){
                throw new System.Exception("Exchange is not specified in the app.config.");
            }
            if (string.IsNullOrEmpty(_ipAddress)){
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
                var factory = new ConnectionFactory() { HostName = _ipAddress };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(_exchange, "fanout");
                        var queue = channel.QueueDeclare();
                        channel.QueueBind(queue, _exchange, "");

                        var consumer = new QueueingBasicConsumer(channel);
                        channel.BasicConsume(queue, true, consumer); 

                        Console.WriteLine("Initialized consumer for {0} on address {1}", _exchange, _ipAddress);

                        while (true)
                        {
                            var msg = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                            var body = msg.Body;

                            using (var ms = new MemoryStream(body))
                            { 
                                var kinectData = ProtoBuf.Serializer.Deserialize<KinectBodyBag>(ms);

                                LogKinectData(kinectData);
                            }
                        }
                    }
                }
            }
            catch(System.Exception ex){
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
