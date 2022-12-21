using LionFire.Persistence.Persisters;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Referencing;
using LionFire.Structures;
using LionFire.Vos.Mounts;
using Microsoft.Extensions.Options;
using LionFire.Persisters.Expanders;
using LionFire.Persistence;
using ICSharpCode.SharpZipLib.Zip;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using LionFire.Persistence.Handles;
using LionFire.Serialization.Adapters;
using static LionFire.Persisters.SharpZipLib_.SharpZipLibExpander;
using LionFire.ExtensionMethods.Poco.Resolvables;
using LionFire.Resolves;
using LionFire.Serialization;
using LionFire.Dependencies;

namespace LionFire.Persisters.SharpZipLib_;

public class SharpZipLibExpander : ExpanderPersister, ISupportsFileExtensions, IPersister<IExpansionReference>
{
    public IServiceProvider ServiceProvider { get; }

    //ITypeTransformer TypeTransformer { get; }
    //ICSharpCode.SharpZipLib.Zip.
    public IOptionsMonitor<SharpZipLibExpanderOptions> OptionsMonitor { get; }


    public SharpZipLibExpander(IServiceProvider serviceProvider, IOptionsMonitor<SharpZipLibExpanderOptions> optionsMonitor)
    {
        ServiceProvider = serviceProvider;
        OptionsMonitor = optionsMonitor;
        //TypeTransformer = typeTransformer;
    }

    public List<string> FileExtensions => OptionsMonitor.CurrentValue.FileExtensions;



    public override Type? SourceReadType() => typeof(byte[]); // TODO: also support Stream?



    //ConcurrentDictionary<string, RZip> zips = new();


    public Type NativeType => typeof(ZipFile);


    public override Task<IReadHandle>? TryGetSourceReadHandle(IReference sourceReference)
    {
        //throw new NotImplementedException();
        //IReadHandle<ZipFile> zipFileHandle = await sourceReference.GetReadHandle<ZipFile>(sourceReference);
        //var resolveResult = await zipFileHandle.Resolve();
        ////IReadHandle<ZipFile> zipFileHandle = await sourceReference.Transform<ZipFile>(sourceReference);


        //IReadHandle archiveHandle;
        //foreach (var sourceType in TypeTransformer.SourceTypesFor(NativeType))
        //{
        //    var potentialSourceHandle = sourceReference.GetReadHandle(sourceType, ServiceProvider);

        //    if (await potentialSourceHandle.Exists<TValue>())
        //    {
        //        archiveHandle = potentialSourceHandle;
        //    }
        //}

        //if (archiveHandle == null)
        //{

        //}

        //IReadHandleProvider rps;
        //rps.GetReadHandle
        var ext = Path.GetExtension(sourceReference.Path)?.TrimStart('.');

        IReadHandle archiveHandle;

        switch (ext)
        {
            case "zip":
                // TODO ENH: Reuse source handles
                archiveHandle = new RZip(sourceReference.Cast<ZipFile>(), ServiceProvider);
                break;
            //case "tar":
            //    break;
            //case "bz2":
            //    break;
            //case "bzip2":
            //    break;
            //case "gzip":
            //    break;
            default:
                throw new ArgumentException("unknown/unimplemented extension and autodetect type is not yet implemented");
        }
        return Task.FromResult(archiveHandle);
    }

