using System.Collections.Generic;
using LionFire.Dependencies;
using LionFire.Structures;

namespace LionFire.Referencing
{
    /// <summary>
    /// Reference interface for all references: OBase, Vos, Handles.
    /// References should be round-trip convertible to and from a string key (from IKeyed)
    /// </summary>
    public interface IReference :
        IUrled
       , IKeyed<string>// - old - is a bonus now.
    //, IHostReference // RECENTCHANGE: Don't depend on IHostReference
    //, ICompatibleWithSome<string>
    {
        string Scheme { get; }

        string Path { get; }
    }

    public interface IReference<out TValue> : IReference
    {
    }
}
