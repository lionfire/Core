using LionFire.Structures;

namespace LionFire.FlexObjects
{
    public interface IFlex //: IWrapper<object>
    {
        object Value { get; set; } // RENAME - FlexData
    }

    public interface IFlexWithMeta : IFlex
    {
        IFlex Meta { get; set; }
    }
}

