#if OLD
using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;
using LionFire.Persistence.Handles;
using LionFire.Persistence;
using LionFire.ObjectBus.Handles;
using LionFire.Persistence.Filesystem;

namespace LionFire.ObjectBus.Filesystem
{
    public class FSOBus : SingletonOBusBase<FSOBus, FSOBase, FileReference>
    {
    }
}

#endif