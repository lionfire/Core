#nullable enable
using LionFire.Dependencies;
using LionFire.ObjectBus.Filesystem;
using LionFire.Persistence;
using LionFire.Persistence.FilesystemFacade;
using LionFire.Referencing;
using LionFire.Serialization;
using LionFire.Structures;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FileMode = System.IO.FileMode;
using System.Threading.Tasks;
using System.Linq;
using LionFire.IO;
using System.IO;
using LionFire.Execution;
using LionFire.Persistence.Persisters;
using Microsoft.Extensions.Options;

namespace LionFire.Persistence.Filesystem
{
#warning NEXT: determining candidate paths should be an async.  But I am not sure I want to unwrap a Task.FromResult(singlePath) for the simple case.

#warning NEXT: What should the flow be for Persistence.Retrieve, and Persistence.Write?  Create an op, and then iterate over Serializers until it succeeds? Do Retrieve/Write just create an op, and then the op can run on its own?  Is it like a blackboard object with its own configurable pipeline?  Is there a hidden IPersisterOperationsProvider interface for CreateRetrieveOperation and CreateWriteOperation?

    //public class RetrieveOperation
    //{
    //}


    //public class Pipeline
    //{
    //    protected List<Action<Pipeline>> pipeline;

    //    public void Next();
    //}

    //public delegate object RetrieveProcessor(object x);

    //public interface IFilesystemWritePersister
    //{
    //    Stream CreateWriteStream(string fsPath, ReplaceMode replaceMode);

    //    Task BytesToPath(string fsPath, byte[] bytes, ReplaceMode replaceMode);
    //    Task StringToPath(string fsPath, string str, ReplaceMode replaceMode);

    //    //Func<T, string, ISerializationStrategy, ReplaceMode, PersistenceOperation, PersistenceContext, SerializationResult> SerializeToString<T>(T obj, string fsPath, ISerializationStrategy strategy, ReplaceMode replaceMode, PersistenceOperation operation, PersistenceContext context) { get;}

    //    // Task<SerializationResult> SerializeToBytes<T>(T obj, string fsPath, ISerializationStrategy strategy, ReplaceMode replaceMode, PersistenceOperation operation, PersistenceContext context)

    //    //Task<SerializationResult> SerializeToStream<T>(T obj, string fsPath, ISerializationStrategy strategy, ReplaceMode replaceMode, PersistenceOperation operation, PersistenceContext context)
    //}

    public static class FilesystemPersisterExtensions
    {
        public static async Task<IPersistenceResult> DeleteResultToPersistenceResult(this Task<bool?> deleteTask)
        {
            var deleteResult = await deleteTask.ConfigureAwait(false);
            if (!deleteResult.HasValue) return PersistenceResult.Indeterminate;
            return deleteResult.Value ? PersistenceResult.SuccessAndFound : PersistenceResult.SuccessNotFound;
        }
    }

