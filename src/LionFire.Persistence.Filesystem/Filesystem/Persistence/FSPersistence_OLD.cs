#if OLD // Triage remants
//#define TRACE_SAVE
#define TRACE_LOAD

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Execution;
using LionFire.Extensions.Collections;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Serialization;
using LionFire.Structures;
using Microsoft.Extensions.Logging;
using LionFire.Dependencies;
using Microsoft.Extensions.Options;
using LionFire.ExtensionMethods.Persistence.Filesystem;
using LionFire.Persistence.FilesystemFacade;

namespace LionFire.ObjectBus.Filesystem
{
    public static class FSPersistence
    {
        

#if false
        public static async Task<object> GetObjectFromPath(string diskPath, Type type = null, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
        #region Give Interceptors a chance to return the result

            // REVIEW

            foreach (var interceptor in FsPersistence.Interceptors)
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
                }).AutoRetry(maxRetries: FsPersistence.Options.MaxGetRetries, millisecondsBetweenAttempts: FsPersistence.Options.MillisecondsBetweenGetRetries);
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

            foreach (var interceptor in FsPersistence.Interceptors)
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
                }).AutoRetry(maxRetries: FsPersistence.Options.MaxGetRetries, millisecondsBetweenAttempts: FsPersistence.Options.MillisecondsBetweenGetRetries);
            }
            catch (Exception ex)
            {
                l.Error("Exception retrieving '" + diskPath + "': " + ex.ToString());
                throw;
            }
        }

#endif

        // MOVE to Extensionless
        //public static async Task<bool> Exists<T>(string objectPath)
        //{
        //    if (type == null)
        //    {
        //        var paths = GetFilePathsForNamePath(objectPath);
        //        return paths.Any();
        //    }

        //    var obj = await TryGet(objectPath, type);
        //    return obj != null;
        //}



        // MOVE to Extensionless:
        //public static async Task<T> TryGet<T>(string objectPath)
        //{
        //    var objects = new List<object>();
        //    object obj = await GetObjectFromDiskPath<T>(objectPath, typeof(T));
        //    objects.Add(obj);

        //    if (objects.Count > 1)
        //    {
        //        return new MultiType(objects);
        //    }
        //    else
        //    {
        //        return objects.SingleOrDefault();
        //    }

        //    (T)await TryGet(objectPath, typeof(T));
        //}


        // TODO - Extract common bits for bottom tier Persistence layers 
        // - this method is for OBases that may return multiple objects?
        //public static async Task<object> TryGetOneOrMany(string objectPath, Type type = null)
        //{
        //    var objects = new List<object>();
        //    object obj = await GetObjectFromPath<T>(objectPath, type);
        //    objects.Add(obj);

        //    if (objects.Count > 1)
        //    {
        //        return new MultiType(objects);
        //    }
        //    else
        //    {
        //        return objects.SingleOrDefault();
        //    }
        //}

        // MOVE to extensionless
        //public static async Task<object> TryGetExtensionless(string objectPath, Type type = null) // MOVE
        //{
        //    var objects = new List<object>();

        //    var paths = GetFilePathsForNamePath(objectPath);

        //    foreach (var path in paths)
        //    {
        //        object obj = await GetObjectFromPathExtensionless(path, type);
        //        objects.Add(obj);
        //    }


        //    if (objects.Count > 1)
        //    {
        //        return new MultiType(objects);
        //    }
        //    else
        //    {
        //        return objects.SingleOrDefault();
        //    }
        //}

        //public static async Task<T> Get<T>(string objectPath, Type type = null) // Move to base or extension
        //{
        //    var obj = await TryGet<T>(objectPath);
        //    if (obj == null)
        //    {
        //        throw new FileNotFoundException();
        //    }
        //    return obj;
        //}

        #region Deferred

        // Rethink these in more complex layers on top of FSPersistence

        /// <summary>
        /// Get the save path.  This may add to the filename to indicate the type, or an auto-incrementing number to prevent conflicts for multitype objects.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objectPath"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        //public static string GetSavePathWithoutExtension(object obj, string objectPath, Type type = null) => objectPath;
        // TODO: Autoincrement
        //   - make sure it's a different type

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


    }
}

#endif