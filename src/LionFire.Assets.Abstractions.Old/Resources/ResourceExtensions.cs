using LionFire.Assets;
using LionFire.Persistence;
using LionFire.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Resources
{
    /// <summary>
    /// Marker interface to indicate the type is intended for the LionFire.Resources framework
    /// </summary>
    public interface IResource
    {
    }
}

namespace LionFire.ExtensionMethods.Poco.Resources
{
    public static class PocoAssetExtensions
    {
        public static Task SaveResource(this object asset)
        {
            throw new NotImplementedException();
        }
    }
}
namespace LionFire.ExtensionMethods
{
    public static class ResourceExtensions
    {
        public static Task Save(this IResource asset)
        {
            throw new NotImplementedException();
        }
    }
}