    /// <summary>
    /// IPersister return IPersistenceResult and IFSPersistence returns simpler results closer to the underlying filesystem (or similar store)
    /// </summary>
    /// <typeparam name="TReference"></typeparam>
    /// <typeparam name="TPersistenceOptions"></typeparam>
    public abstract partial class FilesystemPersister<TReference, TPersistenceOptions>
        : PersisterBase<TPersistenceOptions>
        , IPersister<TReference>
        , IFilesystemPersistence<TReference, TPersistenceOptions>
        , IWriter<string>
        , IReader<string>
        where TReference : IReference
        where TPersistenceOptions : FilesystemPersisterOptions
    {
        public abstract IOCapabilities Capabilities { get; }

        public abstract TReference PathToReference(string fsPath);

        IOptionsMonitor<FilesystemPersisterOptions> optionsMonitor;

        #region Dependencies

        #region SerializationProvider

        [SetOnce]
        public ISerializationProvider SerializationProvider
        {
            get => serializationProvider;
            set
            {
                if (serializationProvider == value) return;
                if (serializationProvider != default) throw new AlreadySetException();
                serializationProvider = value;
            }
        }
        private ISerializationProvider serializationProvider;

        #endregion

        #endregion

        #region Static Defaults

        private static TPersistenceOptions DefaultOptions
        {
            get
            {
                if (defaultOptions == null)
                {
                    defaultOptions = Activator.CreateInstance<TPersistenceOptions>();
                }
                return defaultOptions;
            }
        }
        private static TPersistenceOptions defaultOptions = Activator.CreateInstance<TPersistenceOptions>();

        #endregion

        #region Construction

        public FilesystemPersister(ISerializationProvider serializationProvider, string name, IOptionsMonitor<TPersistenceOptions> optionsMonitor)
        {
            this.serializationProvider = serializationProvider;
            //this.PersistenceOptions = persistenceOptions;
            this.PersistenceOptions = string.IsNullOrEmpty(name) ? optionsMonitor.CurrentValue : optionsMonitor.Get(name);
            this.optionsMonitor = optionsMonitor;
        }

        #endregion


        #region Read: Deserialize

        /// <summary>
        /// Default implementation uses DeserializeFromStream.  If you don't want to use Streams, override and use string or byte[] serialization approaches.
        /// </summary>
        /// <param name="fsPath"></param>
        /// <returns></returns>
        public virtual Task<DeserializationResult<T>> DeserializeFromOutput<T>(string fsPath, T obj, ISerializationStrategy strategy, PersistenceOperation operation, PersistenceContext context)
        {
            bool stream = PersistenceOptions.PreferStreamReading == true && Capabilities.HasFlag(IOCapabilities.ReadStream) && strategy.ImplementsFromStream;
            if (stream)
            {
                return DeserializeFromStream<T>(fsPath, obj, strategy, operation, context);
            }
            else
            {
                if (strategy.ImplementsFromBytes)
                {
                    return DeserializeFromBytes<T>(fsPath, obj, strategy, operation, context); // Will fall back to String if it is a textual serializer
                }
                else
                {
                    return DeserializeFromString<T>(fsPath, obj, strategy, operation, context); // Will fall back to Stream if it is a stream-only serializer
                }
            }
        }

        public virtual async Task<DeserializationResult<T>> DeserializeFromStream<T>(string fsPath, T obj, ISerializationStrategy strategy, PersistenceOperation operation, PersistenceContext context)
        {
            using var fs = await ReadStream(fsPath).ConfigureAwait(false);
            return await Task.Run(() => strategy.ToObject<T>(fs, operation, context)).ConfigureAwait(false);
        }

        public virtual async Task<DeserializationResult<T>> DeserializeFromBytes<T>(string fsPath, T obj, ISerializationStrategy strategy, PersistenceOperation operation, PersistenceContext context)
            => strategy.ToObject<T>(await ReadBytes(fsPath).ConfigureAwait(false), operation, context);
        public virtual async Task<DeserializationResult<T>> DeserializeFromString<T>(string fsPath, T obj, ISerializationStrategy strategy, PersistenceOperation operation, PersistenceContext context)
            => strategy.ToObject<T>(await ReadString(fsPath).ConfigureAwait(false), operation, context);

        #endregion

        #region Write: Serialize

        /// <summary>
        /// Default implementation uses SerializeToStream.  If you don't want to use Streams, override and use string or byte[] serialization approaches.
        /// </summary>
        /// <param name="fsPath"></param>
        /// <returns></returns>
        public virtual Task<SerializationResult> SerializeToOutput<T>(T obj, string fsPath, ISerializationStrategy strategy, ReplaceMode replaceMode, PersistenceOperation? operation, PersistenceContext? context)
        {
            bool stream = PersistenceOptions.PreferStreamWriting == true && Capabilities.HasFlag(IOCapabilities.WriteStream) && strategy.ImplementsToStream;
            if (stream)
            {
                return SerializeToStream<T>(obj, fsPath, strategy, replaceMode, operation, context);
            }
            else
            {
                if (strategy.ImplementsToBytes)
                {
                    return SerializeToBytes<T>(obj, fsPath, strategy, replaceMode, operation, context); // Will fall back to String if it is a textual serializer
                }
                else
                {
                    return SerializeToString<T>(obj, fsPath, strategy, replaceMode, operation, context); // Will fall back to Stream if it is a stream-only serializer
                }
            }
        }

        public virtual async Task<SerializationResult> SerializeToStream<T>(T obj, string fsPath, ISerializationStrategy strategy, ReplaceMode replaceMode, PersistenceOperation? operation, PersistenceContext? context)
        {
            using var fs = await WriteStream(fsPath, replaceMode).ConfigureAwait(false);
            return await Task.Run(() => strategy.ToStream(obj, fs, operation, context)).ConfigureAwait(false);
        }

        public virtual async Task<SerializationResult> SerializeToBytes<T>(T obj, string fsPath, ISerializationStrategy strategy, ReplaceMode replaceMode, PersistenceOperation? operation, PersistenceContext? context)
        {
            var result = strategy.ToBytes(obj, operation, context);
            if (!result.Result.IsSuccess) return result.Result;
            await WriteBytes(fsPath, result.Bytes, replaceMode).ConfigureAwait(false);
            return result.Result;
        }
        public virtual async Task<SerializationResult> SerializeToString<T>(T obj, string fsPath, ISerializationStrategy strategy, ReplaceMode replaceMode, PersistenceOperation? operation, PersistenceContext? context)
        {
            var result = strategy.ToString(obj, operation, context);
            if (!result.Result.IsSuccess) return result.Result;
            await WriteString(fsPath, result.String, replaceMode).ConfigureAwait(false);
            return result.Result;
        }

        #endregion

        #region Create

        public async Task<IPersistenceResult> Create<TValue>(IReferencable<TReference> referencable, TValue value)
            => await Write(referencable.Reference, value, ReplaceMode.Create).ConfigureAwait(false);

        #endregion

        #region Retrieve

        #region Exists

        public async Task<IPersistenceResult> Exists<TValue>(IReferencable<TReference> referencable)
            => await Exists(referencable.Reference.Path) ? ExistsResult.Found : ExistsResult.NotFound;
        public async Task<IPersistenceResult> Exists<TValue>(TReference reference)
            => await Exists(reference.Path) ? ExistsResult.Found : ExistsResult.NotFound;

        #endregion

        #region Retrieve

        public Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<TReference> referencable)  // Unwrap IReferencable
            => Retrieve<TValue>(referencable.Reference);

