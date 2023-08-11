using ReactiveUI;
using LionFire.UI.Components.PropertyGrid;
using LionFire.Mvvm.ObjectInspection;
using LionFire.Data.Async.Gets;

namespace LionFire.UI.Components;

public class PropertyGridRowsVM : ReactiveObject
{
    public ObjectInspectorService ObjectInspectorService { get; }

    public PropertyGridRowsVM(ObjectInspectorService objectInspectorService)
    {
        ObjectInspectorService = objectInspectorService;
    }

    public PropertyGridVM PropertyGridVM { get; set; }
    public IEnumerable<IInspectorNode> MemberVMs { get; set; }

    public IGetterRxO<IEnumerable<object>> MembersGetter { get; set; }

    public IEnumerable<IInspectorNode> VisibleMembers
    {
        get
        {
            if (PropertyGridVM.ShowDataMembers)
            {
                foreach (var m in MemberVMs.Where(m => m.Info.MemberKind == MemberKind.Data))
                {
                    yield return m;
                }
            }
            if (PropertyGridVM.ShowEvents)
            {
                foreach (var m in MemberVMs.Where(m => m.Info.MemberKind == MemberKind.Event))
                {
                    yield return m;
                }
            }
            if (PropertyGridVM.ShowMethods)
            {
                foreach (var m in MemberVMs.Where(m => m.Info.MemberKind == MemberKind.Method))
                {
                    yield return m;
                }
            }
        }
    }

    
}
