using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LionFire.ObjectBus.FsFacade;
using LionFire.ObjectBus.Typing;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Serialization;
using LionFire.Structures;
using Microsoft.Extensions.Logging;

namespace LionFire.ObjectBus.Postgres
{

    public partial class PostgresOBase : WritableFsFacadeOBase<PostgresReference>
    {
        #region Static

        public static PostgresOBase DefaultInstance => ManualSingleton<PostgresOBase>.GetGuaranteedInstance<PostgresOBase>(() => new PostgresOBase(DefaultInstanceConnectionString));
        public static string DefaultInstanceConnectionString { get; set; } = "localhost";

        #endregion

        #region Constants

        public override IEnumerable<string> UriSchemes => PostgresReference.UriSchemes;

        #endregion

        #region Parameters

        #region ConnectionString

        /// <summary>
        /// Comma separated host:port
        /// E.g. "server1:6379,server2:6379"
        /// Order not important; master is automatically identified
        /// </summary>
        public override string ConnectionString
        {
            get => connectionString;
            set
            {
                if (connectionString == value)
                {
                    return;
                }

                connectionString = value;

                if (postgres != null)
                {
                    var temp = postgres;
                    postgres = null;
                    temp.Dispose();
                }
                postgres = ConnectionMultiplexer.Connect(connectionString);
            }
        }
        private string connectionString;

        #endregion

        #endregion

        #region Relationships

        public override IOBus OBus => ManualSingleton<PostgresOBus>.GuaranteedInstance;

        public override IFsFacade FsFacade => postgresFsFacade;

        private PostgresFsFacade postgresFsFacade;

        #endregion

        #region Construction

        public PostgresOBase() { postgresFsFacade = new PostgresFsFacade(this); }
        public PostgresOBase(string connectionString) : this()
        {
            this.ConnectionString = connectionString;
        }

        #endregion

        #region State

        #region Connection

        // TOPORT
        ///// <summary>
        ///// Once you have a ConnectionMultiplexer, there are 3 main things you might want to do:
        /////  - access a postgres database (note that in the case of a cluster, a single logical database may be spread over multiple nodes)
        /////  - make use of the pub/sub features of postgres
        /////  - access an individual server for maintenance / monitoring purposes
        ///// </summary>
        //internal ConnectionMultiplexer Postgres => postgres;
        //private ConnectionMultiplexer postgres;

        #endregion

        #endregion

        #region Referencing

        private void ValidateReference(PostgresReference reference)
        {
            //if (reference.Scheme != PostgresReference.UriScheme) throw new PostgresOBaseException("Invalid scheme");
            //if (!reference.IsLocalhost) throw new PostgresOBaseException("Only localhost supported");
        }

        #endregion

        #region Get

        //public override async Task<IRetrieveResult<ResultType>> TryGet<ResultType>(PostgresReference reference)
        //{
        //    var result = await TryGet(reference, typeof(ResultType));
        //    var converted = (ResultType)TryConvertToType(result.Result, typeof(ResultType));
        //    return new RetrieveResult<ResultType>
        //    {
        //        IsSuccess = true,
        //        Result = converted,
        //    };
        //}

        #region Persistence

        ////public Stream PathToReadStream(string path) => db.StringGet(path);
        ////public byte[] PathToBytes(string path) => File.ReadAllBytes(path);
        //public string PathToString(string path) => postgres.GetDatabase().StringGet(path);

        //protected PersistenceContext OBaseDeserializingPersistenceContext
        //{
        //    get
        //    {
        //        if (obaseDeserializingPersistenceContext == null)
        //        {
        //            obaseDeserializingPersistenceContext = new PersistenceContext
        //            {
        //                SerializationProvider = Defaults.TryGet<ISerializationProvider>(),
        //                Deserialization = new DeserializePersistenceContext
        //                {
        //                    //PathToStream = PathToReadStream,
        //                    //PathToBytes = PathToBytes,
        //                    PathToString = PathToString,
        //                }
        //            };
        //        }
        //        return obaseDeserializingPersistenceContext;
        //    }
        //}
        //private PersistenceContext obaseDeserializingPersistenceContext;

        //public async Task<object> PersGet(PostgresReference reference, Type type = null, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        //{
        //    IDatabase db = postgres.GetDatabase();
        //    var value = await db.StringGetAsync(reference.Path);
        //    var serializationProvider = (context?.SerializationProvider ?? Defaults.TryGet<ISerializationProvider>());
        //    #region Operation

