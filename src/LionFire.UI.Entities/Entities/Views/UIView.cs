#if NOESIS
using Noesis;
#else
#define Windowing
#endif
using LionFire.UI.Entities;

namespace LionFire.UI
{
    public class UIView<T> : UIKeyed, IViewEntity
        where T : class
    {

        #region View

        public T View
        {
            get => view;
            set
            {
                if (view == value) return;
                view = value;
                OnPropertyChanged(nameof(View));
            }
        }
        private T view;
        object IViewEntity.View { get => View; set => View = (T)value; }
        
        #endregion

    }
}
