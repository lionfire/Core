using System.Threading.Tasks;
using LionFire.Assets;
using LionFire.Persistence;
using LionFire.Persistence.Handles;

namespace LionFire.Resources
{
    public class HRResource<T> : RBaseEx<T>
        where T : class
    {
        public static implicit operator HRResource<T>(string resourcePath)
        {
            return new HRResource<T>(resourcePath);
        }

        public HRResource(string resourcePath) : base(new AssetReference<T>(resourcePath)) { }

        public override Task<IRetrieveResult<T>> RetrieveImpl() => throw new System.NotImplementedException();
    }
}
