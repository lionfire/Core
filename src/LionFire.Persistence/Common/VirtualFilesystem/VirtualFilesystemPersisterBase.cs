#nullable enable
using LionFire.Dependencies;
using LionFire.IO;
using LionFire.Persistence.FilesystemFacade;
using LionFire.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LionFire.Execution;
using LionFire.Referencing;
using Microsoft.Extensions.Options;
using LionFire.Ontology;
using LionFire.Persistence.Persisters;

namespace LionFire.Persistence.Filesystemlike
{
    // This file: implement IFilesystemlikePersistence<TReference, TPersistenceOptions>
    //  - default implementations for FilesystemFacade data storage

    public abstract partial class VirtualFilesystemPersisterBase<TReference, TPersistenceOptions>
        : IFilesystemlikePersistence<TReference, TPersistenceOptions>
        where TReference : class, IReference
        where TPersistenceOptions : FilesystemlikePersisterOptionsBase, IPersistenceOptions
    {
        #region IO

        public abstract Task<Stream> WriteStream(string reference, ReplaceMode replaceMode = ReplaceMode.Upsert);
        public abstract Task WriteString(string reference, string str, ReplaceMode replaceMode = ReplaceMode.Upsert);
        public abstract Task WriteBytes(string reference, byte[] bytes, ReplaceMode replaceMode = ReplaceMode.Upsert);
        public abstract Task<Stream> ReadStream(string reference);
        public abstract Task<string> ReadString(string reference);
        public abstract Task<byte[]> ReadBytes(string reference);

        #endregion

        #region Create

        public abstract Task CreateDirectory(string fsPath);

        #endregion

        #region Exists

        // Default Exists implementation: try IPersister.Retrieve and return true/false/null based on TransferResultFlags Found / NotFound / neither

        public virtual async Task<bool> Exists<T>(string fsPath)
            => (await Retrieve<T>(PathToReference(fsPath)).ConfigureAwait(false)).Flags.HasFlag(TransferResultFlags.Found);

        public virtual Task<bool> Exists(string fsPath) => Exists<object>(fsPath);

        public abstract Task<bool> DirectoryExists(string fsPath);

        public abstract Task<string[]> GetFiles(string dir, string? pattern = null);

        #endregion

        //protected abstract PersistenceContext DeserializingContext { get; }
        //protected abstract PersistenceContext SerializingContext { get; }

        //public Task<IGetResult<TValue>> TryRetrieve<TValue>(FileReference fileReference, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        //=> TryRetrieve<TValue>(fileReference, operation);
        //public Task<ITransferResult> Update<TValue>(TValue obj, string diskPath, Type type = null, PersistenceContext context = null) => FSPersistenceStatic.Update(obj, diskPath, type, context);
        //public Task<ITransferResult> Upsert<TValue>(TValue obj, string diskPath, Type type = null, PersistenceContext context = null) => FSPersistenceStatic.Upsert(obj, diskPath, type, context);

        #region Delete

        #region CanDelete

        public virtual Task<bool?> CanDelete(string fsPath) => Task.FromResult<bool?>(null);

        public Task<bool?> CanDelete<T>(string fsPath)
            => Task.Run(async () =>
            {
                var exists = await Exists(fsPath).ConfigureAwait(false);

                if (!exists) return false;

                // FUTURE: Check file permissions

                return (bool?)true;
            });

        #endregion

        public abstract Task<bool?> DeleteFile(string reference);
        public abstract Task<bool?> DeleteDirectory(string reference);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fsPath"></param>
        /// <returns>
        /// If file found, delete and return !Exists
        /// If directory found, delete and return !DirectoryExists
        /// False if neither file nor directory found</returns>
        public async virtual Task<bool?> DeleteFileOrDirectory(string fsPath)
        {

            if (await Exists(fsPath).ConfigureAwait(false))
            {
                var result = await DeleteFile(fsPath).ConfigureAwait(false);
                return !await Exists(fsPath).ConfigureAwait(false);
            }

            if (await DirectoryExists(fsPath).ConfigureAwait(false))
            {
                await DeleteDirectory(fsPath).ConfigureAwait(false);
                return !await DirectoryExists(fsPath).ConfigureAwait(false);
            }

            return false;

        }

        /// <returns>True if successfully deleted, false if nothing deleted, null if unknown whether anything was deleted.</returns>
        public Task<bool?> Delete(string fsPath) // Wrap DeleteFile with AutoRetry
        {
            var PersisterRetryOptions = (PersistenceOptions as IHas<PersisterRetryOptions>)?.Object;

            if (PersisterRetryOptions?.MaxDeleteRetries == 0)
            {
                return DeleteFileOrDirectory(fsPath);
            }
            else
            {
                if (PersisterRetryOptions == null)
                {
                    return DeleteFileOrDirectory(fsPath);
                }
                else
                {
                    return ((Func<Task<bool?>>)(() => DeleteFileOrDirectory(fsPath)))
                        .AutoRetry(maxRetries: PersisterRetryOptions.MaxDeleteRetries, millisecondsBetweenAttempts: PersisterRetryOptions.MillisecondsBetweenDeleteRetries);

                }
            }
        }

        #endregion

    }
}
