namespace LionFire.Mvvm.ObjectInspection;

public abstract class MemberVM<TInfo> : MemberVM
    where TInfo : IInspectorMemberInfo
{
    public new TInfo Info =>(TInfo)base.Info;


    public MemberVM(TInfo info) : base(info)
    {
    }
}

public abstract class MemberVM<TInfo, TState> : MemberVM<TInfo>
    where TInfo : IInspectorMemberInfo
{
    protected MemberVM(TInfo info, TState state) : base(info)
    {
        State = state;
    }

    public TState State { get; }
}