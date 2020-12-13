using LionFire.Referencing;
using System;

namespace LionFire.UI
{
    public class UIKeyed : UIObject, IUIKeyed
    {
        public string Path => LionPath.Combine((Parent as IUIKeyed)?.Path, Key);

        #region Key

        [SetOnce]
        public string Key
        {
            get => key;
            set
            {
                if (key == value) return;
                if (key != default) throw new AlreadySetException();
                if (key == string.Empty) throw new ArgumentException("Key cannot be empty");
                key = value;
            }
        }
        protected string key;

        #endregion
    }
}
