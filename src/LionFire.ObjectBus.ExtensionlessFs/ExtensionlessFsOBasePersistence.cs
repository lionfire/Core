#if TODO
//#define TRACE_SAVE
#define TRACE_LOAD

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Dependencies;
using LionFire.Execution;
using LionFire.ExtensionMethods.Persistence.ExtensionlessFilesystem;
using LionFire.ExtensionMethods.Collections;
using LionFire.MultiTyping;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Serialization;
using LionFire.Structures;
using Microsoft.Extensions.Logging;

namespace LionFire.ObjectBus.Filesystem
{
    public class ExtensionlessFsOBasePersistence
    {
#region Singletons

        public static FSPersistence Instance => Singleton<FSPersistence>.Instance;

#endregion

        protected static PersistenceContext FsOBaseDeserializingPersistenceContext = new PersistenceContext
        {
            SerializationProvider = DependencyLocator.TryGet<ISerializationProvider>(),
            Deserialization = new DeserializePersistenceContext
            {
                PathToStream = ExtensionlessExtensions.PathToReadStream,
                PathToBytes = ExtensionlessExtensions.PathToBytes,
                PathToString = ExtensionlessExtensions.PathToString,
            }
        };

#region Get

        //public static async Task<T> TryRetrieve<T>(string diskPath, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null) => (T)await TryRetrieve(diskPath, operation, context);

        public static async Task<IRetrieveResult<T>> TryRetrieve<T>(ExtensionlessFileReference reference, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            Type type = fileReference.ReferenceType();
#error TODO
        }


        public static async Task<IRetrieveResult<T>> TryRetrieve<T>(string diskPath, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
#error TODO
#region Give Interceptors a chance to return the result

            // REVIEW DESIGNFIXME - this functionality probably doesn't belong here

            foreach (var interceptor in FSPersistenceStatic.Interceptors)
            {
                var obj = interceptor.Read(diskPath, type);
                if (obj is DBNull)
                {
                    return Task.FromResult<object>(null);
                }

                if (obj != null)
                {
                    return Task.FromResult(obj);
                }
            }

#endregion

            try
            {
                return await new Func<object>(() =>
                {
                    //bool deleteFile = false; // FUTURE?
                    try
                    {
#region Directory

                        var dir = Path.GetDirectoryName(diskPath);
                        if (!Directory.Exists(dir))
                        {
                            return null; // DOESNOTEXIST
                        }

#endregion

#region candidatePaths

                        var fileName = Path.GetFileName(diskPath);
                        var candidatePaths = Directory.GetFiles(dir, fileName + "*").ToList();
                        if (candidatePaths.Count == 0)
                        {
                            return null; // DOESNOTEXIST
                        }


#endregion

#region Operation

                        var persistenceOperation = new PersistenceOperation()
                        {
                            Type = type,
                            Deserialization = new DeserializePersistenceOperation()
                            {
#region ENH - optional alternative: combine dir and filenames to get candidatepaths
                                //Directory = dir,
                                //CandidateFilemes = 
#endregion
                                CandidatePaths = candidatePaths.Select(path => Path.Combine(dir, Path.GetFileName(path))),
                            }
                        };

#endregion

#region Context

                        if (context != null)
                        {
                            throw new NotImplementedException($"{nameof(context)} not implemented yet");
                        }
                        var effectiveContext = FsOBaseDeserializingPersistenceContext;

#endregion

                        return persistenceOperation.ToObject<object>(effectiveContext);
                    }
                    finally
                    {
                        //if (deleteFile)
                        //{
                        //    try
                        //    {
                        //        File.Delete(diskPath);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        l.Error("Failed to autodelete empty file: " + diskPath + ". " + ex);
                        //    }
                        //}
                    }
                }).AutoRetry(maxRetries: FSPersistenceStatic.Options.MaxGetRetries, millisecondsBetweenAttempts: FSPersistenceStatic.Options.MillisecondsBetweenGetRetries);
            }
            catch (Exception ex)
            {
                l.Error("Exception retrieving '" + diskPath + "': " + ex.ToString());
                throw;
            }
        }
        public static async Task<object> GetObjectFromPathExtensionless(string diskPath, Type type = null, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
#region Give Interceptors a chance to return the result

            // REVIEW

            foreach (var interceptor in FSPersistenceStatic.Interceptors)
            {
                var obj = interceptor.Read(diskPath, type);
                if (obj is DBNull)
                {
                    return Task.FromResult<object>(null);
                }

                if (obj != null)
                {
                    return Task.FromResult(obj);
                }
            }

#endregion

            try
            {
                return await new Func<object>(() =>
                {
                    //bool deleteFile = false; // FUTURE?
                    try
                    {
#region Directory

                        var dir = Path.GetDirectoryName(diskPath);
                        if (!Directory.Exists(dir))
                        {
                            return null; // DOESNOTEXIST
                        }

#endregion

#region candidatePaths

                        var fileName = Path.GetFileName(diskPath);
                        var candidatePaths = Directory.GetFiles(dir, fileName + "*").ToList();
                        if (candidatePaths.Count == 0)
                        {
                            return null; // DOESNOTEXIST
                        }


#endregion

#region Operation

                        var persistenceOperation = new PersistenceOperation()
                        {
                            Type = type,
                            Deserialization = new DeserializePersistenceOperation()
                            {
#region ENH - optional alternative: combine dir and filenames to get candidatepaths
                                //Directory = dir,
                                //CandidateFilemes = 
#endregion
                                CandidatePaths = candidatePaths.Select(path => Path.Combine(dir, Path.GetFileName(path))),
                            }
                        };

#endregion

#region Context

                        if (context != null)
                        {
                            throw new NotImplementedException($"{nameof(context)} not implemented yet");
                        }
                        var effectiveContext = FsOBaseDeserializingPersistenceContext;

#endregion

                        return persistenceOperation.ToObject<object>(effectiveContext);
                    }
                    finally
                    {
                        //if (deleteFile)
                        //{
                        //    try
                        //    {
                        //        File.Delete(diskPath);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        l.Error("Failed to autodelete empty file: " + diskPath + ". " + ex);
                        //    }
                        //}
                    }
                }).AutoRetry(maxRetries: FSPersistenceStatic.Options.MaxGetRetries, millisecondsBetweenAttempts: FSPersistenceStatic.Options.MillisecondsBetweenGetRetries);
            }
            catch (Exception ex)
            {
                l.Error("Exception retrieving '" + diskPath + "': " + ex.ToString());
                throw;
            }
        }
        public static async Task<bool> Exists(string objectPath, Type type = null)
        {
            if (type == null)
            {
                var paths = GetFilePathsForNamePath(objectPath);
                return paths.Any();
            }

            var obj = await TryGet(objectPath, type);
            return obj != null;
        }

