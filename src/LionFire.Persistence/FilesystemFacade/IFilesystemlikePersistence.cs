//#define TRACE_SAVE
#define TRACE_LOAD

using System;
using System.Threading.Tasks;
using LionFire.IO;
using LionFire.Referencing;
using LionFire.Serialization;

namespace LionFire.Persistence.Filesystemlike
{

    // REVIEW - inherit from IVirtualFilesystem?
    // Why take TReference if API uses string for file path?

    /// <summary>
    /// TODO - Work in progress
    /// </summary>
    public interface IFilesystemlikePersistence<TReference, TPersistenceOptions>
        where TReference : IReference
        where TPersistenceOptions : FilesystemlikePersisterOptionsBase, IPersistenceOptions
    {
        Task<bool?> CanDelete(string fsPath);

        /// <returns>True if it deleted something, False if it didn't, and null if it couldn't tell</returns>
        Task<bool?> Delete(string fsPath);
        Task<bool> Exists(string fsPath);
        Task<bool> Exists<T>(string fsPath);
        //Task<bool> TryDelete<TValue>(string fsPath, bool preview = false);
        //Task<IGetResult<TValue>> TryRetrieve<TValue>(TReference fileReference, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null);
        Task<ITransferResult> Update<T>(string fsPath, T obj, Type type = null, PersistenceContext context = null);
        Task<ITransferResult> Upsert<T>(string fsPath, T obj, Type type = null, PersistenceContext context = null);
        Task<ITransferResult> Write<T>(string fsPath, T obj, ReplaceMode replaceMode = ReplaceMode.Upsert, PersistenceContext context = null, Type type = null);
    }
}
