using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;
using LionFire.Services;
using Microsoft.Extensions.Hosting;

namespace LionFire.Discovery
{
    public interface IDiscoveryService<InfoType, SettingsType> 
        where InfoType : class
        where SettingsType : class
    {
        //ReadOnlySynchronizedObservableCollection<InfoType> Items { get; }
        MultiBindableCollection<InfoType> Items { get; }

        SettingsType Settings { get; set; }
    }
}
