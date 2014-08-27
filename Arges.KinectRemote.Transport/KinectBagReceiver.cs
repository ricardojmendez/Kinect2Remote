using System;
using System.IO;
using System.Collections.Generic;
using Arges.KinectRemote.Data;
using RabbitMQ.Client;

namespace Arges.KinectRemote.Transport
{
    /// <summary>
    /// Handles reception of KinectBodyBags via RabbitMQ. Will process only the last 
    /// message received, since we assume that outdated messages are no longer
    /// important (for instance, we don't particularly care about joint positions
    /// a frame ago if we happened to skip one).
    /// </summary>
    /// <seealso cref="Arges.KinectRemote.Data.KinectBag{T}"></seealso>
    public class KinectBagReceiver<T> : IDisposable
    {
        readonly IConnection _connection;
        readonly IModel _channel;


        public KeepLastOnlyConsumer Consumer { get; private set; }

        /// <summary>
        /// Initializes a Kinect Bag Receiver
        /// </summary>
        /// <param name="ipAddress">IP address of the RabbitMq server</param>
        /// <param name="exchange">Exchange to connect to</param>
        /// <param name="bindingKey">Binding key. The current convention is {sensorId}.{topic}, where topic could be body or gesture.</param>
        /// <param name="username">Username, defaults to guest</param>
        /// <param name="password">Password, defaults to guest</param>
        public KinectBagReceiver(string ipAddress, string exchange, string bindingKey, string username = "guest", string password = "guest")
        {
            var factory = new ConnectionFactory { HostName = ipAddress, UserName = username, Password = password };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange, "topic");

            // Setting up the ttl to 30ms, since we don't particularly care about outdated frames.
            var queueParams = new Dictionary<string, object> { { "x-message-ttl", 30 } };
            var queue = _channel.QueueDeclare("", false, true, true, queueParams);
            _channel.QueueBind(queue, exchange, bindingKey);

            Consumer = new KeepLastOnlyConsumer(_channel);
            _channel.BasicConsume(queue, true, Consumer);
        }

        public void Dispose()
        {
            Consumer = null;
            _channel.Dispose();
            _connection.Dispose();
        }

        /// <summary>
        /// Returns a Kinect data bag if one is ready.  WARNING: This is a blocking call.
        /// </summary>
        /// <returns>KinectBag with the received data.</returns>
        public KinectBag<T> Dequeue()
        {
            KinectBag<T> data = null;
            while (data == null)
            {
                var msg = Consumer.Pop();
                if (msg != null)
                {
                    using (var ms = new MemoryStream(msg.Body))
                    {
                        data = ProtoBuf.Serializer.Deserialize<KinectBag<T>>(ms);
                    }
                }
                else
                {
                    System.Threading.Thread.Sleep(1);
                }
            }
            return data;
        }

        /// <summary>
        /// Returns a Kinect bag if one is ready, or null if otherwise.
        /// </summary>
        /// <returns>KinectBag with the received data, or null.</returns>
        public KinectBag<T> DequeueNoWait()
        {
            var msg = Consumer.Pop();
            if (msg == null || msg.Body == null) return null;

            KinectBag<T> data;
            using (var ms = new MemoryStream(msg.Body))
            {
                data = ProtoBuf.Serializer.Deserialize<KinectBag<T>>(ms);
            }

            return data;
        }
    }
}
