namespace LionFire.Mvvm.ObjectInspection;

public abstract class MemberVM : ReactiveObject, IMemberVM
{
    #region Relationships

    public IInspectorMemberInfo Info { get;  }

    IInspectorMemberInfo IMemberVM.Info => Info;

    #endregion

    protected MemberVM(IInspectorMemberInfo info)
    {
        Info = info;
    }
}
