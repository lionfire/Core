using System.Reflection;
using System.Runtime;

namespace LionFire.Inspection;

public class BuiltInObjectInspectors : IInspector // TODO
{
    public IEnumerable<object> GetInspectedObjects(object @object)
    {
        if (@object is IObservable<object> o) { yield return new ObservableVM(o); }
        else if (@object is IAsyncEnumerable<object> ae) { yield return new AsyncEnumerableVM(ae); }
    }
}

public abstract class ReflectionMemberVM : MemberVM<ReflectionMemberInfo, object>, INode
{
    #region (Static)

    public static IEnumerable<ReflectionMemberVM> GetFor(object? o)
        => TypeInteractionModels.Get(o?.GetType())?.Members.Select(m => ReflectionMemberVM.Create(m, o!)).Where(v => v is not IndexPropertyVM).ToList() ?? Enumerable.Empty<ReflectionMemberVM>();

    public static ReflectionMemberVM Create(ReflectionMemberInfo memberInfoVM, object target)
    {
        // TODO: put the MemberInfo into ctor parameters

        if (memberInfoVM.MemberInfo is PropertyInfo pi)
        {
            if (pi.GetIndexParameters().Length > 0)
            {
                return new IndexPropertyVM(memberInfoVM, target);
            }
            else
            {
                return new PropertyVM(memberInfoVM, target);
            }
        }
        else if (memberInfoVM.MemberInfo is FieldInfo fi)
        {
            return new FieldVM(memberInfoVM, target);
        }
        else if (memberInfoVM.MemberInfo is MethodInfo mi)
        {
            return new MethodVM (memberInfoVM, target);
        }
        else if (memberInfoVM.MemberInfo is EventInfo ei)
        {
            return new EventVM (memberInfoVM, target);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Lifecycle

    protected ReflectionMemberVM(ReflectionMemberInfo info, object source) : base(info, source)
    {
    }

    #endregion
}
