using LionFire.IO;
using LionFire.Persistence.Filesystem;
using LionFire.Referencing;
using LionFire.Serialization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LionFire.Persistence.AutoExtensionFilesystem
{
    public class AutoExtensionFilesystemPersister : AutoExtensionPersister<AutoExtensionFileReference, AutoExtensionFilesystemPersisterOptions, FileReference, FilesystemPersister>
    {
        #region Dependencies
        
        public FilesystemPersister UnderlyingPersister { get; }

        #endregion

        #region Lifecycle
              
        public AutoExtensionFilesystemPersister(FilesystemPersister filesystemPersister, AutoExtensionFilesystemPersisterOptions options, SerializationOptions serializationOptions) : base(serializationOptions)
        {
            UnderlyingPersister = filesystemPersister;
            PersistenceOptions = options;
        }

        #endregion

        protected override FilesystemPersister GetUnderlyingPersister(AutoExtensionFileReference reference) => UnderlyingPersister;

        protected override FileReference ConvertReferenceWithPath(AutoExtensionFileReference reference, string path) => new FileReference(path);

        public override FileReference TranslateReferenceForRead(AutoExtensionFileReference reference)
        {
            var parent = reference.GetParent();

            var list = parent.GetListingsHandle();
            foreach (var item in list.Value.Value)
            {
                Debug.WriteLine("TODO: " + item.Name);                    
            }
            return null;
        }

        public override FileReference TranslateReferenceForWrite(AutoExtensionFileReference reference)
        {
            throw new System.NotImplementedException();
        }

        // OPTIMIZE - this is faster?
        //public override async IAsyncEnumerable<string> CandidateReadPaths(string path)
        //{
        //    HashSet<string> results = null;

        //    //foreach (var selectionResult in PersistenceOptions.SerializationProvider.ResolveStrategies(direction: IODirection.Read))
        //    foreach (var selectionResult in PersistenceOptions.SerializationProvider.Strategies)
        //    {
        //        foreach (var extension in selectionResult.SupportedExtensions(IODirection.Read))
        //        {
        //            var candidatePath = path + "." + extension;
        //            if (results?.Contains(candidatePath) == true) continue;
        //            if (await (UnderlyingPersister.Exists(candidatePath)))
        //            {
        //                yield return extension;
        //                if (results == null) results = new HashSet<string>();
        //                results.Add(candidatePath);
        //            }
        //        }
        //    }
        //}
    }
}
