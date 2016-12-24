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
            
        };

        //public override IEnumerable<string> FileExtensions { get { yield return "json"; } }
        public override string FileExtension { get { return "json"; } }
      //  public string FileExtensionWithDot { get { return ".json"; } }

        public JsonAssetProvider(string rootDir) : base(rootDir)
        {
        }
        
        public T Load<T>(string assetSubPath)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(File.ReadAllText(GetPath<T>(assetSubPath)));
        }

        public void Save<T>(string assetSubPath, T obj)
        {
            File.WriteAllText(GetPath<T>(assetSubPath), JsonConvert.SerializeObject(obj, JsonSettings));
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