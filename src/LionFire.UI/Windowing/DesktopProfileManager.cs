using LionFire.Dependencies;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;

namespace LionFire.UI.Windowing
{
    public class DesktopProfileManager : INotifyPropertyChanged
    {
        public static DesktopProfileManager Instance => DependencyContext.Current.GetService<DesktopProfileManager>();

        #region CurrentProfile

        public DesktopProfile CurrentProfile
        {
            get => currentProfile;
            set
            {
                if (currentProfile == value) return;
                currentProfile = value;

                l.LogInformation("Current DesktopProfile:" + currentProfile);

                OnPropertyChanged(nameof(CurrentProfile));
                CurrentProfileChanged?.Invoke();
            }
        }
        private DesktopProfile currentProfile;

        public event Action CurrentProfileChanged;

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion


        private static ILogger l = Log.Get();

        // FUTURE: Desktop size change detection
        //  - https://stackoverflow.com/questions/17789414/keep-track-of-screen-change-and-screen-resolution-change-in-windows-form-applica
    }
}
