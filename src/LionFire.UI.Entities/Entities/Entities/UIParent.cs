using System;

namespace LionFire.UI
{
    public class UIParent : UIObject
    {

        #region Child

        public IUIObject Child
        {
            get => child;
            set
            {
                if (child == value) return;
                if (value != null && child != default(IUIObject)) throw new NotSupportedException("Child can only be set once or back to null.");
                child = value;
            }
        }
        private IUIObject child;

        #endregion

    }
}
