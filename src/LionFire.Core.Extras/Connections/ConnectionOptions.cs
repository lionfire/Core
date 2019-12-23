namespace LionFire.Data
{
    public class ConnectionOptions<TConcrete>
        where TConcrete : ConnectionOptions<TConcrete>
    {
        public int TimeoutMilliseconds { get; set; }

        protected static string ConfigurationKey => typeof(TConcrete).Name.Replace("ConnectionOptions", "");

        public string ConnectionString { get; set; }
    }
}
