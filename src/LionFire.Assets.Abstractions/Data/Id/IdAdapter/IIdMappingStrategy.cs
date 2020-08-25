namespace LionFire.Data.Id
{
    public interface IIdMappingStrategy
    {
        (bool, string) TryGetId(object obj);
    }
}
