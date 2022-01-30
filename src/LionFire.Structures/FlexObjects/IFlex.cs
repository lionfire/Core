#nullable enable

namespace LionFire.FlexObjects
{
    public interface IFlex //: IWrapper<object>
    {
        object? FlexData { get; set; }
    }

    public interface IFlexWithMeta : IFlex
    {
        IFlex Meta { get; set; }
    }
}

