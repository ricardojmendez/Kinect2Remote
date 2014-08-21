namespace Arges.KinectRemote.Transport
{
    /// <summary>
    /// Abstract message publisher from which we can inherit other transport mechanisms
    /// </summary>
    public abstract class MessagePublisherBase
    {
        public string IpAddress { get; private set; }
        public string ConnectionString { get; private set; }

        public abstract void SendRawData(byte[] data, string topic);

        public abstract void SerializeAndSendObject<T>(T obj, string topic) where T:class;

        protected MessagePublisherBase(string ipAddress, string connectionString)
        {
            IpAddress = ipAddress;
            ConnectionString = connectionString;
        }
    }
}
