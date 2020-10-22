#nullable enable
using LionFire.Dependencies;
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
using System.Reflection;
using MorseCode.ITask;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using LionFire.Exceptions;
using LionFire.Ontology;

namespace LionFire.Persistence.Filesystemlike
{


    /// <summary>
    /// IPersister return IPersistenceResult and IFSPersistence returns simpler results closer to the underlying filesystem (or similar store)
    /// </summary>
    /// <typeparam name="TReference"></typeparam>
    /// <typeparam name="TPersistenceOptions"></typeparam>
    public abstract partial class FilesystemlikePersistence<TReference, TPersistenceOptions> : SerializingPersisterBase<TPersistenceOptions>
    , IPersister<TReference>
    , IWriter<string>
    , IReader<string>

    {
        public abstract IOCapabilities Capabilities { get; }

        public abstract TReference PathToReference(string fsPath);

        public IPersistenceConventions ItemKindIdentifier { get; }

        readonly IOptionsMonitor<TPersistenceOptions> optionsMonitor;

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

        public FilesystemlikePersistence(string name, ISerializationProvider serializationProvider, IOptionsMonitor<TPersistenceOptions> options, IPersistenceConventions itemKindIdentifier, SerializationOptions serializationOptions) : base(serializationOptions)
        {
            this.SerializationProvider = serializationProvider; // MOVE to base ctor, maybe others as well
            //this.PersistenceOptions = persistenceOptions;
            this.PersistenceOptions = string.IsNullOrEmpty(name) ? options.CurrentValue : options.Get(name);
            this.optionsMonitor = options;
            ItemKindIdentifier = itemKindIdentifier;
        }

        #endregion

        #region List

        public Task<IRetrieveResult<IEnumerable<Listing<object>>>> List(IReferencable<TReference> referencable, ListFilter? filter = null)
            => List<object>(referencable.Reference, filter);
        public Task<IRetrieveResult<IEnumerable<Listing<T>>>> List<T>(IReferencable<TReference> referencable, ListFilter? filter = null)
                    => List<T>(referencable.Reference, filter);

        private object List(Type type, TReference reference, ListFilter? filter = null)
        {
            return listMethodInfo.MakeGenericMethod(type).Invoke(this, new object?[] { reference, filter });
        }
        private static MethodInfo listMethodInfo = typeof(FilesystemlikePersistence<,>).MakeGenericType(typeof(TReference), typeof(TPersistenceOptions)).GetMethods().Where(m =>
        {
            if (m.Name != "List") return false;
            var p = m.GetParameters();
            if (p.Length != 2) return false;
            return p[0].ParameterType == typeof(TReference) && p[1].ParameterType == typeof(ListFilter);
        }).FirstOrDefault();

        public async Task<IRetrieveResult<IEnumerable<Listing<T>>>> List<T>(TReference reference, ListFilter? filter = null)
        {
            var listResult = await List<T>(reference.Path, filter);
            return listResult == null
                ? RetrieveResult<IEnumerable<Listing<T>>>.SuccessNotFound
                : (IRetrieveResult<IEnumerable<Listing<T>>>)RetrieveResult<IEnumerable<Listing<T>>>.Found(listResult);
        }

        public Task<IEnumerable<Listing<object>>?> List(string path, ListFilter? filter = null) => List<object>(path, filter);

        /// <summary>
        /// Returns null if directory not found
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Listing<T>>?> List<T>(string path, ListFilter? filter = null)
        {
            return await Task.Run(() =>
            {
                List<Listing<T>>? children = null;
                if (Directory.Exists(path))
                {
                    children = new List<Listing<T>>();

                    bool showFile = filter == null || filter.Flags == ItemFlags.None || filter.Flags.HasFlag(ItemFlags.File) || !filter.Flags.HasFlag(ItemFlags.Directory);
                    bool showDir = filter == null || filter.Flags == ItemFlags.None || filter.Flags.HasFlag(ItemFlags.Directory) || !filter.Flags.HasFlag(ItemFlags.File);
                    //bool showHidden = filter != null && filter.Flags.HasFlag(ItemFlags.Hidden);
                    //bool showMeta = filter != null && filter.Flags.HasFlag(ItemFlags.Meta);
                    //bool showSpecial = filter != null && filter.Flags.HasFlag(ItemFlags.Special);

                    if (showFile)
                    {
                        foreach (var fileName in Directory.GetFiles(path).Select(p => Path.GetFileName(p)))
                        {
                            var kind = ItemKindIdentifier.ResolveItemFlags(fileName);

                            //if (kind.HasFlag(ItemFlags.File) && !showFile) continue;
                            //if (kind.HasFlag(ItemFlags.Hidden) && !showHidden) continue;
                            //if (kind.HasFlag(ItemFlags.Special) && !showSpecial) continue;
                            //if (kind.HasFlag(ItemFlags.Meta) && !showMeta) continue;

                            children.Add(fileName);
                        }
                    }
                    if (showDir)
                    {
                        foreach (var dirName in Directory.GetDirectories(path).Select(p => Path.GetFileName(p) + LionPath.Separator))
                        {
                            //var kind = ItemKindIdentifier.Identify(dir);

                            //if (kind.HasFlag(ItemFlags.File) && !showFile) continue;
                            //if (kind.HasFlag(ItemFlags.Hidden) && !showHidden) continue;
                            //if (kind.HasFlag(ItemFlags.Special) && !showSpecial) continue;
                            //if (kind.HasFlag(ItemFlags.Meta) && !showMeta) continue;

                            children.Add(new Listing<T>(dirName, directory: true));
                        }
                    }
                }
                return children;
            }).ConfigureAwait(false);
        }
        #endregion

        #region Read: Deserialize

        /// <summary>
        /// Default implementation uses DeserializeFromStream.  If you don't want to use Streams, override and use string or byte[] serialization approaches.
        /// </summary>
        /// <param name="fsPath"></param>
        /// <returns></returns>
        public virtual Task<DeserializationResult<T>> Deserialize<T>(string fsPath, ISerializationStrategy strategy, PersistenceOperation operation, PersistenceContext context)
        {
            bool stream = PersistenceOptions.PreferStreamReading == true && Capabilities.HasFlag(IOCapabilities.ReadStream) && strategy.ImplementsFromStream;
            if (stream)
            {
                return DeserializeFromStream<T>(fsPath, strategy, operation, context);
            }
            else
            {
                if (strategy.ImplementsFromBytes)
                {
                    return DeserializeFromBytes<T>(fsPath, strategy, operation, context); // Will fall back to String if it is a textual serializer
                }
                else
                {
                    return DeserializeFromString<T>(fsPath, strategy, operation, context); // Will fall back to Stream if it is a stream-only serializer
                }
            }
        }

        public virtual async Task<DeserializationResult<T>> DeserializeFromStream<T>(string fsPath, ISerializationStrategy strategy, PersistenceOperation operation, PersistenceContext context)
        {
            using var fs = await ReadStream(fsPath).ConfigureAwait(false);
            return await Task.Run(() => strategy.ToObject<T>(fs, operation, context)).ConfigureAwait(false);
        }

        public virtual async Task<DeserializationResult<T>> DeserializeFromBytes<T>(string fsPath, ISerializationStrategy strategy, PersistenceOperation operation, PersistenceContext context)
            => strategy.ToObject<T>(await ReadBytes(fsPath).ConfigureAwait(false), operation, context);
        public virtual async Task<DeserializationResult<T>> DeserializeFromString<T>(string fsPath, ISerializationStrategy strategy, PersistenceOperation operation, PersistenceContext context)
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

        // REVIEW - Yikes this is ugly.  OPTIMIZE?  Use non-generic methods?
        private async Task<IRetrieveResult<TValue>> RetrieveMetadata<TValue, TMetadata>(TReference reference)
        {
            var metadataType = typeof(TValue).GetGenericArguments()[0];

            if (metadataType.IsGenericType && metadataType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var enumerableType = metadataType.GetGenericArguments()[0];

                var listingGenericType = enumerableType.GetGenericTypeDefinition();
                if (enumerableType.IsGenericType && listingGenericType == typeof(Listing<>))
                {
                    var listingType = enumerableType.GetGenericArguments()[0];
                    var resultEnumerableType = typeof(IEnumerable<>).MakeGenericType(typeof(Listing<>).MakeGenericType(listingType));
                    //var resultEnumerableTaskType = typeof(Task<>).MakeGenericType(resultEnumerableType);

                    var listResult = await ((Task<IRetrieveResult<TMetadata>>)List(listingType, reference)).ConfigureAwait(false);

                    //var resultType = typeof(Metadata<>).MakeGenericType(typeof(IEnumerable<>).MakeGenericType(typeof(Listing<>).MakeGenericType(listingType)));

                    //return (IRetrieveResult<TValue>) await List(listResult, reference, filter).ConfigureAwait(false);

                    return (IRetrieveResult<TValue>)Activator.CreateInstance(typeof(RetrieveResult<>).MakeGenericType(typeof(TValue)),
                        Activator.CreateInstance(typeof(TValue), listResult.Value),
                        listResult.Flags,
                        listResult.Error);

                    //return (IRetrieveResult<TValue>)(object)new RetrieveResult<Metadata<IEnumerable<Listing<>>>>(new Metadata<IEnumerable<Listing<TValue>>>(listResult.Value), listResult.Flags) // HARDCAST
                    //{
                    //    Error = listResult.Error,
                    //};
                }
            }

            throw new NotSupportedException($"Don't know how to retrieve Metadata of type '{metadataType}'");
        }
        private static MethodInfo RetrieveMetadataMethodInfo = typeof(FilesystemlikePersistence<,>).MakeGenericType(typeof(TReference), typeof(TPersistenceOptions)).GetMethod("RetrieveMetadata", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <remarks>
        /// Default implementation: pass-through to ReadAndDeserializeExactPath, with AutoRetry
        /// </remarks>
        public virtual async Task<IRetrieveResult<TValue>> Retrieve<TValue>(TReference reference)
        {
            if (typeof(TValue).IsGenericType && typeof(TValue).GetGenericTypeDefinition() == typeof(Metadata<>))
            {
                return await ((Task<IRetrieveResult<TValue>>)RetrieveMetadataMethodInfo.MakeGenericMethod(typeof(TValue), typeof(TValue).GetGenericArguments()[0]).Invoke(this, new object[] { reference })).ConfigureAwait(false);
                //return await RetrieveMetadata<TValue>(reference).ConfigureAwait(false);
            }

            var path = reference.Path;
            var persister = (reference as IPersisterReference)?.Persister;
            if (!string.IsNullOrEmpty(persister))
            {
                var providerOptions = optionsMonitor.Get(persister);
                if (providerOptions.RootDirectory == null) throw new UnknownPersisterException($"Provider '{persister}' is not known."); // REVIEW - better way to determine uninitialized/missing?
                path = LionPath.Combine(providerOptions.RootDirectory, path);
            }

            var operation = PersistenceOperationFactory.Deserialize<TValue>(reference, SerializationOptions
    //    , o =>
    //{
    //    o.AutoAppendExtension = PersistenceOptions.AutoAppendExtension;
    //}
    );

            //((Func<PersistenceOperation>)(() =>
            //       new PersistenceOperation(reference)
            //       {
            //           Direction = IODirection.Read,
            //       })).ToLazy()

            var retryOptions = this.RetryOptions;

            if (retryOptions != null)
            {
                return await new Func<Task<IRetrieveResult<TValue>>>(()
                    => ReadAndDeserializeAutoPath<TValue>(path, operation))
                    .AutoRetry(maxRetries: retryOptions.MaxGetRetries,
                    millisecondsBetweenAttempts: retryOptions.MillisecondsBetweenGetRetries, allowException: AllowAutoRetryForThisException).ConfigureAwait(false);
            }
            else
            {
                return await ReadAndDeserializeAutoPath<TValue>(path, operation).ConfigureAwait(false);
            }
        }

        PersisterRetryOptions? RetryOptions => (PersistenceOptions as IHas<PersisterRetryOptions>)?.Object;

        //public virtual Task<IRetrieveResult<T>> ReadAndDeserializeExactPath<T>(TReference reference, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        //    => ReadAndDeserializeExactPath<T>(reference.Path, operation, context);

        // RENAME to eliminate "ExactPath" since it's no longer needed to distinguish in this class
        public virtual async Task<IRetrieveResult<T>> ReadAndDeserializeAutoPath<T>(string path, Lazy<PersistenceOperation> operation, PersistenceContext? context = null, bool throwOnFail = true)
        {
            // TOTEST:
            // - paths ending in /
            // - paths ending in .
            // - paths starting with .
            // - paths with multiple .'s

            // NEXT: how to invoke these?  Latest idea: persist/depersist pipelines
            //var obj = await persistenceOperation.ToObject<T>(effectiveContext).ConfigureAwait(false);
            //var obj = await (persistenceOperation.Context?.SerializationProvider ?? DependencyLocator.TryGet<ISerializationProvider>()).ToObject<T>(op);

            List<KeyValuePair<ISerializationStrategy, SerializationResult>>? failures = null;

            PersistenceResultFlags flags = PersistenceResultFlags.None;

            var autoAppendExtension = PersistenceOptions.AppendExtensionOnRead;
            //bool fileHasAnyExtension;
            string? fileExtension = null;
            //var potentialFiles = new List<string>();
            IEnumerable<string>? potentialExtensions = null;

            var dir = Path.GetDirectoryName(path) ?? Path.GetPathRoot(path);

            IEnumerable<string> GetPotentialExtensions(string p)
            {
                if (!Directory.Exists(dir)) { return Array.Empty<string>(); }
                IEnumerable<string> result = System.IO.Directory.GetFiles(dir, $"{Path.GetFileName(p)}.*")
                    .Select(p =>
                    Path.GetExtension(p)
                    .Substring(1) // trim the leading .
                    );

                if (File.Exists(path))
                {
                    result = result.Concat(new string[] { "" });
                }
                return result;
            }

            switch (autoAppendExtension)
            {
                case AppendExtensionOnRead.Never:
                default:
                    //fileHasAnyExtension = false; // Value ignored later
                    potentialExtensions = File.Exists(path) ? new string[] { "" } : Enumerable.Empty<string>();
                    break;
                case AppendExtensionOnRead.IfNoExtensions:
                    fileExtension = LionPath.GetExtension(path);
                    if (fileExtension == null)
                    {
                        potentialExtensions = GetPotentialExtensions(path);
                    }
                    if (File.Exists(path))
                    {
                        potentialExtensions = potentialExtensions == null ? new string[] { "" } : potentialExtensions.Concat(new string[] { "" });
                    }
                    break;
                case AppendExtensionOnRead.Always:
                    potentialExtensions = GetPotentialExtensions(path);
                    // Doesn't check for File.Exists(path)
                    break;
            }

            if (!potentialExtensions.Any())
            {
                l.Trace($"NotFound: {path}[.*]");
                return RetrieveResult<T>.NotFound;
            }

            bool foundOne = false;
            //TReference effectiveReference;

            if (potentialExtensions.Count() >= 2 && PersistenceOptions.ValidateOneFilePerPath.HasFlag(ValidateOneFilePerPath.OnRead))
            {
                throw new PersistenceException($"ValidateOneFilePerPath has flag OnRead and there is more than one possible file extension found for path '{path}'");
            }

            operation.Value.PotentialExtensions = potentialExtensions;

            foreach (var attempt in SerializationProvider.ResolveStrategies(operation, context, IODirection.Read))
            {
                var strategy = attempt.Strategy;
                string effectiveFSPath = string.IsNullOrEmpty(attempt.ScoringAttempt.Extension) ? path : $"{path}.{attempt.ScoringAttempt.Extension}";

                //switch (autoAppendExtension)
                //{
                //    case AutoAppendExtension.Disabled:
                //        break;
                //    case AutoAppendExtension.IfNoExtensions:
                //        if (!fileHasAnyExtension)
                //        {
                //            effectiveFSPath += "." + strategy.DefaultFormat.DefaultFileExtension;
                //            effectiveReference = PathToReference(effectiveFSPath);
                //        }
                //        break;
                //    case AutoAppendExtension.IfIncorrectExtension:
                //        var fsExtension = LionPath.GetExtension(fsReference.Path);
                //        if (!fileHasAnyExtension || fsExtension != strategy.DefaultFormat.DefaultFileExtension)
                //        {
                //            effectiveFSPath += "." + strategy.DefaultFormat.DefaultFileExtension;
                //            effectiveReference = PathToReference(effectiveFSPath);
                //        }
                //        break;
                //    default:
                //        throw new ArgumentException(nameof(PersistenceOptions.AutoAppendExtensionOnWrite));
                //}

                byte[]? bytes;

                //if (!await Exists(effectiveFSPath).ConfigureAwait(false)) continue;

                //try
                //{
                bytes = await ReadBytes(effectiveFSPath).ConfigureAwait(false);
                flags |= PersistenceResultFlags.Found; // Found something.  REVIEW: what if this is some unrelated metadata or OOB file?

                //}
                //catch (FileNotFoundException)
                //{
                //    bytes = null;
                //}

                //if (bytes == null)
                //{
                //    flags |= PersistenceResultFlags.NotFound | PersistenceResultFlags.Fail;
                //}
                //else
                {
                    foundOne = true;

                    DeserializationResult<T> result;
                    try
                    {
                        result = strategy.ToObject<T>(bytes, operation, context);
                    }
                    catch (Exception ex)
                    {
                        throw new PermanentException(ex);
                    }

                    if (result.IsSuccess)
                    {
                        flags |= PersistenceResultFlags.Success;
                        OnDeserialized(result.Object);
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
            }
            if (!foundOne) flags |= PersistenceResultFlags.SerializerNotAvailable;
            flags |= PersistenceResultFlags.Fail;

            if (throwOnFail && PersistenceOptions.ThrowOnDeserializationFailure)
            {
                throw new SerializationException(SerializationOperationType.FromBytes, operation, context, failures, noSerializerAvailable: !foundOne);
            }


#nullable disable
            return new RetrieveResult<T>(default, flags)
            {
                Error = failures,
            };
#nullable enable
        }

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

        public Task<IPersistenceResult> Write<TValue>(string fsPath, TValue obj, ReplaceMode replaceMode = ReplaceMode.Upsert, PersistenceContext? context = null, Type? type = null)
            => Write(PathToReference(fsPath), obj, replaceMode, context, type);

        public Task<IPersistenceResult> Write<TValue>(TReference fsReference, TValue obj, ReplaceMode replaceMode = ReplaceMode.Upsert, PersistenceContext? context = null, Type? type = null)
        {
            TReference effectiveReference;
            bool allowOverwrite = replaceMode.HasFlag(ReplaceMode.Update);
            bool requireOverwrite = !replaceMode.HasFlag(ReplaceMode.Create);

            l.Trace($"{replaceMode.DescriptionString()} {obj?.GetType().Name} {replaceMode.ToArrow()} {fsReference}");

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

                var operation = PersistenceOperationFactory.Serialize<TValue>(fsReference, obj, replaceMode, SerializationOptions
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
                AutoAppendExtension autoAppendExtension = PersistenceOptions.AutoAppendExtensionOnWrite;
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

                int retries = 8; // MOVE config, and think about whether retry mechanism belongs here or up or down a level
                var msDelay = 500;

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
                            throw new ArgumentException(nameof(PersistenceOptions.AutoAppendExtensionOnWrite));
                    }

#if MONO
            fullDiskPath = fullDiskPath.Replace('\\', '/');
#else
                    effectiveFSPath = effectiveFSPath.Replace('/', '\\'); // REVIEW
#endif

                    #endregion

                    #region SerializeToOutput

                    SerializationResult? serializationResult;
                retry:
                    try
                    {
                        serializationResult = await SerializeToOutput(obj, effectiveFSPath, strategy, replaceMode, operation, context).ConfigureAwait(false);
                    }
                    catch (IOException ex) when (ex.Message.Contains("used by another process"))
                    {
                        serializationResult = SerializationResult.SharingViolation;
                        if (retries-- > 0)
                        {
                            l.Warn($"Sharing violation when writing to {effectiveFSPath}.  Trying again in {msDelay}ms.");
                            await Task.Delay(msDelay);
                            goto retry;
                        }
                    }
                    catch (Exception ex)
                    {
                        serializationResult = SerializationResult.FromException(ex);
                    }

                    if (!serializationResult.IsSuccess)
                    {
                        l.TraceWarn($"{this.GetType().Name} - Write attempt to '{fsReference}' failed: " + serializationResult);

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


        private static ILogger l = Log.Get();

    }
    // REVIEW What should the flow be for Persistence.Retrieve, and Persistence.Write?  Create an op, and then iterate over Serializers until it succeeds? Do Retrieve/Write just create an op, and then the op can run on its own?  Is it like a blackboard object with its own configurable pipeline?  Is there a hidden IPersisterOperationsProvider interface for CreateRetrieveOperation and CreateWriteOperation?

    // REVIEW - is there a way to do this?
    //public static class PersisterBaseExtensions
    //{
    //    public static Task<IRetrieveResult<TValue>> Retrieve<TValue, TReference>(this IPersister<TReference> persister, TReference reference)
    //        => persister.Retrieve<TValue>((TReference)reference.Path);
    //}

    #region ENH - RecentSaves
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

    #region Scraps

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

    #endregion
}
