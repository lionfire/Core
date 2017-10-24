using System.Collections.Generic;
using LionFire.Structures;
using LionFire.Serialization;
using System.IO;
using LionFire.ExtensionMethods;
using LionFire.Handles;
using System.ComponentModel;
using LionFire.Assets;
using System.Threading.Tasks;
using LionFire.Persistence.Assets;

namespace LionFire.Assets
{
    public class LiveAssetCollection<TAsset> : ObservableHandleDictionary<string, AssetHandle<TAsset>, TAsset>, INotifyPropertyChanged
    where TAsset : class
    {
        #region (Static) Instance

        public static LiveAssetCollection<TAsset> Instance => instance;
        private static LiveAssetCollection<TAsset> instance = new LiveAssetCollection<TAsset>();
        
        #endregion

        protected LiveAssetCollection()
        {
            //fsObjects.RootPath = Path.Combine(LionFireEnvironment.AppProgramDataDir, "HeartMonitors"); //  Use asset ifranstructure to set this more automatically
        }


        #region Cached Properties

        #region AssetProvider

        public IAssetProvider AssetProvider
        {
            get { if (assetProvider == null) { assetProvider = Injection.GetService<IAssetProvider>(); } return assetProvider; }
        }
        private IAssetProvider assetProvider;

        #endregion

        #endregion

        public override async Task RefreshHandles()
        {
            handles.SetToMatch((await AssetProvider.FindHandles<TAsset>()).ToDictionary<string, AssetHandle<TAsset>>());
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
