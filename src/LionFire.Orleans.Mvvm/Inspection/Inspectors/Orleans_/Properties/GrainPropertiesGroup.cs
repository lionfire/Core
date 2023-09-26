using LionFire.Inspection.Nodes;
using LionFire.ExtensionMethods.Orleans_;
using System.Reflection;
using LionFire.ExtensionMethods;
using System.Collections.Concurrent;

namespace LionFire.Inspection;

/// <summary>
/// See remarks for convention
/// </summary>
/// <remarks>
/// Properties are detected as follows
/// 
/// - May be only a get method, or only a set method, or both
/// - Methods with [Property(false)] are ignored
/// - Get methods must return a Task<TValue> or ValueTask<TValue>
/// - Set methods must return Task or ValueTask
/// - Set method must have one parameter which is assignable to the return type of the get method
/// 
/// - If both a getter and setter are present:
///   - The Get/Set prefix on each method is optional
/// 
/// - If only one of getter and setter are present
///   - either [Property] or [Property(true)] must be present
///   - or the method must start with Get or Set
/// </remarks>
public class GrainPropertiesGroup : SyncFrozenGroup
{
    #region (static)

    #region Constants

    public const string GetPrefix = "Get";
    public const string SetPrefix = "Set";

    public static bool HasGetPrefix(string name) => name.HasPrefix(GetPrefix);
    public static bool HasSetPrefix(string name) => name.HasPrefix(SetPrefix);

    #endregion

    private static ConcurrentDictionary<Type, IEnumerable<GrainPropertyInfo>> GrainPropertyInfosCache = new();

    private const bool includeWriteOnly = true; // DEPRECATED

    #endregion

    #region Lifecycle

    public GrainPropertiesGroup(IInspector inspector, INode parent, GroupInfo info, string? key = null, InspectorContext? inspectorContext = null) : base(inspector, parent, info, key ?? "group:Orleans.Grain.Properties", inspectorContext)
    {
    }

    #endregion

    protected IEnumerable<GrainPropertyInfo> GetGrainPropertyInfos(Type? type)
    {
        if (type == null) return Enumerable.Empty<GrainPropertyInfo>();


#if xtrue // TEMP
        GrainPropertyInfosCache.TryRemove(type, out var _);
#endif

        return GrainPropertyInfosCache.GetOrAdd(type, type =>
        {
            Dictionary<string, (MethodInfo get, MethodInfo? set)> readableProperties = new();
            List<MethodInfo> writeOnlyProperties = new();

            foreach (var getMethod in
            type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi =>
                       mi.GetParameters().Length == 0
                    && mi.ReturnType.IsTaskType()
                    && mi.ReturnType.IsGenericType
                    && mi.GetCustomAttribute<PropertyAttribute>()?.IsProperty != false
                ))
            {
                readableProperties.Add(getMethod.Name.TryRemovePrefixFromStart(GetPrefix), (getMethod, null));
            }

            foreach (var setMethod in type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi =>
                 mi.GetParameters().Length == 1
                && mi.ReturnType.IsTaskType()
                //&& mi.Name.StartsWith("Set")
                && (includeWriteOnly
                    || readableProperties.ContainsKey(mi.Name.Substring(SetPrefix.Length))
                    )
                && mi.GetCustomAttribute<PropertyAttribute>()?.IsProperty != false
                ))
            {
                var name = setMethod.Name.TryRemovePrefixFromStart(SetPrefix);
                if (readableProperties.ContainsKey(name)
                    && readableProperties[name].get.ReturnType.GetGenericArguments()[0].IsAssignableFrom(setMethod.GetParameters()[0].ParameterType)
                )
                {
                    readableProperties[name] = (readableProperties[name].get, setMethod);
                }
                else
                {
                    // Write-only properties must
                    //  - have the [Property] attribute
                    //  - or have the Set prefix
                    if (setMethod.GetCustomAttribute<PropertyAttribute>()?.IsProperty == true
                    || setMethod.Name.HasPrefix(SetPrefix))
                    {
                        writeOnlyProperties.Add(setMethod);
                    }
                }
            }

            return readableProperties
                    .Where(rp =>
                        (
                            (rp.Value.get!.GetCustomAttribute<PropertyAttribute>()?.IsProperty == true
                         || (HasGetPrefix(rp.Value.get!.Name)
                         || (rp.Value.set != null && HasSetPrefix(rp.Value.set.Name)
                         )
                        )
                        || (rp.Value.get != null && rp.Value.set != null))))
                    .Select(readableProperties => new GrainPropertyInfo(
                        readableProperties.Key,
                        readableProperties.Value.get,
                        readableProperties.Value.set))

                    .Concat(writeOnlyProperties.Select(mi => new GrainPropertyInfo(name: mi.Name.TryRemoveFromStart(SetPrefix), getMethod: null, setMethod: mi)))

                    .Concat(type.GetInterfaces().SelectMany(i => GetGrainPropertyInfos(i)));
        });

    }

    protected override IEnumerable<KeyValuePair<string, INode>> GetChildren()
    {
        return GetChildren2().ToList();
    }
    protected IEnumerable<KeyValuePair<string, INode>> GetChildren2()
    {
        var type = this.Parent!.Source?.GetType();

        if (type?.IsOrleansProxy() != true) yield break; // return Enumerable.Empty<GrainPropertyInfo>();

        foreach (var info in GetGrainPropertyInfos(type))
        {
            if (info.CanRead())
            {
                if (info.CanWrite())
                {
                    yield return new KeyValuePair<string, INode>(info.Name ?? NullConstants.NullDisplayString, new GrainReadWritePropertyNode(this, Parent.Source, info));

                }
                else
                {
                    yield return new KeyValuePair<string, INode>(info.Name ?? NullConstants.NullDisplayString, new GrainReadPropertyNode(this, Parent.Source, info));
                }
            }
            else if (info.CanWrite())
            {
                yield return new KeyValuePair<string, INode>(info.Name ?? NullConstants.NullDisplayString, new GrainWritePropertyNode(this, Parent.Source, info));
            }
        }
    }
}
