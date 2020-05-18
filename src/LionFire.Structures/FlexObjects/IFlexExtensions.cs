using LionFire.Extensions.DefaultValues;
using LionFire.FlexObjects.Implementation;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace LionFire.FlexObjects
{

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
        public static IFlex Meta(this IFlex flex) => flex is IFlexWithMeta fwm ? fwm.Meta : GlobalFlexMetaDictionary.GetOrCreateValue(flex);
        static ConditionalWeakTable<object, Flex> GlobalFlexMetaDictionary = new ConditionalWeakTable<object, Flex>();

        #endregion

        #region Options

        public static FlexOptions Options(this IFlex flex, bool createIfMissing = false)
        {
            // TODO REFACTOR - change API philosophy ? --> Get(createIfMissing: createIfMissing);
            if (createIfMissing)
            {
                flex.Meta().AsTypeOrCreateDefault<FlexOptions>();
            }
            else
            {
                flex.Meta().Get<FlexOptions>();
            }
            return null;
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

        public static Type SingleValueType(this IFlex flex /*, FUTURE? bool considerCollections = false */)
        {
            if (flex.Value == null) return null;
            if (flex.Value is ITypedObject to) return to.Type;

            if (flex.IsFlexImplementationType())
            {
                /* if (considerCollections) { } else { */
                return typeof(IFlexImplementation);
            }

            return flex.Value.GetType();
        }

        public static bool IsSingleValue(this IFlex flex /*, FUTURE? bool considerCollections = false */)
        {
            return flex.SingleValueType() != typeof(IFlexImplementation);
        }

        public static object SingleValueOrDefault(this IFlex flex)
        {
            if (flex.Value is ITypedObject to) return to.Object;
            if (IsFlexImplementationType(flex.Value?.GetType())) return null;
            return flex.Value;
        }

        public static bool IsSingleTyped(this IFlex flex)
                => flex is ISingleTypeFlex || flex.Options()?.IsSingleType == true;

        #endregion

        #region Implementation

        public static bool IsFlexImplementationType(this IFlex flex) => flex.Value.GetType().IsFlexImplementationType();
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

        public static bool IsEmpty(this IFlex flex) => flex.Value == null; // TODO: Detect if child collections are empty

        #region Get

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flex"></param>
        /// <param name="name"></param>
        /// <param name="createIfMissing"></param>
        /// <param name="createFactory"></param>
        /// <param name="throwIfMissing">Use this to guarantee the return value won't be nulll.  If createIfMissing is true, an attempt will be made to create via the createFactory, and if that result is default(T), a CreationFailureException will be thrown.</param>
        /// <returns></returns>
        public static T Get<T>(this IFlex flex, string name = null, bool createIfMissing = false, Func<T> createFactory = null, bool throwIfMissing = true)
        {
            if (flex.Value is T match) return match;
            if (flex.Value is FlexTypeDictionary d)
            {
                return (T)d.Types[typeof(T)];
            }
            return default;
        }

        #region Convenience / Backporting

        public static T AsTypeOrCreateDefault<T>(this IFlex flex, Func<T> factory = null) => flex.Get(createIfMissing: true, createFactory: factory, throwIfMissing: true);

        #endregion

        #endregion

        #region Set

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flex"></param>
        /// <param name="value"></param>
        /// <param name="allowReplace"></param>
        /// <param name="throwOnFail">Set to true to ensure Set always succeeds in setting the value, otherwise an Exception is thrown.</param>
        /// <param name="onlyReplaceSameType"></param>
        /// <returns>True if an existing value was replaced, false if not.</returns>
        public static bool Set<T>(this IFlex flex, T value, bool allowReplace = true, bool throwOnFail = false, bool onlyReplaceSameType = true)
        {
            var valueType = typeof(T) != typeof(object) ? typeof(T) : value?.GetType();
            //Type existingType = flex.Value is ITypedObject to ? to.Type : flex.Value?.GetType();

            if (flex.Value == null)
            {
                flex.Value = EffectiveSingleValue<T>(value);
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
                flex.Value = EffectiveSingleValue<T>(value);
                return true; // ENH: Return replaced value
            }
        }

        #region Convenience

        // TODO: Different Set behavior depending on whether IFlex is single or multi typed.
        public static void Set_Old<T>(this IFlex flex, T value, bool allowReplace = true)
        {
            if (flex.Get<T>().IsDefault())
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

        public static void Add<T>(this IFlex flex, T obj)
        {
            // REVIEW - are all the corner cases covered?  Can this be refactored?

            object effectiveObject = EffectiveSingleValue(obj);

            if (flex.Value == null)
            {
                flex.Value = effectiveObject;
            }
            else
            {
                if (flex.Value is List<T> existingList)
                {
                    // 3rd (or later) item of list
                    existingList.Add(obj);
                }
                else if (flex.Value is T existingItem)
                {
                    // Convert a single existing value into a list and add the parameter
                    var list = new List<T> { existingItem, obj };
                    flex.Value = list;
                }
                else if (flex.Value is FlexTypeDictionary ftd)
                {
                    if (ftd.Types.ContainsKey(typeof(List<T>)))
                    {
                        ((List<T>)ftd.Types[typeof(List<T>)]).Add(obj);
                    }
                    if (ftd.Types.ContainsKey(typeof(T)))
                    {
                        var list = new List<T> { (T)ftd.Types[typeof(T)], obj };
                        ftd.Types.TryRemove(typeof(T), out _);
                        ftd.Add(list);
                    }
                    else
                    {
                        ftd.Add(obj);
                    }
                }
                else
                {
                    throw new UnreachableCodeException();
                    //// Something else is in the Flex, so allow for both

                    //Type collectionType = flex.Value is ITypedObject to ? to.Type : flex.Value.GetType();

                    //var dict = new FlexTypeDictionary(flex.Value, effectiveObject);
                    //dict.Add(list);
                    //dict.Add(flex.Value);
                    //flex.Value = dict;
                }
            }
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

}
