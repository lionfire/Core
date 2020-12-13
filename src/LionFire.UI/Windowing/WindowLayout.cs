using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.UI.Windowing
{

    public class WindowLayout : INotifyPropertyChanged
    {
        public static WindowLayout CreateDefault
            => new WindowLayout
            {
                X = 100,
                Y = 100,
                Width = 900,
                Height = 700,
            };

        #region X

        public int X
        {
            get => x;
            set
            {
                if (x == value) return;
                x = value;
                OnPropertyChanged(nameof(X));
            }
        }
        private int x;

        #endregion

        #region Y

        public int Y
        {
            get => y;
            set
            {
                if (y == value) return;
                y = value;
                OnPropertyChanged(nameof(Y));
            }
        }
        private int y;

        #endregion

        #region Width

        public int Width
        {
            get => width;
            set
            {
                if (width == value) return;
                width = value;
                OnPropertyChanged(nameof(Width));
            }
        }
        private int width;

        #endregion

        #region Height

        public int Height
        {
            get => height;
            set
            {
                if (height == value) return;
                height = value;
                OnPropertyChanged(nameof(Height));
            }
        }
        private int height;

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
