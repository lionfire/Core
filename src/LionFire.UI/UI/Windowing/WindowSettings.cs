using LionFire.ExtensionMethods;
using LionFire.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.UI.Windowing
{
    public class WindowSettings : INotifyPropertyChanged, INotifyDeserialized
    {
        public const string DefaultWindowName = "DefaultWindow";

        public Dictionary<string, WindowProfile> Profiles { get; set; } = new Dictionary<string, WindowProfile>();

        public WindowSettings()
        {
            System.Diagnostics.Debug.WriteLine("WindowSettings ctor");
        }

        public WindowProfile GetProfile(int desktopWidth, int desktopHeight)
        {
            var key = new DesktopProfile(desktopWidth, desktopHeight).Key;
            bool created = false;
            var profile = Profiles.GetOrAdd(key, _ =>
            {
                var obj = new WindowProfile();
                obj.PropertyChanged += (s, e) => OnPropertyChanged(nameof(Profiles));
                created = true;
                return obj;
            });
            if (created) OnPropertyChanged(nameof(Profiles));
            return profile;
        }


        #region INotifyPropertyChanged Implementation

        //public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler PropertyChanged { add
            {
                propertyChanged += value;
            }
            remove
            {
                propertyChanged -= value;
            }
        }
        private event PropertyChangedEventHandler propertyChanged;



        protected void OnPropertyChanged(string propertyName) => propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public void OnDeserialized()
        {
            if (Profiles != null)
            {
                foreach (var profile in Profiles?.Values)
                {
                    profile.PropertyChanged += (s, e) => OnPropertyChanged(nameof(Profiles));
                }
            }
        }

        #endregion

    }
}