﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Persistence;
using LionFire.Structures;

namespace LionFire.Serialization
{
    public static class IResolvesSerializationStrategiesExtensions
    {
        public static bool ThrowWithSerializationFailureData { get; set; } = true;

        public static IEnumerable<SerializationSelectionResult> Strategies(this IResolvesSerializationStrategies resolves, Func<PersistenceOperation> operation, PersistenceContext context = null) => resolves.ResolveStrategies(operation.ToLazy(), context);

        //public static ISerializationStrategy Strategy(this IResolvesSerializationStrategies resolves, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null) => resolves.Strategies(operation, context).Select(result => result.Strategy).FirstOrDefault();

        //private static TValue ForEachStrategy(this IResolvesSerializationStrategies resolves, Func<ISerializationStrategy, TValue>, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)

        #region ToBytes

        public static byte[] ToBytes(this IResolvesSerializationStrategies resolves, object obj, Func<PersistenceOperation> operation, PersistenceContext context = null) => resolves.ToBytes(obj, operation.ToLazy(), context);

        public static byte[] ToBytes<T>(this IResolvesSerializationStrategies resolves, T obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.ResolveStrategies(operation, context).Select(r => r.Strategy))
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
            foreach (var strategy in resolves.ResolveStrategies(operation, context).Select(r => r.Strategy))
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
            foreach (var strategy in resolves.ResolveStrategies(operation, context).Select(r => r.Strategy))
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

        //public struct Wrapper<TValue>
        //{
        //    public TValue Object { get; set; }
        //    public static implicit operator TValue(Wrapper<TValue> w) => w.Object;
        //    public static implicit Wrapper<TValue>(TValue obj) => new Wrapper<TValue> { Object = obj };
        //}
        //public struct WrapperOrFunc<TValue>
        //{
        //    //public TValue Object { get; set; }

        //    #region Object

        //    public TValue Object
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
        //    private TValue obj;

        //    #endregion

        //    public Func<TValue> ObjectFunc { get; set; }
        //    public static implicit operator TValue(WrapperOrFunc<TValue> w) => w.Object;
        //    public static implicit WrapperOrFunc<TValue>(TValue obj) => new WrapperOrFunc<TValue> { Object = obj };
        //    public static implicit WrapperOrFunc<TValue>(Func<TValue> func) => new WrapperOrFunc<TValue> { ObjectFunc = func };
        //}

        #region ToObject - byte[]

        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, byte[] bytes, Func<PersistenceOperation> operation, PersistenceContext context = null) => resolves.ToObject<T>(bytes, operation.ToLazy(), context);

        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, byte[] bytes, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.ResolveStrategies(operation, context).Select(r => r.Strategy))
            {
                var result = strategy.ToObject<T>(bytes, operation, context);
                if (result.IsSuccess)
                {
                    return result.Object;
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
            throw new SerializationException(SerializationOperationType.FromBytes, operation, context, failures);
        }

        #endregion

        #region ToObject - string

        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, string str, Func<PersistenceOperation> operation, PersistenceContext context = null) => resolves.ToObject<T>(str, operation.ToLazy(), context);

        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, string str, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.ResolveStrategies(operation, context).Select(r => r.Strategy))
            {
                var result = strategy.ToObject<T>(str, operation, context);
                if (result.IsSuccess)
                {
                    return result.Object;
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
            throw new SerializationException(SerializationOperationType.FromString, operation, context, failures);
        }

        #endregion

        #region ToObject - Stream

        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, Stream stream, Func<PersistenceOperation> operation, PersistenceContext context = null) => resolves.ToObject<T>(stream, operation.ToLazy(), context);

        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, Stream stream, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.ResolveStrategies(operation, context).Select(r => r.Strategy))
            {
                var result = strategy.ToObject<T>(stream, operation, context);
                if (result.IsSuccess)
                {
                    return result.Object;
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
            //return (default(TValue), new SerializationResult { AggregateResults = failures }));
            throw new SerializationException(SerializationOperationType.FromStream, operation, context, failures);
        }

        //public static Task<TValue> ToObject<TValue>(this IResolvesSerializationStrategies resolves, Func<PersistenceOperation> operation, PersistenceContext context = null)
            //=> resolves.ToObject<TValue>(operation.ToLazy(), context);

#nullable enable
        public static async Task<T> ToObject<T>(this IResolvesSerializationStrategies resolves, PersistenceOperation operation)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>>? failures = null;
            foreach (var strategy in resolves.ResolveStrategies(operation, operation.Context).Select(r => r.Strategy))
            {                
                await foreach (var stream in operation.CandidateStreams(strategy).ConfigureAwait(false))
                {
                    try
                    {
                        var result = strategy.ToObject<T>(stream, operation, operation.Context);
                        if (result.IsSuccess)
                        {
                            return result.Object;
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
                    finally
                    {
                        stream.Dispose();
                    }
                }
            }
            //return (default(TValue), new SerializationResult { AggregateResults = failures }));
            throw new SerializationException(SerializationOperationType.FromStream, operation, failReasons: failures);

            //List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;

            //foreach (var stream in streams)
            //{
            //    try
            //    {
            //        var result = resolves.ToObject<TValue>(stream, operation, context);
            //        return result;
            //    }
            //    catch(SerializationException sex)
            //    {
            //        failures.AddRange(sex.FailReasons);
            //    }
            //}
            //throw new SerializationException(SerializationOperationType.FromStream, operation, context, failures);
        }
#nullable disable


        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, Func<ISerializationStrategy, IEnumerable<Stream>> streams,  Func<PersistenceOperation> operation, PersistenceContext context = null) => resolves.ToObject<T>(streams, operation.ToLazy(), context);
        public static T ToObject<T>(this IResolvesSerializationStrategies resolves, Func<ISerializationStrategy, IEnumerable<Stream>> streams, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;
            foreach (var strategy in resolves.ResolveStrategies(operation, context).Select(r => r.Strategy))
            {
                foreach (var stream in streams(strategy))
                {
                    try
                    {
                        var result = strategy.ToObject<T>(stream, operation, context);
                        if (result.IsSuccess)
                        {
                            return result.Object;
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
                    finally
                    {
                        stream.Dispose();
                    }
                }
            }
            //return (default(TValue), new SerializationResult { AggregateResults = failures }));
            throw new SerializationException(SerializationOperationType.FromStream, operation, context, failures);

            //List<KeyValuePair<ISerializationStrategy, SerializationResult>> failures = null;

            //foreach (var stream in streams)
            //{
            //    try
            //    {
            //        var result = resolves.ToObject<TValue>(stream, operation, context);
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
