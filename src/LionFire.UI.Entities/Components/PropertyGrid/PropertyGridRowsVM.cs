using ReactiveUI;
using LionFire.UI.Components.PropertyGrid;
using LionFire.Mvvm.ObjectInspection;

namespace LionFire.UI.Components;

public class PropertyGridRowsVM : ReactiveObject
{
    public PropertyGridVM PropertyGridVM { get; set; }
    public IEnumerable<MemberVM> MemberVMs { get; set; }

    public IEnumerable<MemberVM> VisibleMembers
    {
        get
        {
            if (PropertyGridVM.ShowDataMembers)
            {
                foreach (var m in MemberVMs.Where(m => m.MemberKind == MemberKind.Data))
                {
                    yield return m;
                }
            }
            if (PropertyGridVM.ShowEvents)
            {
                foreach (var m in MemberVMs.Where(m => m.MemberKind == MemberKind.Event))
                {
                    yield return m;
                }
            }
            if (PropertyGridVM.ShowMethods)
            {
                foreach (var m in MemberVMs.Where(m => m.MemberKind == MemberKind.Method))
                {
                    yield return m;
                }
            }
        }
    }
}
