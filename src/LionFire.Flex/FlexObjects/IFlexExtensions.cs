using LionFire.ExtensionMethods;
using LionFire.Extensions.DefaultValues;
using LionFire.FlexObjects.Implementation;
using System.Runtime.CompilerServices;

namespace LionFire.FlexObjects;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// How to:
///  - for multitype objects: set primary type
///  - lock down IFlex to support only a single type
/// </remarks>
public static class IFlexExtensions
{

    #region Meta

    /// <summary>
    /// Any IFlex can have meta accessible via this method.  If the IFlex object implements IFlexWithMeta, the metadata will be stored with the object; otherwise,
    /// it will be stored in a private ConditionalWeakTable GlobalFlexMetaDictionary.
    /// </summary>
    /// <param name="flex"></param>
    /// <returns></returns>
    // REVIEW: better idea: just add a class FlexMetaData { object Flex; } to the Flex object
    public static IFlex Meta(this IFlex flex) => flex is IFlexWithMeta fwm ? fwm.Meta : GlobalFlexMetaDictionary.GetOrCreateValue(flex);
    static ConditionalWeakTable<object, Flex> GlobalFlexMetaDictionary = new ConditionalWeakTable<object, Flex>();

    #endregion

    #region Options

    public static FlexOptions? Options(this IFlex flex, bool createIfMissing = false)
    {
        // TODO REFACTOR - change API philosophy ? --> Get(createIfMissing: createIfMissing);
        if (createIfMissing)
        {
            return flex.Meta().AsTypeOrCreateDefault<FlexOptions>();
        }
        else
        {
            return flex.Meta().GetOrCreate<FlexOptions>();
        }
    }

    #endregion

    #region TypedObject wrapper

    internal static object EffectiveSingleValue<T>(T obj)
    {
        object effectiveObject = obj;
        if (typeof(T) != obj?.GetType() && typeof(T) != typeof(object))
        {
            effectiveObject = new TypedObject<T> { Object = obj };
        }
        return effectiveObject;
    }

    #endregion

    #region Typing

    #region Single typing

    public static Type? SingleValueType(this IFlex flex /*, FUTURE? bool considerCollections = false */)
    {
        if (flex.FlexData == null) return null;
        if (flex.FlexData is ITypedObject to) return to.Type;

        if (flex.IsFlexImplementationType())
        {
            /* if (considerCollections) { } else { */
            return typeof(IFlexImplementation);
        }

        return flex.FlexData.GetType();
    }

    public static bool IsSingleValue(this IFlex flex /*, FUTURE? bool considerCollections = false */)
    {
        return flex.SingleValueType() != typeof(IFlexImplementation);
    }

    public static object? SingleValueOrDefault(this IFlex flex)
    {
        if (flex.FlexData is ITypedObject to) return to.Object;
        if (IsFlexImplementationType(flex.FlexData?.GetType())) return null;
        return flex.FlexData;
    }

    public static bool IsSingleTyped(this IFlex flex)
            => flex is ISingleTypeFlex || flex.Options()?.IsSingleType == true;

    #endregion

    #region Implementation

    public static bool IsFlexImplementationType(this IFlex flex) => flex.FlexData?.GetType().IsFlexImplementationType() ?? false;
    public static bool IsFlexImplementationType(this object obj) => obj.GetType().IsFlexImplementationType();
    public static bool IsFlexImplementationType(this Type t)
    {
        return t switch
        {
            _ => false,
        };
    }

    #endregion

    #endregion

    #region Add // TODO: change to Set


    #endregion

    public static bool IsEmpty(this IFlex flex) => flex.FlexData == null; // TODO: Detect if child collections are empty

    #region Get

