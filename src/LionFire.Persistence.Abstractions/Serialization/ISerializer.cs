using System;
using System.IO;
using LionFire.Persistence;

namespace LionFire.Serialization
{
    public interface ISerializer
    {
        //(byte[] Bytes, SerializationResult Result) ToBytes(object obj, SerializationOperation operation = null, SerializationContext context = null);
        //(string String, SerializationResult Result) ToString(object obj, SerializationOperation operation = null, SerializationContext context = null);
        //SerializationResult ToStream(object obj, Stream stream, SerializationOperation operation = null, SerializationContext context = null);

        //(T Object, SerializationResult Result) ToObject<T>(byte[] bytes, SerializationOperation operation = null, SerializationContext context = null);
        //(T Object, SerializationResult Result) ToObject<T>(string str, SerializationOperation operation = null, SerializationContext context = null);
        //(T Object, SerializationResult Result) ToObject<T>(Stream stream, SerializationOperation operation = null, SerializationContext context = null);

        (byte[] Bytes, SerializationResult Result) ToBytes(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null);
        (string String, SerializationResult Result) ToString(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null);
        SerializationResult ToStream(object obj, Stream stream, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null);

        DeserializationResult<T> ToObject<T>(byte[] bytes, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null);
        DeserializationResult<T> ToObject<T>(string str, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null);
        DeserializationResult<T> ToObject<T>(Stream stream, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null);
    }
}
