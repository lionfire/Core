using System;
using System.IO;
using System.Threading.Tasks;
using LionFire.DependencyInjection;
using LionFire.Execution;
using LionFire.IO;
using LionFire.ObjectBus.Filesystem;
using LionFire.Persistence;
using LionFire.Serialization.Contexts;

namespace LionFire.Serialization
{
    /// <summary>
    /// Uses ISerializationService from DependencyContext.Current to save/load an object to a local file 
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

        public PersistenceOperation PersistenceOperation { get; set; }

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
                //if (serializationService == null) { serializationService = DependencyContext.Current.GetService<ISerializationService>(); }
                //var serializationService = DependencyContext.Current.GetService<ISerializationService>();
                var serializationService = Defaults.TryGet<ISerializationProvider>();

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

        public PersistenceOperation GetPersistenceOperation()
        {
            if (PersistenceOperation == null)
            {
                PersistenceOperation = new PersistenceOperation();
            }

            //if (SerializationOperation.Path == null) // Don't allow path spoofing
            //{
            // REVIEW MOVE this note - LionFire.ObjectBus.Filesystem is brought in just from this LocalFileReference?  Move it out?
                PersistenceOperation.Reference = new LocalFileReference(Path);
                //SerializationOperation.Path = Path;
            //}

            if (PersistenceOperation.Path == null)
            {
                throw new ArgumentNullException("Path or SerializationOperation.Path must be set");
            }
            return PersistenceOperation;
        }

        public PersistenceContext GetPersistenceContext()
        {
            //if (persistenceContext == null) persistenceContext = new PersistenceContext();
            var persistenceContext = new PersistenceContext();

            persistenceContext.SerializationContext = SerializationContext ?? DefaultSerializationContext;
            persistenceContext.GetPersistenceOperation = GetPersistenceOperation;

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
                        Object = SerializationFacility.Default.ToObject<T>(fs, new Lazy<PersistenceOperation>(GetPersistenceOperation));
                    }
#else
                    var bytes = File.ReadAllBytes(Path);
                    Object = Defaults.Get<ISerializationProvider>().ToObject<T>(bytes, operation: SerializationOperation, context: DefaultSerializationContext);
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

        protected override async Task<bool?> DeleteObject(object persistenceContext = null)
        {
            Action deleteAction = () => File.Delete(Path);
            await deleteAction.AutoRetry(); // TODO: Use File IO parameters registered in DI.
            return true;
        }

        #endregion

        #region Save

        protected override async Task WriteObject(object persistenceContext = null)
        {
            await Task.Run(() =>
            {
                //var sc = new FileSerializationContext();
                var op = new PersistenceOperation();

                var bytes = SerializationFacility.Default.ToBytes(Object, () => op);

                var writePath = Path;
                if (op?.Extension != null)
                {
                    writePath += "." +op.Extension;
                }

                File.WriteAllBytes(writePath, bytes);
            }).ConfigureAwait(false);
        }

        #endregion
    }
}
