using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LionFire.UI.Windowing
{
    public class WpfDesktopProfileDetector : IHostedService
    {
        #region Dependencies

        public DesktopProfileManager DesktopProfileManager { get; }

        #endregion

        #region Construction

        public WpfDesktopProfileDetector(DesktopProfileManager desktopProfileManager)
        {
            DesktopProfileManager = desktopProfileManager;
        }

        #endregion

        #region (Private) Methods

        private void UpdateCurrentProfile()
        {
            var cur = DesktopProfileManager.CurrentProfile;
            if (cur == null
                || cur.DesktopHeight !=  (int)SystemParameters.VirtualScreenHeight
                || cur.DesktopWidth != (int)SystemParameters.VirtualScreenWidth)
            {
                DesktopProfileManager.CurrentProfile = new DesktopProfile(SystemParameters.VirtualScreenHeight, SystemParameters.VirtualScreenWidth);
            }
        }

        #endregion

        #region Event Handling

        private void SystemParameters_StaticPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SystemParameters.VirtualScreenWidth):
                case nameof(SystemParameters.VirtualScreenHeight):
                    UpdateCurrentProfile();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region IHostedService

        public Task StartAsync(CancellationToken cancellationToken)
        {
            UpdateCurrentProfile();
            SystemParameters.StaticPropertyChanged += SystemParameters_StaticPropertyChanged;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            SystemParameters.StaticPropertyChanged -= SystemParameters_StaticPropertyChanged;
            return Task.CompletedTask;
        }

        #endregion
    }
}