    /// <summary>
    /// See also: GetMany which returns an IEnumerable<T> of 0 or 1 or more (TODO)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="flex"></param>
    /// <param name="name"></param>
    /// <param name="createIfMissing"></param>
    /// <param name="createFactory"></param>
    /// <param name="throwIfMissing">Use this to guarantee the return value won't be nulll.  If createIfMissing is true, an attempt will be made to create via the createFactory, and if that result is default(T), a CreationFailureException will be thrown.</param>
    /// <param name="createArgs">Ignored if createFactory is specified</param>
    /// <returns></returns>
    public static T GetOrCreate<T>(this IFlex flex, string? name = null, Func<T>? createFactory = null, object[]? createArgs = null)
    {
        if (Query<T>(flex, out var result, name)) { return result!; }
        //var result = Query<T>(flex, name); // OLD
        //if (!EqualityComparer<T>.Default.Equals(result, default)) { return result; }

        lock (flex) // REVIEW - locks on the flex object itself.  This is generally not advised. Is there another way? Should we add a new object() to the flex?  Dedicated ConditionalWeakTable?
        {
            #region redo inside lock
            if (Query(flex, out result, name)) { return result!; }
            //result = Query<T>(flex, name);
            //if (!EqualityComparer<T>.Default.Equals(result, default)) { return result; }
            #endregion

            result = createFactory != null ? createFactory()
                : createArgs != null ? (T)FlexGlobalOptions.DefaultCreateWithOptionsFactory(typeof(T), createArgs)
                    : (T)FlexGlobalOptions.DefaultCreateFactory(typeof(T));

            if (name == null) Add<T>(flex, result, allowMultipleOfSameType: false);
            else Add<T>(flex, name, result);
        }
        return result;
    }

    public static T? Query<T>(this IFlex flex, string? name = null)
        => Query<T>(flex, out var result, name) ? result : default;

    public static bool Query<T>(this IFlex flex, out T? result, string? name = null) // RENAME TryGet
    {
        if (name == null)
        {
            if (flex.FlexData is T directMatch)
            {
                result = directMatch;
                return true;
            }
            else if (flex.FlexData is TypedObject<T> match)
            {
                result = match.Object;
                return true;
            }
            else if (flex.FlexData is FlexTypeDictionary d && d.Types?.ContainsKey(typeof(T)) == true)
            {
                result = (T)d.Types[typeof(T)];
                return true;
            }
        }
        else
        {
            var dict = Query<Dictionary<string, T>>(flex);
            if (dict != null && dict.TryGetValue(name, out result))
            {
                return true;
            }
        }
        result = default;
        return false;
    }

    public static object? Query(this IFlex flex, Type type, string? name = null) 
        => Query(flex, type, out var result, name) ? result : default;

    public static bool Query(this IFlex flex, Type type, out object? result, string? name = null)
    {
        if (name == null)
        {
            if (flex.FlexData?.GetType() == type)
            {
                result = flex.FlexData;
                return true;
            }
            else if (flex.FlexData is ITypedObject typedObject && typedObject.Type == type)
            {
                result = typedObject.Object;
                return true;
            }
            else if (flex.FlexData is FlexTypeDictionary d && d.Types?.ContainsKey(type) == true)
            {
                result = d.Types[type];
                return true;
            } 
        }
        else
        {
            var dictType = typeof(Dictionary<,>).MakeGenericType(typeof(string), type);
            if (flex.Query(dictType, out var dict))
            {
                if (dict != null)
                {
                    result = ((System.Collections.IDictionary)dict)[name];
                    return result != null;
                }
            }
        }

        if (type != typeof(ITypedObjectProvider) && flex.Query<ITypedObjectProvider>(out var top))
        {
            result = top!.Query(type, name);
            if (result != null) return true;
        }

        result = default;
        return false;
    }


    #region Convenience / Backporting

    public static T AsTypeOrCreateDefault<T>(this IFlex flex, Func<T>? factory = null) => flex.GetOrCreate(createFactory: factory);

    #endregion

    #endregion

    #region Set

