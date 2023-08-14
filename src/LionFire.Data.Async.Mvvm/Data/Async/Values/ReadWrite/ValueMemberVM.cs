
using LionFire.Data.Async;
using LionFire.Inspection;

namespace LionFire.Data.Mvvm;

public class ValueMemberVM<T> : MemberVM<CustomMemberInfo, ValueVM<T>>
{
    public ValueMemberVM(CustomMemberInfo info, ValueVM<T> state) : base(info, state)
    {
    }

    //public override IODirection IODirection => IODirection.Read;
    public IValueRxO<T> Value { get; init; }
}