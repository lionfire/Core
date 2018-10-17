//#define TRACE_SAVE
#define TRACE_LOAD

using System;
//using LionFire.Extensions.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LionFire.DependencyInjection;
using LionFire.Execution;
using LionFire.Extensions.Collections;
using LionFire.IO.Filesystem;
using LionFire.MultiTyping;
using LionFire.Serialization;
using LionFire.Structures;
using Microsoft.Extensions.Logging;

namespace LionFire.IO.Filesystem // MOVE to LionFire.IO.Filesystem
{
    public interface IFsPersistenceInterceptor
    {
        object Read(string diskPath, Type type = null);

        //bool Write(object obj, string fullDiskPath, Type type, LionSerializer serializer); // TOPORT
    }

    public class FsPersistence
    {
        public static List<IFsPersistenceInterceptor> Interceptors => interceptors;
        private static readonly List<IFsPersistenceInterceptor> interceptors = new List<IFsPersistenceInterceptor>();

        // TODO: get this from InjectionContext
        public static FsPersistenceOptions Options { get; set; } = new FsPersistenceOptions();
    }

    public class FsPersistenceOptions
    {
        public int MaxGetRetries { get; set; } = 10;
        public int MillisecondsBetweenGetRetries { get; set; } = 500;
    }
}

namespace LionFire.Serialization.Filesystem // MOVE to LionFire.Serialization.Filesystem
{
    /// <summary>
    /// LionFire Filesystem serialization
    /// </summary>
    public class FsSerialization
    {
    }
}
namespace LionFire.Serialization.Net // MOVE to LionFire.Serialization.Net
{
    public class NetSerialization
    {
    }
}

namespace LionFire.ObjectBus.Filesystem
{
    public class FsOBasePersistence
    {
        #region Singletons

        public static FsOBasePersistence Instance => Singleton<FsOBasePersistence>.Instance;
        
        #endregion

        #region Constants

        public static string EndOfNameMarker = "'";

        #endregion

        #region Configuration

        public static readonly bool AutoDeleteEmptyFiles = true;
        //public static readonly bool AutoDeleteNullFiles = true; // FUTURE? Delete if file is all null (saw this on my SSDs after a machine crash)

        #endregion

        #region Get

        public static T GetFromPath<T>(string diskPath) => (T)GetObjectFromPath(diskPath, typeof(T));
        public static object GetObjectFromPath(string diskPath, Type type = null)
        {
            foreach (var interceptor in FsPersistence.Interceptors)
            {
                var obj = interceptor.Read(diskPath, type);
                if (obj is DBNull)
                {
                    return null;
                }

                if (obj != null)
                {
                    return obj;
                }
            }
            try
            {
                return new Func<object>(() =>
                {
                    bool deleteFile = false;
                    try
                    {
                        using (FileStream fs = new FileStream(diskPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
#if TRACE_LOAD
                            l.Debug("[FS LOAD] " + diskPath);
#endif
                            if (fs.Length == 0)
                            {
                                l.Error("FsPersistence.GetFromPath found empty file. " + (AutoDeleteEmptyFiles ? "Autodeleting it." : "") + " - " + diskPath);
                                deleteFile = AutoDeleteEmptyFiles;
                                return null;
                            }

                            object obj = Deserialize(fs, type: type, path: diskPath);
                            return obj;
                        }
                    }
                    finally
                    {
                        if (deleteFile)
                        {
                            try
                            {
                                File.Delete(diskPath);
                            }
                            catch (Exception ex)
                            {
                                l.Error("Failed to autodelete empty file: " + diskPath + ". " + ex);
                            }
                        }
                    }
                }).AutoRetry(maxRetries: FsPersistence.Options.MaxGetRetries, millisecondsBetweenAttempts: FsPersistence.Options.MillisecondsBetweenGetRetries); ;
            }
            catch (Exception ex)
            {
                l.Error("Exception retrieving '" + diskPath + "': " + ex.ToString());
                throw;
            }
        }
        public static bool Exists(string objectPath, Type type = null)
        {
            if (type == null)
            {
                var paths = GetFilePathsForNamePath(objectPath);
                return paths.Any();
            }

            var obj = TryGet(objectPath, type);
            return obj != null;
        }

        public static T TryGet<T>(string objectPath) => (T)TryGet(objectPath, typeof(T));
        // TODO - Extract common bits for bottom tier Persistence layers 
        // - this method is for OBases that may return multiple objects?
        public static object TryGet(string objectPath, Type type = null)
        {
            List<object> objects = new List<object>();

