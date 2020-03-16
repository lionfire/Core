using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Serialization
{
    
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public sealed class UseAliasesForSerializationAttribute : Attribute
    {
        public int a;
        public UseAliasesForSerializationAttribute() { a = 4; }
    }
}
