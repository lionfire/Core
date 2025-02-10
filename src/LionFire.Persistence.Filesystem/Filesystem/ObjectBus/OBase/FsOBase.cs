#if false // TODO SCAVENGE - OBase concept deprecated in favor of Persisters -- scavenge anything useful from here

namespace LionFire.ObjectBus.Filesystem;

public class FSOBase : OBase<FileReference>
{
    #region Static

    public static FSOBase Instance => (FSOBase)ManualSingleton<FSOBus>.Instance?.SingleOBase;

    #endregion

    protected FilesystemPersister FSPersister { get; }

    public FSOBase()
    {
        System.Diagnostics.Debug.WriteLine("fso create");
    }


    public override IOBus OBus => ManualSingleton<FSOBus>.GuaranteedInstance;

    public override IEnumerable<string> UriSchemes => FileReference.UriSchemes;

    public override IObjectWatcher GetObjectWatcher(IReference reference)
    {
        if (!(reference is FileReference))
        {
            return null;
        }

        var dir = System.IO.Path.GetDirectoryName(reference.Path);
        if (!Directory.Exists(dir))
        {
            //l.Trace("FUTURE: Support watching in directories that don't exist yet");
            return null;
        }
        return new FsObjectWatcher()
        {
            Reference = reference,
        };
    }

    private void ValidateReference(FileReference reference)
    {
        //if (reference.Scheme != LocalFileReference.UriScheme) throw new FsOBaseException("Invalid scheme");
        //if (!reference.IsLocalhost) throw new FsOBaseException("Only localhost supported");
    }

#region Read

    public override async Task<(bool exists, ITransferResult result)> Exists(FileReference reference)
    {
        //var result = new RetrieveResult<bool>();

        var result = await FSPersister.Exists(reference.Path).ConfigureAwait(false);
        //bool existsResult = 
        return (existsResult, existsResult ? PersistenceResult.Found : PersistenceResult.NotFound);
        //result.Object = existsResult;
        //result.Flags |= TransferResultFlags.Success;
        //return result;
    }

#region Get

    public override async Task<IGetResult<T>> TryGet<T>(FileReference reference)
    {
        var result = new RetrieveResult<T>();
        try
        {
            T converted = await FSPersistence.TryGet<T>(reference.Path).ConfigureAwait(false);
            //ResultType converted = (ResultType)OBaseTypeUtils.TryConvertToType(obj, typeof(ResultType));

            if (converted == null)
            {
                foreach (var encapsulatedRef in OBaseTypeUtils.GetEncapsulatedPaths(reference, typeof(T)))
                {

                    var obj = await FSPersistence.TryGet<T>(encapsulatedRef.Path).ConfigureAwait(false);
                    //converted = (T)OBaseTypeUtils.TryConvertToType(obj, typeof(T));
                    if (obj != null)
                    {
                        break;
                    }
                }
            }

            if (converted != null)
            {
                OBaseEvents.OnRetrievedObjectFromExternalSource(converted); // Put reference in here?

                // TOPORT
                //if (optionalRef != null)
                //{
                //    optionalRef.Value.UltimateOBase = this;
                //    optionalRef.Value.UltimateReference = reference;
                //}
            }

            result.Value = converted;
            result.Flags |= TransferResultFlags.Success;
            if (converted != null) result.Flags |= TransferResultFlags.Found;
            else result.Flags |= TransferResultFlags.NotFound;

            return result;
        }
        catch (Exception ex)
        {
            OBaseEvents.OnException(OBusOperations.Get, reference, ex);
            throw ex;
        }
    }

#endregion

#region List

    public override Task<IEnumerable<string>> List<T>(FileReference parent) => throw new NotImplementedException();

#endregion

#endregion

#region Write

#region Delete

    public override async Task<ITransferResult> CanDelete<T>(FileReference reference)
    {
        var existsResult = await Exists(reference);
        if (!existsResult.exists) return PersistenceResult.PreviewFail;

        // FUTURE: Check filesystem permissions
        return PersistenceResult.PreviewSuccess;

        //return new RetrieveResult<bool?>
        //{
        //    IsSuccess = existsResult.IsSuccess,
        //    Result = existsResult.Result,
        //};
        //string filePath = reference.Path;
        //return FsPersistence.TryDelete(filePath);
    }

    public override async Task<ITransferResult> TryDelete<T>(FileReference reference)
    {
        string filePath = reference.Path;
        //if (!defaultTypeForDirIsT)
        //{
        //    filePath = filePath + FileTypeDelimiter + type.Name + FileTypeEndDelimiter;
        //}

        var succeeded = await FSPersistence.TryDelete<T>(filePath).ConfigureAwait(false);

        return succeeded ? PersistenceResult.Success : PersistenceResult.NotFound;
    }

#endregion

#endregion

#region Events

#endregion


    //public override async Task<IGetResult<ResultType>> TryGet<ResultType>(LocalFileReference reference)
    //{
    //    var result = await TryGet(reference, typeof(ResultType));
    //    var converted = (ResultType)OBaseTypeUtils.TryConvertToType(result.Result, typeof(ResultType));
    //    return new RetrieveResult<ResultType>
    //    {
    //        IsSuccess = true,
    //        Result = converted,
    //    };
    //}

    // OLD
    //public override async Task<IGetResult<T>> TryGet<T>(LocalFileReference reference)
    //{
    //    Type ResultType = typeof(T); // REFACTOR
    //    var result = new RetrieveResult<T>();
    //    try
    //    {
    //        object obj = await FsOBasePersistence.TryGet(reference.Path, type: typeof(T)).ConfigureAwait(false);
    //        obj = OBaseTypeUtils.TryConvertToType(obj, ResultType);

