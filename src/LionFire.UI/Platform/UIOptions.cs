using System.Drawing;

namespace LionFire.UI
{
    public class UIOptions
    {
        public DisplayKind DisplayKind { get; set; } = DisplayKind.PC;

        public virtual Size DefaultWindowedSize
        {
            get
            {
                switch (DisplayKind)
                {
                    case DisplayKind.Unspecified:
                    case DisplayKind.PC:
                    default:
                        return new Size(1368, 768);
                    case DisplayKind.Tablet:
                    case DisplayKind.Phone:
                    case DisplayKind.Television:
                        return new Size(1024, 768);
                }
            }
        }
    }
}

