#nullable enable

namespace LionFire.FlexObjects
{
    public interface IFlex //: IWrapper<object>
    {
        /// <summary>
        /// Avoid using this directly for normal usage.  Instead, use the Flex extension methods.
        /// Implementors should implement this explicitly, so users of the class do not access it directly.
        /// </summary>
        object? FlexData { get; set; }
    }

    public interface IFlexWithMeta : IFlex
    {
        IFlex Meta { get; set; }
    }
}

