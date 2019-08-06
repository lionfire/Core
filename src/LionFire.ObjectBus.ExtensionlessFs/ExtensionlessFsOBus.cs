using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Persistence.Handles;
using LionFire.Referencing;

namespace LionFire.ObjectBus.ExtensionlessFs
{
    public class ExtensionlessFSOBus : SingletonOBusBase<ExtensionlessFSOBus, ExtensionlessFSOBase, ExtensionlessFileReference>
    {
        //public ExtensionlessFsOBus(IServiceProvider serviceProvider) : base(serviceProvider) { }
    }
}