        /// <remarks>
        /// Default implementation: pass-through to ReadAndDeserializeExactPath, with AutoRetry
        /// </remarks>
        public virtual async Task<IRetrieveResult<TValue>> Retrieve<TValue>(TReference reference)
        {
            var path = reference.Path;
            if (!string.IsNullOrEmpty(reference.Persister))
            {
                var providerOptions = optionsMonitor.Get(reference.Persister);
                if (providerOptions.RootDirectory == null) throw new UnknownPersisterException($"Provider '{reference.Persister}' is not known."); // REVIEW - better way to determine uninitialized/missing?
                path = LionPath.Combine(providerOptions.RootDirectory, path);
            }

            return await new Func<Task<IRetrieveResult<TValue>>>(()
                => ReadAndDeserializeExactPath<TValue>(path, ((Func<PersistenceOperation>)(() =>
                    new PersistenceOperation(reference)
                    {
                        Direction = IODirection.Read,
                    })).ToLazy()))
                .AutoRetry(maxRetries: PersistenceOptions.MaxGetRetries,
                millisecondsBetweenAttempts: PersistenceOptions.MillisecondsBetweenGetRetries, allowException: AllowAutoRetryForThisException).ConfigureAwait(false);
        }

        //public virtual Task<IRetrieveResult<T>> ReadAndDeserializeExactPath<T>(TReference reference, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        //    => ReadAndDeserializeExactPath<T>(reference.Path, operation, context);

