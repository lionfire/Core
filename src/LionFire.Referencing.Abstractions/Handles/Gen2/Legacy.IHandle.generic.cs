using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
//#if !AOT  //150309 RECENTCHANGE - try this again  TOAOT  - comment?
    public interface ILegacyHandle<T> :
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
#if LEGACY
        void AssignFrom(T other, AssignmentMode assignmentMode);
#endif

        new T Object { get; set; }
        //T ObjectField { get; set; } // Set not needed?

        //void ForgetObject();
        
    }
//#endif
}
