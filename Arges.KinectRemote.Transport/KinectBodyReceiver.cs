using System;
using System.IO;
using Arges.KinectRemote.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Arges.KinectRemote.Transport
{
    /// <summary>
    /// Handles reception of KinectBodyBags via RabbitMQ.
    /// </summary>
    /// <seealso cref="Arges.KinectRemote.Data.KinectBodyBag"/>
    public class KinectBodyReceiver: IDisposable
    {
        IConnection _connection;
        IModel _channel;

        public QueueingBasicConsumer Consumer { get; private set; }

        public KinectBodyReceiver(string ipAddress, string exchange)
        {
            var factory = new ConnectionFactory() { HostName = ipAddress };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange, "fanout");

            var queue = _channel.QueueDeclare();
            _channel.QueueBind(queue, exchange, "");

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
        /// <returns>KinectBodyBag with the received data.</returns>
        public KinectBodyBag Dequeue()
        {
            var msg = (BasicDeliverEventArgs)Consumer.Queue.Dequeue();
            var body = msg.Body;

            KinectBodyBag data;
            using (var ms = new MemoryStream(body))
            {
                data = ProtoBuf.Serializer.Deserialize<KinectBodyBag>(ms);
            }
            return data;
        }

        /// <summary>
        /// Returns a body bag if the body is ready, or null if otherwise.
        /// </summary>
        /// <returns>KinectBodyBag with the received data, or null.</returns>
        public KinectBodyBag DequeueNoWait()
        {
            KinectBodyBag data = null;

            var msg = (BasicDeliverEventArgs)Consumer.Queue.DequeueNoWait(null);
            if (msg != null && msg.Body != null)
            {
                using (var ms = new MemoryStream(msg.Body))
                {
                    data = ProtoBuf.Serializer.Deserialize<KinectBodyBag>(ms);
                }

            }

            return data;
        }
    }
}
