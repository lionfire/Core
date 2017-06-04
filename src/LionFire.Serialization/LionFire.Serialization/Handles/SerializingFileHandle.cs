using LionFire.DependencyInjection;
using LionFire.Execution;
using LionFire.MultiTyping;
using LionFire.Serialization;
using LionFire.Serialization.Contexts;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Handles
{


    public class SerializingFileHandle<T> : HandleBase<T>
        where T : class
    {

        [SetOnce]
        public string Path { get => Key; private set => Key = value; }

        public BinaryFileHandle UnderlyingHandle
        {
            get
            {
                if (underlyingHandle == null)
                {
                    underlyingHandle = new BinaryFileHandle(Path);
                }
                return underlyingHandle;
            }
        }
        private BinaryFileHandle underlyingHandle;

        public SerializingFileHandle(string path)
        {
            this.Path = path;
        }

        public override Task<bool> TryResolveObject(object persistenceContext = null)
        {
            var serializationService = persistenceContext.ObjectAsType<ISerializationService>();
            if (serializationService == null) { serializationService = InjectionContext.Current.GetService<ISerializationService>(); }

            if (serializationService == null) throw new HasUnresolvedDependenciesException($"No {typeof(ISerializationService).Name} available");

            this.Object = serializationService.FileToObject<T>(Path);
            return Task.FromResult(HasObject);
        }


        public override async Task Save(object persistenceContext = null)
        {
            if (DeletePending)
            {
                Action deleteAction = () => File.Delete(Path);
                await deleteAction.AutoRetry();
                DeletePending = false;
                return;
            }

            var serializationService = persistenceContext.ObjectAsType<ISerializationService>();
            if (serializationService == null) { serializationService = InjectionContext.Current.GetService<ISerializationService>(); }

            if (serializationService == null) throw new HasUnresolvedDependenciesException($"No {typeof(ISerializationService).Name} available");

            var sc = new FileSerializationContext();

            var bytes = serializationService.ToBytes(Object, sc);

            var writePath = Path;
            if (sc?.FileExtension != null)
            {
                writePath += "." + sc.FileExtension;
            }

            File.WriteAllBytes(writePath, bytes);
        }
    }
}