        public virtual async Task<IRetrieveResult<T>> ReadAndDeserializeExactPath<T>(string path, Lazy<PersistenceOperation>? operation = null, PersistenceContext? context = null, bool throwOnFail = true)
        {
            // NEXT: how to invoke these?  Latest idea: persist/depersist pipelines
            //var obj = await persistenceOperation.ToObject<T>(effectiveContext).ConfigureAwait(false);
            //var obj = await (persistenceOperation.Context?.SerializationProvider ?? DependencyLocator.TryGet<ISerializationProvider>()).ToObject<T>(op);

            List<KeyValuePair<ISerializationStrategy, SerializationResult>>? failures = null;

            PersistenceResultFlags flags = PersistenceResultFlags.None;

            byte[]? bytes;

            if (!await Exists(path).ConfigureAwait(false)) bytes = null;
            else
            {
                try
                {
                    bytes = await ReadBytes(path).ConfigureAwait(false);
                }
                catch (FileNotFoundException)
                {
                    bytes = null;
                }
            }
            if (bytes == null)
            {
                flags |= PersistenceResultFlags.NotFound | PersistenceResultFlags.Fail;
            }
            else
            {
                flags |= PersistenceResultFlags.Found;
                bool foundOne = false;
                foreach (var strategy in SerializationProvider.ResolveStrategies(operation, context, IODirection.Read).Select(r => r.Strategy))
                {
                    foundOne = true;
                    var result = strategy.ToObject<T>(bytes, operation, context);
                    if (result.IsSuccess)
                    {
                        flags |= PersistenceResultFlags.Success;
                        return new RetrieveResult<T>(result.Object, flags);
                    }
                    else if (PersistenceOptions.ThrowDeserializationFailureWithReasons)
                    {
                        if (failures == null)
                        {
                            failures = new List<KeyValuePair<ISerializationStrategy, SerializationResult>>();
                        }
                        failures.Add(new KeyValuePair<ISerializationStrategy, SerializationResult>(strategy, result));
                    }
                    // else don't initialize failures and throw
                }
                if (!foundOne) flags |= PersistenceResultFlags.SerializerNotAvailable;
                flags |= PersistenceResultFlags.Fail;

                if (throwOnFail && PersistenceOptions.ThrowOnDeserializationFailure)
                {
                    throw new SerializationException(SerializationOperationType.FromBytes, operation, context, failures, noSerializerAvailable: !foundOne);
                }
            }

#nullable disable
            return new RetrieveResult<T>(default, flags)
            {
                Error = failures,
            };
#nullable enable
        }

        #endregion

        #endregion

        #region Put

        public Task<IPersistenceResult> Write<T>(string fsPath, T obj, ReplaceMode replaceMode = ReplaceMode.Upsert, PersistenceContext? context = null, Type? type = null)
            => Write(PathToReference(fsPath), obj, replaceMode, context, type);

