using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.MultiTyping;

namespace LionFire.Vos
{
    public partial class Vob
    {
        
        [AotReplacement]
        public object AsTypeOrCreate(Type type) => throw new NotImplementedException("Vob.AsTypeOrCreate");

        //#region IReadOnlyMultiTyped OLD

        //public object this[Type type]
        //{
        //    get { return handle[type]; }
        //}

        //public ChildType AsType<ChildType>() where ChildType : class
        //{
        //    return handle.AsType<ChildType>();
        //}

        //public ChildType[] OfType<ChildType>() where ChildType : class
        //{
        //    return handle.OfType<ChildType>();
        //}

        //public IEnumerable<object> SubTypes
        //{
        //    get { return handle.SubTypes; }
        //}

        //#endregion

        //public Vob<T> FilterAsType<T>()
        //{
        //}
    }
}
