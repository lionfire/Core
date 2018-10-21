using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LionFire.Serialization;
using LionFire.Structures;
using LionFire.Referencing;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace LionFire.ObjectBus.Filesystem
{

#if AOT
	public class FsOBase : OBaseBase<FileReference>
	{
		public static FsOBase Instance { get { return Singleton<FsOBase>.Instance; } }
		
		public override string[] UriSchemes { get { return FileReference.UriSchemes; } }
		
		private void ValidateReference(FileReference reference)
		{
			if (reference.Scheme != FileReference.UriScheme) throw new FsOBaseException("Invalid scheme");
			if (!reference.IsLocalhost) throw new FsOBaseException("Only localhost supported");
			
		}
		
		public override ResultType TryGet<ResultType>(FileReference reference)
		{
			var result = (ResultType) TryGet(reference, typeof(ResultType));
			return result;
		}
		public override object TryGet(FileReference reference, Type ResultType)
		{
			return null;
		}
		
		
		public override bool Exists(FileReference reference)
		{
			return false;
		}
		
		public override bool TryDelete(FileReference reference)
		{
			return false;
		}
		
		public override void Set(FileReference reference, object obj, bool allowOverwrite = true)
		{
			return;
		}
		
		
		public override IEnumerable<string> GetChildrenNames(FileReference parent)
		{
			yield break;
		}

		public override IEnumerable<string> GetChildrenNamesOfType<T>(FileReference parent)
		{
			yield break;
		}
		
//		private static ILogger l = Log.Get();
		
	}
#else
    public class FsOBase : WritableOBaseBase<LocalFileReference>
    {
        #region Static

        public static FsOBase Instance { get { return Singleton<FsOBase>.Instance; } }
        
        #endregion
                

        public override string[] UriSchemes { get { return LocalFileReference.UriSchemes; } }

        public override IObjectWatcher GetWatcher(IReference reference)
        {
            LocalFileReference fref = reference as LocalFileReference;
            if (fref == null) return null;
            var dir = System.IO.Path.GetDirectoryName(reference.Path);
            if (!Directory.Exists(dir))
            {
                //l.Trace("FUTURE: Support watching in directories that don't exist yet");
                return null;
            }
            return new FsWatcher()
            {
                Reference = reference,
            };
        }

        private void ValidateReference(LocalFileReference reference)
        {
            //if (reference.Scheme != LocalFileReference.UriScheme) throw new FsOBaseException("Invalid scheme");
            //if (!reference.IsLocalhost) throw new FsOBaseException("Only localhost supported");
        }

        public override ResultType TryGet<ResultType>(LocalFileReference reference, OptionalRef<RetrieveInfo> optionalRef = null)
        {
            var result = (ResultType)TryGet(reference, typeof(ResultType), optionalRef);
            return result;
        }

        public static IEnumerable<string> GetEncapsulatedTypeNames(Type ResultType)
        {
            yield return ResultType.Name;
            yield return ResultType.FullName;
        }

        public static IEnumerable<Func<string, string>> EncapsulatedFileNameConverters {
            get {
                yield return x => "(" + x + ")";
                //yield return x => "_" + x;
                //yield return x => x + ".";
                //yield return x => "." + x; // No, means hidden
            }
        }

        public static IEnumerable<LocalFileReference> GetEncapsulatedPaths(LocalFileReference reference, Type ResultType)
        {
            foreach (var typeName in GetEncapsulatedTypeNames(ResultType))
            {
                foreach (var fileName in EncapsulatedFileNameConverters.Select(converter => converter(typeName)))
                {
                    yield return (LocalFileReference)reference.GetChild(fileName);
                }
            }
        }

        private object TryConvertToType(object obj, Type ResultType)
        {
            //                ResultType result = obj as ResultType;
            //                if (obj != null && result == null)

            if (obj != null && !ResultType.IsAssignableFrom(obj.GetType()))
            {
                l.Debug("Retrieved object of type '" + obj.GetType().FullName + "' but it cannot be cast to the desired type: " + ResultType.FullName);
                return null;
            }
            return obj;
        }
        public override object TryGet(LocalFileReference reference, Type ResultType, OptionalRef<RetrieveInfo> optionalRef = null)
        {
            try
            {
                object obj = FsOBasePersistence.TryGet(reference.Path).ConfigureAwait(false).GetAwaiter().GetResult();
                obj = TryConvertToType(obj, ResultType);

                if (obj == null)
                {
                    foreach (var encapsulatedRef in GetEncapsulatedPaths(reference, ResultType))
                    {
                        obj = FsOBasePersistence.TryGet(encapsulatedRef.Path).ConfigureAwait(false).GetAwaiter().GetResult();
                        obj = TryConvertToType(obj, ResultType);
                        if (obj != null) break;
                    }
                }

                if (obj != null)
                {
                    OBusEvents.OnRetrievedObjectFromExternalSource(obj); // Put reference in here?

                    if (optionalRef != null)
                    {
                        optionalRef.Value.UltimateOBase = this;
                        optionalRef.Value.UltimateReference = reference;
                    }
                }

                return obj;
            }
            catch (Exception ex)
            {
                OBusEvents.OnException(OBusOperations.Get, reference, ex);
                throw ex;
            }
        }


        public override bool Exists(LocalFileReference reference)
        {
            bool result = FsOBasePersistence.Exists(reference.Path).ConfigureAwait(false).GetAwaiter().GetResult();
            return result;
        }

        public override bool? CanDelete(LocalFileReference reference)
        {
            // FUTURE: Check filesystem permissions
            return Exists(reference);
            //string filePath = reference.Path;
            //return FsPersistence.TryDelete(filePath);
        }

        public override bool TryDelete(LocalFileReference reference, bool preview = false)
        {
            string filePath = reference.Path;
            //if (!defaultTypeForDirIsT)
            //{
            //    filePath = filePath + FileTypeDelimiter + type.Name + FileTypeEndDelimiter;
            //}

            return FsOBasePersistence.TryDelete(filePath, preview: preview).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public override void Set(LocalFileReference reference, object obj, bool allowOverwrite = true, bool preview = false)
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

            FsOBasePersistence.Set(obj, reference.Path, preview: preview);
        }

        public const bool AppendTypeNameToFileNames = false; // TEMP - TODO: Figure out a way to do this in VOS land

        public override IEnumerable<string> GetChildrenNames(LocalFileReference parent)
        {
            LocalFileReference fileRef = LocalFileReference.ConvertFrom(parent);

            if (fileRef == null) throw new ArgumentException("Could not convert to FileReference");

            return FsOBasePersistence.GetChildrenNames(fileRef.Path);
        }
        
        //private static string GetDirNameForType(string filePath)
        //{
        //    var chunks = VosPath.ToPathArray(filePath);
        //    if (chunks == null || chunks.Length == 0) yield break;
        //    string parentDirName = chunks[chunks.Length - 1];

        //    return Assets.AssetPath.GetDefaultDirectory(typeof(T));
        //}

        public override IEnumerable<string> GetChildrenNamesOfType<T>(LocalFileReference parent)
        {
            throw new NotImplementedException();
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
        }

        private static ILogger l = Log.Get();

    }
#endif
        }