    /// <summary>
    /// Set the IFlex to a particular value.  Only allows one value of any type at a time for the entire Flex.  For multiple values, use Add instead.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="flex"></param>
    /// <param name="value"></param>
    /// <param name="allowReplace"></param>
    /// <param name="throwOnFail">Set to true to ensure Set always succeeds in setting the value, otherwise an Exception is thrown.</param>
    /// <param name="onlyReplaceSameType"></param>
    /// <returns>True if an existing value was replaced, false if not.</returns>
    public static bool SetExclusive<T>(this IFlex flex, T value, string? name = null, bool allowReplace = false, bool throwOnFail = false, bool onlyReplaceSameType = true)
    {
        if (name != null) throw new NotImplementedException(nameof(name));
        var valueType = typeof(T) != typeof(object) ? typeof(T) : value?.GetType();
        //Type existingType = flex.Value is ITypedObject to ? to.Type : flex.Value?.GetType();

        if (flex.FlexData == null)
        {
            flex.FlexData = EffectiveSingleValue<T>(value);
            return false;
        }
        else if (!allowReplace)
        {
            throw new AlreadySetException();
        }
        else if (flex.SingleValueType() != valueType && onlyReplaceSameType)
        {
            throw new AlreadySetException("Set onlyReplaceSameType to false to allow replacing values of different types");
        }
        else
        {
            flex.FlexData = EffectiveSingleValue<T>(value);
            return true; // ENH: Return replaced value
        }
    }

    #region Convenience

    // TODO: Different Set behavior depending on whether IFlex is single or multi typed.
    public static void Set_Old<T>(this IFlex flex, T value, bool allowReplace = true)
    {
        if (flex.GetOrCreate<T>().IsDefault())
        {
            if (allowReplace) flex.Set<T>(value);
            else throw new AlreadySetException();
        }
    }

    public static void AddRange<T>(this IFlex flex, IEnumerable<T> objects)
    {
        if (objects == null) return;
        foreach (var obj in objects)
        {
            flex.Add(obj);
        }
    }

    public static void Add(this IFlex flex, Type type, object obj)
    {
        throw new NotImplementedException("TODO: Call generic method");
        ////flex.Set(obj, allowReplace: false, throwOnFail: true);
        //flex.Add
    }
    //private static MethodInfo addMethod;


    public static void Set<T>(this IFlex flex, T obj) => _AddOrReplace<T>(flex, obj, allowMultipleOfSameType: false, replace: true);


    public class CollectionOptions<T>
    {
        public bool? AllowMultipleOfSameInstance { get; set; }

    }

    /// <summary>
    /// For the T type slot, there must be no existing value.  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="flex"></param>
    /// <param name="obj"></param>
    public static void AddSingle<T>(this IFlex flex, T obj)
        => Add(flex, obj, allowMultipleOfSameType: false, allowMultipleOfSameInstance: null);

    /// <summary>
    /// For the T type slot, there must be no existing value.  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="flex"></param>
    /// <param name="obj"></param>
    public static void AddOrReplace<T>(this IFlex flex, T obj)
        => _AddOrReplace<T>(flex, obj, allowMultipleOfSameType: false, allowMultipleOfSameInstance: null, replace: true);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="flex"></param>
    /// <param name="obj"></param>
    /// <param name="allowMultipleOfSameType"></param>
    /// <param name="allowMultipleOfSameInstance">true: multiple can be added.  false: throws if already contains obj.  null: ignores if already contains obj.</param>
    /// <exception cref="ArgumentException"></exception>
    public static void Add<T>(this IFlex flex, T obj, bool allowMultipleOfSameType = true, bool? allowMultipleOfSameInstance = null)
        => _AddOrReplace<T>(flex, obj, allowMultipleOfSameType, allowMultipleOfSameInstance);

    #region Keyed

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="flex"></param>
    /// <param name="obj"></param>
    /// <param name="allowMultipleOfSameType"></param>
    /// <param name="allowMultipleOfSameInstance"></param>
    public static void Add<T>(this IFlex flex, string key, T obj)
        => _AddOrReplaceKeyed<T>(flex, key, obj, replace: false);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="flex"></param>
    /// <param name="obj"></param>
    /// <param name="allowMultipleOfSameType"></param>
    /// <param name="allowMultipleOfSameInstance"></param>
    public static void Set<T>(this IFlex flex, string key, T obj)
        => _AddOrReplaceKeyed<T>(flex, key, obj, replace: true);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="flex"></param>
    /// <param name="obj"></param>
    /// <param name="allowMultipleOfSameType"></param>
    /// <param name="allowMultipleOfSameInstance">true: multiple can be added.  false: throws if already contains obj.  null: ignores if already contains obj.</param>
    /// <param name="replace">if true, replace existing with new obj.  Only happens if allowMultipleOfSameType is false</param>
    /// <exception cref="ArgumentException"></exception>
    private static void _AddOrReplaceKeyed<T>(this IFlex flex, string key, T obj, bool replace = false)
    {
        var dict = flex.GetOrCreate(createFactory: () => new Dictionary<string, T>());

        if (dict.TryGetValue(key, out var existing))
        {
            if (replace)
            {
                dict[key] = obj;
            }
            else
            {
                throw new AlreadySetException($"{nameof(replace)} is false but there is already a '{typeof(T).FullName}'");
            }
        }
        else
        {
            dict.Add(key, obj);
        }
    }