        //    //var persistenceOperation = new PersistenceOperation()
        //    //{
        //    //    Reference = reference,
        //    //    Type = type,
        //    //    //Deserialization = new DeserializePersistenceOperation()
        //    //    //{
        //    //    //    #region ENH - optional alternative: combine dir and filenames to get candidatepaths
        //    //    //    //Directory = dir,
        //    //    //    //CandidateFilemes = 
        //    //    //    #endregion
        //    //    //    //CandidatePaths = candidatePaths.Select(path => Path.Combine(dir, Path.GetFileName(path))),
        //    //    //    CandidatePaths = new string[] { path }
        //    //    //}
        //    //};

        //    #endregion

        //    #region Context

        //    if (context != null)
        //    {
        //        throw new NotImplementedException($"{nameof(context)} not implemented yet");
        //    }
        //    //var effectiveContext = OBaseDeserializingPersistenceContext;

        //    #endregion

        //    //return persistenceOperation.ToObject<object>(effectiveContext);
        //    //return persistenceOperation.ToObject<object>();
        //}

        //public async Task<object> PersGet(PostgresReference reference, Type type = null /*, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null*/)
        //{

        //}

        #endregion

        #region DefaultSerializationProvider

        public ISerializationProvider DefaultSerializationProvider
        {
            get
            {
                if (defaultSerializationProvider == null)
                {
                    defaultSerializationProvider = (/*context?.SerializationProvider ??*/ Defaults.TryGet<ISerializationProvider>());
                }
                return defaultSerializationProvider;
            }
            set => defaultSerializationProvider = value;
        }
        private ISerializationProvider defaultSerializationProvider;

        #endregion

        public override async Task<IRetrieveResult<object>> TryGet(PostgresReference reference, Type ResultType)
        {
            var result = new RetrieveResult<object>();
            try
            {
                IDatabase db = postgres.GetDatabase();
                var value = await db.StringGetAsync(reference.Path).ConfigureAwait(false);
                if (!value.HasValue)
                {
                    return null;
                }

                string str = value;

                object obj = DefaultSerializationProvider.ToObject<object>(str);

                obj = OBaseTypeUtils.TryConvertToType(obj, ResultType);

                // ... TODO Multitype multiplex layer

                if (obj != null)
                {
                    OBaseEvents.OnRetrievedObjectFromExternalSource(obj); // Put reference in here?
                }

                result.Result = obj;
                result.IsSuccess = true; // True regardless of whether an object was found

                return result;
            }
            catch (Exception ex)
            {
                OBaseEvents.OnException(OBusOperations.Get, reference, ex);
                throw ex;
            }
        }

