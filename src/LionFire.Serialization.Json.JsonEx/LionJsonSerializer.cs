// OLD?

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
//using LionFire.Serialization.Newtonsoft.Json;
//using JsonExSerializer.TypeConversion; OLD
//using JsonExSerializer.Collections;
//using JsonExSerializer.Framework.ExpressionHandlers;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace LionFire.Serialization
{
    // Static portion
    public partial class LionJsonSerializer
    {
        #region Static

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

#if UNITY
		public delegate void ConfiguringHandler(JsonExSerializer.Serializer s);
		public static event ConfiguringHandler Configuring;
#else
        public static event Action<JsonExSerializer.Serializer> Configuring;
#endif

        public static void EnableAliases(Assembly assembly)
        {
            Configuring += serializer => serializer.InitializeAliases(assembly);
        }

        #endregion
    }

    public partial class LionJsonSerializer : LionSerializer, IStringLionSerializer
    {
        #region Configuration

        public override string Name => "LionJson";

        public override string DefaultFileExtension => "lson";

        public override byte[][] IdentifyingHeaders => new byte[][] { UTF8Encoding.UTF8.GetBytes("("), UTF8Encoding.UTF8.GetBytes("{") };

        #endregion

        #region (Public) Methods

        #region Deserialize

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

        #endregion

        #region Serialize
                
        public override void Serialize(Stream stream, object obj)
        {
            if (obj == null) throw new ArgumentNullException();

            lock (objectSerializerLock)
            {
                ObjectSerializer.Serialize(obj, stream);
            }
        }

        #endregion

        #endregion
    }

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

}