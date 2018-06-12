
using System;
using System.Net;
using System.Linq;
using System.Windows;
//#if !NOESISGUI
//using System.Windows.Media;
//#else
//using Noesis;
//#endif
using System.IO;

#if MSGPACK
using SUtils.Serialization;
using SUtils.IO;
using SUtils.Serialization.MsgPack;
#endif

using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics.Contracts;
using System.ComponentModel;
using System.Collections;
using LionFire.Events;
using Microsoft.Extensions.Logging;

namespace LionFire.Serialization
{
    public enum LionSerializationMode
    {
        Unspecified = 0,
        Rpc,
        Persistence,
    }

    public abstract class LionTypeConverter
    {
        public abstract Type SerializationType { get; }
        public abstract Type Type { get; }

        public abstract object ConvertToSerializationType(object obj);
        public abstract object ConvertFromSerializationType(object obj);
    }
    
    public class LionTypeSerializer
    {
        #region Members

        private static ILogger l = Log.Get("LionFire.Serialization");

        /// <summary>
        /// Set by LionRing, if available, to serialize services as references.
        /// </summary>
        public IServiceSerializer ServiceSerializer = null;

        public Dictionary<Type, LionTypeConverter> TypeConverters;

        #endregion

        #region Construction

        public LionTypeSerializer()
        {
            TypeConverters = new Dictionary<Type, LionTypeConverter>();
            AddConverter(new LionTypeTypeConverter());
        }

        public void AddConverter(LionTypeConverter converter)
        {
            TypeConverters.Add(converter.Type, converter);
        }

        #endregion

        public LionSerializationMode LionSerializationMode = LionSerializationMode.Rpc;

        public bool SerializeEnumerableAsArrayWhenExpectedTypeIsAbstract = true;

