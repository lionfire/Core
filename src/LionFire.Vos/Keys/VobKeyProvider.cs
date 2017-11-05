using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LionFire.ObjectBus
{

    //public static class VobMetaData
    //{
    //    public static VobHandle<T> GetMetaData<T>(Vob vob)
    //        where T : class, new()
    //    {
    //        VobHandle<T> vh = vob[VosPaths.MetaDataSubPath].ToHandle<T>();
    //        vh.RetrieveOrCreateDefault(() => VobKeyProvider.CreateDefault(vob));
    //        return vh;
    //    }
    //}


    public class VobKeyProvider // MOVE to vob namespace?
    {
        public static VobHandle<IVobKeyProvider> GetOrCreateDefault(Vob vob)
        {
            VobHandle<IVobKeyProvider> vh = vob[VosPaths.MetaDataSubPathPrefix + VobStringKeyProvider.MetaDataSlot].ToHandle<IVobKeyProvider>(); // HARDCODE HARDTYPE
            if (!vh.TryEnsureRetrieved())
            {
                vh = CreateDefault(vob);
            }
            
            return vh;
        }

        public static VobHandle<IVobKeyProvider> CreateDefault(Vob vob)
        {
            //VobHandle<IVobKeyProvider> vh = vob[VosPaths.MetaDataSubPath].ToHandle<IVobKeyProvider>();

            return new VobStringKeyProvider(vob).VobHandle;
        }
    }

    /// <summary>
    /// Unnamed metadata decorator.
    /// Goes in /path/to/vob/._/(VobStringKeyProvider)
    /// 
    /// TODO: Don't allow changing mode once set.
    /// </summary>
    public class VobStringKeyProvider : Vobo<VobStringKeyProvider, IVobKeyProvider>, IVobKeyProvider
    {
        public static string MetaDataSlot { get {
            return "KeyProvider";
            //return typeof(IVobKeyProvider).Name;
        } }

        #region Fields

        [Ignore]
        private object sync = new object();

        #endregion

        #region State

        [SerializeDefaultValue(false)]
        public List<int> ReturnedKeys { get; set; }
        public int NextKey = 1;

        #endregion

        #region Configuration

        public KeyProviderMode Mode
        {
            get
            {
                if (mode == KeyProviderMode.Unspecified)
                {
                    mode = KeyProviderMode.Increment;
                }
                return mode;
            }
            set
            {
                if (mode != KeyProviderMode.Unspecified && mode != value)
                {
                    throw new ArgumentException("Mode cannot be changed after it has been set");
                }
                mode = value;
            }
        }
        private KeyProviderMode mode = KeyProviderMode.Unspecified;

        //private static KeyProviderMode DefaultMode = KeyProviderMode.Increment;  UNUSED

        #endregion

        #region Construction

        public VobStringKeyProvider()
        {
        }

        #endregion

        private void QueueSave()
        {
            ThrottledSaveManager.Instance.OnChanged(this);
        }

        public void ReturnKey(object key)
        {
            switch (Mode)
            {
                case KeyProviderMode.Increment:
                    if (ReturnedKeys != null) ReturnedKeys = new List<int>();
                    int i = -1;
                    try
                    {
                        i = Convert.ToInt32(key);
                    }
                    catch { }
                    if (i < NextKey && i > 0)
                    {
                        ReturnedKeys.Add(i);
                        QueueSave();
                    }
                    break;
                case KeyProviderMode.Guid:
                    break;
                case KeyProviderMode.Unspecified:
                default:
                    throw new NotSupportedException("Unsupported mode: " + Mode.ToString());
            }
        }

        public VobStringKeyProvider(Vob vob)
            : base(vob, LionPath.Combine(VosPaths.MetaDataSubPathPrefix, MetaDataSlot))
        {
        }

        public object GetNextKey()
        {
            return GetNextKeyAsString();
        }
        public string GetNextKeyAsString()
        {
            switch (Mode)
            {
                case KeyProviderMode.Increment:
                    lock (sync)
                    {
                        var key = NextKey.ToString();
                        NextKey++;
                        QueueSave();
                        return key;
                    }
                case KeyProviderMode.Guid:
                    return Guid.NewGuid().ToString();
                case KeyProviderMode.Unspecified:
                default:
                    throw new NotSupportedException("Unsupported mode: " + Mode.ToString());
            }
        }

    }
}
