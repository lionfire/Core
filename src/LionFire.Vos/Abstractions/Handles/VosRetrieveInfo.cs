
using LionFire.Persistence;
using LionFire.Vos.Mounts;

namespace LionFire.Vos
{
    public class VosRetrieveInfo 
        //: RetrieveInfo TOPORT
    {
        public IReadHandleBase<object> ReadHandle { get; set; }
        public Mount Mount { get; set; }
    }
}
