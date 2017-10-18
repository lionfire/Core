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
using LionFire.ObjectBus;
using LionFire.Handles;
using LionFire.IO;

namespace LionFire.Serialization
{
    public class SerializingFileHandle<T> : WritableHandleBase<T>
        //, IReferencable, IChangeableReferencable
        where T : class
    {
        [SetOnce]
        public string Path { get => Key; private set => Key = value; }

        #region UnderlyingHandle

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

        #endregion

        #region Reference

        // TODO: 

        //public IReference Reference { get => new FsReference(Path); set => throw new NotImplementedException(); }

        //IReference IReferencable.Reference => throw new NotImplementedException();

        //public event Action<IChangeableReferencable, IReference> ReferenceChangedForFrom;

        #endregion

        #region Changing reference

        public Task Move(string newPath)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Construction

        public SerializingFileHandle(string path)
        {
            this.Path = path;
        }

        #endregion

        #region Load

        public override Task<bool> TryResolveObject(object persistenceContext = null)
        {
            var serializationService = persistenceContext.ObjectAsType<ISerializationService>();
            if (serializationService == null) { serializationService = InjectionContext.Current.GetService<ISerializationService>(); }

            if (serializationService == null) throw new HasUnresolvedDependenciesException($"No {typeof(ISerializationService).Name} available");

            this.Object = serializationService.FileToObject<T>(Path);
            return Task.FromResult(HasObject);
        }

        #endregion

        #region Delete

        public override async Task DeleteObject(object persistenceContext = null)
        {
            Action deleteAction = () => File.Delete(Path);
            await deleteAction.AutoRetry(); // TODO: Use File IO parameters registered in DI.
        }

        #endregion

        #region Save

        public override async Task WriteObject(object persistenceContext = null)
        {
            await Task.Run(() =>
            {
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
            }).ConfigureAwait(false);
        }

        #endregion
    }
}
