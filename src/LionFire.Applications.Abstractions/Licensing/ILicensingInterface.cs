using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Licensing
{
    public interface ILicensingInterface
    {
        string TryRegister(string key);
    }
}
