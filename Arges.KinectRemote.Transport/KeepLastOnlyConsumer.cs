using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace Arges.KinectRemote.Transport
{
    /// <summary>
    /// Consumer which keeps only the last received item.
    /// </summary>
    public class KeepLastOnlyConsumer : DefaultBasicConsumer
    {
        readonly object _lock = new object();
        BasicDeliverEventArgs _lastItem;

        public KeepLastOnlyConsumer(IModel model)
            : base(model)
        {
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            lock (_lock)
            {
                var eventArgs = new BasicDeliverEventArgs
                {
                    ConsumerTag = consumerTag,
                    DeliveryTag = deliveryTag,
                    Redelivered = redelivered,
                    Exchange = exchange,
                    RoutingKey = routingKey,
                    BasicProperties = properties,
                    Body = body
                };
                _lastItem = eventArgs;
            }
        }

        public BasicDeliverEventArgs Peek()
        {
            return _lastItem;
        }

        public BasicDeliverEventArgs Pop()
        {
            BasicDeliverEventArgs result;
            lock (_lock)
            {
                result = _lastItem;
                _lastItem = null;
            }
            return result;
        }
    }
}