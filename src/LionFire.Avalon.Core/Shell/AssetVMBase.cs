using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Assets;
using LionFire.ObjectBus;
using LionFire.Shell;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using LionFire.Alerting;
using Microsoft.Extensions.Logging;

namespace LionFire.Assets
{
    /// <summary>
    /// TODO: Move this to a LionFire.Shell, and have multiple versions: one for WPF, one for others?
    /// This depends on ICommand.
    /// </summary>
    /// <typeparam name="AssetType"></typeparam>
    public abstract class AssetVMBase<AssetType> : IAssetVM
        where AssetType : class//, IAssetBase
    {
        IHAsset IAssetVM.HAsset { get { return HAsset; } }

        #region HAsset

        public HAsset<AssetType> HAsset
        {
            get { return hAsset; }
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
                OnPropertyChanged("HAsset");
            }
        }  private HAsset<AssetType> hAsset;

        protected virtual void OnHAssetChanged()
        {
            
        }

        #endregion

        public AssetType Asset
        {
            get { return HAsset == null ? null : HAsset.Object; }
        }

        #region Construction

        public AssetVMBase() {
            this.delete = new DelegateCommand(() =>
                {
                });
        }

        public AssetVMBase(HAsset<AssetType> hAsset) : this()
        {
            this.HAsset = hAsset;
        }

        #endregion

        #region Persistence

        public void Save()
        {
            try
            {
                HAsset.Save();
                //HAsset.Object.Save();
            }
            catch (Exception ex)
            {
                Alerter.Alert("Error saving", ex);
            }
        }

        public void Delete()
        {
            try
            {
                l.Trace("Delete clicked for " + HAsset);
                //FrameworkElement fe = sender as FrameworkElement;
                //if (fe == null) return;
                //Orbat loadout = fe.DataContext as Orbat;
                //if (loadout == null) return;
                //l.Info("Deleting loadout: " + loadout);
                HAsset.Delete();
            }
            catch (Exception ex)
            {
                Alerter.Alert("Exception when deleting", ex);
            }
        }
        public ICommand DeleteCommand { get { return delete; } }
        private ICommand delete;

        #endregion

        #region Misc

        public abstract string DisplayName { get; }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private static readonly ILogger l = Log.Get();
		
        #endregion
    }
}
