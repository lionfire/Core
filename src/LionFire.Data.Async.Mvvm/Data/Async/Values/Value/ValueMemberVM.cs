
using LionFire.Data.Reactive;
using LionFire.Mvvm.ObjectInspection;

namespace LionFire.Data.Mvvm;

public class ValueMemberVM<T> : MemberVM<CustomMemberInfo, ValueVM<T>>
{
    public ValueMemberVM(CustomMemberInfo info, ValueVM<T> state) : base(info, state)
    {
    }

    //public override IODirection IODirection => IODirection.Read;
    public Reactive.IValueRx<T> Value { get; init; }
}