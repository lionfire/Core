using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.ObjectBus // RENAME Referencing
{
    public delegate void ObjectChanged(IHandle handle, string propertyName);

}