    public override async Task<IRetrieveResult<TValue>> RetrieveTarget<TValue>(IReadHandle sourceReadHandle, string targetPath)
    {
        #region Source: Resolve

        var sourceResolveResult = await sourceReadHandle.Resolve().ConfigureAwait(false);

        if (!sourceResolveResult.HasValue)
        {
            return sourceResolveResult.IsSuccess switch
            {
                true => RetrieveResult<TValue>.SuccessNotFound,
                false => new RetrieveResult<TValue>()
                {
                    Flags = PersistenceResultFlags.Fail | PersistenceResultFlags.InnerFail,
                    InnerResult = (IResolveResult<object>)sourceResolveResult,
                },
                null => RetrieveResult<TValue>.NotFound,
            };
        }

        #endregion

        var zipFile = sourceResolveResult.Value as ZipFile;

        if (zipFile == null)
        {
            throw new UnreachableCodeException($"SourceReadHandle retrieved unexpected type {sourceResolveResult.Value?.GetType().FullName ?? "null"}.  Expected: {typeof(ZipFile).FullName}");
        }

        var nativeTargetPath = targetPath.TrimStart('/');
        var entryIndex = zipFile.FindEntry(nativeTargetPath, OptionsMonitor.CurrentValue.IgnoreCaseInTargetPath);

        bool foundFile = false;
        bool foundDirectory = false;

        List<DeserializationResult<TValue>>? deserializationResults = null;

        if (entryIndex == -1)
        {
            return RetrieveResult<TValue>.SuccessNotFound;
        }
        else
        {
            var entry = zipFile[entryIndex];

            if (entry.IsFile)
            {
                foundFile = true;
                var ext = Path.GetExtension(targetPath).TrimFirstDot();
                //throw new NotImplementedException("Deserialize ZipEntry");
                var ss = DependencyContext.Current.GetService<ISerializationProvider>(); // FIXME SERVICELOCATOR TEMP
                var strategies = ss.GetDistinctRankedStrategiesByExtension(s => s.SupportsExtension(ext));
                if (!strategies.Any())
                {
                    return new RetrieveResult<TValue>
                    {
                        Error = $"Found file {nativeTargetPath}, but {nameof(ISerializationProvider)} provided no serializers",
                        Flags = PersistenceResultFlags.SerializerNotAvailable | PersistenceResultFlags.Fail,
                    };
                }

                using var stream = zipFile.GetInputStream(entry);
                //new PersistenceOperation(null, new DeserializePersistenceOperation() { }))


                foreach (var s in strategies)
                {
                    //s.strategy.SupportedCapabilities.HasFlag(SerializationFlags.)
                    var deserializeResult = s.strategy.ToObject<TValue>(stream);

                    if (deserializeResult.IsSuccess)
                    {
                        var flags = PersistenceResultFlags.Success | PersistenceResultFlags.Found;
                        return new RetrieveResult<TValue>(deserializeResult.Object, flags);
                    }
                    else
                    {
                        deserializationResults ??= new List<DeserializationResult<TValue>>();
                        deserializationResults.Add(deserializeResult);
                    }
                }
            }
            else if (entry.IsDirectory)
            {
                foundDirectory = true;
                throw new NotImplementedException();
            }
            else
            {
                throw new NotSupportedException("!IsFile && !IsDirectory");
            }

            //return new RetrieveResult<TValue>(,)
        }

        if (deserializationResults != null)
        {
            return new RetrieveResult<TValue>
            {
                Error = $"Found file {nativeTargetPath}, but {(deserializationResults.Count == 1 ? "serializer" : $"{deserializationResults.Count} serializers")} failed to deserialize.  See ErrorDetail.",
                ErrorDetail = deserializationResults,
                Flags = PersistenceResultFlags.Fail,
            };
        }

        throw new UnreachableCodeException();
    }

    public override Task<RetrieveResult<TValue>> Retrieve<TValue>(ExpanderReadHandle<TValue> readHandle)
    {
        throw new NotImplementedException();
    }

    public override Type? SourceReadTypeForReference(IReference reference)
    {
        throw new NotImplementedException();
    }

    public Task<IPersistenceResult> Exists<TValue>(IReferencable<IExpansionReference> referencable)
    {
        throw new NotImplementedException();
    }

    public Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<IExpansionReference> referencable)
    {
        throw new NotImplementedException();
    }

    public Task<IPersistenceResult> Create<TValue>(IReferencable<IExpansionReference> referencable, TValue value)
    {
        throw new NotImplementedException();
    }

    public Task<IPersistenceResult> Update<TValue>(IReferencable<IExpansionReference> referencable, TValue value)
    {
        throw new NotImplementedException();
    }

    public Task<IPersistenceResult> Upsert<TValue>(IReferencable<IExpansionReference> referencable, TValue value)
    {
        throw new NotImplementedException();
    }

    public Task<IPersistenceResult> Delete(IReferencable<IExpansionReference> referencable)
    {
        throw new NotImplementedException();
    }

    public Task<IRetrieveResult<IEnumerable<Listing<T>>>> List<T>(IReferencable<IExpansionReference> referencable, ListFilter? filter = null)
    {
        throw new NotImplementedException();
    }
}
