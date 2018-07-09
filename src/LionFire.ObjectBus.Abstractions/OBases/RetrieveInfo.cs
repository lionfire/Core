namespace LionFire.ObjectBus
{
    /// <summary>
    /// Contains the "physical" location representing the lowest level reference before integrating with a data source outside the ObjectBus framework
    /// </summary>
    public class RetrieveInfo
    {
        public IOBase UltimateOBase { get; set; }
        public IReference UltimateReference { get; set; }
    }

}
