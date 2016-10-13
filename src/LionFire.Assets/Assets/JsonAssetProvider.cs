using LionFire.Applications.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using LionFire.Assets;

namespace LionFire.Assets
{

    public interface IFileExtensionHandler
    {
        IEnumerable<string> FileExtensions { get; }
    }

    public class JsonAssetProvider : IAssetProvider, IFileExtensionHandler
    {
        public IEnumerable<string> FileExtensions { get { yield return "json"; } }

        public string RootDir { get; set; }

        public JsonAssetProvider(string rootDir)
        {
            RootDir = rootDir;
        }
       

        public string GetPath<T>(string assetSubpath)
        {
            return Path.Combine(RootDir, AssetPathUtils.GetSubpath<T>(assetSubpath)) + ".json"; ;
        }
        
        public T Load<T>(string assetSubPath)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(File.ReadAllText(GetPath<T>(assetSubPath)));
        }

        public void Save<T>(string assetSubPath, T obj)
        {
            File.WriteAllText(GetPath<T>(assetSubPath), JsonConvert.SerializeObject(obj));
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