    //        // TODO: Move this to an optional MultiType Multiplexing layer
    //        if (obj == null)
    //        {
    //            foreach (var encapsulatedRef in OBaseTypeUtils.GetEncapsulatedPaths(reference, ResultType))
    //            {
    //                obj = FsOBasePersistence.TryGet(encapsulatedRef.Path).ConfigureAwait(false).GetAwaiter().GetResult();
    //                obj = OBaseTypeUtils.TryConvertToType(obj, ResultType);
    //                if (obj != null)
    //                {
    //                    break;
    //                }
    //            }
    //        }

    //        if (obj != null)
    //        {
    //            OBaseEvents.OnRetrievedObjectFromExternalSource(obj); // Put reference in here?

    //            // TOPORT
    //            //if (optionalRef != null)
    //            //{
    //            //    optionalRef.Value.UltimateOBase = this;
    //            //    optionalRef.Value.UltimateReference = reference;
    //            //}
    //        }

    //        result.Object = obj;
    //        result.Flags |= TransferResultFlags.Success;

    //        return result;
    //    }
    //    catch (Exception ex)
    //    {
    //        OBaseEvents.OnException(OBusOperations.Get, reference, ex);
    //        throw ex;
    //    }
    //}
    //public override Task<IGetResult<ResultType>> TryGetName<ResultType>(LocalFileReference reference)
    //{
    //    throw new NotImplementedException();
    //}

    //protected override async Task _Set<T>(LocalFileReference reference, T obj, bool allowOverwrite = true, bool preview = false)
    //{
    //    await FsOBasePersistence.Set(obj, reference.Path, preview: preview, type: typeof(T));
    //}

    protected override async Task<ISetResult<TValue>> SetImpl<T>(FileReference reference, T obj, bool allowOverwrite = true)
    {
#region TODO

        //bool defaultTypeForDirIsT = false;
        //Type type = obj.GetType();
        //var chunks = LionPath.ToPathArray(reference.Path);
        //if (chunks == null || chunks.Length < 2)
        //{
        //    defaultTypeForDirIsT = false;
        //}
        //else // TOPORT
        //{
        //    string parentDirName;
        //    parentDirName = chunks[chunks.Length - 2];
        //    defaultTypeForDirIsT = Assets.AssetPaths.GetAssetTypeFolder(type).TrimEnd('/').Equals(parentDirName);
        //}

        // TODO - This should be done up a layer, since most persistance mechanisms will not support multi-type
        //string filePath = reference.Path;
        //if (AppendTypeNameToFileNames && !defaultTypeForDirIsT) // TOPORT
        //{
        //    filePath = filePath + VosPath.TypeDelimiter + type.Name + VosPath.TypeEndDelimiter;
        //}

#endregion

        await FSPersistence.Set(obj, reference.Path, allowOverwrite: allowOverwrite);
        return PersistenceResult.Success;
    }

    public const bool AppendTypeNameToFileNames = false; // TEMP - TODO: Figure out a way to do this in VOS land

    // OLD - use <object>
    //public override IEnumerable<string> List(FileReference parent)
    //{
    //    FileReference fileRef = FileReference.ConvertFrom(parent);

    //    if (fileRef == null)
    //    {
    //        throw new ArgumentException("Could not convert to FileReference");
    //    }

    //    return FsOBasePersistence.List(fileRef.Path);
    //}

    //private static string GetDirNameForType(string filePath)
    //{
    //    var chunks = VosPath.ToPathArray(filePath);
    //    if (chunks == null || chunks.Length == 0) yield break;
    //    string parentDirName = chunks[chunks.Length - 1];

    //    return Assets.AssetPath.GetDefaultDirectory(typeof(T));
    //}


#if TOPORT
        var chunks = VosPath.ToPathArray(parent.Path);
        if (chunks == null || chunks.Length == 0) yield break;
        string parentDirName = chunks[chunks.Length - 1];

        bool defaultTypeForDirIsT = Assets.AssetPaths.GetAssetTypeFolder(typeof(T)).TrimEnd('/').Equals(parentDirName);

        foreach (var name in FsPersistence.GetChildrenNames(parent.Path))
        {
            string typeName = FsOBaseSettings.GetTypeNameFromFileName(name);

            if (typeName == null)
            {
                if (defaultTypeForDirIsT)
                {
                    yield return name;
                    continue;
                }
            }
            else if (typeName == typeof(T).Name)
            {
                yield return name;
                continue;
            }
        }

        //l.Warn("PARTIALLY IMPLEMENTED: GetChildrenNamesOfType - does not filter types");
        //return GetChildrenNames(parent);
#endif


    private static readonly ILogger l = Log.Get();

#if UNUSED
    #region Options


    //protected FsOptions Options
    //{
    //    get
    //    {
    //        return localOptions.Value ?? options ?? OptionsDefaults;
    //    }
    //    set
    //    {
    //        options = value;
    //    }
    //}
    //private FsOptions options;

    //AsyncLocal<FsOptions> localOptions = new AsyncLocal<FsOptions>();

    //protected FsOptions OptionsDefaults = new FsOptions
    //{
    //    //ReferenceResolutionService = FileReferenceResolutionPolicies.Default.ReferenceResolutionService
    //};
    #endregion
#endif


}
#endif