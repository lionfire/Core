using System;
using System.IO;
using System.Threading.Tasks;
using LionFire.DependencyInjection;
using LionFire.Execution;
using LionFire.IO;
using LionFire.Persistence;
using LionFire.Serialization.Contexts;

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

        //#region UnderlyingHandle

        //public HBinaryFile UnderlyingHandle
        //{
        //    get
        //    {
        //        if (underlyingHandle == null)
        //        {
        //            underlyingHandle = new HBinaryFile(Path);
        //        }
        //        return underlyingHandle;
        //    }
        //}
        //private HBinaryFile underlyingHandle;

        //#endregion

        public PersistenceOperation SerializationOperation { get; set; }

        #region Construction

        public SerializingFileHandle(string path) : base(path)
        {
        }

        #endregion

        protected virtual ISerializationProvider ValidatedSerializationService
        {
            get
            {
                //var serializationService = persistenceContext.ObjectAsType<ISerializationService>();
                //if (serializationService == null) { serializationService = InjectionContext.Current.GetService<ISerializationService>(); }
                //var serializationService = InjectionContext.Current.GetService<ISerializationService>();
                var serializationService = Defaults.TryGet<ISerializationProvider>() ?? ;

                if (serializationService == null)
                {
                    throw new HasUnresolvedDependenciesException($"No {typeof(ISerializationProvider).Name} available");
                }
                return serializationService;
            }
        }

        public static SerializationContext DefaultSerializationContext { get; set; }
        public SerializationContext SerializationContext { get; set; }

        #region Load

        //public async Task<bool> TryRetrieveObject(Func<PersistenceContext> context)
        //{
        //    Object = SerializationFacility.Default.ToObject<T>(fs, operation: SerializationOperation, context: DefaultSerializationContext, );
        //}

        public PersistenceOperation GetSerializationOperation()
        {
            if (SerializationOperation == null)
            {
                SerializationOperation = new PersistenceOperation();
            }

            //if (SerializationOperation.Path == null) // Don't allow path spoofing
            //{
                SerializationOperation.Path = Path;
            //}

            if (SerializationOperation.Path == null)
            {
                throw new ArgumentNullException("Path or SerializationOperation.Path must be set");
            }
            return SerializationOperation;
        }

        public PersistenceContext GetPersistenceContext()
        {
            //if (persistenceContext == null) persistenceContext = new PersistenceContext();
            var persistenceContext = new PersistenceContext();

            persistenceContext.SerializationContext = SerializationContext ?? DefaultSerializationContext;
            persistenceContext.GetPersistenceOperation = GetSerializationOperation;

            return persistenceContext;
        }

        public override async Task<bool> TryRetrieveObject()
        {
            // TODO: Change TryRetrieveObject() to TryRetrieveObject(Func<PersistenceContext>) which contains SerializationOperation?
            
            //SerializationResult Result;
            //UnderlyingHandle.Object

            return await Task.Run(() =>
            {
                if (!File.Exists(Path)) { return false; }

                try
                {
                    T Object;
#if true // Stream
                    using (var fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        //Object = SerializationFacility.Default.ToObject<T>(fs, operation: SerializationOperation, context: DefaultSerializationContext);
                        Object = SerializationFacility.Default.ToObject<T>(fs, new Lazy<PersistenceContext>(GetPersistenceContext));
                    }
#else
            var bytes = File.ReadAllBytes(Path);
            Object = Defaults.Get<ISerializationProvider>.ToObject<T>(bytes, operation: SerializationOperation, context: DefaultSerializationContext)
#endif

                    //if(Result.IsSuccess)
                    {
                        this.Object = Object;
                    }
                    return true;
                }
                catch (SerializationException)
                {
                    return false;
                }
            });
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

                var bytes = SerializationFacility.Default.ToBytes(Object, sc);

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
