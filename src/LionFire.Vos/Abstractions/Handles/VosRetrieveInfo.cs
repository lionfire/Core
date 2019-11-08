
using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.Vos
{
    public class VosRetrieveInfo 
        //: RetrieveInfo TOPORT
    {
        public IReadHandleBase<object> ReadHandle { get; set; }
        public Mount Mount { get; set; }
    }
}
