

namespace LionFire.Mvvm.ObjectInspection;

public class ObservableVM : ObjectInspectionAdapter<IObservable<object>>
{
    //public override MemberKind MemberKind => MemberKind.Event;

    // ENH - get creative with UI for this
    public ObservableVM(IObservable<object> obj) : base(obj)
    {
    }
}
