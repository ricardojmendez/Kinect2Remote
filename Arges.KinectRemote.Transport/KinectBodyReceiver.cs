using System;
using System.IO;
using System.Collections.Generic;
using Arges.KinectRemote.Data;
using RabbitMQ.Client;

namespace Arges.KinectRemote.Transport
{
    /// <summary>
    /// Handles reception of KinectBags via RabbitMQ.
    /// </summary>
    /// <seealso cref="Arges.KinectRemote.Data.KinectBag{T}"></seealso>
    public class KinectBodyReceiver: IDisposable
    {
        readonly IConnection _connection;
        readonly IModel _channel;

        public QueueingBasicConsumer Consumer { get; private set; }
        
        /// <summary>
        /// Initializes a new KinectBodyReceiver
        /// </summary>
        /// <param name="ipAddress">RabbitMQ server IP address</param>
        /// <param name="exchange">Exchange to connect to</param>
        /// <param name="bindingKey">Binding key to get body data from. We currently support only one.</param>
        public KinectBodyReceiver(string ipAddress, string exchange, string bindingKey)
        {
            var factory = new ConnectionFactory { HostName = ipAddress };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange, "topic");

            // Setting up the ttl to 30ms, since we don't particularly care about outdated frames.
            var queueParams = new Dictionary<string, object> { {"x-message-ttl", 30} };
            var queue = _channel.QueueDeclare("", false, true, true, queueParams);
            _channel.QueueBind(queue, exchange, bindingKey);

            Consumer = new QueueingBasicConsumer(_channel);
            _channel.BasicConsume(queue, true, Consumer);

        }

        public void Dispose()
        {
            Consumer = null;
            _channel.Dispose();
            _connection.Dispose();
        }

        /// <summary>
        /// Returns a body bag if the body is ready (/reggie).  WARNING: This is a blocking call.
        /// </summary>
        /// <returns>KinectBag with the received data.</returns>
        public KinectBag<KinectBody> Dequeue()
        {
            var msg = Consumer.Queue.Dequeue();
            var body = msg.Body;

            KinectBag<KinectBody> data;
            using (var ms = new MemoryStream(body))
            {
                data = ProtoBuf.Serializer.Deserialize<KinectBag<KinectBody>>(ms);
            }
            return data;
        }

        /// <summary>
        /// Returns a body bag if the body is ready, or null if otherwise.
        /// </summary>
        /// <returns>KinectBag with the received data, or null.</returns>
        public KinectBag<KinectBody> DequeueNoWait()
        {

            var msg = Consumer.Queue.DequeueNoWait(null);
            if (msg == null || msg.Body == null) return null;

            KinectBag<KinectBody> data;
            using (var ms = new MemoryStream(msg.Body))
            {
                data = ProtoBuf.Serializer.Deserialize<KinectBag<KinectBody>>(ms);
            }

            return data;
        }
    }
}
