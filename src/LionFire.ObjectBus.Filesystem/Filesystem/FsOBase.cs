using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LionFire.Serialization;

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
    public class FsOBase : OBaseBase<FileReference>
    {
        public static FsOBase Instance { get { return Singleton<FsOBase>.Instance; } }

        public override string[] UriSchemes { get { return FileReference.UriSchemes; } }

        public override IObjectWatcher GetWatcher(IReference reference)
        {
            FileReference fref = reference as FileReference;
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

        private void ValidateReference(FileReference reference)
        {
            if (reference.Scheme != FileReference.UriScheme) throw new FsOBaseException("Invalid scheme");
            if (!reference.IsLocalhost) throw new FsOBaseException("Only localhost supported");

        }



        public override ResultType TryGet<ResultType>(FileReference reference, OptionalRef<RetrieveInfo> optionalRef = null)
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

        public static IEnumerable<FileReference> GetEncapsulatedPaths(FileReference reference, Type ResultType)
        {
            foreach (var typeName in GetEncapsulatedTypeNames(ResultType))
            {
                foreach (var fileName in EncapsulatedFileNameConverters.Select(converter => converter(typeName)))
                {
                    yield return (FileReference)reference.GetChild(fileName);
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
        public override object TryGet(FileReference reference, Type ResultType, OptionalRef<RetrieveInfo> optionalRef = null)
        {
            try
            {
                object obj = FsPersistence.TryGet(reference.Path);
                obj = TryConvertToType(obj, ResultType);

                if (obj == null)
                {
                    foreach (var encapsulatedRef in GetEncapsulatedPaths(reference, ResultType))
                    {
                        obj = FsPersistence.TryGet(encapsulatedRef.Path);
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


        public override bool Exists(FileReference reference)
        {
            bool result = FsPersistence.Exists(reference.Path);
            return result;
        }

        public override bool? CanDelete(FileReference reference)
        {
            // FUTURE: Check filesystem permissions
            return Exists(reference);
            //string filePath = reference.Path;
            //return FsPersistence.TryDelete(filePath);
        }

        public override bool TryDelete(FileReference reference, bool preview = false)
        {
            string filePath = reference.Path;
            //if (!defaultTypeForDirIsT)
            //{
            //    filePath = filePath + FileTypeDelimiter + type.Name + FileTypeEndDelimiter;
            //}

            return FsPersistence.TryDelete(filePath, preview: preview);
        }

        public override void Set(FileReference reference, object obj, bool allowOverwrite = true, bool preview = false)
        {
            bool defaultTypeForDirIsT;
            Type type = obj.GetType();
            var chunks = reference.GetPathArray();
            if (chunks == null || chunks.Length < 2)
            {
                defaultTypeForDirIsT = false;
            }
            else
            {
                string parentDirName;
                parentDirName = chunks[chunks.Length - 2];
                defaultTypeForDirIsT = Assets.AssetPaths.GetAssetTypeFolder(type).TrimEnd('/').Equals(parentDirName);
            }

            // REVIEW - This should be done up a layer, since most persistance mechanisms will not support multi-type
            string filePath = reference.Path;
            if (AppendTypeNameToFileNames && !defaultTypeForDirIsT)
            {
                filePath = filePath + VosPath.TypeDelimiter + type.Name + VosPath.TypeEndDelimiter;
            }

            FsPersistence.Set(obj, filePath, preview: preview);
        }

        public const bool AppendTypeNameToFileNames = false; // TEMP - TODO: Figure out a way to do this in VOS land

        public override IEnumerable<string> GetChildrenNames(FileReference parent)
        {
            FileReference fileRef = FileReference.ConvertFrom(parent);

            if (fileRef == null) throw new ArgumentException("Could not convert to FileReference");

            return FsPersistence.GetChildrenNames(fileRef.Path);
        }

        //public const char FileTypeDelimiter = '(';
        //public const char FileTypeEndDelimiter = ')';

        private string GetTypeNameFromFileName(string fileName)
        {
            int index = fileName.IndexOf(VosPath.TypeDelimiter);
            if (index == -1) return null;
            return fileName.Substring(index, fileName.IndexOf(VosPath.TypeEndDelimiter, index) - index);
        }

        //private static string GetDirNameForType(string filePath)
        //{
        //    var chunks = VosPath.ToPathArray(filePath);
        //    if (chunks == null || chunks.Length == 0) yield break;
        //    string parentDirName = chunks[chunks.Length - 1];

        //    return Assets.AssetPath.GetDefaultDirectory(typeof(T));
        //}

        public override IEnumerable<string> GetChildrenNamesOfType<T>(FileReference parent)
        {
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
        }

        private static ILogger l = Log.Get();

    }
#endif
}