        private Type GetEnumerableTypeForSerializingAsArray(Type expectedType)
        {
            if (SerializeEnumerableAsArrayWhenExpectedTypeIsAbstract && expectedType.IsInterface || expectedType.IsAbstract)
            {
                if (!expectedType.IsArray)
                {
                    bool isEnumerableNG = typeof(System.Collections.IEnumerable).IsAssignableFrom(expectedType);

                    if (!expectedType.IsArray && isEnumerableNG)
                    {
                        return expectedType.GetInterfaces()
                            .Where(t => t.IsGenericType)
                            .Where(t => t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                            //.Where(t => t.GetGenericTypeDefinition() == typeof(ICollection<>))
                            //.Select(t => t.GetGenericArguments()[0])
                            .FirstOrDefault();
                    }
                }
            }
            return null;
        }

        #region (Public) Methods



        public void Serialize(Stream stream, object obj, Type expectedType = null, SerializationReflection.TypeSerializationInfo tsi = null, bool? byValue = null)
        {
#if MSGPACK
            #region FUTURE: Embedded type information

            //bool nullExpectedType = false;

            //if (expectedType == null)
            //{
            //    nullExpectedType = true;
            //    expectedType = obj.GetType();
            //}
            if (expectedType == null) throw new NotSupportedException("expectedType must currently be known.");

            #endregion


            if (expectedType != null && expectedType.IsByRef && expectedType.HasElementType && expectedType.Name.StartsWith(expectedType.GetElementType().Name))
            {
                // For ByRef parameters: Serialize them by their value
                expectedType = expectedType.GetElementType();
                l.Warn("(UNTESTED) ByRef parameter detected, unwrapping to: " + expectedType);
            }    

            if (obj == null)
            {
                if (expectedType.IsValueType)
                {
                    if (!(expectedType.IsGenericType && expectedType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        throw new NotSupportedException("obj cannot be null for expectedType.IsValueType == true");
                    }
                }
            }
            else  // Validate obj type is ok for expectedType
            {
                Contract.Assert(obj != null);

                Type objectType = obj.GetType();

                if (obj as IEventInvoker != null && typeof(Delegate).IsAssignableFrom(expectedType))
                {
                    // Ok, special case for delegates
                }
                else 
                {
                    if (!expectedType.IsValueType && !expectedType.IsArray && !expectedType.IsAssignableFrom(objectType))
                    {
                        if (objectType.IsSubclassOf(expectedType))
                        {
                            throw new NotSupportedException("Object does not match expected type.  Derived classes are not supported yet.");
                        }
                        else
                        {
                            throw new ArgumentException("Object is not assignable to expected type.");
                        }
                    }
                }
            }

            // REVIEW - this could be moved up above validation so that converters
            // could have a chance to support incompatible obj.GetType() to expectedType pairings.
            if (TypeConverters.ContainsKey(expectedType))
            {
                var converter = TypeConverters[expectedType];
                object serializationTypeValue = converter.ConvertToSerializationType(obj);
                Serialize(stream, serializationTypeValue, converter.SerializationType);
                return;
            }

            if (expectedType.IsEnum)
            {
#if NET35
                expectedType = Enum.GetUnderlyingType(expectedType);
#else
                expectedType = expectedType.GetEnumUnderlyingType();
#endif

            }

            try
            {
                if (IsSimplePackType(expectedType))
                {
                    SimplePack.DoSimplePack(stream, GetSerializableValue(obj, expectedType));
                    return;
                }

                Type enumerableType = GetEnumerableTypeForSerializingAsArray(expectedType);


                if (expectedType.IsArray || enumerableType != null && byValue.HasValue && byValue.Value)
                {
                    if (obj == null)
                    {
                        SimplePack.PackNull(stream);
                    }
                    else
                    {
                        Array arr;
                        Type arrayElementType;
                        if (enumerableType != null)
                        {
                            arr = System.Linq.Enumerable.OfType<object>((IEnumerable)obj).ToArray<object>();
                            l.Trace("UNTESTED - Serializing IEnumerable<T> of size " + arr.Length);
                            arrayElementType = enumerableType.GetGenericArguments()[0];
                        }
                        else
                        {
                            arrayElementType = expectedType.GetElementType();
                            arr = (Array)obj;
                        }

                        if (IsSimplePackType(arrayElementType))
                        {
                            SimplePack.PackArray(stream, arr.OfType<object>().Select((o) => GetSerializableValue(o, arrayElementType)).ToArray());
                            return;
                        }
                        else
                        {
                            SUtils.Serialization.MsgPack.SimplePack.PackArray_HeaderOnly(stream, arr.Length);

                            var arrayElementTypeSerializationInfos = SerializationReflection.GetSerializationInfo(arrayElementType);

                            foreach (object arrayItem in arr)
                            {
                                //Type arrayItemType = arrayItem.GetType();
                                //if (arrayItemType != arrayElementType)
                                //{
                                //    throw new SerializationException("Currently, all items of array must match the array's ElementType.");
                                //}

                                Serialize(stream, arrayItem, arrayElementType);
                            }
                            return;
                        }
                    }
                }
                else if (expectedType.IsClass || expectedType.IsInterface || expectedType.IsStruct())
                {
                    // Recursive to serialize members of class or struct

                    if (typeof(Type).IsAssignableFrom(expectedType))
                    {
                        l.Warn("UNTESTED - Serializing Type: Type");
                    }

                    #region Serialize object as a service (or delegate)

                    if (byValue != true)
                    {
                        if (this.LionSerializationMode == Serialization.LionSerializationMode.Rpc && ServiceSerializer == null)
                        {
                            l.Trace("No service serializer is available with RPC serialization mode.");
                        }
                        else if (ServiceSerializer.CanSerialize(obj, expectedType))
                        {
                            // TODO - support multiple LionSerializableAttributes for different serialization methods?
                            var attr = expectedType.GetCustomAttribute<LionSerializableAttribute>(true); // REFLECTION OPTIMIZE
                            if (attr == null || attr.Method == SerializeMethod.ByServiceReference)
                            {
                                // Serialize by service reference
                                if (obj == null)
                                {
                                    SimplePack.PackNull(stream);
                                    return;
                                }
                                else
                                {
                                    ServiceSerializer.Serialize(stream, obj, expectedType);
                                    return;
                                }
                            }
                            // Fall thru to serialize by value
                        }
                    }
                    #endregion

                    if (typeof(Delegate).IsAssignableFrom(expectedType))
                    {
                        l.Warn("Warning: delegate was not serialized by service serializer.  It will not be serialized at all.");
                        return;
                    }

                    #region Serialize Class/Interface/Struct by value

                    if (expectedType.IsAbstract || expectedType.IsInterface)
                    {
                        l.Trace("Cannot yet serialize to an expected type that is abstract or an interface unless it has LionServiceAttribute.  Ignoring object of type: " + expectedType.FullName);
                        //throw new NotImplementedException("Cannot yet serialize to an expected type that is abstract or an interface unless it has LionServiceAttribute.");
                    }
                    if (obj == null)
                    {
                        SimplePack.PackNull(stream);
                    }
                    else
                    {
                        if (tsi == null)
                        {
                            tsi = SerializationReflection.GetSerializationInfo(expectedType);
                        }
                        //if (tsi.ByValue) byValue = true; // Already in byValue code

                        //bool isFirst = true;

                        IgnoreAttribute ignoreAttribute;
                        foreach (FieldInfo mi in tsi.FieldInfos)
                        {
                            if (mi.Name.Length == 1 && Char.IsLower(mi.Name[0])) continue; // TEMP HACK 

                            ignoreAttribute = mi.GetCustomAttribute<IgnoreAttribute>(); // REFLECTION OPTIMIZE
                            if (ignoreAttribute != null && ((ignoreAttribute.Ignore & LionSerializeContext.Network) != LionSerializeContext.None)) continue;  // REFLECTION OPTIMIZE

                            var attr = mi.GetCustomAttribute<LionSerializableAttribute>(true); // REFLECTION OPTIMIZE
                            bool memberByValue = attr != null && attr.Method == SerializeMethod.ByValue;

                            object value = mi.GetValue(obj);

                            if (value == null) { stream.WriteByte((byte)LionPackEncodingTypes.Literal_ChildNull); continue; }
                            //if (isFirst && value == null)
                            //{
                            //    stream.WriteByte((byte)LionPackEncodingTypes.Literal_ChildNull);
                            //    //throw new NotImplementedException("BUG: First field/property cannot be null."); 
                            //}
                            //isFirst = false;

                            Serialize(stream, value, mi.FieldType, null, memberByValue);
                        }
                        foreach (PropertyInfo mi in tsi.PropertyInfos)
                        {
                            if (mi.Name.Length == 1 && Char.IsLower(mi.Name[0])) continue; // TEMP HACK 

                            ignoreAttribute = mi.GetCustomAttribute<IgnoreAttribute>(); // REFLECTION OPTIMIZE
                            if (ignoreAttribute != null && ((ignoreAttribute.Ignore & LionSerializeContext.Network) != LionSerializeContext.None)) continue;  // REFLECTION OPTIMIZE

                            var attr = mi.GetCustomAttribute<LionSerializableAttribute>(true); // REFLECTION OPTIMIZE
                            bool memberByValue = attr != null && attr.Method == SerializeMethod.ByValue;

                            object value = mi.GetValue(obj, null);

                            if (value == null) { stream.WriteByte((byte)LionPackEncodingTypes.Literal_ChildNull); continue; }
                            //if (isFirst && value == null)
                            //{
                            //    stream.WriteByte((byte)LionPackEncodingTypes.Literal_ChildNull);
                            //    //throw new NotImplementedException("BUG: First field/property cannot be null."); 
                            //}
                            //isFirst = false;

                            Serialize(stream, value, mi.PropertyType, null, memberByValue);
                        }
                    }
                    #endregion

                    return;
                }
                else
                {
                    throw new UnreachableCodeException("Unexpected type: " + expectedType.FullName);
                }
            }
            catch (Exception ex)
            {
                string msg = "Error during serialization of type " + expectedType.FullName + ". ";
                l.Error(msg + ex);
                throw new SerializationException(msg, ex);
            }
#else
            throw new NotImplementedException("MSGPACK disabled in this build");
#endif
        }

        public class LionTypeTypeConverter : LionTypeConverter
        {

            public override Type SerializationType
            {
                get { return typeof(string); }
            }

            public override Type Type
            {
                get { return typeof(Type); }
            }

            public override object ConvertToSerializationType(object obj)
            {
                // TODO NETOPTIMIZE - use numeric tokens
                Type type = (Type)obj;
                return type.AssemblyQualifiedName;
            }

            public override object ConvertFromSerializationType(object obj)
            {
                string typeFullName = (string)obj;
                return Type.GetType(typeFullName);
            }
        }

        public object Deserialize(Stream stream, Type expectedType, bool? byValue = null)
        {
#if !MSGPACK
            throw new NotImplementedException("MSGPACK disabled in this build");
#else
            if (TypeConverters.ContainsKey(expectedType))
            {
                var converter = TypeConverters[expectedType];
                object serializationTypeValue = Deserialize(stream, converter.SerializationType);
                object value = converter.ConvertFromSerializationType(serializationTypeValue);
                return value;
            }

            try
            {
                if (expectedType.IsEnum)
                {
                    expectedType = expectedType.GetEnumUnderlyingType();
                }
                if (IsSimplePackType(expectedType))
                {
                    SerializableValue serializableValue = SUtils.Serialization.MsgPack.SimpleUnpack.DoSimpleUnpack(stream);

                    object obj = GetValue(serializableValue, expectedType);
                    return obj;
                }

                Type enumerableType = GetEnumerableTypeForSerializingAsArray(expectedType);

                if (SerializeEnumerableAsArrayWhenExpectedTypeIsAbstract &&
                    (expectedType.IsInterface || expectedType.IsAbstract) && 
                    !expectedType.IsArray)
                {
                    bool isEnumerable = typeof(System.Collections.IEnumerable).IsAssignableFrom(expectedType);

                    if (isEnumerable)
                    {
                        enumerableType = expectedType.GetInterfaces()
                            .Where(t => t.IsGenericType)
                            .Where(t => t.GetGenericTypeDefinition() == typeof(ICollection<>))
                            //.Select(t => t.GetGenericArguments()[0])
                            .FirstOrDefault();
                    }
                }

                if (expectedType.IsArray || enumerableType != null && byValue.HasValue && byValue.Value)
                {
                    bool isNull = MessagePackUtils.IsNull(stream);
                    if (isNull)
                    {
                        return null;
                    }
                    else
                    {
                        Type elementType;
                        MethodInfo addMethod;

                        if (enumerableType != null)
                        {
                            addMethod = enumerableType.GetMethod("Add");
                            elementType = enumerableType.GetGenericArguments()[0];
                            l.Trace("UNTESTED - Deserializing IEnumerable<" + elementType.Name + ">");
                        }
                        else
                        {
                            addMethod = null;
                            elementType = expectedType.GetElementType();
                        }

                        if (IsSimplePackType(elementType))
                        {
                            SerializableValue serializableValue = SUtils.Serialization.MsgPack.SimpleUnpack.UnpackArray(EncodingType.GetEncodingTypeOf(stream.EnsureReadByte()), stream);
                            Array array = (Array)GetValue(serializableValue, expectedType);

                            if (enumerableType != null)
                            {
                                l.Trace("UNTESTED - Deserializing ICollection<" + elementType.Name + "> (simple) of size " + array.Length);
                                object collectionResult = Activator.CreateInstance(enumerableType); // UNTESTED
                                foreach (object item in array)
                                {
                                    addMethod.Invoke(collectionResult, new object[] { item });
                                }
                                return collectionResult;
                            }
                            else
                            {
                                return array;
                            }
                        }
                        else // !IsSimplePackType(elementType)
                        {
                            var arrayLength = (int)SimpleUnpack.UnpackArray_GetLength(EncodingType.GetEncodingTypeOf(stream.EnsureReadByte()), stream);

                            //l.Trace("UNTESTED - Deserializing ICollection<" + elementType.Name + "> (non-simple) of size " + arrayLength);

                            //var tsi = SerializationReflection.GetSerializationInfo(elementType);

                            if (enumerableType != null)
                            {
                                object items = Activator.CreateInstance(expectedType); // UNTESTED

                                for (int i = 0; i < arrayLength; i++)
                                {
                                    //object item = DeserializeObject(stream, elementType, tsi);
                                    object item = Deserialize(stream, elementType);
                                    addMethod.Invoke(items, new object[] { item });
                                }
                                return items;
                            }
                            else
                            {
                                List<object> items = new List<object>();

                                for (int i = 0; i < arrayLength; i++)
                                {
                                    //items.Add(DeserializeObject(stream, elementType, tsi));
                                    items.Add(Deserialize(stream, elementType));
                                }

                                MethodInfo castMethod = typeof(System.Linq.Enumerable).GetMethod("Cast").MakeGenericMethod(elementType); // REFLECTION OPTIMIZE
                                MethodInfo toArrayMethod = typeof(System.Linq.Enumerable).GetMethod("ToArray").MakeGenericMethod(elementType); // REFLECTION OPTIMIZE
                                object castedEnum = castMethod.Invoke(null, new object[] { items });
                                object castedArray = toArrayMethod.Invoke(null, new object[] { castedEnum });
                                return castedArray;
                            }
                        }
                    }
                }
                else if (expectedType.IsClass || expectedType.IsInterface || expectedType.IsStruct())
                {
                    if (expectedType == typeof(Object))
                    {
                        throw new NotSupportedException("Deserializing Objects of unknown type are not supported");
                    }
                    else if (expectedType.IsAssignableFrom(typeof(Type)))
                    {
                        l.Warn("UNTESTED - Deserializing Type: Type");
                        throw new UnreachableCodeException("Classes deriving from Type are not supported");
                    }

                    #region Deserialize object as a service

                    if (this.LionSerializationMode == Serialization.LionSerializationMode.Rpc)
                    {
                        if (ServiceSerializer == null)
                        {
                            //l.Trace("No service serializer is available with RPC serialization mode.");

                            l.Trace("No service serializer is available.  Not deserializing object of type: " + expectedType.FullName + ".  This will (or should) cause a serialization failure if the serializer did not also skip this object.");
                            //throw new NotSupportedException("No service serializer is available.  Not deserializing object of type: " + expectedType.FullName + ".  This will (or should) cause a serialization failure if the serializer did not also skip this object.  If this is undesired, ensure LionRpc or another ServiceSerialier has been initialized.");
                        } // Fall through to deserialize by value
                        else if (byValue != true && ServiceSerializer.CanDeserialize(expectedType))
                        {
                            var attr = expectedType.GetCustomAttribute<LionSerializableAttribute>(true); // REFLECTION OPTIMIZE
                            if (attr == null || attr.Method == SerializeMethod.ByServiceReference)
                            {
                                bool isNull = MessagePackUtils.IsNull(stream);
                                if (isNull)
                                {

                                    return null;
                                }
                                else
                                {
                                    object obj = ServiceSerializer.Deserialize(stream, expectedType);
                                    return obj;
                                }
                            } //else Fall through to deserialize by value                            
                        } //else Fall through to deserialize by value                        
                    }  //else Fall through to deserialize by value                        
                    #endregion

                    if (typeof(Delegate).IsAssignableFrom(expectedType))
                    {
                        l.Warn("Warning: delegate was not deserialized by service serializer.  It will not be deserialized at all.");
                        return null;
                    }

                    #region Deserialize by value

                    if (expectedType.IsAbstract || expectedType.IsInterface)
                    {
                        l.Trace("Cannot yet deserialize by value an expected type that is abstract or an interface.  Ignoring object of type: " + expectedType.FullName);
                        //throw new NotImplementedException("Abstract and interface types are not yet supported.");
                        return null;
                    }

                    if (MessagePackUtils.IsNull(stream))
                    {
                        return null;
                    }
                    else
                    {
                        return DeserializeObject(stream, expectedType);
                    }

                    #endregion
                }

                throw new UnreachableCodeException("Unexpected: do not know how to deserialize object of type: " + expectedType.FullName);

                //switch (parameterType.Name) // OLD
                //{
                //    case "String":
                //        obj = SUtils.Serialization.MsgPack.SimpleUnpack.DoSimpleUnpack(ms).StringValue;
                //        break;

                //    case "Int16":
                //        obj = (Int16)SUtils.Serialization.MsgPack.SimpleUnpack.DoSimpleUnpack(ms).Int32value;
                //        break;
                //    case "UInt16":
                //        obj = (UInt16)SUtils.Serialization.MsgPack.SimpleUnpack.DoSimpleUnpack(ms).UInt64value;
                //        break;

                //    case "Int32":
                //        obj = SUtils.Serialization.MsgPack.SimpleUnpack.DoSimpleUnpack(ms).Int32value;
                //        break;
                //    case "UInt32":
                //        obj = (UInt32)SUtils.Serialization.MsgPack.SimpleUnpack.DoSimpleUnpack(ms).UInt64value;
                //        break;

                //    case "Int64":
                //        obj = SUtils.Serialization.MsgPack.SimpleUnpack.DoSimpleUnpack(ms).Int64value;
                //        break;
                //    case "UInt64":
                //        obj = SUtils.Serialization.MsgPack.SimpleUnpack.DoSimpleUnpack(ms).UInt64value;
                //        break;

                //    default:
                //        {
                //            if (parameterType.IsArray)
                //            {
                //                IEnumerable<SerializableValue> items = SUtils.Serialization.MsgPack.SimpleUnpack.DoSimpleUnpack(ms).ArrayItems;
                //                int arrayLength = items.Count();
                //                Array array = Array.CreateInstance(parameterType.GetElementType(), arrayLength);

                //                foreach(SerializableValue sv in items)
                //                {
                //                    array[i] = sv.
                //                }
                //                for(int i = 0; i < arrayLength; i++)
                //                {

                //                }
                //                List<SerializableValue> values = new List<SerializableValue>();
                //                foreach (object arrayItem in arr)
                //                {
                //                    values.Add(GetSerializableValue(arrayItem, parameterType.GetElementType()));
                //                }
                //                SUtils.Serialization.MsgPack.SimplePack.(rpcMessageStream, (SerializableValue[])obj);
                //                break;
                //            }
                //            else
                //            {
                //                throw new NotSupportedException("Unsupported serialization type. (TODO: Support more types.");
                //            }
                //        }

                //}

            }
            catch (Exception ex)
            {
                string msg = "Error during deserialization of type " + expectedType.FullName + ". ";
                l.Error(msg + ex.ToString());

                throw new SerializationException(msg, ex);
            }
#endif
        }

        private object DeserializeObject(Stream stream, Type objectType, SerializationReflection.TypeSerializationInfo tsi = null)
        {
#if !MSGPACK
            throw new NotImplementedException("MSGPACK disabled in this build");
#else
            if (tsi == null)
            {
                tsi = SerializationReflection.GetSerializationInfo(objectType);
            }


            if (!objectType.IsClass && !objectType.IsStruct())
            {
                l.Warn("DeserializeObject objectType unexpected: " + objectType.FullName);
            }
            Contract.Assert(objectType.IsClass || objectType.IsStruct());

            object obj = Activator.CreateInstance(objectType);

            IgnoreAttribute ignoreAttribute;
            foreach (FieldInfo mi in tsi.FieldInfos)
            {
                if (mi.Name.Length == 1 && Char.IsLower(mi.Name[0])) continue; // TEMP HACK - OLD not needed anymore???? 

                ignoreAttribute = mi.GetCustomAttribute<IgnoreAttribute>(); // REFLECTION OPTIMIZE
                if (ignoreAttribute != null && ((ignoreAttribute.Ignore & LionSerializeContext.Network) != LionSerializeContext.None)) continue;  // REFLECTION OPTIMIZE

                if (!mi.FieldType.IsValueType && (stream.IsNull() || stream.IsChildNull()))
                {
                    // Do nothing to keep default value of null
                    //mi.SetValue(obj, null);
                    continue;
                }

                if (IsSimplePackType(mi.FieldType))
                {
                    SerializableValue serializableValue = SimpleUnpack.DoSimpleUnpack(stream);
                    object val = GetValue(serializableValue, mi.FieldType);
                    mi.SetValue(obj, val);
                }
                else
                {
                    var attr = mi.GetCustomAttribute<LionSerializableAttribute>(true); // REFLECTION OPTIMIZE
                    bool memberByValue = attr != null && attr.Method == SerializeMethod.ByValue;

                    object memberValue = Deserialize(stream, mi.FieldType, memberByValue);
                    mi.SetValue(obj, memberValue);
                }

            }
            foreach (PropertyInfo mi in tsi.PropertyInfos)
            {
                if (mi.Name.Length == 1 && Char.IsLower(mi.Name[0])) continue; // TEMP HACK 

                ignoreAttribute = mi.GetCustomAttribute<IgnoreAttribute>(); // REFLECTION OPTIMIZE
                if (ignoreAttribute != null && ((ignoreAttribute.Ignore & LionSerializeContext.Network) != LionSerializeContext.None)) continue;  // REFLECTION OPTIMIZE

                if (stream.IsNull() || stream.IsChildNull())
                {
                    if (!mi.PropertyType.IsValueType)
                    {
                        // Do nothing to keep default value of null
                        //mi.SetValue(obj, null, null); 
                    }
                    else
                    {
                        mi.SetValue(obj, null, null); // OPTIMIZE Unneeded?
                    }
                    continue;
                }

                if (IsSimplePackType(mi.PropertyType))
                {
                    SerializableValue serializableValue = SimpleUnpack.DoSimpleUnpack(stream);
                    object val = GetValue(serializableValue, mi.PropertyType);
                    mi.SetValue(obj, val, null);
                }
                else
                {
                    var attr = mi.GetCustomAttribute<LionSerializableAttribute>(true); // REFLECTION OPTIMIZE
                    bool memberByValue = attr != null && attr.Method == SerializeMethod.ByValue;

                    object memberValue = Deserialize(stream, mi.PropertyType, memberByValue);
                    mi.SetValue(obj, memberValue, null);
                }

            }


            return obj;
#endif
        }

        #endregion

        #region (Static Private) Helper methods

        private static bool IsSimplePackType(Type type)
        {
            switch (type.Name)
            {
                case "UInt32":
                case "Int32":
                case "UInt64":
                case "Int64":
                case "UInt16":
                case "Int16":
                case "Single":
                case "Double":
                case "Byte":
                case "SByte":
                case "String":
                case "Boolean":
                case "Null":
                    return true;
                default:
                    return false;
            }
        }

#if MSGPACK
        private static SerializableValue GetSerializableValue(object obj, Type expectedType)
        {
            switch (expectedType.Name)
            {
                case "Single": return (Single)obj;  // UNTESTED?

                case "UInt32": return Convert.ToInt64(obj);
                case "Int32": return (Int32)obj;

                case "Boolean": return (Boolean)obj;

                case "String": return (string)obj;

                case "UInt64": return (UInt64)obj;
                case "Int64": return (Int64)obj;

                case "UInt16": return (ulong)obj;
                case "Int16": return (Int16)obj;

                case "Byte": return (int)obj;
                case "SByte": return (SByte)obj;

                case "Double": return (Double)obj;  // UNTESTED?


                default: throw new NotSupportedException("Unsupported serialization type. (TODO: Support more types.)");
            }
        }

        private static object GetValue(SerializableValue serializableValue, Type expectedType)
        {
            if (serializableValue.IsNull) return null;
            object obj;
            switch (expectedType.Name)
            {
                case "String":
                    obj = serializableValue.StringValue;
                    break;

                case "Int16":
                    obj = (Int16)serializableValue.Int32value;
                    break;
                case "UInt16":
                    obj = (UInt16)serializableValue.UInt64value;
                    break;

                case "Int32":
                    obj = serializableValue.Int32value;
                    break;
                case "UInt32":
                    obj = (UInt32)serializableValue.UInt64value;
                    break;

                case "Int64":
                    obj = serializableValue.Int64value;
                    break;
                case "UInt64":
                    obj = serializableValue.UInt64value;
                    break;

                case "Single":
                    obj = serializableValue.SingleValue;
                    break;
                case "Double":
                    obj = serializableValue.DoubleValue;
                    break;

                case "Byte":
                    obj = (byte)serializableValue.Int32value;
                    break;
                case "SByte":
                    obj = (sbyte)serializableValue.Int32value;
                    break;

                case "Boolean":
                    obj = serializableValue.BooleanValue;
                    break;

                default:
                    {
                        if (expectedType.IsArray)
                        {
                            IEnumerable<SerializableValue> items = serializableValue.ArrayItems;
                            int arrayLength = items.Count();
                            Type elementType = expectedType.GetElementType();
                            Array array = Array.CreateInstance(elementType, arrayLength);

                            int i = 0;
                            foreach (SerializableValue arrayItemSerializableValue in items)
                            {
                                array.SetValue(GetValue(arrayItemSerializableValue, elementType), i++);
                            }

                            obj = array;
                        }
                        else
                        {
                            throw new NotSupportedException("Unsupported serialization type: " + expectedType.FullName + " (TODO: Support more types.");
                        }
                    }
                    break;
            }
            return obj;
        }
#endif

        #endregion
    }
    //#endif
}


#if false // Protobuf

                ParameterInfo pi = paramInfos[i];
                if (pi.IsOut) continue;
                //ProtoBuf.Serializer.NonGeneric.Serialize(rpcMessageStream, invocation.Arguments[i]);
            

            //invocationMessage.Write(ms.ToArray());
#endif

