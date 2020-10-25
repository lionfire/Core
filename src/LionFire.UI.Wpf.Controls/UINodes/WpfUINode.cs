#if NOESIS
using Noesis;
#else
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms.VisualStyles;
#endif

namespace LionFire.UI.Wpf
{
    public class WpfUINode : IUINode
    {

        #region View

        public UIElement View
        {
            get => view;
            set
            {
                if (view == value) return;
                view = value;
                OnPropertyChanged(nameof(View));
            }
        }
        private UIElement view;

        object IUINode.View
        {
            get => View;
            set => View = (UIElement)value;
        }

        #endregion


        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

    }
}
