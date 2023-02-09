using Overby.Extensions.Attachments;
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
using System.Linq;
using LionFire.Persistence.Handles;
using LionFire.Serialization.Adapters;
using static LionFire.Persisters.SharpZipLib_.SharpZipLibExpander;
using LionFire.ExtensionMethods.Poco.Resolvables;
using LionFire.Resolves;
using LionFire.Serialization;
using LionFire.Dependencies;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Collections;
using System.Diagnostics.Metrics;
using LionFire.Referencing.Ex;
using System.Runtime.CompilerServices;
using MorseCode.ITask;

namespace LionFire.Persisters.SharpZipLib_;

//public class SharpZipLibHandleProvider : ObjectHandleRegistrar<HandleRegistrarOptions>
//{
//    protected override IReadHandle<TValue> CreateReadHandle<TValue>(TValue obj)
//    {
//        return new RZipFile(sourceReference.Cast<ZipFile>(), ServiceProvider);
//    }

//    protected override IReadWriteHandle<TValue> CreateReadWriteHandle<TValue>(TValue obj)
//    {
//        throw new NotImplementedException();
//    }

//    protected override IWriteHandle<TValue> CreateWriteHandle<TValue>(TValue obj)
//    {
//        throw new NotImplementedException();
//    }
//}

public class SharpZipLibExpander : ExpanderPersister, ISupportsFileExtensions, IPersister<IExpansionReference>
{

    #region Metrics

    private static readonly Meter Meter = new Meter("LionFire.Persisters.SharpZipLib.SharpZipLibExpander");
    private static readonly UpDownCounter<long> ReadZipFileStreamC = Meter.CreateUpDownCounter<long>("ReadZipFileStream");

    #endregion

    #region Dependencies

    public IServiceProvider ServiceProvider { get; }

    public IOptionsMonitor<SharpZipLibExpanderOptions> OptionsMonitor { get; }
    public ISerializationProvider SerializationProvider { get; }

    #region Derived

    public List<string> FileExtensions => OptionsMonitor.CurrentValue.FileExtensions;

    #endregion

    #endregion

    #region Lifecycle

    public SharpZipLibExpander(IServiceProvider serviceProvider, IOptionsMonitor<SharpZipLibExpanderOptions> optionsMonitor
        , ISerializationProvider serializationProvider
        )
    {
        ServiceProvider = serviceProvider;
        OptionsMonitor = optionsMonitor;
        SerializationProvider = serializationProvider;
    }

    #endregion

    public override Type? SourceReadType() => typeof(byte[]); // TODO: also support Stream?

    public Type NativeType => typeof(ZipFile);

