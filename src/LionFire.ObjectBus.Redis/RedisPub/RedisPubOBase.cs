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
using StackExchange.Redis;

namespace LionFire.ObjectBus.RedisPub
{

    public partial class RedisPubOBase : WritableFsFacadeOBase<RedisPubReference>
    {
        #region Static

        public static RedisPubOBase DefaultInstance => ManualSingleton<RedisPubOBase>.GetGuaranteedInstance<RedisPubOBase>(() => new RedisPubOBase(DefaultInstanceConnectionString));
        public static string DefaultInstanceConnectionString { get; set; } = "localhost";

        #endregion

        #region Constants

        public override IEnumerable<string> UriSchemes => RedisPubReference.UriSchemes;

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

                if (redis != null)
                {
                    var temp = redis;
                    redis = null;
                    temp.Dispose();
                }
                redis = ConnectionMultiplexer.Connect(connectionString);
            }
        }
        private string connectionString;

        #endregion

        #endregion

        #region Relationships

        public override IOBus OBus => ManualSingleton<RedisPubOBus>.GuaranteedInstance;

        public override IFsFacade FsFacade => redisFsFacade;

        private RedisPubFsFacade redisFsFacade;

        #endregion

        #region Construction

        public RedisPubOBase() { redisFsFacade = new RedisPubFsFacade(this); }
        public RedisPubOBase(string connectionString) : this()
        {
            this.ConnectionString = connectionString;
        }

        #endregion

        #region State

        #region Connection

        /// <summary>
        /// Once you have a ConnectionMultiplexer, there are 3 main things you might want to do:
        ///  - access a redis database (note that in the case of a cluster, a single logical database may be spread over multiple nodes)
        ///  - make use of the pub/sub features of redis
        ///  - access an individual server for maintenance / monitoring purposes
        /// </summary>
        private ConnectionMultiplexer redis;

        #endregion

        #endregion

        #region Referencing

        private void ValidateReference(RedisPubReference reference)
        {
            //if (reference.Scheme != RedisPubReference.UriScheme) throw new RedisPubOBaseException("Invalid scheme");
            //if (!reference.IsLocalhost) throw new RedisPubOBaseException("Only localhost supported");
        }

        #endregion

        #region Get

        //public override async Task<IRetrieveResult<ResultType>> TryGet<ResultType>(RedisPubReference reference)
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
        //public string PathToString(string path) => redis.GetDatabase().StringGet(path);

        //protected PersistenceContext OBaseDeserializingPersistenceContext
        //{
        //    get
        //    {
        //        if (obaseDeserializingPersistenceContext == null)
        //        {
        //            obaseDeserializingPersistenceContext = new PersistenceContext
        //            {
        //                SerializationProvider = DependencyLocator.TryGet<ISerializationProvider>(),
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

        //public async Task<object> PersGet(RedisPubReference reference, Type type = null, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        //{
        //    IDatabase db = redis.GetDatabase();
        //    var value = await db.StringGetAsync(reference.Path);
        //    var serializationProvider = (context?.SerializationProvider ?? DependencyLocator.TryGet<ISerializationProvider>());
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

        //public async Task<object> PersGet(RedisPubReference reference, Type type = null /*, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null*/)
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
                    defaultSerializationProvider = (/*context?.SerializationProvider ??*/ DependencyLocator.TryGet<ISerializationProvider>());
                }
                return defaultSerializationProvider;
            }
            set => defaultSerializationProvider = value;
        }
        private ISerializationProvider defaultSerializationProvider;

        #endregion

        public override Task<IRetrieveResult<T>> TryGet<T>(RedisPubReference reference)
        {
            throw new NotImplementedException("TODO: think about what TryGet means in PubOBase");
            //try
            //{
            //    IDatabase db = redis.GetDatabase();
            //    var value = await db.StringGetAsync(reference.Path).ConfigureAwait(false);
            //    if (!value.HasValue)
            //    {
            //        return RetrieveResult<T>.RetrievedNull;
            //    }

            //    string str = value;

            //    object obj = DefaultSerializationProvider.ToObject<object>(str);
            //    var converted = OBaseTypeUtils.TryConvertToType<T>(obj);

            //    // ... TODO Multitype multiplex layer

            //    if (converted.success)
            //    {
            //        OBaseEvents.OnRetrievedObjectFromExternalSource(converted.result); // Put reference in here?
            //    }

            //    return new RetrieveResult<T>
            //    {
            //        Object = converted.result,
            //        Flags =PersistenceResultFlags.Success | PersistenceResultFlags.Found | PersistenceResultFlags.Retrieved,
            //    };
            //}
            //catch (Exception ex)
            //{
            //    OBaseEvents.OnException(OBusOperations.Get, reference, ex);
            //    throw ex;
            //}
        }

        #endregion

        
        protected override Task<IPersistenceResult> SetImpl<T>(RedisPubReference reference, T obj, bool allowOverwrite = true)
        {
            throw new NotImplementedException();
            //#region TODO

            ////bool defaultTypeForDirIsT = false;
            ////Type type = obj.GetType();
            ////var chunks = LionPath.ToPathArray(reference.Path);
            ////if (chunks == null || chunks.Length < 2)
            ////{
            ////    defaultTypeForDirIsT = false;
            ////}
            ////else // TOPORT
            ////{
            ////    string parentDirName;
            ////    parentDirName = chunks[chunks.Length - 2];
            ////    defaultTypeForDirIsT = Assets.AssetPaths.GetAssetTypeFolder(type).TrimEnd('/').Equals(parentDirName);
            ////}

            //// TODO - This should be done up a layer, since most persistance mechanisms will not support multi-type
            ////string filePath = reference.Path;
            ////if (AppendTypeNameToFileNames && !defaultTypeForDirIsT) // TOPORT
            ////{
            ////    filePath = filePath + VosPath.TypeDelimiter + type.Name + VosPath.TypeEndDelimiter;
            ////}

            //#endregion

            //var bytes = this.DefaultSerializationProvider.ToBytes(obj, type != null ? () => new PersistenceOperation { Type = type } : (Func<PersistenceOperation>)null);

            //var path = reference.Path;
            //var dir = LionPath.GetDirectoryName(path) + "/";
            //var filename = LionPath.GetFileName(path);
            //await redis.GetDatabase().SetAddAsync(dir, filename);

            //await FsFacade.WriteAllBytes(reference.Path, bytes).ConfigureAwait(false);
        }

        public const bool AppendTypeNameToFileNames = false; // TEMP - TODO: Figure out a way to do this in VOS land

        #region List

        //public async override Task<IEnumerable<string>> List<T>(RedisPubReference parent)
        //{
        //    throw new NotImplementedException();
        //    RedisPubReference fileRef = RedisPubReference.ConvertFrom(parent);

        //    if (fileRef == null)
        //    {
        //        throw new ArgumentException("Could not convert to FileReference");
        //    }

        //    return (await FsFacade.List(fileRef.Path)).Select(p => Path.GetFileName(p));
        //}


        public override async Task<IEnumerable<string>> List<T>(RedisPubReference parent) 
        {
            RedisPubReference fileRef = RedisPubReference.ConvertFrom(parent);

            if (fileRef == null)
            {
                throw new ArgumentException("Could not convert to FileReference");
            }

            return (await FsFacade.List(fileRef.Path + "/").ConfigureAwait(false)).Select(p => Path.GetFileName(p));
        }

        #endregion

        //private static string GetDirNameForType(string filePath)
        //{
        //    var chunks = VosPath.ToPathArray(filePath);
        //    if (chunks == null || chunks.Length == 0) yield break;
        //    string parentDirName = chunks[chunks.Length - 1];

        //    return Assets.AssetPath.GetDefaultDirectory(typeof(T));
        //}

        public override IReadWriteHandleBase<T> GetHandle<T>(IReference reference) => throw new NotImplementedException();
        public override IReadHandleBase<T> GetReadHandle<T>(IReference reference) => throw new NotImplementedException();
        

        public override Task<IPersistenceResult> TryDelete<T>(RedisPubReference reference) => throw new NotImplementedException();

#if TOPORT
            var chunks = VosPath.ToPathArray(parent.Path);
            if (chunks == null || chunks.Length == 0) yield break;
            string parentDirName = chunks[chunks.Length - 1];

            bool defaultTypeForDirIsT = Assets.AssetPaths.GetAssetTypeFolder(typeof(T)).TrimEnd('/').Equals(parentDirName);

            foreach (var name in FsPersistence.GetChildrenNames(parent.Path))
            {
                string typeName = RedisPubOBaseSettings.GetTypeNameFromFileName(name);

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