        public Task<IPersistenceResult> Write<T>(TReference fsReference, T obj, ReplaceMode replaceMode = ReplaceMode.Upsert, PersistenceContext? context = null, Type? type = null)
        {
            TReference effectiveReference;
            bool allowOverwrite = replaceMode.HasFlag(ReplaceMode.Update);
            bool requireOverwrite = !replaceMode.HasFlag(ReplaceMode.Create);

            return Task.Run<IPersistenceResult>(async () =>
            {
                var fsPath = fsReference.Path;

                #region !allowOverwrite

                //#warning FIXME: Use Exists() instead of Exists<T>
                //if (!allowOverwrite && (await Exists<T>(fsPath).ConfigureAwait(false))) throw new AlreadySetException($"File already exists at '{fsReference}'"); // TOTEST
                if (!allowOverwrite)
                {
                    if ((await Exists(fsPath).ConfigureAwait(false))) throw new AlreadySetException($"File already exists at '{fsReference}'"); // TOTEST
                }
                else if (requireOverwrite)
                {
                    if (!(await Exists(fsPath).ConfigureAwait(false))) throw new NotFoundException($"Cannot (strictly) update file, because it is not found at '{fsReference}'"); // TOTEST
                }


                #endregion

                #region AutoCreateParentDirectories

                if (PersistenceOptions.AutoCreateParentDirectories && !requireOverwrite)
                {
                    string objectSaveDir = System.IO.Path.GetDirectoryName(fsPath);
                    if (!await DirectoryExists(objectSaveDir).ConfigureAwait(false))
                    {
                        await CreateDirectory(objectSaveDir).ConfigureAwait(false);
                    }
                }

                #endregion

                //foreach (var interceptor in FSPersistence.Interceptors)
                //{
                //    throw new NotImplementedException("TOPORT");
                //    //if (interceptor.Write(obj, fullDiskPath, type, serializer)) return;
                //}

                #region PersistenceOperation

                var operation = PersistenceOperation.Serialize<T>(fsReference, obj, replaceMode
                //    , o =>
                //{
                //    o.AutoAppendExtension = PersistenceOptions.AutoAppendExtension;
                //}
                );


                #endregion

                #region Get Serialization Strategies

                var serializationProvider = (context?.SerializationProvider ?? this.SerializationProvider)
                    ?? throw new HasUnresolvedDependenciesException(typeof(ISerializationProvider).FullName);

                var strategyResults = serializationProvider.ResolveStrategies(operation, context);

                #endregion

                List<KeyValuePair<ISerializationStrategy, SerializationResult>>? failedSerializationResults = null;
                SerializationResult? successfulSerializationResult = null;

                //AutoAppendExtension autoAppendExtension = operation.Value.AutoAppendExtension ?? AutoAppendExtension.Disabled;
                AutoAppendExtension autoAppendExtension = PersistenceOptions.AutoAppendExtension;
                bool fileHasAnyExtension;

                switch (autoAppendExtension)
                {
                    case AutoAppendExtension.Disabled:
                    default:
                        fileHasAnyExtension = false; // Value ignored later
                        break;
                    case AutoAppendExtension.IfNoExtensions:
                    case AutoAppendExtension.IfIncorrectExtension:
                        fileHasAnyExtension = LionPath.GetFileName(fsPath).Contains(".");
                        break;
                }

                foreach (var strategyResult in strategyResults)
                {
                    var strategy = strategyResult.Strategy;

                    #region effectiveFSPath (add extension)

                    string effectiveFSPath = fsPath;

                    switch (autoAppendExtension)
                    {
                        case AutoAppendExtension.Disabled:
                            break;
                        case AutoAppendExtension.IfNoExtensions:
                            if (!fileHasAnyExtension)
                            {
                                effectiveFSPath += "." + strategy.DefaultFormat.DefaultFileExtension;
                                effectiveReference = PathToReference(effectiveFSPath);
                            }
                            break;
                        case AutoAppendExtension.IfIncorrectExtension:
                            var fsExtension = LionPath.GetExtension(fsReference.Path);
                            if (!fileHasAnyExtension || fsExtension != strategy.DefaultFormat.DefaultFileExtension)
                            {
                                effectiveFSPath += "." + strategy.DefaultFormat.DefaultFileExtension;
                                effectiveReference = PathToReference(effectiveFSPath);
                            }
                            break;
                        default:
                            throw new ArgumentException();
                    }

#if MONO
            fullDiskPath = fullDiskPath.Replace('\\', '/');
#else
                    effectiveFSPath = effectiveFSPath.Replace('/', '\\'); // REVIEW
#endif

                    #endregion

                    #region SerializeToOutput

                    var serializationResult = await SerializeToOutput(obj, effectiveFSPath, strategy, replaceMode, operation, context);

                    if (!serializationResult.IsSuccess)
                    {
                        if (failedSerializationResults == null) failedSerializationResults = new List<KeyValuePair<ISerializationStrategy, SerializationResult>>();
                        failedSerializationResults.Add(new KeyValuePair<ISerializationStrategy, SerializationResult>(strategyResult.Strategy, serializationResult));
                        continue;
                    }

                    successfulSerializationResult = serializationResult;

#if RecentSaves
                    RecentSaves.AddOrUpdate(fullDiskPath, DateTime.UtcNow, (x, y) => DateTime.UtcNow);
#endif

                    break; // Break on first success

                    #endregion

                }

                if (successfulSerializationResult == null)
                {
                    if (failedSerializationResults != null)
                    {
                        throw new SerializationException(SerializationOperationType.ToStream, operation.Value, context, failedSerializationResults);
                    }
                    if (PersistenceOptions.ThrowOnMissingSerializer)
                    {
                        throw new SerializationException(SerializationOperationType.ToStream, operation.Value, context, failedSerializationResults, noSerializerAvailable: true);
                    }
                    else
                    {
                        return PersistenceResultFlags.SerializerNotAvailable.ToResult(); // TODO: return failed serialization results?
                    }
                }

                return (PersistenceResultFlags.Success | (allowOverwrite ? PersistenceResultFlags.NotFound : PersistenceResultFlags.None)).ToResult();
            });
        }



        #region RecentSaves
#if RecentSaves
        // REVIEW - what is the point of this?
        // TODO: Move this to some sort of monitoring service

        public ConcurrentDictionary<string, DateTime> RecentSaves => recentSaves;
        private readonly ConcurrentDictionary<string, DateTime> recentSaves = new ConcurrentDictionary<string, DateTime>();

        public void CleanRecentSaves()
        {
            foreach (var kvp in RecentSaves)
            {
                if (DateTime.UtcNow - kvp.Value > TimeSpan.FromMinutes(3))
                {
                    RecentSaves.TryRemove(kvp.Key, out DateTime dt);
                }
            }
        }
#endif
        #endregion


        public Task<IPersistenceResult> Upsert<TValue>(string diskPath, TValue value, Type? type = null, PersistenceContext? context = null)
            => Write(diskPath, value, ReplaceMode.Upsert, context: context, type: type);

