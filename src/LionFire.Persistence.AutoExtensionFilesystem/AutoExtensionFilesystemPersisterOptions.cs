using LionFire.Dependencies;
using LionFire.Persistence.Filesystem;
using LionFire.Serialization;

namespace LionFire.Persistence.AutoExtensionFilesystem
{
    public class AutoExtensionFilesystemPersisterOptions : FilesystemPersisterOptions
    {
        public AutoExtensionFilesystemPersisterOptions()
        {
            //SerializationProvider = ServiceLocator.Get<ISerializationProvider>();
        }

        //public AutoExtensionFilesystemPersisterOptions(ISerializationProvider serializationProvider)
        //{
        //    SerializationProvider = serializationProvider;
        //}

        //public ISerializationProvider SerializationProvider { get; set; }
    }
}
