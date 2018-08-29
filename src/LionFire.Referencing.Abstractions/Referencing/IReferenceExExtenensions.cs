using System.Collections.Generic;

namespace LionFire.Referencing
{
    public static class IReferenceExExtenensions
    {
        public static IReference GetChildSubpath(this IReferenceEx2 reference, params string[] subpath)
        {
            return reference.GetChildSubpath((IEnumerable<string>)subpath);
        }
    }
}
