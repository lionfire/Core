using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Structures
{
    public interface IDisplayNamable : IDisplayNamed
    {
        new string DisplayName { get; set; } 
    }

    public interface IDisplayNamed
    {
        string DisplayName { get; }
    }
}
