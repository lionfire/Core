using Orleans.Http.Abstractions;
using System.Threading.Tasks;

namespace LionFire.Metaverse
{
    public interface IMetaverseHello : Orleans.IGrainWithStringKey
    {
        [HttpGet]
        Task<string> SayHelloFromMetaverse([FromQuery] string greeting);
    }
}