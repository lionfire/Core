// OLD?

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using LionFire.Serialization.Newtonsoft.Json;

//using JsonExSerializer.TypeConversion; OLD
//using JsonExSerializer.Collections;
//using JsonExSerializer.Framework.ExpressionHandlers;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace LionFire.Serialization
{

    //public class TypeCollectionHandler : CollectionHandler
    //{
    //    public override ICollectionBuilder ConstructBuilder(object collection)
    //    {
    //        return new TypeCollectionBuilder(); 
    //    }

//    public override ICollectionBuilder ConstructBuilder(Type collectionType, int itemCount)
//    {
//        return new TypeCollectionBuilder();
//    }

//    public override bool IsCollection(Type collectionType)
//    {
//        return collectionType == typeof(List<Type>);
//    }
//}

//public class TypeCollectionBuilder : ICollectionBuilder
//{
//    List<Type> types = new List<Type>();
//    public void Add(object item)
//    {
//        types.Add((Type)item);            
//    }

//    public object GetReference()
//    {
//        return types;
//    }

//    public object GetResult()
//    {
//        return types;
//    }
//}

//class TypeExpressionHandler : IExpressionHandler
//{
//    public bool CanHandle(JsonExSerializer.Framework.Expressions.Expression expression)
//    {
//        throw new NotImplementedException();
//    }

//    public bool CanHandle(Type objectType)
//    {
//        throw new NotImplementedException();
//    }

//    public object Evaluate(JsonExSerializer.Framework.Expressions.Expression expression, object existingObject, IDeserializerHandler deserializer)
//    {
//        throw new NotImplementedException();
//    }

//    public object Evaluate(JsonExSerializer.Framework.Expressions.Expression expression, IDeserializerHandler deserializer)
//    {
//        throw new NotImplementedException();
//    }

//    public JsonExSerializer.Framework.Expressions.Expression GetExpression(object data, JsonExSerializer.Framework.Expressions.JsonPath currentPath, IExpressionBuilder serializer)
//    {
//        throw new NotImplementedException();
//    }

//    public bool IsReferenceable(object value)
//    {
//        throw new NotImplementedException();
//    }
//}

    public static class LionJsonSerializerExtensions
    {
        public static void InitializeAliases(this JsonExSerializer.Serializer serializer, Assembly a)
        {
#if !AOT
            //var timing = Timing.StartNow("InitializeAliases");
            foreach (var type in a.GetTypes())
            {
                if (type.IsInterface) continue;
                if (!type.IsPublic) continue;
                if (type.IsAbstract) continue;
                if (type.IsGenericTypeDefinition) continue; // TODO: JsonEx Parser doesn't parse apostrophe into the identifier yet
                try
                {
                    serializer.Config.TypeAliases.Add(type, type.Name);
                }
                catch (Exception ex)
                {
                    l.Warn("Failed to add type alias for: " + type.FullName + ".  Exception: " + ex);
                }
            }
            //timing.StopAndRecord();
#endif
        }

        private static readonly ILogger l = Log.Get();
		
    }

    public class LionJsonSerializer : LionSerializer, IStringLionSerializer
    {
        public override string Name => "LionJson";

        public override string DefaultFileExtension => "lson";

        public override byte[][] IdentifyingHeaders => new byte[][] { UTF8Encoding.UTF8.GetBytes("("), UTF8Encoding.UTF8.GetBytes("{") };

        //public override T Deserialize<T>(Stream stream)
        //{
        //    throw new NotImplementedException();
        //}

        //public override object Deserialize(Stream stream, Type type)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Serialize(Stream stream, object graph)
        //{
        //    throw new NotImplementedException();
        //}



        public override T Deserialize<T>(Stream stream)
        {
            var serializer = new JsonExSerializer.Serializer(typeof(T));

            Configure(serializer);
            T obj = (T)serializer.Deserialize(stream);
            return obj;
        }
        public override object Deserialize(Stream stream, Type type)
        {
            if (stream.Length == 0) throw new Exception("Empty stream");

            if (type == null)
            {
                type = typeof(object);
            }
            JsonExSerializer.Serializer serializer = new JsonExSerializer.Serializer(type);
            Configure(serializer);
            object obj = serializer.Deserialize(stream);

            return obj;
        }

        private static void Configure(JsonExSerializer.Serializer objectSerializer)
        {
            throw new NotImplementedException("TODO");
#if TODO
#if AOTWARNING // TOAOT
            l.Warn("AOTWARNING LionJsonSerializer.Configure, ICollection<>.Add x3.  TODO: Cast to concrete type before calling add. ");
#endif
            //objectSerializer.Config.TypeHandlerFactory.AttributeProcessors.Add(new UnignoreAttributeProcessor());

            objectSerializer.Config.TypeHandlerFactory.AttributeProcessors.Add(
    new IgnoreProcessor()
    {
        IgnoreContexts = LionSerializeContext.Persistence,
    });
            objectSerializer.Config.OutputTypeComment = false;
            //objectSerializer.Config.OutputTypeInformation = false;
            objectSerializer.Config.TypeHandlerFactory.AttributeProcessors.Add(new SerializeDefaultProcessor());
            objectSerializer.Config.TypeHandlerFactory.AttributeProcessors.Add(new DefaultValueProcessor());

            objectSerializer.Config.IgnoredPropertyAction = JsonExSerializer.SerializationContext.IgnoredPropertyOption.Ignore;

#if !NET35
            objectSerializer.Config.TypeAliases.Add(typeof(System.Dynamic.ExpandoObject), "_");

            objectSerializer.Config.RegisterTypeConverter(typeof(System.Dynamic.ExpandoObject), new ExpandoObjectJsonExConverter());
#endif
            
            objectSerializer.Config.DefaultValueSetting = JsonExSerializer.DefaultValueOption.SuppressDefaultValues;

            //objectSerializer.Config.RegisterTypeConverter(typeof(Type), new LionFire.Serialization.Serializers.JsonEx.JsonExTypeConverter());
            //objectSerializer.Config.RegisterTypeConverter(typeof(Type), new TypeToStringConverter());

            //objectSerializer.Config.TypeAliases.Add(typeof(), "");
            //objectSerializer.Config.CollectionHandlers.Add(new TypeCollectionHandler());
            //objectSerializer.Config.ExpressionHandlers.Add(
            //serializer.Config.RegisterTypeConverter(typeof(Type), new LionFire.Assets.AssetReferenceSerializationConverter());
            //serializer.Config.RegisterTypeConverter(typeof(Type), new LionFire.Assets.AssetIDSerializationConverter());
#endif
            Configuring?.Invoke(objectSerializer);
        }


        private static JsonExSerializer.Serializer ObjectSerializer
        {
            get
            {
                if (objectSerializer == null)
                {
                    objectSerializer = new JsonExSerializer.Serializer(typeof(object));
                    //JsonExSerializer.Serializer serializer = new JsonExSerializer.Serializer(obj.GetType());

                    Configure(objectSerializer);


                }
                return objectSerializer;
            }
        }
        private static JsonExSerializer.Serializer objectSerializer;
        private static object objectSerializerLock = new object();

        public override void Serialize(Stream stream, object obj)
        {
            if (obj == null) throw new ArgumentNullException();

            lock (objectSerializerLock)
            {
                ObjectSerializer.Serialize(obj, stream);
            }
        }

#if TODO


#endif

#if UNITY
		public delegate void  ConfiguringHandler(JsonExSerializer.Serializer s);
		public static event ConfiguringHandler Configuring;
#else
        public static event Action<JsonExSerializer.Serializer> Configuring;
#endif

        public static void EnableAliases(Assembly assembly)
        {
            Configuring += serializer => serializer.InitializeAliases(assembly);
        }
#region Misc

        private static readonly ILogger l = Log.Get();

#endregion


    }


}