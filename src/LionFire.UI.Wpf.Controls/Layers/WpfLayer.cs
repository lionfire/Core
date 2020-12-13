#if NOESIS
using Noesis;
#else
#define Windowing
#endif
using LionFire.UI.Entities;

namespace LionFire.UI
{
    public class WpfLayer : UIKeyed, ILayer
    {

        public object View { get; set; }
    }


}
