using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Structures
{
    public interface IEnableable
    {
        bool IsEnabled { get; set; }
        // TODO: public event Action<IEnableable> IsEnabledChangedFor;
    }
}
