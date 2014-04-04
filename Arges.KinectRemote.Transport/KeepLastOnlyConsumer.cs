using System.Collections;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace Arges.KinectRemote
{
    /// <summary>
    /// Consumer which keeps only the last received item.
    /// </summary>
    public class KeepLastOnlyConsumer : DefaultBasicConsumer
    {
        object _lock = new object();
        BasicDeliverEventArgs _lastItem;

        public KeepLastOnlyConsumer(IModel model)
            : base(model)
        {
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            lock (_lock)
            {
                var eventArgs = new BasicDeliverEventArgs();
                eventArgs.ConsumerTag = consumerTag;
                eventArgs.DeliveryTag = deliveryTag;
                eventArgs.Redelivered = redelivered;
                eventArgs.Exchange = exchange;
                eventArgs.RoutingKey = routingKey;
                eventArgs.BasicProperties = properties;
                eventArgs.Body = body;
                _lastItem = eventArgs;
            }
        }

        public BasicDeliverEventArgs Peek()
        {
            return _lastItem;
        }

        public BasicDeliverEventArgs Pop()
        {
            BasicDeliverEventArgs result = null;
            lock (_lock)
            {
                result = _lastItem;
                _lastItem = null;
            }
            return result;
        }
    }
}