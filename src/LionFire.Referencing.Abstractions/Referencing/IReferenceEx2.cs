using System.Collections.Generic;

namespace LionFire.Referencing
{
    public interface IReferenceEx2 : IReference
    {
        /// <summary>
        /// Clones the reference and appends the path with the specified child name
        /// </summary>
        /// <param name="childName"></param>
        /// <returns></returns>
        IReference GetChild(string subPath);
        //IReference GetChildSubpath(params string[] subpath);
        IReference GetChildSubpath(IEnumerable<string> subpath);
    }
}
