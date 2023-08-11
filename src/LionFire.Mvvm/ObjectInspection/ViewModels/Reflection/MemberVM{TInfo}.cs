namespace LionFire.Mvvm.ObjectInspection;

public abstract class MemberVM<TInfo> : InspectorNode
    where TInfo : IInspectorMemberInfo
{
    public new TInfo Info =>(TInfo)base.Info;


    public MemberVM(TInfo info) : base(info)
    {
    }
}

public abstract class MemberVM<TInfo, TSource> : MemberVM<TInfo>
    where TInfo : IInspectorMemberInfo
    where TSource : notnull
{
    protected MemberVM(TInfo info, TSource state) : base(info)
    {
        TypedSource = state;
    }

    public TSource TypedSource { get; }
    public override object Source => TypedSource;
}