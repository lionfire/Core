
namespace LionFire.Data.Async.Gets;

public static class IGetterX
{
    /// <summary>
    /// (Uses reflection)
    /// </summary>
    /// <param name="gets"></param>
    /// <returns></returns>
    public static async Task<IGetResult<object>> GetUnknownType(this IGetter gets, CancellationToken cancellationToken = default)
    {
        var getsInterface = gets.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IStatelessGetter<>)).Single();
        return await ((ITask<IGetResult<object>>)getsInterface.GetMethod(nameof(IStatelessGetter<object>.Get))!.Invoke(gets, new object?[] { cancellationToken })!).ConfigureAwait(false);
    }

    public static IEnumerable<Type> GetGetterTypes(this IGetter potentialGetter)
    {
        if (potentialGetter is null) yield break;

        foreach (var interfaceType in potentialGetter.GetType().GetInterfaces().Where(t => t.IsGenericType))
        {
            var genericArguments = interfaceType.GetGenericArguments();
            if (genericArguments.Length != 1) continue;
            if (interfaceType.GetGenericTypeDefinition() != typeof(IGetter<>)) continue;
            yield return genericArguments[0];
        }
    }

    public static async Task<T> GetValue<T>(this IStatelessGetter<T> resolves)
    {
        if (resolves is IGetter<T> lazilyResolves)
        {
            return await lazilyResolves.GetIfNeeded<T>().ConfigureAwait(false);
        }

        var result = await resolves.Get().ConfigureAwait(false);
        if (result.IsSuccess == true)
        {
            return result.Value!; // REVIEW nullability
        }
        else
        {
            throw result.ToException();
        }
    }
}
