using LionFire.DependencyInjection;
using LionFire.Execution;
using LionFire.IO;
using LionFire.MultiTyping;
using LionFire.Serialization.Contexts;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.Serialization
{
    /// <summary>
    /// Uses ISerializationService from InjectionContext.Current to save/load an object to a local file 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SerializingFileHandle<T> : HLocalFileBase<T>
        //, IReferencable, IChangeableReferencable
        where T : class
    {

        #region UnderlyingHandle

        public HBinaryFile UnderlyingHandle
        {
            get
            {
                if (underlyingHandle == null)
                {
                    underlyingHandle = new HBinaryFile(Path);
                }
                return underlyingHandle;
            }
        }
        private HBinaryFile underlyingHandle;

        #endregion


        #region Construction

        public SerializingFileHandle(string path) : base(path)
        {
        }

        #endregion

        protected virtual ISerializationService ValidatedSerializationService
        {
            get
            {
                //var serializationService = persistenceContext.ObjectAsType<ISerializationService>();
                //if (serializationService == null) { serializationService = InjectionContext.Current.GetService<ISerializationService>(); }
                var serializationService = InjectionContext.Current.GetService<ISerializationService>();

                if (serializationService == null)
                {
                    throw new HasUnresolvedDependenciesException($"No {typeof(ISerializationService).Name} available");
                }
                return serializationService;
            }
        }

        #region Load

        public override Task<bool> TryRetrieveObject()
        {
            this.Object = ValidatedSerializationService.FileToObject<T>(Path);
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
                var sc = new FileSerializationContext();

                var bytes = ValidatedSerializationService.ToBytes(Object, sc);

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
