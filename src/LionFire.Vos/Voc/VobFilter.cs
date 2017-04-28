using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    public class VobFilter
    {
        public IEnumerable<Type> ExactTypes;

        /// <summary>
        /// Interface or base classes
        /// </summary>
        public IEnumerable<Type> AncestorTypes;

        public Predicate<Vob> FilterMethod;

        /// <summary>
        /// Requires IIsValid.IsValid == true
        /// </summary>
        public bool RequireIsValid;
    }
}