    #endregion


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="flex"></param>
    /// <param name="obj"></param>
    /// <param name="allowMultipleOfSameType"></param>
    /// <param name="allowMultipleOfSameInstance">true: multiple can be added.  false: throws if already contains obj.  null: ignores if already contains obj.</param>
    /// <param name="replace">if true, replace existing with new obj.  Only happens if allowMultipleOfSameType is false</param>
    /// <exception cref="ArgumentException"></exception>
    private static void _AddOrReplace<T>(this IFlex flex, T obj, bool allowMultipleOfSameType = true, bool? allowMultipleOfSameInstance = null, bool replace = false)
    {
        // REVIEW - are all the corner cases covered?  Can this be refactored?

        // TODO: Refactor to use this KVP everywhere: GetTypeForValue(flex.FlexData), flex.FlexData
        //var effectiveTypeForType = GetTypeForType(typeof(T));

        object effectiveObject = EffectiveSingleValue(obj);

        if (flex.FlexData == null)
        {
            // TODO: REVIEW: what if obj is null?
            flex.FlexData = effectiveObject;
        }
        else
        {
            if (flex.FlexData is List<T> existingList)
            {
                //if (!allowMultipleOfSameType) { throw new AlreadySetException($"{nameof(allowMultipleOfSameType)} is false but there is already a list of type '{typeof(T).FullName}'"); }
                // 3rd (or later) item of list
                if (OnExistingList(existingList)) { existingList.Add(obj); }
            }
            //else if (flex.FlexData is TypedObject<T> existingItem)
            //{

            //}
            else if (flex.FlexData is T existingItem) // TODO FIXME - I think we need to support TypedObject<T> here too
            {
                bool followThrough = true;
                if (!allowMultipleOfSameType)
                {
                    if (replace)
                    {
                        flex.FlexData = obj;
                        followThrough = false;
                    }
                    else
                    {
                        throw new AlreadySetException($"{nameof(allowMultipleOfSameType)} and {nameof(replace)} are false but there is already a '{typeof(T).FullName}'");
                    }
                }
                if (existingItem.Equals(obj))
                {
                    switch (allowMultipleOfSameInstance)
                    {
                        case true:
                            break;
                        case false:
                            throw new AlreadySetException();
                        case null:
                        default:
                            followThrough = false;
                            break;
                    }
                }
                if (followThrough)
                {
                    // Convert a single existing value into a list and add the parameter
                    var list = new List<T> { existingItem, obj };
                    flex.FlexData = list;
                }
            }
            else if (flex.FlexData is FlexTypeDictionary ftd)
            {
                if (ftd.Types.ContainsKey(typeof(List<T>)))
                {
                    List<T> existingList2 = (List<T>)ftd.Types[typeof(List<T>)];
                    if (OnExistingList(existingList2)) { existingList2.Add(obj); }
                }
                else if (ftd.Types.ContainsKey(typeof(T)))
                {
                    bool followThrough = true;
                    if (!allowMultipleOfSameType)
                    {
                        if (replace)
                        {
                            ftd.Types[typeof(T)] = obj;
                            followThrough = false;
                        }
                        else
                        {
                            throw new AlreadySetException($"{nameof(allowMultipleOfSameType)} and {nameof(replace)} are false but there is already a '{typeof(T).FullName}'");
                        }
                    }

                    var existingItem2 = ftd.Types[typeof(T)];
                    if (existingItem2.Equals(obj))
                    {
                        switch (allowMultipleOfSameInstance)
                        {
                            case true:
                                break;
                            case false:
                                throw new AlreadySetException();
                            case null:
                            default:
                                followThrough = false;
                                break;
                        }
                    }
                    if (followThrough)
                    {
                        var list = new List<T> { (T)ftd.Types[typeof(T)], obj };
                        ftd.Types.TryRemove(typeof(T), out _);
                        ftd.Add(list);
                    }
                }
                else
                {
                    if (obj != null) ftd.Add(typeof(T), obj);
                }
            }
            else
            {
                // Something else is in the Flex, so allow for both
                var dict = new FlexTypeDictionary(flex.FlexData);
                dict.Add(flex.FlexData);

                // TODO - this will fail if both objects resolve to the same type
                if (obj != null) dict.Add(obj);

                flex.FlexData = dict;
            }
        }

        #region (local methods)

        bool OnExistingList(List<T> existingList)
        {
            bool followThrough = true;
            switch (allowMultipleOfSameInstance)
            {
                case true:
                    break;
                case false:
                    if (existingList.Contains(obj)) throw new AlreadySetException();
                    break;
                case null:
                default:
                    followThrough = false;
                    break;
            }
            return followThrough;
        }

        #endregion
    }

