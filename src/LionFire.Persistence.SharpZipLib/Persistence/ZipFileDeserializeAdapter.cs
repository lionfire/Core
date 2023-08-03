//using LionFire.Persistence.Persisters;
//using LionFire.Persistence.Persisters.Vos;
//using LionFire.Referencing;
//using LionFire.Structures;
//using LionFire.Vos.Mounts;
//using Microsoft.Extensions.Options;
//using LionFire.Persisters.Expanders;
//using LionFire.Persistence;
//using ICSharpCode.SharpZipLib.Zip;
//using System.Net.Http.Headers;
//using Newtonsoft.Json.Linq;
//using System.Collections.Concurrent;
//using LionFire.Persistence.Handles;
//using LionFire.Serialization.Adapters;
//using static LionFire.Persisters.SharpZipLib_.SharpZipLibExpander;
//using LionFire.ExtensionMethods.Poco.Getters;

//namespace LionFire.Persisters.SharpZipLib_;

//public class TypeTransformerOptions
//{
//    public Dictionary<Type, List<Type>> TargetTypeToSourceTypes { get; } = new();

//    public TypeTransformerOptions RegisterSourceForTarget<TSource, TTarget>()
//    {
//        if (!TargetTypeToSourceTypes.ContainsKey(typeof(TTarget)))
//        {
//            TargetTypeToSourceTypes.Add(typeof(TTarget), new List<Type>());
//        }
//        TargetTypeToSourceTypes[typeof(TTarget)].Add(typeof(TSource));
//        return this;
//    }
//}
//public class TypeTransformer : ITypeTransformer
//{
//    TypeTransformerOptions Options { get; }

//    public TypeTransformer(IOptionsMonitor<TypeTransformerOptions> options)
//    {
//        Options = options.CurrentValue;
//    }

//    public IEnumerable<Type> SourceTypesFor(Type targetType)
//        => Options.TargetTypeToSourceTypes[targetType];

//    public IReadHandle<TTarget> Convert<TSource, TTarget>(IReference source)
//    {
//        throw new NotImplementedException();
//    }
//}

//public interface ITypeTransformer
//{
//    IEnumerable<Type> SourceTypesFor(Type targetType);

//    IReadHandle<TTarget> Convert<TSource, TTarget>(IReference source);
//}


//public class ZipFileDeserializeAdapter : IDeserializeAdapter<byte[], ZipFile>, IDeserializeAdapter<Stream, ZipFile>
//{
//    public Task<RetrieveResult<ZipFile>> Retrieve(IReadHandle<byte[]> source)
//    {
//        var result = new RetrieveResult<ZipFile>();


//        throw new NotImplementedException();

//        //return result;
//    }

//    public Task<RetrieveResult<ZipFile>> Retrieve(IReadHandle<Stream> source)
//    {
//        var result = new RetrieveResult<ZipFile>();


//        throw new NotImplementedException();

//        //return result;
//    }
//}
