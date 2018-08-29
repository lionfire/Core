using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Referencing
{
    public interface IUriProvider
    {
        string[] UriSchemes { get; }
    }
}
