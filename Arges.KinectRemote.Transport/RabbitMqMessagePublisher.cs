using System;
using System.IO;
using RabbitMQ.Client;

namespace Arges.KinectRemote.Transport
{
    /// <summary>
    /// Used for sending out data through RabbitMQ
    /// </summary>
    public sealed class RabbitMqMessagePublisher : MessagePublisherBase, IDisposable
    {
        /// <summary>
        /// Sender identifier, used by the receiver to filter which senders they
        /// care about. It is independent from the sensor ID since a sender may 
        /// have multiple sensors.
        /// </summary>
        readonly string _senderId;

        ConnectionFactory _factory;
        readonly IConnection _connection;
        readonly IModel _channel;

        public RabbitMqMessagePublisher(string ipAddress, string exchangeName, string senderId, string username = "guest", string password = "guest")
            : base(ipAddress, exchangeName)
        {
            Console.WriteLine("[RMQ] Creating RabbitMq publisher on {0} for protocol {1}", IpAddress, ConnectionString);

            _senderId = senderId;
            _factory = new ConnectionFactory { HostName = IpAddress, UserName = username, Password = password };
        
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare a fanout exchange so that every queue gets all messages
            _channel.ExchangeDeclare(ConnectionString, "topic");

            Console.WriteLine("[RMQ] Created {0}", ConnectionString);
        }

        /// <summary>
        /// Sends out raw data through RabbitMQ
        /// </summary>
        /// <param name="data">Byte array to send</param>
        /// <remarks>
        /// Body data is sent under a topic of senderId.body, so that remotes can 
        /// filter out any senders they don't particularly care about.
        /// </remarks>
        public override void SendRawData(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                _channel.BasicPublish(ConnectionString, string.Format("{0}.body", _senderId), null, data);
            }
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
            _factory = null;
        }


        /// <summary>
        /// Serializes an object using ProtoBuf and sends it through RabbitMQ
        /// </summary>
        /// <typeparam name="T">Type of object to serialize and send</typeparam>
        /// <param name="obj">The object instance to send</param>
        public override void SerializeAndSendObject<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                // Console.WriteLine("Serializing {0}", typeof(T));
                ProtoBuf.Serializer.Serialize(ms, obj);
                SendRawData(ms.ToArray());
            }
        }
    }
}
