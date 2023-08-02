using ReactiveUI;
using LionFire.UI.Components.PropertyGrid;
using LionFire.Mvvm.ObjectInspection;

namespace LionFire.UI.Components;

public class PropertyGridRowsVM : ReactiveObject
{
    public PropertyGridVM PropertyGridVM { get; set; }
    public IEnumerable<ReflectionMemberVM> MemberVMs { get; set; }

    public IEnumerable<ReflectionMemberVM> VisibleMembers
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
