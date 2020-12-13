using LionFire.ExtensionMethods;
using System.Collections.Generic;
using System.ComponentModel;

namespace LionFire.UI.Windowing
{
    public class WindowProfile : INotifyPropertyChanged
    {
        public const string MainName = "_Main";

        //public WindowLayout MainWindow { get; set; }

        #region MainWindow

        public WindowLayout MainWindow
        {
            get => mainWindow;
            set
            {
                if (mainWindow != null) { mainWindow.PropertyChanged -= (s,e) => OnPropertyChanged(nameof(MainWindow)); }
                mainWindow = value;
                if (mainWindow != null) { mainWindow.PropertyChanged += (s,e) => OnPropertyChanged(nameof(MainWindow)); }
            }
        }
        private WindowLayout mainWindow;

        #endregion



        public Dictionary<string, WindowLayout> OtherWindows { get; set; }

        public WindowLayout GetWindow(string name)
        {
            if (name == MainName) { return MainWindow ??= new WindowLayout(); }

            if (OtherWindows == null) OtherWindows = new Dictionary<string, WindowLayout>();
            return OtherWindows.GetOrAdd(name, _ =>
            {
                var result = new WindowLayout();
                result.PropertyChanged += (s, e) => OnPropertyChanged(nameof(OtherWindows));
                return result;
            });
        }


        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

    }
}
