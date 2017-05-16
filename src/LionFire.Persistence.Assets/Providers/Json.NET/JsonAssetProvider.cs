using LionFire.Applications.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using LionFire.Assets.Providers.FileSystem;
using LionFire.Assets;
using LionFire.DependencyInjection;
using LionFire.MultiTyping;
using LionFire.Structures;
using LionFire.Execution;
using LionFire.Persistence;
using Newtonsoft.Json.Serialization;
using LionFire.Instantiating;
using Newtonsoft.Json.Linq;
using LionFire.Validation;

namespace LionFire.Assets.Providers.FileSystem
{

    

    // FUTURE?
    //[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    //sealed class ContextPathAttribute : Attribute
    //{
    //    public string Path { get; private set; }
    //    public ContextPathAttribute(string path)
    //    {
    //        this.Path = path;
    //    }
    //}

    //[ContextPath("Persistence.FileSystem.Json")]
    public class JsonAssetProvider : FileSystemAssetProviderBase, IAssetProvider, IInitializable
    //, IFileExtensionHandler
    {

        public JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            ContractResolver = ShouldSerializeContractResolver.Instance,
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            Converters = new List<JsonConverter>
                {
                     new Newtonsoft.Json.Converters.StringEnumConverter()
                },
            //TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, // TODO: Does simple work in .NET Core?

        };

        //public override IEnumerable<string> FileExtensions { get { yield return "json"; } }
        public override string FileExtension { get { return "json"; } }
        //  public string FileExtensionWithDot { get { return ".json"; } }

        public JsonAssetProvider(string rootDir) : base(rootDir)
        {
        }

        //protected T _PostLoadConvert<T>(object obj)
        //    where T : class
        //{
        //    if (obj == null) return null;

        //    T result = obj as T;
        //    if (result != null) return result;

        //    var inst = obj as IInstantiator;
        //    if (inst != null)
        //    {
        //        return inst.Instantiate<T>();
        //    }

        //    throw new InvalidDataException($"After deserializing, expected type {typeof(T).FullName} but got {obj.GetType().FullName}");
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetSubPath"></param>
        /// <returns>Returns default(T) if file not found.</returns>
        public T Load<T>(string assetSubPath)
            where T : class
        {
            var path = GetPath<T>(assetSubPath);
            if (!File.Exists(path)) return default(T);


            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(File.ReadAllText(path), JsonSettings);
            var jObject = obj as JObject;

            if (obj as T != null) return (T)obj;

            var inst = obj as InstantiationPipeline;
            if (inst != null)
            {
                return inst.Instantiate<T>();
            }

            T result;
            if (jObject != null)
            {
                result = jObject.ToObject<T>();
                if (result != null) return result;

                inst = jObject.ToObject<InstantiationPipeline>();
                if (inst != null)
                {
                    return inst.Instantiate<T>();
                }
                var typeName = jObject["_type"];

                throw new InvalidDataException($"After deserializing, expected type {typeof(T).FullName} but got {typeName ?? obj.GetType().FullName}");

            }

            throw new InvalidDataException($"After deserializing, expected type {typeof(T).FullName} but got {obj.GetType().FullName}");

            //var typeName = obj["_type"];
            //(String)((Newtonsoft.Json.Linq.JObject)obj)["_type"];

            //return _PostLoadConvert<T>(obj);

        }

        public const string ContextPath = "Persistence/FileSystem";

        AutoRetryContext autoRetry => InjectionContextUtils.AsTypeInPathOrDefault<AutoRetryContext>(ContextPath);

        private void _SaveMethod(object obj, string assetSubPath, PersistenceContext context)
        {
            Type saveType = context?.SaveType ?? typeof(object);
            File.WriteAllText(GetPath(obj, assetSubPath, context), JsonConvert.SerializeObject(obj, saveType, JsonSettings));
        }
        public void Save(string assetSubPath, object obj, PersistenceContext context = null)
        {
            autoRetry.AutoRetry(() => _SaveMethod(obj, assetSubPath, context)).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        public async Task SaveAsync(string assetSubPath, object obj, PersistenceContext context = null) // FUTURE replacement
        {
            await autoRetry.AutoRetry(() => _SaveMethod(obj, assetSubPath, context)).ConfigureAwait(false);
        }

        public Task<bool> Initialize()
        {            
            if (RootDir == null)
            {
                RootDir = LionFireEnvironment.AppProgramDataDir;
            }

            // TODO: return ValidationContext
            return Task.FromResult(new ValidationContext()
                .IsTrue(!string.IsNullOrEmpty(RootDir), () => "RootDir not set after trying to set it from LionFireEnvironment.AppProgramDataDir")
                .IsTrue(!Directory.Exists(RootDir), () => "RootDir does not exist: " + RootDir)
                .IsValid)
                ;
        }
    }
}

namespace LionFire.Applications.Hosting
{

    public static class JsonAssetProviderExtensions
    {
        public static IAppHost AddJsonAssetProvider(this IAppHost app, string rootDir = null)
        {
            app.ServiceCollection.AddSingleton<IAssetProvider, JsonAssetProvider>(p => new JsonAssetProvider(rootDir));
            return app;
        }
    }

}