        public override async Task<IRetrieveResult<ResultType>> TryGet<ResultType>(PostgresReference reference)
        {
            try
            {
                IDatabase db = postgres.GetDatabase();
                var value = await db.StringGetAsync(reference.Path).ConfigureAwait(false);
                if (!value.HasValue)
                {
                    return RetrieveResult<ResultType>.NullSuccessful;
                }

                string str = value;

                object obj = DefaultSerializationProvider.ToObject<object>(str);
                ResultType converted = (ResultType)OBaseTypeUtils.TryConvertToType(obj, typeof(ResultType));

                // ... TODO Multitype multiplex layer

                if (converted != null)
                {
                    OBaseEvents.OnRetrievedObjectFromExternalSource(converted); // Put reference in here?
                }

                return new RetrieveResult<ResultType>
                {
                    Result = converted,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                OBaseEvents.OnException(OBusOperations.Get, reference, ex);
                throw ex;
            }
        }

        #endregion


        public override async Task<IRetrieveResult<bool>> Exists(PostgresReference reference)
        {
            var result = new RetrieveResult<bool>();

            bool existsResult = await FsFacade.Exists(reference.Path).ConfigureAwait(false);

            result.Result = existsResult;
            result.IsSuccess = true;
            return result;
        }

        #region Delete

        public override async Task<bool?> CanDelete(PostgresReference reference)
        {
            // FUTURE: Check filesystem permissions
            var existsResult = await Exists(reference);
            if (!existsResult.IsSuccess)
            {
                return null;
            }

            return existsResult.Result;
            //return new RetrieveResult<bool?>
            //{
            //    IsSuccess = existsResult.IsSuccess,
            //    Result = existsResult.Result,
            //};
            //string filePath = reference.Path;
            //return FsPersistence.TryDelete(filePath);
        }

        public override async Task<bool?> TryDelete(PostgresReference reference/*, bool preview = false*/)
        {
            string filePath = reference.Path;
            //if (!defaultTypeForDirIsT)
            //{
            //    filePath = filePath + FileTypeDelimiter + type.Name + FileTypeEndDelimiter;
            //}

            //if (preview)
            //{
            //    return await FsFacade.Exists(filePath).ConfigureAwait(false);
            //}
            //else
            //{
                return await FsFacade.Delete(filePath).ConfigureAwait(false);
            //}
        }
        
        #endregion

        //protected override async Task _Set<T>(PostgresReference reference, T obj, bool allowOverwrite = true, bool preview = false)
        //{
        //    await PostgresOBasePersistence.Set(obj, reference.Path, preview: preview, type: typeof(T));
        //}

        protected override async Task _Set(PostgresReference reference, object obj, Type type = null, bool allowOverwrite = true, bool preview = false)
        {
            #region TODO

            //bool defaultTypeForDirIsT = false;
            //Type type = obj.GetType();
            //var chunks = LionPath.ToPathArray(reference.Path);
            //if (chunks == null || chunks.Length < 2)
            //{
            //    defaultTypeForDirIsT = false;
            //}
            //else // TOPORT
            //{
            //    string parentDirName;
            //    parentDirName = chunks[chunks.Length - 2];
            //    defaultTypeForDirIsT = Assets.AssetPaths.GetAssetTypeFolder(type).TrimEnd('/').Equals(parentDirName);
            //}

            // TODO - This should be done up a layer, since most persistance mechanisms will not support multi-type
            //string filePath = reference.Path;
            //if (AppendTypeNameToFileNames && !defaultTypeForDirIsT) // TOPORT
            //{
            //    filePath = filePath + VosPath.TypeDelimiter + type.Name + VosPath.TypeEndDelimiter;
            //}

            #endregion

            if (preview) throw new NotImplementedException("preview");
            var bytes = this.DefaultSerializationProvider.ToBytes(obj, type != null ? () => new PersistenceOperation { Type = type } : (Func<PersistenceOperation>)null );

            var path = reference.Path;
            var dir = LionPath.GetDirectoryName(path) + "/";
            var filename = LionPath.GetFileName(path);
            await postgres.GetDatabase().SetAddAsync(dir, filename);

            await FsFacade.WriteAllBytes(reference.Path, bytes).ConfigureAwait(false);
        }

        public const bool AppendTypeNameToFileNames = false; // TEMP - TODO: Figure out a way to do this in VOS land

        #region TODO: Rename to Keys() and make async

        public override IEnumerable<string> GetChildrenNames(PostgresReference parent)
        {
            PostgresReference fileRef = PostgresReference.ConvertFrom(parent);

            if (fileRef == null)
            {
                throw new ArgumentException("Could not convert to FileReference");
            }

            return (FsFacade.GetKeys(fileRef.Path).Result).Select(p=>Path.GetFileName(p));
        }

        public override async Task<IEnumerable<string>> GetKeys(PostgresReference parent) // 
        {
            PostgresReference fileRef = PostgresReference.ConvertFrom(parent);

            if (fileRef == null)
            {
                throw new ArgumentException("Could not convert to FileReference");
            }

            return (await FsFacade.GetKeys(fileRef.Path + "/").ConfigureAwait(false)).Select(p => Path.GetFileName(p));
        }

        #endregion

        //private static string GetDirNameForType(string filePath)
        //{
        //    var chunks = VosPath.ToPathArray(filePath);
        //    if (chunks == null || chunks.Length == 0) yield break;
        //    string parentDirName = chunks[chunks.Length - 1];

        //    return Assets.AssetPath.GetDefaultDirectory(typeof(T));
        //}

        public override IEnumerable<string> GetChildrenNamesOfType<T>(PostgresReference parent) => throw new NotImplementedException();
        public override H<T> GetHandle<T>(IReference reference) => throw new NotImplementedException();
        public override RH<T> GetReadHandle<T>(IReference reference) => throw new NotImplementedException();

#if TOPORT
            var chunks = VosPath.ToPathArray(parent.Path);
            if (chunks == null || chunks.Length == 0) yield break;
            string parentDirName = chunks[chunks.Length - 1];

            bool defaultTypeForDirIsT = Assets.AssetPaths.GetAssetTypeFolder(typeof(T)).TrimEnd('/').Equals(parentDirName);

            foreach (var name in FsPersistence.GetChildrenNames(parent.Path))
            {
                string typeName = PostgresOBaseSettings.GetTypeNameFromFileName(name);

                if (typeName == null)
                {
                    if (defaultTypeForDirIsT)
                    {
                        yield return name;
                        continue;
                    }
                }
                else if (typeName == typeof(T).Name)
                {
                    yield return name;
                    continue;
                }
            }

            //l.Warn("PARTIALLY IMPLEMENTED: GetChildrenNamesOfType - does not filter types");
            //return GetChildrenNames(parent);
#endif


        private static ILogger l = Log.Get();

    }



}
