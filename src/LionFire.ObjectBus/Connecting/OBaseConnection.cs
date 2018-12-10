namespace LionFire.ObjectBus
{
    public class OBaseConnection<TOBase>
    {
        public string ConnectionString => OBase.ConnectionString;
        public IConnectingOBase OBase { get; set; }

        public OBaseConnection(IConnectingOBase obase)
        {
            this.OBase = obase;
        }
    }
}
