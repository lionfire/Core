#nullable enable

namespace LionFire.Structures
{
    public class NamedSingletonProviderOptions<TItem>
    {
        /// <summary>
        /// If user provides no parameters, use these parameters.  Not used if user specifies (object[])null as parameters
        /// </summary>
        public object[]? DefaultParameters { get; set; } = null;

        /// <summary>
        /// Always add these parameters (even to the DefaultParameters)
        /// </summary>
        public object[]? AlwaysUseParameters { get; set; } = null;

    }

}

