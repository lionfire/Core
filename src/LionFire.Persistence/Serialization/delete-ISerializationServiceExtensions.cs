// OLD - See IResolvesSerializationStrategiesExtensions
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//namespace LionFire.Serialization
//{
//    public static class ISerializationServiceExtensions
//    {
//        #region ISerializer Demux 

//        internal static T ForAllSerializers<T>(this IHasSerializationStrategies has, Func<ISerializationStrategy, SerializationOperation, SerializationContext, T> doIt, SerializationContext serializationContext = null, SerializerSelectionContext selectionContext = null) // REFACTOR?
//        {
//            var resolves = has as IResolvesSerializationStrategies;
//            var strategies = resolves != null ? resolves.Strategies(selectionContext).Select(result => result.Strategy) : has.AllStrategies;

//            List<Exception> exceptions = null;
//            foreach (var strategy in strategies)
//            {
//                try
//                {
//                    return doIt(strategy, serializationContext);
//                }
//                catch (Exception ex)
//                {
//                    if (exceptions == null)
//                    {
//                        exceptions = new List<Exception>();
//                    }
//                    exceptions.Add(ex);
//                }
//            }
//            if (exceptions == null)
//            {
//                throw new Exception("No serializers available for SerializerSelectionContext: " + selectionContext);
//            }
//            throw exceptions.Count == 1 ? exceptions[0] : new AggregateException(exceptions.ToArray());
//        }

//        public static byte[] ToBytes(this IHasSerializationStrategies has, SerializationOperation operation, SerializationContext context = null)
//        {
//            return has.ForAllSerializers<byte[]>((s, c) =>
//            {
//                try
//                {
//                    return s.ToBytes(c);
//                }
//                finally
//                {
//                    c.OnSerialized(s);
//                }
//            }
//            , context, selectionContext);

//            //List<Exception> exceptions = null;
//            //foreach (var serializer in service.SerializersForContext(context))
//            //{
//            //    try
//            //    {
//            //        return serializer.ToBytes(context);
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        if (exceptions == null)
//            //        {
//            //            exceptions.Add(ex);
//            //        }
//            //    }
//            //}
//            //throw exceptions.Count == 1 ? exceptions[0] : new AggregateException(exceptions.ToArray());
//        }

//        public static string ToString(this IHasSerializationStrategies has, SerializationContext serializationContext, SerializerSelectionContext selectionContext = null)
//        {
//            return has.ForAllSerializers<string>((s, c) =>
//            {
//                try
//                {
//                    return s.ToString(c);
//                }
//                finally
//                {
//                    c.OnSerialized(s);
//                }
//            }
//            , serializationContext, selectionContext);

//            //List<Exception> exceptions = null;
//            //foreach (var serializer in service.SerializersForContext(context))
//            //{
//            //    try
//            //    {
//            //        return serializer.ToString(context);
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        if (exceptions == null)
//            //        {
//            //            exceptions.Add(ex);
//            //        }
//            //    }
//            //}
//            //throw exceptions.Count == 1 ? exceptions[0] : new AggregateException(exceptions.ToArray());
//        }

//        //public static T ToObject<T>(this IHasSerializationStrategies has, Stream stream, SerializationContext serializationContext = null, SerializerSelectionContext selectionContext = null) => 
//        //    has.ForAllSerializers<T>(
//        //        (strategy, serializationContext2) => strategy.ToObject<T>(stream, s), serializationContext, selectionContext);


//        //List<Exception> exceptions = null;//foreach (var serializer in service.SerializersForContext(context))//{//    try//    {//        return serializer.ToObject<T>(context);//    }//    catch (Exception ex)//    {//        if (exceptions == null)//        {//            exceptions.Add(ex);//        }//    }//}//throw exceptions.Count == 1 ? exceptions[0] : new AggregateException(exceptions.ToArray());

//        #endregion

//        // REVIEW - is this a good idea?
//        //public static ISerializationStrategy Serializer(this SerializationContext context, IHasSerializationStrategies serializationService = null)
//        //{
//        //    if (serializationService == null)
//        //    {
//        //        serializationService = InjectionContext.Current.GetService<ISerializationService>();
//        //    }
//        //    return serializationService.GetSerializationStrategies(context).FirstOrDefault();
//        //}

//    }

//}
