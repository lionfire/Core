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

namespace LionFire.Assets.Providers.FileSystem
{

    public class JsonAssetProvider : FileSystemAssetProviderBase, IAssetProvider
        //, IFileExtensionHandler
    {
        
        public JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            
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
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }

        public void Save<T>(string assetSubPath, T obj)
        {
            File.WriteAllText(GetPath(obj, assetSubPath), JsonConvert.SerializeObject(obj, JsonSettings));
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