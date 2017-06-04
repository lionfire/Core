using LionFire.DependencyInjection;
using LionFire.Serialization.Contexts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Serialization
{
    public interface ISerializationService : ISerializer
    {
        IEnumerable<ISerializerStrategy> Serializers { get; }

        IEnumerable<ISerializerStrategy> SerializersForContext(SerializationContext context);
    }

    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class RegisterInterfaceAttribute : Attribute // FUTURE MOVE if desired
    {
        public Type[] Types { get; set; }
        public RegisterInterfaceAttribute(params Type[] types)
        {
            this.Types = types;
        }
    }

    [RegisterInterface(typeof(ISerializationService))]
    public class SerializationService : ISerializationService
    {

        public IEnumerable<ISerializerStrategy> Serializers
        {
            get
            {
                if (serializers == null)
                {
                    serializers = InjectionContext.Current.ServiceProvider.GetServices<ISerializerStrategy>();
                }
                return serializers;
            }
        }
        private IEnumerable<ISerializerStrategy> serializers;

        public virtual IEnumerable<ISerializerStrategy> SerializersForContext(SerializationContext context)
        {
            if (context == null)
            {
                return Serializers;
            }

            SortedList<float, ISerializerStrategy> results = new SortedList<float, ISerializerStrategy>();
            foreach (var serializer in Serializers)
            {
                var priority = serializer.GetPriorityForContext(context);
                if (float.IsNaN(priority)) continue;
                results.Add(-priority, serializer);
            }
            return results.Values;
        }

        #region ISerializer Implementation

        private T ForAllSerializers<T>(Func<ISerializerStrategy, SerializationContext, T> doIt, SerializationContext context) // REFACTOR?
        {
            List<Exception> exceptions = null;
            foreach (var serializer in SerializersForContext(context))
            {
                try
                {
                    return doIt(serializer, context);
                }
                catch (Exception ex)
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }
                    exceptions.Add(ex);
                }
            }
            if (exceptions == null)
            {
                throw new Exception("No serializers configured for context: " + context);
            }
            throw exceptions.Count == 1 ? exceptions[0] : new AggregateException(exceptions.ToArray());
        }

        public byte[] ToBytes(SerializationContext context)
        {
            return ForAllSerializers<byte[]>((s, c) =>
            {
                try
                {
                    return s.ToBytes(c);
                }
                finally
                {
                    c.OnSerialized(s);
                }
            }
            , context);

            //List<Exception> exceptions = null;
            //foreach (var serializer in service.SerializersForContext(context))
            //{
            //    try
            //    {
            //        return serializer.ToBytes(context);
            //    }
            //    catch (Exception ex)
            //    {
            //        if (exceptions == null)
            //        {
            //            exceptions.Add(ex);
            //        }
            //    }
            //}
            //throw exceptions.Count == 1 ? exceptions[0] : new AggregateException(exceptions.ToArray());
        }

        public string ToString(SerializationContext context)
        {
            return ForAllSerializers<string>((s, c) =>
            {
                try
                {
                    return s.ToString(c);
                }
                finally
                {
                    c.OnSerialized(s);
                }
            }
            , context);

            //List<Exception> exceptions = null;
            //foreach (var serializer in service.SerializersForContext(context))
            //{
            //    try
            //    {
            //        return serializer.ToString(context);
            //    }
            //    catch (Exception ex)
            //    {
            //        if (exceptions == null)
            //        {
            //            exceptions.Add(ex);
            //        }
            //    }
            //}
            //throw exceptions.Count == 1 ? exceptions[0] : new AggregateException(exceptions.ToArray());
        }

        public T ToObject<T>(SerializationContext context)
        {
            return ForAllSerializers<T>((s, c) => s.ToObject<T>(c), context);

            //List<Exception> exceptions = null;
            //foreach (var serializer in service.SerializersForContext(context))
            //{
            //    try
            //    {
            //        return serializer.ToObject<T>(context);
            //    }
            //    catch (Exception ex)
            //    {
            //        if (exceptions == null)
            //        {
            //            exceptions.Add(ex);
            //        }
            //    }
            //}
            //throw exceptions.Count == 1 ? exceptions[0] : new AggregateException(exceptions.ToArray());
        }

        #endregion
    }


    public static class ISerializationServiceExtensions
    {
        public static ISerializerStrategy Serializer(this SerializationContext context, ISerializationService serializationService = null)
        {
            if (serializationService == null)
            {
                serializationService = InjectionContext.Current.GetService<ISerializationService>();
            }
            return serializationService.SerializersForContext(context).FirstOrDefault();
        }

    }

}
