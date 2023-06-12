using LionFire.MultiTyping;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Structures;

public class ComponentsViaMultiType : IComponentizable 
{
    #region Lifecycle

    public ComponentsViaMultiType()
    {
        MultiType = new();
    }
    public ComponentsViaMultiType(MultiType multiType)
    {
        MultiType = multiType;
    }

    #endregion

    #region State

    public MultiType MultiType { get; private set; }

    #endregion

    #region IComponentizable

    public T GetOrAddComponent<T>() where T : class => MultiType.AsTypeOrCreateDefault<T>();

    public T TryGetComponent<T>() where T : class => MultiType.AsType<T>();
    
    #endregion
}
