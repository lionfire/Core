
namespace LionFire.Mvvm.ObjectInspection;

public class AsyncEnumerableVM : ObjectInspectionAdapter<IAsyncEnumerable<object>>
{
    //public override MemberKind MemberKind => MemberKind.Event;
    // ENH - get creative with UI for this
    public AsyncEnumerableVM(IAsyncEnumerable<object> obj) : base(obj)
    {
    }
}