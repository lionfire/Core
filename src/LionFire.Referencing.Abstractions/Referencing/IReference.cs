using System.Collections.Generic;
using LionFire.DependencyInjection;
using LionFire.Structures;

namespace LionFire.Referencing
{
    /// <summary>
    /// Reference interface for all references: OBase, Vos, Handles.
    /// References should be round-trip convertible to and from a string key (from IKeyed)
    /// </summary>
    public interface IReference : IKeyed<string>, IMachineReference, ICompatibleWithSome<string>
    {
        string Scheme { get; }
        string Path { get; }

        #region TODO: MOVE to extension methods, and add IReferenceProvider?

        //IReferenceProvider ReferenceProvider { get; }

        /// <summary>
        /// Clones the reference and appends the path with the specified child name
        /// </summary>
        /// <param name="childName"></param>
        /// <returns></returns>
        IReference GetChild(string subPath);
        //IReference GetChildSubpath(params string[] subpath);
        IReference GetChildSubpath(IEnumerable<string> subpath);

        #endregion
    }

    

    public interface IReference<T> // REVIEW - is this type helpful?
    {
        IReference Reference { get; }
    }

}