    public static Type GetTypeForValue(object val)
        => val is ITypedObject typedObject ? typedObject.Type : val.GetType();

    public static Type GetTypeForType(Type type)
    {
        // TODO: Get attribute on type
        //type.GetCustomAttribute<FlexTypeAttribute>();
        return type;
    }

    //public static void GetOrAdd<T>(this ConcurrentDictionary<string, object> dict, string key, T value)
    //{
    //    dict.GetOrAdd()
    //}

    #endregion

    #endregion

    //#region Options

    //#region Default

    //public static Func<FlexMemberOptions> DefaultOptionsFactory = () => new FlexMemberOptions();

    //public static Func<FlexMemberOptions> GetDefaultOptionsFactory(this IFlex flex)
    //    => (Func<FlexMemberOptions>)flex.FlexDictionary.GetOrAdd("_optionsFactory", DefaultOptionsFactory);

    //#endregion

    //public static string GetOptionsKey<T>() => $"_options:({typeof(T).FullName})";
    //public static string GetOptionsKey<T>(string name) => $"_options:({typeof(T).FullName}){name}";

    //public static bool TryGetOptions<T>(this IFlex flex, out FlexMemberOptions options)
    //{
    //    var result = flex.FlexDictionary.TryGetValue(GetOptionsKey<T>(), out var o);
    //    options = (FlexMemberOptions)o;
    //    return result;
    //}

    //public static bool TryGetOptions<T>(this IFlex flex, string name, out FlexMemberOptions options)
    //{
    //    var result = flex.FlexDictionary.TryGetValue(GetOptionsKey<T>(name), out var o);
    //    options = (FlexMemberOptions)o;
    //    return result;
    //}

    //public static FlexMemberOptions Options<T>(this IFlex flex)
    //    => (FlexMemberOptions)flex.FlexDictionary.GetOrAdd(GetOptionsKey<T>(), flex.GetDefaultOptionsFactory());
    //public static FlexMemberOptions Options<T>(this IFlex flex, string name)
    //           => (FlexMemberOptions)flex.FlexDictionary.GetOrAdd(GetOptionsKey<T>(name), flex.GetDefaultOptionsFactory());

    //#endregion

    //public static Func<T> DefaultFactory<T>() => () => Activator.CreateInstance<T>();

    //public static string GetTypeKey<T>() => $"({typeof(T).FullName})";

    //public static T AsType<T>(this IFlex flex) => flex.FlexDictionary.TryGetValue(GetTypeKey<T>(), out var v) ? (T)v : default;
    //public static T AsTypeOrCreateDefault<T>(this IFlex flex, Func<T> factory = null) => (T)flex.FlexDictionary.GetOrAdd(GetTypeKey<T>(), (factory ?? DefaultFactory<T>())());

    public static IFlex ToFlex(this object obj, params object[] additionalObjects)
    {
        var result = new FlexObject(obj);
        if (additionalObjects != null)
        {
            result.AddRange(additionalObjects);
        }
        return result;
    }
}
