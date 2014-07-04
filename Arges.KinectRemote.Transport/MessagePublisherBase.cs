namespace Arges.KinectRemote.Transport
{
    public abstract class MessagePublisherBase
    {
        public string IpAddress { get; private set; }
        public string ConnectionString { get; private set; }

        public abstract void SendRawData(byte[] data);

        public abstract void SerializeAndSendObject<T>(T obj) where T:class;

        protected MessagePublisherBase(string ipAddress, string connectionString)
        {
            IpAddress = ipAddress;
            ConnectionString = connectionString;
        }
    }
}
