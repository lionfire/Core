using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.ObjectBus
{
    public interface IReferenceFactory : IUriProvider
    {
        IReference ToReference(string uri);
    }

}