        public static async Task<T> TryGet<T>(string objectPath) => (T)await TryGet(objectPath, typeof(T));
        // TODO - Extract common bits for bottom tier Persistence layers 
        // - this method is for OBases that may return multiple objects?
        public static async Task<object> TryGet(string objectPath, Type type = null)
        {
            var objects = new List<object>();

            object obj = await GetObjectFromPath(objectPath, type);
            objects.Add(obj);

            if (objects.Count > 1)
            {
                return new MultiType(objects);
            }
            else
            {
                return objects.SingleOrDefault();
            }
        }
        public static async Task<object> TryGetExtensionless(string objectPath, Type type = null) // MOVE
        {
            var objects = new List<object>();

            var paths = GetFilePathsForNamePath(objectPath);

            foreach (var path in paths)
            {
                object obj = await GetObjectFromPathExtensionless(path, type);
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

        public static async Task<object> Get(string objectPath, Type type = null) // Move to base or extension
        {
            object obj = await TryGet(objectPath, type);
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
        public static string GetSavePathWithoutExtension(object obj, string objectPath, Type type = null) =>
            // TODO: Autoincrement
            //   - make sure it's a different type

            //if (!allowOverwrite && File.Exists(filePath)) throw new IOException("File already exists and overwrite is disabled.");

            objectPath;

        public static async Task Set(object obj, string objectPath, Type type = null, bool preview = false)
        {
            try
            {
#if TRACE_SAVE
                l.Debug("[FS SAVE] " + objectPath);
#endif
                //string objectDiskPath = GetSavePathWithoutExtension(obj, objectPath, type); // (No extension)
                string objectDiskPath = objectPath; // GetSavePathWithoutExtension(obj, objectPath, type); // (No extension)

                await Write(obj, objectDiskPath, type, preview);
            }
            catch (Exception ex)
            {
                l.Error("Saving '" + objectPath + "' threw exception: " + ex.ToString());
                throw;
            }
        }


        public static ISerializationProvider DefaultSerializationProvider
        {
            get
            {
                if (defaultSerializationProvider == null)
                {
                    defaultSerializationProvider = DependencyLocator.TryGet<ISerializationProvider>();
                }
                return defaultSerializationProvider;
            }
        }
        private static ISerializationProvider defaultSerializationProvider;

        public static async Task Write(object obj, string diskPath, Type type = null, bool preview = false, PersistenceContext context = null)
        {
            if (preview)
            {
                return; // TOPREVIEW - REVIEW - is this still useful?
            }

            await Task.Run(() =>
            {
                string objectSaveDir = System.IO.Path.GetDirectoryName(diskPath);
                Directory.CreateDirectory(objectSaveDir); // TODO SECURITY - set permissions to all users writable

                //foreach (var interceptor in FsPersistence.Interceptors)
                //{
                //    throw new NotImplementedException("TOPORT");
                //    //if (interceptor.Write(obj, fullDiskPath, type, serializer)) return;
                //}

                var op = ((Func<PersistenceOperation>)(() => new PersistenceOperation()
                {
                    Serialization = new SerializePersistenceOperation()
                    {
                        Object = obj,
                    },
                    Reference = new PathReference(diskPath),
                    //PathIsMissingExtension = true,
                    PathIsMissingExtension = false,
                })).ToLazy();

                var strategyResults = (context?.SerializationProvider ?? DefaultSerializationProvider).ResolveStrategies(op, context);

                foreach (var strategyResult in strategyResults)
                {
                    var strategy = strategyResult.Strategy;
                    string fullDiskPath = diskPath;
                    if (op.Value.PathIsMissingExtension == true)
                    {
                        fullDiskPath += "." + strategy.DefaultFormat.DefaultFileExtension;
                    }
#if MONO
            fullDiskPath = fullDiskPath.Replace('\\', '/');
#else
                    fullDiskPath = fullDiskPath.Replace('/', '\\'); // REVIEW
#endif

                    using (var fs = new FileStream(fullDiskPath, FileMode.Create))
                    {
                        strategy.ToStream(obj, fs, op, context);
                    }

                    RecentSaves.AddOrUpdate(fullDiskPath, DateTime.UtcNow, (x, y) => DateTime.UtcNow);
                }
            });
        }

        //public static void Set<T>(object obj, string objectPath)
        //    where T : class
        //{
        //}

#endregion

#region Serialization

        //public static ISerializationStrategy GetSerializer(object obj = null, string path = null)
        //{
        //    return DependencyContext.Current.GetService<ISerializationProvider>().Strategy(
        //        new SerializerSelectionContext
        //        {
        //            Serialize = true,
        //            Object = obj,
        //            Path = path,
        //        });
        //}
        //public static ISerializationStrategy GetDeserializer(object obj = null, string path = null)
        //{
        //    throw new NotImplementedException();
        //    return DependencyContext.Current.GetService<ISerializationProvider>().Strategy(
        //        new SerializerSelectionContext
        //        {
        //            Deserialize = true,
        //            Object = obj,
        //            Path = path,
        //        });
        //}

        //public static object Deserialize(Stream stream, Type type = null, string path = null, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        //{


        //    //var serializer = DependencyContext.Current.GetService<ISerializationProvider>().Strategy(
        //    //    new SerializerSelectionContext
        //    //    {
        //    //        Deserialize = true,
        //    //        Type = type,
        //    //        Path = path,
        //    //    });
        //    //return serializer.ToObject(stream, new SerializationContext { Type = type });
        //}

#endregion

#region Delete

        public static async Task<bool?> TryDelete(string objectPath, Type type = null, bool preview = false)
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
                object obj = await GetObjectFromPath(path, type);
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

            int indexOfNameEnd = name.IndexOf(FSPersistence.EndOfNameMarker);

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
                children.AddRange(Directory.GetFiles(path).Select(FSPersistence.GetNameFromFileName));
                children.TryAddRange(Directory.GetDirectories(path).Select(FSPersistence.GetNameFromFileName));
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
#endif