using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Orleans_;

public interface IAsyncEnumerableGrain<out T>
{
    IAsyncEnumerable<T> Items();
}
