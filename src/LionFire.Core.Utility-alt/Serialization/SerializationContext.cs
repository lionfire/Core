using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Serialization
{
    public class SerializationContext : IDisposable
    {

        #region Static

        [ThreadStatic]
        public static Stack<SerializationContext> Stack = new Stack<SerializationContext>();

        #endregion

        #region Construction and Destruction

        public SerializationContext()
        {
            if (Stack == null) Stack = new Stack<SerializationContext>();
            Stack.Push(this);

        }

        public void Dispose()
        {
#if SanityChecks
            if (Stack.Pop() != this) throw new UnreachableCodeException("Stack.Pop() != this");
#else
            Stack.Pop();
#endif
        }

        #endregion

        public Predicate<MemberInfo> MemberFilter = null;

        #region FUTURE: Settings, perhaps with an inheritance overlay stack

        //public bool Fields = false;
        //public bool Properties = true;
        //public string Format;

        ///// <summary>
        ///// For JSON
        ///// </summary>
        //public bool HumanReadable = true;

        //public bool SelfDocumenting = true;

        //public bool SerializeDefaultValues;

        //public bool SerializeDefaultValuesAsComments;
        #endregion

    }
}
