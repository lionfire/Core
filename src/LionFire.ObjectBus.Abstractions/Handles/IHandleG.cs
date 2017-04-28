using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
//#if !AOT  //150309 RECENTCHANGE - try this again  TOAOT  - comment?
    public interface IHandle<T> :
        IHandle,
        IHandlePersistence
        #if !AOT
        <T>
#endif
        ,
        IReadHandle
        #if !AOT
        <T>
#endif
        where T : class//, new()
    {
        void AssignFrom(T other, AssignmentMode assignmentMode);

        new T Object { get; set; }
        //T ObjectField { get; set; } // Set not needed?

        void ForgetObject();
        
    }
//#endif
}
