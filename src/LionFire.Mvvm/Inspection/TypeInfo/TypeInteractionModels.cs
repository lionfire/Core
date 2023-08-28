//using System.Collections.Concurrent;

//namespace LionFire.Inspection;

//public static class TypeInteractionModels
//{
//    static ConcurrentDictionary<Type, TypeInteractionModel> models = new();

//    public static TypeInteractionModel? Get(Type? type) => type == null ? null : models.GetOrAdd(type, type => TypeInteractionModelGenerator.InitType(type));
//    public static TypeInteractionModel Get<T>() => models.GetOrAdd(typeof(T), type=> TypeInteractionModelGenerator.InitType(type));

//}
