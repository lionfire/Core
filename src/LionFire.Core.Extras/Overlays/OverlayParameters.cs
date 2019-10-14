#if !NET35
#endif

namespace LionFire.Overlays
{
    public class OverlayParameters
    {

        public bool DisallowAdd;
        public bool DisallowInsert;
        public OverlayPrecedence Precedence;
        public OverlaySetTarget SetTarget;

        public object DefaultObject;

        /// <summary>
        /// If DefaultObject is not specified, it will be created upon Overlay Instantiation.
        /// </summary>
        public bool AutoCreateDefaultObject;

        ///// <summary>
        ///// If true and no overlays have values and DefaultObject is null, gets will return default(PropertyType).
        ///// Otherwise, a default will be created using LionFire.Extensions.Defaults.DefaultExtensions.SetDefaults(defaultInstance);
        ///// </summary>
        //public bool AllowNoDefault;

        // FUTURE:
        ///// <summary>
        ///// If true and no overlays have values and DefaultObject is null, gets that are reference types will return null and gets that return value types will throw exceptions.
        ///// 
        ///// </summary>
        //public bool ThrowOnNoDefault;

        ///// <summary>
        ///// If true and no overlays have values and DefaultObject is null, gets will throw
        ///// 
        ///// </summary>
        //public bool ThrowOnNoDefaultOrNull;

        public static OverlayParameters UnlockedTopDown
        {
            get
            {
                return unlockedTopDown;
            }
        }

        private static readonly OverlayParameters unlockedTopDown = new OverlayParameters()
        {
            AutoCreateDefaultObject = true,
            DisallowAdd = false,
            DisallowInsert = false,
            SetTarget = OverlaySetTarget.Top,
            Precedence = OverlayPrecedence.Top,
        };

        public static OverlayParameters LockedBottomUp
        {
            get
            {
                return lockedBottomUp;
            }
        }
        private static readonly OverlayParameters lockedBottomUp = new OverlayParameters()
        {
            AutoCreateDefaultObject = true,
            DisallowAdd = false,
            DisallowInsert = true,
            SetTarget = OverlaySetTarget.Top,
            Precedence = OverlayPrecedence.Bottom,
        };
    }
}
