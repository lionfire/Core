#nullable enable

namespace LionFire.Structures
{
    public class NamedSingletonProviderOptions<TItem>
    {
        /// <summary>
        /// Not used if user specifies (object[])null as parameters
        /// </summary>
        public object[]? DefaultParameters { get; set; } = null;
        public object[]? AlwaysUseParameters { get; set; } = null;

    }

}