        public Task<IPersistenceResult> Update<TValue>(string diskPath, TValue value, Type? type = null, PersistenceContext? context = null)
            => Write(diskPath, value, ReplaceMode.Update, context: context, type: type);

        // TODO: overwrite modes, etc.

        //        public static async Task Set(object obj, string objectPath, Type type = null, bool allowOverwrite = true)
        //        {
        //            try
        //            {
        //#if TRACE_SAVE
        //                l.Debug("[FS SAVE] " + objectPath);
        //#endif
        //                //string objectDiskPath = GetSavePathWithoutExtension(obj, objectPath, type); // (No extension)
        //                string objectDiskPath = objectPath; // GetSavePathWithoutExtension(obj, objectPath, type); // (No extension)

        //                await Write(obj, objectDiskPath, type, allowOverwrite);
        //            }
        //            catch (Exception ex)
        //            {
        //                l.Error("Saving '" + objectPath + "' threw exception: " + ex.ToString());
        //                throw;
        //            }
        //        }

        #endregion

        #region Update

        public async Task<IPersistenceResult> Update<TValue>(IReferencable<TReference> referencable, TValue value)
            => await Write(referencable.Reference.Path, value, ReplaceMode.Update).ConfigureAwait(false);

        public async Task<IPersistenceResult> Upsert<TValue>(IReferencable<TReference> referencable, TValue value)
            => await Write(referencable.Reference.Path, value, ReplaceMode.Upsert).ConfigureAwait(false);

        #endregion

        #region Delete

        #region Delete specific Type

        // REVIEW: Should this be moved to an outside mechanism?

        public virtual Task<IPersistenceResult> Delete<T>(IReferencable<TReference> referencable) // Unwrap IReferencable
            => (PersistenceOptions.VerifyExistsAsTypeBeforeDelete
                ? VerifyExistsAsTypeAndDelete<T>(referencable.Reference.Path)
                : Delete(referencable.Reference.Path).DeleteResultToPersistenceResult());


        public virtual Task<IPersistenceResult> Delete<T>(TReference reference) // Unwrap IReference, (optional Verify)
            => (PersistenceOptions.VerifyExistsAsTypeBeforeDelete
                ? VerifyExistsAsTypeAndDelete<T>(reference.Path)
                : Delete(reference.Path).DeleteResultToPersistenceResult());

        public async virtual Task<IPersistenceResult> VerifyExistsAsTypeAndDelete<TValue>(string fsPath)
        {
            if (await Exists<TValue>(fsPath).ConfigureAwait(false))
            {
                var deleteResult = await Delete(fsPath).ConfigureAwait(false);
                return PersistenceResult.SuccessAndFound;
            }
            else
            {
                return PersistenceResult.SuccessNotFound;
            }
        }

        #endregion

        public virtual Task<IPersistenceResult> Delete(IReferencable<TReference> referencable) // Unwrap IReferencable
            => (PersistenceOptions.VerifyExistsBeforeDelete
                ? VerifyExistsAndDelete(referencable.Reference.Path)
                : Delete(referencable.Reference.Path).DeleteResultToPersistenceResult());

        public virtual Task<IPersistenceResult> Delete(TReference reference) // Unwrap IReference
            => (PersistenceOptions.VerifyExistsBeforeDelete
                ? VerifyExistsAndDelete(reference.Path)
                : Delete(reference.Path).DeleteResultToPersistenceResult());

        public async virtual Task<IPersistenceResult> VerifyExistsAndDelete(string fsPath) // No type checking
        {
            var existsResult = await Exists(fsPath);
            if (existsResult)
            {
                var deleteResult = await Delete(fsPath).ConfigureAwait(false);
                var flags = PersistenceResultFlags.Found;
                if (deleteResult == true) flags |= PersistenceResultFlags.Success;
                if (deleteResult == false) flags |= PersistenceResultFlags.Fail;
                return new PersistenceResult { Flags = flags };
            }
            return PersistenceResult.SuccessNotFound;
        }

        #endregion
    }

    // REVIEW - is there a way to do this?
    //public static class PersisterBaseExtensions
    //{
    //    public static Task<IRetrieveResult<TValue>> Retrieve<TValue, TReference>(this IPersister<TReference> persister, TReference reference)
    //        => persister.Retrieve<TValue>((TReference)reference.Path);
    //}
}
