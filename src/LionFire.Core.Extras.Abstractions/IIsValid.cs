using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire
{
    public interface IIsValid
    {
        bool IsValid { get; }
        event Action<IIsValid> IsValidChangedFor;
    }
}
