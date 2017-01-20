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
    public class JsonAssetProvider : FileSystemAssetProviderBase, IAssetProvider
    //, IFileExtensionHandler
    {

        public JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetSubPath"></param>
        /// <returns>Returns default(T) if file not found.</returns>
        public T Load<T>(string assetSubPath)
        {
            var path = GetPath<T>(assetSubPath);
            if (!File.Exists(path)) return default(T);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(File.ReadAllText(path), JsonSettings);
        }

        public const string ContextPath = "Persistence/FileSystem";

        AutoRetryContext autoRetry => InjectionContextUtils.AsTypeInPathOrDefault<AutoRetryContext>(ContextPath);

        public void Save(string assetSubPath, object obj)
        {
            autoRetry.AutoRetry(() => File.WriteAllText(GetPath(obj, assetSubPath), JsonConvert.SerializeObject(obj, JsonSettings))).ConfigureAwait(false).GetAwaiter().GetResult();

            //tryagain:
            //try
            //{
            //}
            //catch (IOException ioex)
            //{
            //    // REFACTOR into retry mechanism, and make it async
            //    if (retries > 0 && ioex.Message.Contains(" because it is being used by another process"))
            //    {
            //        retries--;
            //        System.Threading.Thread.Sleep(delay); 
            //        goto tryagain;
            //    }
            //}
        }
        public async Task SaveAsync(string assetSubPath, object obj) // FUTURE replacement
        {
            await autoRetry.AutoRetry(() => File.WriteAllText(GetPath(obj, assetSubPath), JsonConvert.SerializeObject(obj, JsonSettings))).ConfigureAwait(false);

            //int retries = 5;
            //int delay = 1000;
            //tryagain:
            //try
            //{
            //    File.WriteAllText(GetPath(obj, assetSubPath), JsonConvert.SerializeObject(obj, JsonSettings));
            //}
            //catch (IOException ioex)
            //{
            //    // REFACTOR into retry mechanism, and make it async
            //    if (retries > 0 && ioex.Message.Contains(" because it is being used by another process"))
            //    {
            //        retries--;
            //        await Task.Delay(delay);
            //        goto tryagain;
            //    }
            //}
        }
    }
}

namespace LionFire.Applications.Hosting
{

    public static class JsonAssetProviderExtensions
    {
        public static IAppHost AddJsonAssetProvider(this IAppHost app, string rootDir)
        {
            app.ServiceCollection.AddSingleton<IAssetProvider, JsonAssetProvider>(p => new JsonAssetProvider(rootDir));
            return app;
        }
    }

}