            var paths = GetFilePathsForNamePath(objectPath);

            foreach (var path in paths)
            {
                object obj = GetObjectFromPath(path, type);
                objects.Add(obj);
            }


            if (objects.Count > 1)
            {
                return new MultiType(objects);
            }
            else
            {
                return objects.SingleOrDefault();
            }
        }

        public static object Get(string objectPath, Type type = null) // Move to base or extension
        {
            object obj = TryGet(objectPath, type);
            if (obj == null)
            {
                throw new FileNotFoundException();
            }
            return obj;
        }

        #endregion

        #region Set

        // TODO: overwrite modes, etc.

        /// <summary>
        /// Get the save path.  This may add to the filename to indicate the type, or an auto-incrementing number to prevent conflicts for multitype objects.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objectPath"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetSavePath(object obj, string objectPath, Type type = null) =>
            // TODO: Autoincrement
            //   - make sure it's a different type

            //if (!allowOverwrite && File.Exists(filePath)) throw new IOException("File already exists and overwrite is disabled.");

            objectPath;

        public static void Set(object obj, string objectPath, Type type = null, bool preview = false)
        {
            try
            {
#if TRACE_SAVE
                l.Debug("[FS SAVE] " + objectPath);
#endif
                string objectDiskPath = GetSavePath(obj, objectPath, type); // (No extension)

                Write(obj, objectDiskPath, type, preview);


            }
            catch (Exception ex)
            {
                l.Error("Saving '" + objectPath + "' threw exception: " + ex.ToString());
                throw;
            }
        }
               
        public static void Write(object obj, string diskPath, Type type = null, bool preview = false)
        {
            if (preview)
            {
                return; // TOPREVIEW
            }

            string objectSaveDir = System.IO.Path.GetDirectoryName(diskPath);
            Directory.CreateDirectory(objectSaveDir); // TODO SECURITY - set permissions to all users writable
            var serializer = GetSerializer(obj, diskPath);
            
            string fullDiskPath = diskPath + "." + serializer.DefaultFormat.DefaultFileExtension;
#if MONO
            fullDiskPath = fullDiskPath.Replace('\\', '/');
#else
            fullDiskPath = fullDiskPath.Replace('/', '\\'); // REVIEW
#endif

            foreach (var interceptor in FsPersistence.Interceptors)
            {
                throw new NotImplementedException("TOPORT");
                //if (interceptor.Write(obj, fullDiskPath, type, serializer)) return;
            }

            using (FileStream fs = new FileStream(fullDiskPath, FileMode.Create))
            {
                var ctx = new SerializationContext {
                    Object = obj,
                    Stream = fs
                };
                serializer.ToStream(ctx);
                RecentSaves.AddOrUpdate(fullDiskPath, DateTime.UtcNow, (x, y) => DateTime.UtcNow);
            }
        }

        //public static void Set<T>(object obj, string objectPath)
        //    where T : class
        //{
        //}

        #endregion

        #region Serialization

        public static ISerializationStrategy GetSerializer(object obj = null, string path = null)
        {
            return InjectionContext.Current.GetService<ISerializationProvider>().Strategy(
                new SerializerSelectionContext
                {
                    Serialize = true,
                    Object = obj,
                    Path = path,
                });
        }
        public static ISerializationStrategy GetDeserializer(object obj = null, string path = null)
        {
            return InjectionContext.Current.GetService<ISerializationProvider>().Strategy(
                new SerializerSelectionContext
                {
                    Deserialize = true,
                    Object = obj,
                    Path = path,
                });
        }

        public static object Deserialize(Stream stream, Type type = null, string path = null)
        {
            var serializer = InjectionContext.Current.GetService<ISerializationProvider>().Strategy(
                new SerializerSelectionContext
                {
                    Deserialize = true,
                    Type = type,
                    Path = path,
                });
            return serializer.ToObject(stream, new SerializationContext { Type = type });
        }

        #endregion

        #region Delete

        public static bool TryDelete(string objectPath, Type type = null, bool preview = false)
        {
            bool result = false;
            var paths = GetFilePathsForNamePath(objectPath);
            if (type == null)
            {
                if (paths.Any())
                {
                    foreach (var path in paths)
                    {
                        if (!preview)
                        {
                            File.Delete(path);
                        }
                        result = true;
                    }
                }
                return result;
            }

            foreach (var path in paths)
            {
                object obj = GetObjectFromPath(path, type);
                if (obj != null)
                {
                    if (!preview)
                    {
                        File.Delete(path);
                    }
                    result = true;
                }
            }
            return result;
        }

