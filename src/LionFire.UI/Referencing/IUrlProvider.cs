using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    public interface IUrlProvider
    {
        string GetUrl(object obj);
    }
}
