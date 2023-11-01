namespace LionFire.Data.Id;


/// <summary>
/// A strategy that can determine the Id of an object, given the object.
/// </summary>
public interface IIdMappingStrategy
{
    (bool, string) TryGetId(object obj);
}