    public override Task<IReadHandle>? TryGetSourceReadHandle(IReference sourceReference)
    {
        var ext = Path.GetExtension(sourceReference.Path)?.TrimStart('.');

        IReadHandle archiveHandle;

        //var reuse = true;

        switch (ext)
        {
            case "zip":
                // TODO: ZipFileReadHandleProvider.GetReadHandle();
                //var uri = sourceReference.Cast<ZipFile>().Url;

                //archiveHandle = HandleRegistry2.GetOrAddRead<ZipFile>(sourceReference.Url, uri => new RZipFile(sourceReference.Cast<ZipFile>(), ServiceProvider));
                archiveHandle = HandleRegistry2.GetOrAddRead<ZipFile>(sourceReference.Url, uri => new RZipFile(sourceReference));

                //RZipFile create() => new RZipFile(sourceReference.Cast<ZipFile>(), ServiceProvider);
                //if (reuse)
                //{
                //    var referenceString = string.Intern(sourceReference.Cast<ZipFile>().ToString()!);
                //    archiveHandle = HandleCache.GetOrAdd(referenceString, () => create());
                //}
                //else
                //{
                //    archiveHandle = create();
                //}
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
        try
        {
            #region Source: Resolve

            var sourceResolveResult = await sourceReadHandle.TryGetValue().ConfigureAwait(false);

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

            var listingItemType = Listing.GetListingType<TValue>();

            var zipFile = sourceResolveResult.Value as ZipFile;

            if (zipFile == null)
            {
                throw new UnreachableCodeException($"SourceReadHandle retrieved unexpected type {sourceResolveResult.Value?.GetType().FullName ?? "null"}.  Expected: {typeof(ZipFile).FullName}");
            }

            var nativeTargetPath = targetPath.TrimStart('/');

            int entryIndex;
            if (nativeTargetPath.Length == 0)
            {
                entryIndex = -2;
            }
            else
            {
                entryIndex = zipFile.FindEntry(nativeTargetPath, OptionsMonitor.CurrentValue.IgnoreCaseInTargetPath);

                if (entryIndex == -1) // Try again to get opposite of: file or directory
                {
                    if (nativeTargetPath.EndsWith('/'))
                    {
                        nativeTargetPath = nativeTargetPath.TrimEnd('/');
                    }
                    else
                    {
                        nativeTargetPath += "/";
                    }
                    entryIndex = zipFile.FindEntry(nativeTargetPath, OptionsMonitor.CurrentValue.IgnoreCaseInTargetPath);
                }
            }

            bool foundFile = false;
            bool foundDirectory = false;

            if (entryIndex == -1)
            {
                return RetrieveResult<TValue>.SuccessNotFound;
            }
            else
            {
                var entry = entryIndex == -2 ? null : zipFile[entryIndex];

                if (entry?.IsFile == true)
                {
                    List<DeserializationResult<TValue>>? deserializationResults = null;

                    if (listingItemType != null) { return RetrieveResult<TValue>.SuccessNotFound; }
                    foundFile = true;
                    var ext = Path.GetExtension(targetPath).TrimFirstDot();
                    var strategies = SerializationProvider.GetDistinctRankedStrategiesByExtension(s => s.SupportsExtension(ext));
                    if (!strategies.Any())
                    {
                        return new RetrieveResult<TValue>
                        {
                            Error = $"Found file {nativeTargetPath}, but {nameof(ISerializationProvider)} provided no serializers",
                            Flags = PersistenceResultFlags.SerializerNotAvailable | PersistenceResultFlags.Fail,
                        };
                    }

                    using var stream = zipFile.GetInputStream(entry);
                    ReadZipFileStreamC.IncrementWithContext();
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

                    if (deserializationResults != null)
                    {
                        return new RetrieveResult<TValue>
                        {
                            Error = $"Found file {nativeTargetPath}, but {(deserializationResults.Count == 1 ? "serializer" : $"{deserializationResults.Count} serializers")} failed to deserialize.  See ErrorDetail.",
                            ErrorDetail = deserializationResults,
                            Flags = PersistenceResultFlags.Fail,
                        };
                    }
                    else
                    {
                        return RetrieveResult<TValue>.NotFound;
                    }
                }
                else if (entry == null || entry.IsDirectory)
                {
                    foundDirectory = true;
                    var flags = PersistenceResultFlags.Found;
                    if (listingItemType == null)
                    {
                        // We are trying to retrieve a Directory as some sort of object
                        if (entryIndex == -2) { return RetrieveResult<TValue>.Found(); } // TODO: Return some sort of VosDirectory as value?
                        else
                        {
                            throw new NotImplementedException("UNTESTED");
                            return RetrieveResult<TValue>.Found();    // TODO: Return some sort of VosDirectory as value?
                        }
                    }
                    else
                    {
                        return await retrieveListingForDirectory(nativeTargetPath).ConfigureAwait(false);

                        async Task<RetrieveResult<TValue>> retrieveListingForDirectory(string nativeTargetDirPath)
                        {
                            TValue retrieveValue;

                            IEnumerable<ZipEntry> zipEntries = new List<ZipEntry>(zipFile.OfType<ZipEntry>().Where(e =>
                                    (e.Name == string.Empty || e.Name.StartsWith(nativeTargetDirPath))
                                    && Path.GetDirectoryName(e.IsDirectory ? Path.GetDirectoryName(e.Name) : e.Name) + (nativeTargetDirPath == string.Empty ? "" : "/") == nativeTargetDirPath
                                    && (e.Name.Length > Path.GetDirectoryName(e.Name!)!.Length + 1)
                                ));

                            if (listingItemType == typeof(object))
                            {
                                retrieveValue = (TValue)(object)new Metadata<IEnumerable<Listing<object>>>(zipEntries.Select(i => new Listing<object>(Path.GetFileName(i.Name)))); // HARDCAST
                            }
                            else
                            {
                                // REVIEW - this is a ton of reflection.
                                // TODO ENH - replace typed Listings with an API parameter to filter type (and potentially other things)

                                if (listingItemType.Name == "IArchive") // HARDCODE - type name
                                {
                                    zipEntries = zipEntries.Where(i => i.Name.EndsWith(".zip")); // STUB - Simplistic implementation
                                }
                                else
                                {
                                    zipEntries = await zipEntries.ToAsyncEnumerable().WhereAwait(async zipEntry =>
                                    {
                                        // FUTURE ENH: determine the types of all the listings (potentially expensive: worst case is attempting deserialization of everything)
                                        var typeHandle = new ExpansionReference(sourceReadHandle.Reference.Url, zipEntry.Name).GetReadHandle<Metadata<Type>>();
                                        var typeResolve = await typeHandle.Resolve().ConfigureAwait(false);
                                        if (typeResolve.IsSuccess == true && typeResolve.HasValue)
                                        {
                                            return listingItemType.IsAssignableFrom(typeResolve.Value);
                                        }
                                        else
                                        {
                                            throw new NotImplementedException("Type filtering on Listings");
                                        }
                                    }).ToListAsync().ConfigureAwait(false);
                                }


                                var listingEntryType = typeof(Listing<>).MakeGenericType(listingItemType);

                                var enumerableOfListings = Activator.CreateInstance(typeof(List<>).MakeGenericType(typeof(Listing<>).MakeGenericType(listingItemType)));
                                var mi = enumerableOfListings!.GetType().GetMethod("Add");
                                foreach (var zipEntry in zipEntries)
                                {
                                    mi!.Invoke(enumerableOfListings, new object[] { Activator.CreateInstance(listingEntryType, Path.GetFileName(zipEntry.Name))! });
                                }
                                //pi!.SetValue(retrieveValue, enumerableOfListings);
                                retrieveValue = (TValue)Activator.CreateInstance(typeof(TValue), enumerableOfListings)!;
                            }
                            flags |= PersistenceResultFlags.Success;
                            return new RetrieveResult<TValue>(retrieveValue, flags);
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException("!IsFile && !IsDirectory");
                }
                //return new RetrieveResult<TValue>(,)
            }
            throw new UnreachableCodeException();
        }
        finally
        {
            //sourceReadHandle.DiscardValue();
        }
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

    public Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<IExpansionReference> referencable, RetrieveOptions? options = null)
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

public static class IReadHandleX
{
    public static async Task<IResolveResult<object>> TryGetValue(this IReadHandle rh)
    {
        if (rh is ILazilyResolves<object> lr)
        {
            return await lr.TryGetValue().ConfigureAwait(false);
        }

        return await rh.Resolve().ConfigureAwait(false);
    }
}