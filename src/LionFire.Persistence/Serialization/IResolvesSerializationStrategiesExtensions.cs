using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LionFire.Persistence;
using LionFire.Structures;

namespace LionFire.Serialization
{
    public static class IResolvesSerializationStrategiesExtensions
    {
        public static IEnumerable<SerializationSelectionResult> Strategies(this IResolvesSerializationStrategies resolves, Func<PersistenceOperation> operation, PersistenceContext context = null) => resolves.Strategies(operation.ToLazy(), context);

        public static bool ThrowWithSerializationFailureData = true;

        //public static ISerializationStrategy Strategy(this IResolvesSerializationStrategies resolves, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null) => resolves.Strategies(operation, context).Select(result => result.Strategy).FirstOrDefault();

        //private static T ForEachStrategy(this IResolvesSerializationStrategies resolves, Func<ISerializationStrategy, T>, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)

        #region ToBytes

        public static byte[] ToBytes(this IResolvesSerializationStrategies resolves, object obj, Func<PersistenceOperation> operation, PersistenceContext context = null) => resolves.ToBytes(obj, operation.ToLazy(), context);

        public static byte[] ToBytes(this IResolvesSerializationStrategies resolves, object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.Strategies(operation, context).Select(r => r.Strategy))
            {
                var (Bytes, Result) = strategy.ToBytes(obj, operation, context);
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
            throw new SerializationException(SerializationOperationType.ToBytes, operation, context, failures);
        }
        #endregion

        #region ToString

        public static string ToString(this IResolvesSerializationStrategies resolves, object obj, Func<PersistenceOperation> operation, PersistenceContext context = null) => resolves.ToString(obj, operation.ToLazy(), context);

        public static string ToString(this IResolvesSerializationStrategies resolves, object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.Strategies(operation, context).Select(r => r.Strategy))
            {
                var (String, Result) = strategy.ToString(obj, operation, context);
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
            throw new SerializationException(SerializationOperationType.ToString, operation, context, failures);
        }

        #endregion

        #region ToStream

        public static void ToStream(this IResolvesSerializationStrategies resolves, object obj, Stream stream, Func<PersistenceOperation> operation, PersistenceContext context = null) => resolves.ToStream(obj, stream, operation.ToLazy(), context);

        public static void ToStream(this IResolvesSerializationStrategies resolves, object obj, Stream stream, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.Strategies(operation, context).Select(r => r.Strategy))
            {
                var result = strategy.ToStream(obj, stream, operation, context);
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
            throw new SerializationException(SerializationOperationType.ToStream, operation, context, failures);
        }

        #endregion

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

        #region ToObject - byte[]

        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, byte[] bytes, Func<PersistenceOperation> operation, PersistenceContext context = null) => resolves.ToObject<T>(bytes, operation.ToLazy(), context);

        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, byte[] bytes, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.Strategies(operation, context).Select(r => r.Strategy))
            {
                var (Object, Result) = strategy.ToObject<T>(bytes, operation, context);
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
            throw new SerializationException(SerializationOperationType.FromBytes, operation, context, failures);
        }

        #endregion

        #region ToObject - string

        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, string str, Func<PersistenceOperation> operation, PersistenceContext context = null) => resolves.ToObject<T>(str, operation.ToLazy(), context);

        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, string str, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.Strategies(operation, context).Select(r => r.Strategy))
            {
                var (Object, Result) = strategy.ToObject<T>(str, operation, context);
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
            throw new SerializationException(SerializationOperationType.FromString, operation, context, failures);
        }

        #endregion

        #region ToObject - Stream

        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, Stream stream, Func<PersistenceOperation> operation, PersistenceContext context = null) => resolves.ToObject<T>(stream, operation.ToLazy(), context);

        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, Stream stream, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.Strategies(operation, context).Select(r => r.Strategy))
            {
                var (Object, Result) = strategy.ToObject<T>(stream, operation, context);
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
            throw new SerializationException(SerializationOperationType.FromStream, operation, context, failures);
        }

        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, Func<ISerializationStrategy, IEnumerable<Stream>> streams,  Func<PersistenceOperation> operation, PersistenceContext context = null) => resolves.ToObject<T>(streams, operation.ToLazy(), context);
        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, Func<ISerializationStrategy, IEnumerable<Stream>> streams, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.Strategies(operation, context).Select(r => r.Strategy))
            {
                foreach (var stream in streams(strategy))
                {
                    try
                    {
                        var (Object, Result) = strategy.ToObject<T>(stream, operation, context);
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
                    finally
                    {
                        stream.Dispose();
                    }
                }
            }
            //return (default(T), new SerializationResult { AggregateResults = failures }));
            throw new SerializationException(SerializationOperationType.FromStream, operation, context, failures);

            //List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;

            //foreach (var stream in streams)
            //{
            //    try
            //    {
            //        var result = resolves.ToObject<T>(stream, operation, context);
            //        return result;
            //    }
            //    catch(SerializationException sex)
            //    {
            //        failures.AddRange(sex.FailReasons);
            //    }
            //}
            //throw new SerializationException(SerializationOperationType.FromStream, operation, context, failures);
        }

        #endregion
    }
}
