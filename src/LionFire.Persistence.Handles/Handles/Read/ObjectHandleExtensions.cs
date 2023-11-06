#nullable enable

namespace LionFire.Persistence.Handles;

public static class ObjectHandleExtensions
{

    /// <summary>
    /// See also: ObjectHandleProviderExtensions.GetObjectReadHandle and other methods
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ObjectHandle<TValue> ToObjectHandle<TValue>(this TValue? value) => new ObjectHandle<TValue>(value);
    
}
