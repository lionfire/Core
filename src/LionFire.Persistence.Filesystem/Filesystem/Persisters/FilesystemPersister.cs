using LionFire.Dependencies;
using LionFire.ExtensionMethods.Collections;
using LionFire.ExtensionMethods.Persistence.Filesystem;
using LionFire.IO;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Persistence.Filesystem
{
    // REVIEW - is there a way to do this?
    //public static class PersisterBaseExtensions
    //{
    //    public static Task<IRetrieveResult<TValue>> Retrieve<TValue, TReference>(this IPersister<FileReference> persister, FileReference reference)
    //        => persister.Retrieve<TValue>((TReference)reference.Path);
    //}

    public class FilesystemPersister : FilesystemPersister<FileReference, FilesystemPersisterOptions>, IPersister<FileReference>
    {

        #region Static

        public static FilesystemPersister Current { get; } = ServiceLocator.Get<FilesystemPersister>();

        #endregion

        #region Construction

        public FilesystemPersister(ISerializationProvider serializationProvider, IOptionsMonitor<FilesystemPersisterOptions> optionsMonitor, IPersistenceConventions itemKindIdentifier) : this(serializationProvider, null, optionsMonitor, itemKindIdentifier)
        {
        }
        public FilesystemPersister(ISerializationProvider serializationProvider, string name, IOptionsMonitor<FilesystemPersisterOptions> options, IPersistenceConventions itemKindIdentifier) : base(name, serializationProvider, options, itemKindIdentifier)
        {
        }

        #endregion

        public override FileReference PathToReference(string fsPath) => fsPath;

        //protected override PersistenceContext DeserializingContext => deserializingContext;
        //private readonly PersistenceContext deserializingContext = new PersistenceContext
        //{
        //    SerializationProvider = DependencyLocator.TryGet<ISerializationProvider>(),
        //    Deserialization = new DeserializePersistenceContext
        //    {
        //        PathToStream = FSPersistenceExtensions.PathToReadStream,
        //        PathToBytes = FSPersistenceExtensions.PathToBytes,
        //        PathToString = FSPersistenceExtensions.PathToString,
        //    }
        //};

        #region IO

        public override IOCapabilities Capabilities => IOCapabilities.All;

        #region Read

        public override Task<Stream> ReadStream(string path) => Task.FromResult<Stream>(new FileStream(path, FileMode.Open));
        public override Task<string> ReadString(string path) => Task.Run(() => File.ReadAllText(path, PersistenceOptions.Encoding));
        public override Task<byte[]> ReadBytes(string path) => Task.Run(() => File.ReadAllBytes(path));

        #endregion

        #region Write

        public override Task<Stream> WriteStream(string path, ReplaceMode replaceMode)
            => Task.FromResult<Stream>(new FileStream(path, replaceMode.ToFileMode()));

        public override Task WriteBytes(string path, byte[] bytes, ReplaceMode replaceMode) => Task.Run(() => File.WriteAllBytes(path, bytes));
        public override Task WriteString(string path, string str, ReplaceMode replaceMode) => Task.Run(() => File.WriteAllText(path, str));

        #endregion

        #region ReadMeta

        // TODO
        // Simple meta: Get File size, mod date
        // Full meta: permissions, NTFS attributes

        #endregion

        #region List



        #endregion

        #endregion

        #region Directories

        public override Task CreateDirectory(string fsPath) => Task.Run(() => Directory.CreateDirectory(fsPath)); // TODO SECURITY - set permissions to all users writable

        #endregion

        #region Retrieve

        #region Exists

        public override async Task<bool> Exists(string fsPath)
            => await FileExists(fsPath).ConfigureAwait(false) ? true : await DirectoryExists(fsPath).ConfigureAwait(false);

        public Task<bool> FileExists(string fsPath) => Task.Run(() => File.Exists(fsPath));
        public override Task<bool> DirectoryExists(string fsPath) => Task.Run(() => Directory.Exists(fsPath));

        #endregion

        #endregion

        #region Delete

        public override Task<bool?> DeleteFile(string fsPath) => Task.Run(() => { File.Delete(fsPath); return (bool?)null; });
        public override Task<bool?> DeleteDirectory(string fsPath) => Task.Run(() => { Directory.Delete(fsPath); return (bool?)null; });

        #endregion

        #region IMPORTED - REVIEW all, move to base class

        #region (Static) Directory accessors

        /// <summary>
        /// WARNING REVIEW: If files have multiple extensions, there may be strange behavior
        /// </summary>
        /// <param name="namePath"></param>
        /// <returns></returns>
        public List<string> GetFilePathsForNamePath(string namePath) // MOVE to VOS
        {
            var paths = new List<string>();

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

        public string GetNameFromFileName(string filename) // MOVE to VOS
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(filename);

            int indexOfNameEnd = name.IndexOf(PersistenceOptions.EndOfNameMarker);

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

        // MOVE to Vos (or VosApp?)
        private static Type GetDefaultChildTypeForPath(string path) => throw new NotImplementedException("TOPORT");//#if AOT // TOPORT//            var metadata = VFSMetadata[path].AsType(typeof(FSMetaData)) as FSMetaData;//#else//            var metadata = VFSMetadata[path].AsType<FSMetaData>();//#endif//            if (metadata == null) return null;//            return metadata.DefaultType;

        #endregion
    
        #endregion



        private static readonly ILogger l = Log.Get();

        #endregion

    }

    // REVIEW - is there a way to do this?
    //public static class PersisterBaseExtensions
    //{
    //    public static Task<IRetrieveResult<TValue>> Retrieve<TValue, TReference>(this IPersister<FileReference> persister, FileReference reference)
    //        => persister.Retrieve<TValue>((TReference)reference.Path);
    //}
}
