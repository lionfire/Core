using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    public interface ITreeHandlePersistence // RENAME? to ITreeReadHandle
    {
        IEnumerable<IHandle> GetChildren();
#if !AOT
        IEnumerable<IHandle<ChildType>> GetChildrenOfType<ChildType>() where ChildType : class;
#endif
        IEnumerable<IHandle> GetChildrenOfType(Type childType);

        IEnumerable<string> GetChildrenNames(bool includeHidden = false);
        IEnumerable<string> GetChildrenNamesOfType<ChildType>() where ChildType : class,new();
        IEnumerable<string> GetChildrenNamesOfType(Type childType);
    }
}
