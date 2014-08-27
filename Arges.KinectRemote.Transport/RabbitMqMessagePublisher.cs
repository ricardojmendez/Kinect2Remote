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
        /// have multiple sensors (once that is supported by the SDK).
        /// </summary>
        readonly string _senderId;

        ConnectionFactory _factory;
        readonly IConnection _connection;
        readonly IModel _channel;

        /// <summary>
        /// Initializes a RabbitMqMessagePublisher
        /// </summary>
        /// <param name="ipAddress">IP address of the RabbitMq server</param>
        /// <param name="exchangeName">Exchange to connect to</param>
        /// <param name="senderId">Sender identifier to use, normally the Kinect sensor id or possibly a user-assigned identifier</param>
        /// <param name="username">Username, defaults to guest</param>
        /// <param name="password">Password, defaults to guest</param>
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
        /// <param name="topic">Topic to send the data under</param>
        /// <remarks>
        /// Item data is sent under a topic of senderId.topic, so that remotes can 
        /// filter out any senders they don't particularly care about. topic is
        /// expected to be body or gesture right now.
        /// </remarks>
        public override void SendRawData(byte[] data, string topic)
        {
            if (data != null && data.Length > 0)
            {
                _channel.BasicPublish(ConnectionString, string.Format("{0}.{1}", _senderId, topic), null, data);
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
        public override void SerializeAndSendObject<T>(T obj, string topic)
        {
            using (var ms = new MemoryStream())
            {
                // Console.WriteLine("Serializing {0}", typeof(T));
                ProtoBuf.Serializer.Serialize(ms, obj);
                SendRawData(ms.ToArray(), topic);
            }
        }
    }
}
