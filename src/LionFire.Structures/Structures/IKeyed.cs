//#nullable enable
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Structures;

public interface IKeyed : IKeyed<string> { }

public interface IKeyed<TKey>
{
    TKey? Key { get; }
}
