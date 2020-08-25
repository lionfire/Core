namespace LionFire.Data.Id
{
    public interface IIdAdapter : IIdMappingStrategy
    {
        string GetId(object obj);
    }
}
