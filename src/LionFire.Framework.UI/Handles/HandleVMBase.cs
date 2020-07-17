using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using LionFire.Alerting;
using LionFire.Persistence;
using Microsoft.Extensions.Logging;
using LionFire.ExtensionMethods.Persistence;
using LionFire.Avalon;

namespace LionFire.UI
{
    public abstract class AssetVMBase<AssetType> // RENAME Asset to ReadWriteHandleVM once everything is compiling
        where AssetType : class
    {
        //IHAsset IAssetVM.HAsset { get { return HAsset; } }

        public IReadWriteHandle<AssetType> Handle => HAsset;
        public IReadWriteHandle<AssetType> HAsset
        {
            get => hAsset;
            set
            {
                if (hAsset == value) return;

                var oldValue = hAsset;

                hAsset = value;

                if (oldValue != null)
                {
                    l.Trace("OnHAssetChanged from " + oldValue + "  to " + HAsset);
                }

                OnHAssetChanged();
                OnPropertyChanged(nameof(HAsset));
            }
        }
        private IReadWriteHandle<AssetType> hAsset;

        protected virtual void OnHAssetChanged()
        {
        }

        public AssetType Asset => HAsset?.Value;

        #region Construction

        public AssetVMBase()
        {
            this.DeleteCommand = new LionDelegateCommand(_ =>
            {
            });
        }

        public AssetVMBase(IReadWriteHandle<AssetType> hAsset) : this()
        {
            this.HAsset = hAsset;
        }

        #endregion

        #region Persistence

        public async Task Save()
        {
            try
            {
                await HAsset.Save().ConfigureAwait(false);
                //HAsset.Object.Save();
            }
            catch (Exception ex)
            {
                Alerter.Alert("Error saving", ex);
            }
        }

        public async Task Delete()
        {
            try
            {
                l.Trace("Delete clicked for " + HAsset);
                //FrameworkElement fe = sender as FrameworkElement;
                //if (fe == null) return;
                //Orbat loadout = fe.DataContext as Orbat;
                //if (loadout == null) return;
                //l.Info("Deleting loadout: " + loadout);
                await HAsset.Delete().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Alerter.Alert("Exception when deleting", ex);
            }
        }
        public ICommand DeleteCommand { get; private set; }

        #endregion

        #region Misc

        public abstract string DisplayName { get; }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        private static readonly ILogger l = Log.Get();

        #endregion
    }
}
