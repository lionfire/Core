using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LionFire.ObjectBus.Typing;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Referencing.Filesystem;
using LionFire.Structures;
using Microsoft.Extensions.Logging;

namespace LionFire.ObjectBus.Filesystem
{
    public class FsOBase : OBase<LocalFileReference>
    {
        #region Static

        public static FsOBase Instance => ManualSingleton<FsOBase>.GuaranteedInstance;

        #endregion

        #region Options


        protected FsOptions Options
        {
            get
            {
                return localOptions.Value ?? options ?? OptionsDefaults;
            }
            set
            {
                options = value;
            }
        }
        private FsOptions options;

        AsyncLocal<FsOptions> localOptions = new AsyncLocal<FsOptions>();

        protected FsOptions OptionsDefaults = new FsOptions
        {
            //ReferenceResolutionService = FileReferenceResolutionPolicies.Default.ReferenceResolutionService
        };

        #endregion


        public override IEnumerable<string> UriSchemes => LocalFileReference.UriSchemes;

        public override IObjectWatcher GetObjectWatcher(IReference reference)
        {
            if (!(reference is LocalFileReference))
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

        private void ValidateReference(LocalFileReference reference)
        {
            //if (reference.Scheme != LocalFileReference.UriScheme) throw new FsOBaseException("Invalid scheme");
            //if (!reference.IsLocalhost) throw new FsOBaseException("Only localhost supported");
        }

        //public override async Task<IRetrieveResult<ResultType>> TryGet<ResultType>(LocalFileReference reference)
        //{
        //    var result = await TryGet(reference, typeof(ResultType));
        //    var converted = (ResultType)OBaseTypeUtils.TryConvertToType(result.Result, typeof(ResultType));
        //    return new RetrieveResult<ResultType>
        //    {
        //        IsSuccess = true,
        //        Result = converted,
        //    };
        //}

        //LogicalReferenceResolver


        public override async Task<RetrieveResult<object>> TryGet(LocalFileReference reference, Type ResultType)
        {
            var result = new RetrieveResult<object>();
            try
            {
                object obj = await FsOBasePersistence.TryGet(reference.Path).ConfigureAwait(false);
                obj = OBaseTypeUtils.TryConvertToType(obj, ResultType);

                // TODO: Move this to an optional MultiType Multiplexing layer
                if (obj == null)
                {
                    foreach (var encapsulatedRef in OBaseTypeUtils.GetEncapsulatedPaths(reference, ResultType))
                    {
                        obj = FsOBasePersistence.TryGet(encapsulatedRef.Path).ConfigureAwait(false).GetAwaiter().GetResult();
                        obj = OBaseTypeUtils.TryConvertToType(obj, ResultType);
                        if (obj != null)
                        {
                            break;
                        }
                    }
                }

                if (obj != null)
                {
                    OBaseEvents.OnRetrievedObjectFromExternalSource(obj); // Put reference in here?

                    // TOPORT
                    //if (optionalRef != null)
                    //{
                    //    optionalRef.Value.UltimateOBase = this;
                    //    optionalRef.Value.UltimateReference = reference;
                    //}
                }

                result.Object = obj;
                result.IsSuccess = true;

                return result;
            }
            catch (Exception ex)
            {
                OBaseEvents.OnException(OBusOperations.Get, reference, ex);
                throw ex;
            }
        }
        //public override Task<IRetrieveResult<ResultType>> TryGetName<ResultType>(LocalFileReference reference)
        //{
        //    throw new NotImplementedException();
        //}

        public override async Task<RetrieveResult<ResultType>> TryGet<ResultType>(LocalFileReference reference)
        {
            var result = new RetrieveResult<ResultType>();
            try
            {
                object obj = await FsOBasePersistence.TryGet(reference.Path).ConfigureAwait(false);
                ResultType converted = (ResultType)OBaseTypeUtils.TryConvertToType(obj, typeof(ResultType));

                if (converted == null)
                {
                    foreach (var encapsulatedRef in OBaseTypeUtils.GetEncapsulatedPaths(reference, typeof(ResultType)))
                    {
                        obj = await FsOBasePersistence.TryGet(encapsulatedRef.Path).ConfigureAwait(false);
                        converted = (ResultType)OBaseTypeUtils.TryConvertToType(obj, typeof(ResultType));
                        if (converted != null)
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

                result.Object = converted;
                result.IsSuccess = true;

                return result;
            }
            catch (Exception ex)
            {
                OBaseEvents.OnException(OBusOperations.Get, reference, ex);
                throw ex;
            }
        }

        public override IOBus OBus => ManualSingleton<FsOBus>.GuaranteedInstance;


        public override async Task<RetrieveResult<bool>> Exists(LocalFileReference reference)
        {
            var result = new RetrieveResult<bool>();

            bool existsResult = await FsOBasePersistence.Exists(reference.Path).ConfigureAwait(false);

            result.Object = existsResult;
            result.IsSuccess = true;
            return result;
        }

        public override async Task<bool?> CanDelete(LocalFileReference reference)
        {
            // FUTURE: Check filesystem permissions
            var existsResult = await Exists(reference);
            if (!existsResult.IsSuccess) return null;
            return existsResult.Object;
            //return new RetrieveResult<bool?>
            //{
            //    IsSuccess = existsResult.IsSuccess,
            //    Result = existsResult.Result,
            //};
            //string filePath = reference.Path;
            //return FsPersistence.TryDelete(filePath);
        }

        public override async Task<bool?> TryDelete(LocalFileReference reference/*, bool preview = false*/)
        {
            string filePath = reference.Path;
            //if (!defaultTypeForDirIsT)
            //{
            //    filePath = filePath + FileTypeDelimiter + type.Name + FileTypeEndDelimiter;
            //}

            return await FsOBasePersistence.TryDelete(filePath).ConfigureAwait(false);
        }

        //protected override async Task _Set<T>(LocalFileReference reference, T obj, bool allowOverwrite = true, bool preview = false)
        //{
        //    await FsOBasePersistence.Set(obj, reference.Path, preview: preview, type: typeof(T));
        //}

        protected override async Task _Set(LocalFileReference reference, object obj, Type type = null, bool allowOverwrite = true, bool preview = false)
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

            await FsOBasePersistence.Set(obj, reference.Path, preview: preview, type: type);
        }

        public const bool AppendTypeNameToFileNames = false; // TEMP - TODO: Figure out a way to do this in VOS land

        public override IEnumerable<string> GetChildrenNames(LocalFileReference parent)
        {
            LocalFileReference fileRef = LocalFileReference.ConvertFrom(parent);

            if (fileRef == null)
            {
                throw new ArgumentException("Could not convert to FileReference");
            }

            return FsOBasePersistence.GetChildrenNames(fileRef.Path);
        }

        //private static string GetDirNameForType(string filePath)
        //{
        //    var chunks = VosPath.ToPathArray(filePath);
        //    if (chunks == null || chunks.Length == 0) yield break;
        //    string parentDirName = chunks[chunks.Length - 1];

        //    return Assets.AssetPath.GetDefaultDirectory(typeof(T));
        //}

        public override IEnumerable<string> GetChildrenNamesOfType<T>(LocalFileReference parent) => throw new NotImplementedException();

        public override Task<IEnumerable<string>> GetKeys(LocalFileReference parent) => throw new NotImplementedException();
        public override Task<IEnumerable<string>> GetKeysOfType<T>(LocalFileReference parent) => throw new NotImplementedException();

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


        private static ILogger l = Log.Get();

    }
}
