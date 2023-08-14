using LionFire.Inspection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectInspection;


public class ReflectionInspector : IInspector
{
    public IObservableCache<IInspectorNode, string> GroupsForObject(object @object)
    {
        
    }

    public bool IsSourceSubscribable(object source)
    {
        throw new NotImplementedException();
    }

    IEnumerable<IObservableCache<IInspectorNode, string>> IInspector.GroupsForObject(object @object)
    {
        throw new NotImplementedException();
    }
}
public class FlexInspector : IInspector
{
    public IObservableCache<IInspectorNode, string> GroupsForObject(object @object)
    {
        throw new NotImplementedException();
    }

    public bool IsSourceSubscribable(object source)
    {
        throw new NotImplementedException();
    }

    IEnumerable<IObservableCache<IInspectorNode, string>> IInspector.GroupsForObject(object @object)
    {
        throw new NotImplementedException();
    }
}

public class GrainInspector : IInspector
{
    public IObservableCache<IInspectorNode, string> GroupsForObject(object @object)
    {
        throw new NotImplementedException();
    }

    public bool IsSourceSubscribable(object source)
    {
        throw new NotImplementedException();
    }

    IEnumerable<IObservableCache<IInspectorNode, string>> IInspector.GroupsForObject(object @object)
    {
        throw new NotImplementedException();
    }
}

public class VobInspector : IInspector
{
    public IObservableCache<IInspectorNode, string> GroupsForObject(object @object)
    {
        throw new NotImplementedException();
    }

    public bool IsSourceSubscribable(object source)
    {
        throw new NotImplementedException();
    }

    IEnumerable<IObservableCache<IInspectorNode, string>> IInspector.GroupsForObject(object @object)
    {
        throw new NotImplementedException();
    }
}
