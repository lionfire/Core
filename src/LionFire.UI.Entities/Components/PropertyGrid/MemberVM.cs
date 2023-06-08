using LionFire.UI.Metadata;
using ReactiveUI;
using System;
using System.Reflection;

namespace LionFire.UI.Components.PropertyGrid;
public abstract class MemberVM : ReactiveObject
{
    #region (Static)

    public static IEnumerable<MemberVM> GetFor(object? o)
        => TypeInteractionModels.Get(o?.GetType())?.Members.Select(m => MemberVM.Create(m, o!)).Where(v => v is not IndexPropertyVM).ToList() ?? Enumerable.Empty<MemberVM>();

    public static MemberVM Create(MemberInfoVM memberInfoVM, object target)
    {
        if (memberInfoVM.MemberInfo is PropertyInfo pi)
        {
            if (pi.IsObservable())
            {
                return new ObservableVM { MemberInfoVM = memberInfoVM, Target = target };
            }
            if (pi.IsAsyncEnumerable())
            {
                return new AsyncEnumerableVM { MemberInfoVM = memberInfoVM, Target = target };
            }
            else
            {
                if (pi.GetIndexParameters().Length > 0)
                {
                    return new IndexPropertyVM { MemberInfoVM = memberInfoVM, Target = target };
                }
                else
                {
                    return new PropertyVM { MemberInfoVM = memberInfoVM, Target = target };
                }
            }
        }
        else if (memberInfoVM.MemberInfo is FieldInfo fi)
        {
            return new FieldVM { MemberInfoVM = memberInfoVM, Target = target };
        }
        else if (memberInfoVM.MemberInfo is MethodInfo mi)
        {
            return new MethodVM { MemberInfoVM = memberInfoVM, Target = target };
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    public abstract MemberKind MemberKind { get; }
    #region Dependencies

    public MemberInfoVM MemberInfoVM { get; set; }
    public object Target { get; set; }

    #endregion

    public bool ReadOnly { get; set; }

    
}
