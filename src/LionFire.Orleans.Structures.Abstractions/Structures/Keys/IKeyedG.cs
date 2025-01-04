using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Orleans_;

public interface IKeyedG
{
    ValueTask<string> Key() => ValueTask.FromResult(((Orleans.IGrainWithStringKey)(object)this).GetPrimaryKeyString());
}
