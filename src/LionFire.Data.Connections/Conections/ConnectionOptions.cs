namespace LionFire.Data.Connections
{
    public abstract class ConnectionOptions<TConcrete>
        where TConcrete : ConnectionOptions<TConcrete>
    {
        public int TimeoutMilliseconds { get; set; }

        protected static string ConfigurationKey => typeof(TConcrete).Name.Replace("ConnectionOptions", "");

        public abstract string ConnectionString { get; }
    }
}