        #endregion

        #region (Static) Directory accessors

        /// <summary>
        /// WARNING REVIEW: If files have multiple extensions, there may be strange behavior
        /// </summary>
        /// <param name="namePath"></param>
        /// <returns></returns>
        public static List<string> GetFilePathsForNamePath(string namePath)
        {
            List<string> paths = new List<string>();

            string name = Path.GetFileName(namePath);

            string dirPath = Path.GetDirectoryName(namePath);

            if (Directory.Exists(dirPath))
            {
                var fileList = Directory.GetFiles(dirPath, name + "*").OfType<string>();
                string nameWithoutExtension = Path.GetFileNameWithoutExtension(namePath);
                if (nameWithoutExtension != name)
                {
                    fileList = fileList.Concat(Directory.GetFiles(dirPath, nameWithoutExtension + "*").OfType<string>()).Distinct();
                }

                foreach (var potentialPath in fileList)
                {
                    string potentialName = GetNameFromFileName(potentialPath);
                    if (potentialName == name
                        || potentialName == nameWithoutExtension
                        )
                    {
                        paths.Add(potentialPath);
                    }
                }

            }
            return paths;
        }

        public static string GetNameFromFileName(string filename)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(filename);

            int indexOfNameEnd = name.IndexOf(EndOfNameMarker);

            if (indexOfNameEnd != -1)
            {
                name = name.Substring(0, indexOfNameEnd);
            }
            //else
            //{
            //    name = 
            //}

            //// TODO: detect type name in brackets: []
            //if (name.EndsWith("]"))
            //{
            //    int index = name.LastIndexOf("[");
            //    if (index != -1)
            //    {
            //        name = name.Substring(0, index);
            //    }
            //}

            return name;
        }

        public static List<string> GetChildrenNames(string path)
        {
            List<string> children = new List<string>();
            if (Directory.Exists(path))
            {
                children.AddRange(Directory.GetFiles(path).Select(FsOBasePersistence.GetNameFromFileName));
                children.TryAddRange(Directory.GetDirectories(path).Select(FsOBasePersistence.GetNameFromFileName));
            }
            return children;
        }

        #region Metadata

        //private static Vob VFSMetadata // TOPORT
        ////private Vob<FSMetaData> FSMetadata // TODO
        //{
        //    get
        //    {
        //        if (fsMetadata == null)
        //        {
        //            fsMetadata = Vos.Default["/`/fs/md"];
        //        }
        //        return fsMetadata;
        //    }
        //}
        //private static Vob fsMetadata;

        private class FSMetaData
        {
            public Type DefaultType = null;
        }

        private static Type GetDefaultChildTypeForPath(string path) => throw new NotImplementedException("TOPORT");//#if AOT // TOPORT//            var metadata = VFSMetadata[path].AsType(typeof(FSMetaData)) as FSMetaData;//#else//            var metadata = VFSMetadata[path].AsType<FSMetaData>();//#endif//            if (metadata == null) return null;//            return metadata.DefaultType;

        #endregion

        public static List<string> GetChildrenNamesOfType<ChildType>(string path)
            where ChildType : class, new() => throw new NotImplementedException("TOPORT");//List<string> children = new List<string>();//if (typeof(ChildType) == typeof(VosDirectory))//{//    l.Warn("TEMP Behaviour - Scanning for VosDirectory returns all directories in filesystem");//    children.AddRange(Directory.GetDirectories(path));//}//else//{//    Type defaultType = GetDefaultChildTypeForPath(path); // Use a Vob overlay over the filesystem to access this?//    if (defaultType != null)//    {//    }//    else//    {//    }//    children.AddRange(Directory.GetFiles(path).Select(FsPersistence.GetNameFromFileName));//}//return children;
        #endregion

        #region RecentSaves

        public static ConcurrentDictionary<string, DateTime> RecentSaves => recentSaves;
        private static readonly ConcurrentDictionary<string, DateTime> recentSaves = new ConcurrentDictionary<string, DateTime>();

        public static void CleanRecentSaves()
        {
            foreach (var kvp in RecentSaves)
            {
                if (DateTime.UtcNow - kvp.Value > TimeSpan.FromMinutes(3))
                {
                    RecentSaves.TryRemove(kvp.Key, out DateTime dt);
                }
            }
        }

        #endregion

        private static ILogger l = Log.Get();
    }
}
