﻿// OLD - incorporate into AEFP
#if OLD
using LionFire.IO;
using LionFire.Persistence.Filesystem;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.Serialization;
using System.IO;
using LionFire.Execution;
using System.Linq;
using Microsoft.Extensions.Options;
using LionFire.Persistence.Persisters;

namespace LionFire.Persistence.AutoExtensionFilesystem
{

    public class AutoExtensionFilesystemOverlayPersister
    : OverlayPersister<AutoExtensionFileReference, AutoExtensionFilesystemPersisterOptions, FilesystemPersister>
    {

        #region Construction

        public AutoExtensionFilesystemOverlayPersister(string name, ISerializationProvider serializationProvider, IOptionsMonitor<AutoExtensionFilesystemPersisterOptions> options, IPersistenceConventions itemKindIdentifier) : base(name, serializationProvider, options, itemKindIdentifier)
        {
        }

        #endregion

        #region List

        public List<string> List(string path)
        {
            throw new NotImplementedException("Implement based on FilesystemPersister");
            //var children = new List<string>();

            //if (Directory.Exists(path))
            //{
            //    children.AddRange(Directory.GetFiles(path).Select(GetNameFromFileName)); 
            //    children.TryAddRange(Directory.GetDirectories(path).Select(GetNameFromFileName));
            //}
            //return children;
        }

        #endregion
        #region Retrieve

        /// <remarks>
        /// Override implementation: use default paths.  Auto-retry.
        /// </remarks>
        public async Task<IGetResult<TValue>> Retrieve<TValue>(AutoExtensionFileReference reference)
        {
            return await new Func<Task<IGetResult<TValue>>>(()
                => RetrieveUsingCandidatePaths<TValue>(reference))
                      .AutoRetry(maxRetries: PersistenceOptions.MaxGetRetries,
                          millisecondsBetweenAttempts: PersistenceOptions.MillisecondsBetweenGetRetries).ConfigureAwait(false);
        }


        public virtual async Task<IGetResult<TValue>> RetrieveUsingCandidatePaths<TValue>(AutoExtensionFileReference reference, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            var type = reference.ReferenceType() ?? typeof(TValue);
            var fsPath = reference.Path;
            //var fileName = Path.GetFileName(fsPath);

            return await DoPersistenceForCandidatePaths<TValue, IGetResult<TValue>>(reference, async candidatePath =>
            {
                //if (!await Exists(reference.Path).ConfigureAwait(false)) return null; // null means NotFound

                return await ReadAndDeserializeExactPath<TValue>(candidatePath, throwOnFail: false).ConfigureAwait(false);

                //// REVIEW TODO: 
                //// - What should be done if a path is found but deserialization fails?  This could perhaps happen if two types are saved with the same filename but different extensions.
                //// - I think we should fail here, and do more sophisticated things in Vos if desired, perhaps with more metadata in filenames.

                //List<KeyValuePair<ISerializationStrategy, SerializationResult>> serializationFailuresRollup = null;

                //var result = 

                //if (result.Flags.HasFlag(TransferResultFlags.Success))
                //{
                //    if (result.Flags.HasFlag(TransferResultFlags.Found))
                //    {
                //        return result; // Success
                //    }
                //}

                //if (result.Error is List<KeyValuePair<ISerializationStrategy, SerializationResult>> serializationFailures)
                //{
                //    serializationFailuresRollup.AddRange(serializationFailures);
                //}

                //return new RetrieveResult<TValue> { Flags = TransferResultFlags.Fail | TransferResultFlags.Found, Error = serializationFailuresRollup };

            }, RetrieveResult<TValue>.SuccessNotFound);


            var existingCandidatePaths = new List<string>();

            await foreach (var candidatePath in CandidateReadPaths(fsPath))
            {
                if (await Exists(reference.Path).ConfigureAwait(false)) existingCandidatePaths.Add(candidatePath);
            }
            if (!existingCandidatePaths.Any()) return new RetrieveResult<TValue> { Flags = TransferResultFlags.NotFound | TransferResultFlags.Fail };

            //var effectiveContext = FSDeserializingPersistenceContext;

            var persistenceOperation = new PersistenceOperation()
            {
                Reference = reference,
                Type = type,
                Deserialization = new DeserializePersistenceOperation()
                {
                    #region ENH - optional alternative: combine dir and filenames to get candidatepaths
                    //Directory = dir,
                    //CandidateFilemes = 
                    #endregion
                    CandidatePaths = existingCandidatePaths,
                },
                Context = context,
            };

            #region Context

            if (context != null)
            {
                throw new NotImplementedException($"{nameof(context)} not implemented yet");
            }

            #endregion

            List<KeyValuePair<ISerializationStrategy, SerializationResult>> serializationFailuresRollup = null;

            foreach (var candidatePath in existingCandidatePaths)
            {
                var result = await RetrieveAndDeserializeExactPath<TValue>(candidatePath, operation, context, throwOnFail: false).ConfigureAwait(false);
                if (result.IsSuccess())
                {
                    return result;
                }
                if (result.Error is List<KeyValuePair<ISerializationStrategy, SerializationResult>> serializationFailures)
                {
                    serializationFailuresRollup.AddRange(serializationFailures);
                }
            }
            return new RetrieveResult<TValue> { Flags = TransferResultFlags.Fail, Error = serializationFailuresRollup };
        }


        #endregion

        #region Path to AutoExtension Path Utilities

        public virtual async IAsyncEnumerable<string> CandidateReadPaths(string fsPath)
        {
            HashSet<string> results = null;

            foreach (var selectionResult in PersistenceOptions.SerializationProvider.ResolveStrategies())
            {
                foreach (var extension in selectionResult.Strategy.SupportedExtensions(IODirection.Read))
                {
                    var path = fsPath + "." + extension;
                    if (results?.Contains(path) == true) continue;
                    if (await (Exists(path)))
                    {
                        yield return extension;
                        if (results == null) results = new HashSet<string>();
                        results.Add(path);
                    }
                }
            }
        }

        /// <typeparam name="TValue"></typeparam>
        /// <param name="reference"></param>
        /// <param name="func">Can return null if file not found</param>
        /// <param name="onNoSuccess">If null, will be set to PersistenceResult.FailAndNotFound</param>
        /// <returns></returns>
        public async Task<TResult> DoPersistenceForCandidatePaths<TValue, TResult>(FileReference reference,
            Func<string, Task<TResult>> func,
            TResult onNoSuccess)
            where TResult : ITransferResult
        {
            //if (onNoSuccess == null) onNoSuccess = PersistenceResult.FailAndNotFound;

            var fsPath = reference.Path;
            await foreach (var candidatePath in CandidateReadPaths(fsPath))
            {
                var result = await func(candidatePath).ConfigureAwait(false);
                if (result != null && (result.IsFound() == true)) // Return the first found result, even if not Success (REVIEW)
                {
                    return result;
                }
            }

            return onNoSuccess;
        }

        #endregion


        #region Delete

        //public override Task<ITransferResult> Delete(TReference reference) // No type checking
            //=> DoPersistenceForCandidatePaths<object>(reference, fsPath => DeleteExactPath(fsPath));
        //{
        //    //return DoPersistenceForCandidatePaths(PersistenceOptions.VerifyExistsAsTypeBeforeDelete ? VerifyExistsAsTypeAndDeleteExactPath<T>(reference) : DeleteExactPath(reference));
        //    return 

        //    //await .ConfigureAwait(false);

        //    //var fsPath = reference.Path;
        //    //await foreach (var candidatePath in CandidateReadPaths(fsPath))
        //    //{
        //    //    var result = await(PersistenceOptions.VerifyExistsAsTypeBeforeDelete ? VerifyExistsAsTypeAndDeleteExactPath<T>(fsPath) : DeleteExactPath(fsPath)).ConfigureAwait(false);
        //    //    if (result.IsSuccess())
        //    //    {
        //    //        return result;
        //    //    }
        //    //}
        //    //return PersistenceResult.FailAndNotFound;
        //}

        //public override Task<ITransferResult> Delete<TValue>(TReference reference)
        //{
        //    return DoPersistenceForCandidatePaths<TValue>(reference,
        //        PersistenceOptions.VerifyExistsAsTypeBeforeDelete
        //        ? (VerifyExistsAsTypeAndDelete<TValue>)
        //        : (Func<string, Task<ITransferResult>>)DeleteExactPath);
        //}

        //        {
        //            var fsPath = reference.Path;
        //        await foreach (var candidatePath in CandidateReadPaths(fsPath))
        //            {
        //                var result = await(PersistenceOptions.VerifyExistsAsTypeBeforeDelete ? VerifyExistsAsTypeAndDeleteExactPath<T>(fsPath) : DeleteExactPath(fsPath)).ConfigureAwait(false);
        //                if (result.IsSuccess())
        //                {
        //                    return result;
        //                }
        //}

        //            return PersistenceResult.FailAndNotFound;
        //        }

        #endregion


    }
}
#endif