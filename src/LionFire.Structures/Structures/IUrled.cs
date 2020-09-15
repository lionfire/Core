using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Structures
{
    public interface IUrled
    {
        string Url { get; }
    }

    public interface IUrlable : IUrled
    {
        new string Url { set; }
    }

}
