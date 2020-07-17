using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Services;
using LionFire.Collections;

namespace LionFire.Discovery
{
    public abstract class DiscoveryServiceBase<InfoType, SettingsType> : IDiscoveryService<InfoType, SettingsType>
        where InfoType : class
        where SettingsType : class

    {
        public DiscoveryServiceBase()
        {
#if AOT
				throw new NotImplementedException();
#else
            itemsReadOnly = new MultiBindableCollection<InfoType>(items);
#endif
        }

        #region Items

        public MultiBindableCollection<InfoType> Items { get { return itemsReadOnly; } }
        private MultiBindableCollection<InfoType> itemsReadOnly;
        //public ReadOnlySynchronizedObservableCollection<InfoType> Items { get { return itemsReadOnly; } }
        //private ReadOnlySynchronizedObservableCollection<InfoType> itemsReadOnly;
        protected SynchronizedObservableCollection<InfoType> items = new SynchronizedObservableCollection<InfoType>();

        #endregion

        #region Settings

        public SettingsType Settings { get; set; }

        #endregion
    }
}
