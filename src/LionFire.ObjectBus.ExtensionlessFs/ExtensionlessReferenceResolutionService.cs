using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.DependencyInjection;
using LionFire.ObjectBus.Filesystem;
using LionFire.Ontology;
using LionFire.Persistence.Resolution;
using LionFire.Referencing;
using LionFire.Serialization;

namespace LionFire.ObjectBus.ExtensionlessFs
{
    /// <summary>
    /// Look in SerializationService for all possible file extensions, and return those as possibilities for resolving a reference.
    /// </summary>
    [Immutable]
    public class ExtensionlessReferenceResolutionService : IReferenceToReferenceResolver
    {
        #region Static

        //public static ISerializationService DefaultSerializationService => DependencyContext.Default.GetService<ISerializationService>();
        //public static ISerializationService CurrentSerializationService => DependencyContext.Current.GetService<ISerializationService>();

        #endregion

        #region Relationships

        public IOBase OBase { get; }

        public ISerializationProvider SerializationService { get; }

        #endregion

        [Immutable]
        private readonly IReadOnlyList<(string extension, ISerializationStrategy strategy, SerializationFormat format)> extensions;

        #region Construction

        public ExtensionlessReferenceResolutionService(IOBase obase, ISerializationProvider serializationProvider)
        {
            this.OBase = obase;
            this.SerializationService = serializationProvider; // ?? CurrentSerializationService ?? DefaultSerializationService;
            // if (serializationService == null) throw new ArgumentNullException(nameof(serializationService)); FIXME

            extensions = SerializationService.GetDistinctRankedStrategiesByExtension();
        }

        #endregion

        // FUTURE NETSTANDARD3 IAsyncEnumerable
        //public async Task<IEnumerable<ReadResolutionResult<TValue>>> ResolveAll<TValue>(IReference r, ResolveOptions options = null)
        //{
        //    return await Task.Run(async () =>
        //    {
        //        var result = new List<ReadResolutionResult<TValue>>();
        //        foreach (var x in await _ResolveAll<TValue>(r, options)) {
        //            result.Add(await x);
        //        }
        //        return result;
        //    });
        //}
        public async Task<IEnumerable<ReadResolutionResult<T>>> ResolveAll<T>(IReference r, ResolveOptions options = null)
        {
            var results = new List<ReadResolutionResult<T>>();
            foreach (var (extension, strategy, format) in extensions)
            {
                results.Add(await x());
                async Task<ReadResolutionResult<T>> x() {
                    //results.Add (await Task.Run(async () =>
                    //{
                    //var referenceWithExtension = r.WithRelativePath("." + extension, FileReference.Constants.UriScheme);
                    var referenceWithExtension = new FileReference(r.Path + "." + extension);
                    Persistence.IRetrieveResult<T> retrieveResult = null;
                    if (options?.VerifyDeserializable == true)
                    {
                        var readHandle = referenceWithExtension.ToReadHandle<T>();

                        retrieveResult = await OBase.Get<T>(referenceWithExtension).ConfigureAwait(false);

                        if(!(await readHandle.GetValue<T>()).HasObject)
                        {
                            return null;
                        }
                        else
                        {
                            return new ReadResolutionResult<T>(readHandle);
                        }
                    }
                    else if (options?.VerifyExists == true)
                    {
                        var readHandle = referenceWithExtension.ToReadHandle<T>();
                        
                        if (await readHandle.Exists().ConfigureAwait(false))
                        {
                            return null;
                        }
                        else
                        {
                            return new ReadResolutionResult<T>(readHandle);
                        }
                    }
                    else
                    {
                        return new ReadResolutionResult<T>(referenceWithExtension.ToReadHandle<T>());
                    }
                    //});
                }
            }
            return results;
        }

        private static ResolveOptions DefaultResolveReadOptions = new ResolveOptions
        {
            VerifyDeserializable = true,
            VerifyExists = true,
        };

//        /// <summary>
//        /// Returns the first potentially valid underlying reference for the given logical reference.  If options specifies VerifyDeserializable = true, the object will actually be read into
//        /// ReadResolutionResult.ReadHandle.  
//        /// </summary>
//        /// <typeparam name="TValue">Non-multitype OBases should always use object</typeparam>
//        /// <param name="reference"></param>
//        /// <param name="options"></param>
//        /// <returns></returns>
//        public async IAsyncEnumerable<ReadResolutionResult<TValue>> ResolveForRead<TValue>(IReference reference, ResolveOptions options = null)
//        {
//            if (options == null) options = DefaultResolveReadOptions;

//            foreach (var (extension, strategy, format) in extensions)
//            {
//                var referenceWithExtension = reference.WithPath(reference.Path + "." + extension);

//                if (options.VerifyDeserializable)
//                {

//#warning FIXME ObjectBus.mmap - see Overview TODO



//                    var retrieveResult = OBase.TryGet<TValue>(referenceWithExtension);
//                    if (retrieveResult != null)
//                    {
//                        yield return new ReadResolutionResult<TValue>(new RH<TValue>(referenceWithExtension, retrieveResult));
//                    }
//                }
//                else if (options.VerifyExists && (await OBase.Exists(referenceWithExtension).ConfigureAwait(false)))
//                {
//                    yield return new ReadResolutionResult<TValue>(referenceWithExtension);
//                }
//                else
//                {
//                    yield return new ReadResolutionResult<TValue>(referenceWithExtension);
//                }

//                if (await OBase.ExistsAsType<TValue>(referenceWithExtension).ConfigureAwait(false))
//                {
//                    // So a question is, when deserializing, whether to do: 1) throw exception if unexpected type, 2) return nothing if unexpected type, 3) attempt duck typing.
//                    // Another question, when serializing: merge or replace if using duck typing.
//                    // I don't want to impose any limitations -- that means a more complicated / powerful Vos/OBase API.
//                    yield return referenceWithExtension;
//                }
//            }
//        }


        public Task<IEnumerable<WriteResolutionResult<T>>> ResolveAllForWrite<T>(IReference r, ResolveOptions options = null) => throw new NotImplementedException();
    }

    //public class ReferenceResolver
    //{
    //    public List<ReferenceResolutionStrategy> Strategies { get; private set; }

    //    public IEnumerable<R<TValue>> Resolve<TValue>(IReference r, ResolveOptions options = null)
    //    {
    //        //DependencyContext
    //    }

    //    public Task<TValue> Get(IReference r)
    //    {

    //    }

    //    public Task<bool> Exists<TValue>(IReference r)
    //    {

    //    }


    //}
}
