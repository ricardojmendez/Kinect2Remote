using System;
using System.IO;
using System.Collections.Generic;
using Arges.KinectRemote.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Arges.KinectRemote.Transport
{
    /// <summary>
    /// Handles reception of KinectBodyBags via RabbitMQ. Will process only the last message received.
    /// </summary>
    /// <seealso cref="Arges.KinectRemote.Data.KinectBodyBag"/>
    /// <seealso cref="Arges.KinectRemote.Transport.KinectBodyReceiver"/>
    public class KinectBodyReceiverLastOnly : IDisposable
    {
        IConnection _connection;
        IModel _channel;

        public KeepLastOnlyConsumer Consumer { get; private set; }

        public KinectBodyReceiverLastOnly(string ipAddress, string exchange)
        {
            var factory = new ConnectionFactory() { HostName = ipAddress };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange, "fanout");

            // Setting up the ttl to 30ms, since we don't particularly care about outdated frames.
            var queueParams = new Dictionary<string, object>() { { "x-message-ttl", 30 } };
            var queue = _channel.QueueDeclare("", false, true, true, queueParams);
            _channel.QueueBind(queue, exchange, "");

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
        /// Returns a body bag if the body is ready (/reggie).  WARNING: This is a blocking call.
        /// </summary>
        /// <returns>KinectBodyBag with the received data.</returns>
        public KinectBodyBag Dequeue()
        {
            KinectBodyBag data = null;
            while (data == null)
            {
                var msg = Consumer.Pop();
                if (msg != null)
                {
                    using (var ms = new MemoryStream(msg.Body))
                    {
                        data = ProtoBuf.Serializer.Deserialize<KinectBodyBag>(ms);
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
        /// Returns a body bag if the body is ready, or null if otherwise.
        /// </summary>
        /// <returns>KinectBodyBag with the received data, or null.</returns>
        public KinectBodyBag DequeueNoWait()
        {
            KinectBodyBag data = null;

            var msg = Consumer.Pop();
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
