namespace LionFire.Collections.Async;

// Separated from IAsyncCreates for Orleans grains
public interface ICreatesAsync<TValue>
{
    /// <summary>
    /// Adds a new instance of a specified type.  
    /// 
    /// If this object is a collection, the newly created instance should typically be added to the collection
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    Task<TValue> Create(Type type, params object[] constructorParameters);

}

