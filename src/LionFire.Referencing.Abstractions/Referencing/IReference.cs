using System.Collections.Generic;
using LionFire.Dependencies;
using LionFire.Structures;

namespace LionFire.Referencing
{
    /// <summary>
    /// Reference interface for all references: OBase, Vos, Handles.
    /// References should be round-trip convertible to and from a string key (from IKeyed)
    /// </summary>
    public interface IReference : IKeyed<string>
        //, IHostReference // RECENTCHANGE: Don't depend on IHostReference
        , ICompatibleWithSome<string>
    {
        string Scheme { get; }

        /// <summary>
        /// Name of the persister to be used for this reference.  For example, each SQL database might have its own name, which in turn is configured with a paricular connection string.  Some handle providers, such as Filesystem, may have a default persister with a null (or empty) name.
        /// </summary>
        string Persister { get; }

        string Path { get; }
    }

    public interface IReference<T> // REVIEW - is this type helpful?  If so, document what T is for here
    {
        IReference Reference { get; }
    }

}
