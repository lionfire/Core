using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LionFire.Persistence;
using LionFire.Structures;

namespace LionFire.Serialization
{
    public interface IResolvesSerializationStrategies : IHasSerializationStrategies
    {
        /// <summary>
        /// Get available strategies, sorted to have best scores first
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerable<SerializationSelectionResult> Strategies(Lazy<PersistenceContext> context = null);
        //IEnumerable<SerializationSelectionResult> Strategies(Lazy<PersistenceContext> context = null);
    }

    public static class IResolvesSerializationStrategiesExtensions
    {
        public static bool ThrowWithSerializationFailureData = true;

        //public static ISerializationStrategy Strategy(this IResolvesSerializationStrategies resolves, Lazy<PersistenceContext> context = null) => resolves.Strategies(context).Select(result => result.Strategy).FirstOrDefault();

        //private static T ForEachStrategy(this IResolvesSerializationStrategies resolves, Func<ISerializationStrategy, T>, Lazy<PersistenceContext> context = null)
        

        public static byte[] ToBytes(this IResolvesSerializationStrategies resolves, object obj, Lazy<PersistenceContext> context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.Strategies(context).Select(r => r.Strategy))
            {
                var (Bytes, Result) = strategy.ToBytes(obj, context);
                if (Result.IsSuccess)
                {
                    return Bytes;
                }
                else if (ThrowWithSerializationFailureData)
                {
                    if (failures == null)
                    {
                        failures = new List<KeyValuePair<ISerializationStrategy, SerializationResult>>();
                    }
                    failures.Add(new KeyValuePair<ISerializationStrategy, SerializationResult>(strategy, Result));
                }
            }
            throw new SerializationException(SerializationOperationType.ToBytes, context.Value, failures);
        }

        public static string ToString(this IResolvesSerializationStrategies resolves, object obj, Lazy<PersistenceContext> context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.Strategies(context).Select(r => r.Strategy))
            {
                var (String, Result) = strategy.ToString(obj, context);
                if (Result.IsSuccess)
                {
                    return String;
                }
                else if (ThrowWithSerializationFailureData)
                {
                    if (failures == null)
                    {
                        failures = new List<KeyValuePair<ISerializationStrategy, SerializationResult>>();
                    }
                    failures.Add(new KeyValuePair<ISerializationStrategy, SerializationResult>(strategy, Result));
                }
            }
            throw new SerializationException(SerializationOperationType.ToString, context.Value, failures);
        }
        public static void ToStream(this IResolvesSerializationStrategies resolves, object obj, Stream stream, Lazy<PersistenceContext> context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.Strategies(context).Select(r => r.Strategy))
            {
                var result = strategy.ToStream(obj, stream, context);
                if (result.IsSuccess)
                {
                    return;
                }
                else if (ThrowWithSerializationFailureData)
                {
                    if (failures == null)
                    {
                        failures = new List<KeyValuePair<ISerializationStrategy, SerializationResult>>();
                    }
                    failures.Add(new KeyValuePair<ISerializationStrategy, SerializationResult>(strategy, result));
                }
            }
            throw new SerializationException(SerializationOperationType.ToStream, context.Value, failures);
        }

        //public struct Wrapper<T>
        //{
        //    public T Object { get; set; }
        //    public static implicit operator T(Wrapper<T> w) => w.Object;
        //    public static implicit Wrapper<T>(T obj) => new Wrapper<T> { Object = obj };
        //}
        //public struct WrapperOrFunc<T>
        //{
        //    //public T Object { get; set; }

        //    #region Object

        //    public T Object
        //    {
        //        get
        //        {
        //            if (obj == null && ObjectFunc != null)
        //            {
        //                obj = ObjectFunc();
        //            }
        //            return obj;
        //        }
        //        set => obj = value;
        //    }
        //    private T obj;

        //    #endregion

        //    public Func<T> ObjectFunc { get; set; }
        //    public static implicit operator T(WrapperOrFunc<T> w) => w.Object;
        //    public static implicit WrapperOrFunc<T>(T obj) => new WrapperOrFunc<T> { Object = obj };
        //    public static implicit WrapperOrFunc<T>(Func<T> func) => new WrapperOrFunc<T> { ObjectFunc = func };
        //}

        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, byte[] bytes, Lazy<PersistenceContext> context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.Strategies(context).Select(r => r.Strategy))
            {
                var (Object, Result) = strategy.ToObject<T>(bytes, context);
                if (Result.IsSuccess)
                {
                    return Object;
                }
                else if (ThrowWithSerializationFailureData)
                {
                    if (failures == null)
                    {
                        failures = new List<KeyValuePair<ISerializationStrategy, SerializationResult>>();
                    }
                    failures.Add(new KeyValuePair<ISerializationStrategy, SerializationResult>(strategy, Result));
                }
            }
            throw new SerializationException(SerializationOperationType.FromBytes, context.Value, failures);
        }
        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, string str, Lazy<PersistenceContext> context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.Strategies(context).Select(r => r.Strategy))
            {
                var (Object, Result) = strategy.ToObject<T>(str, context);
                if (Result.IsSuccess)
                {
                    return Object;
                }
                else if (ThrowWithSerializationFailureData)
                {
                    if (failures == null)
                    {
                        failures = new List<KeyValuePair<ISerializationStrategy, SerializationResult>>();
                    }
                    failures.Add(new KeyValuePair<ISerializationStrategy, SerializationResult>(strategy, Result));
                }
            }
            throw new SerializationException(SerializationOperationType.FromString, context.Value, failures);
        }
        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, Stream stream, Lazy<PersistenceContext> context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.Strategies(context).Select(r => r.Strategy))
            {
                var (Object, Result) = strategy.ToObject<T>(stream, context);
                if (Result.IsSuccess)
                {
                    return Object;
                }
                else if (ThrowWithSerializationFailureData)
                {
                    if (failures == null)
                    {
                        failures = new List<KeyValuePair<ISerializationStrategy, SerializationResult>>();
                    }
                    failures.Add(new KeyValuePair<ISerializationStrategy, SerializationResult>(strategy, Result));
                }
            }
            //return (default(T), new SerializationResult { AggregateResults = failures }));
            throw new SerializationException(SerializationOperationType.FromStream, context.Value, failures);
        }
    }
}
