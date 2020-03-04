namespace LionFire.Assets
{
    public class AssetBase : AssetBase<object> { }

    public class AssetBase<TValue> : AssetBaseBase<TValue>
    {
        #region Key

        public override string Key
        {
            get => key;
            set
            {
                if (key == value) return;
                key = value;
                OnKeyChanged();
                OnPropertyChanged(nameof(Key));
            }
        }
        private string key;

        #endregion

     
    }
}
