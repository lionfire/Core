#nullable enable
using System.Linq;

namespace LionFire.Structures
{
    public static class NamedSingletonProviderOptionsExtensions
    {
        public static object[] Augment<TItem>(this NamedSingletonProviderOptions<TItem> options, object[]? parameters)
        {
            if (parameters != null && parameters.Length == 0 && options.DefaultParameters != null)
            {
                // parameters != null - Don't do this for null parameters. This allows user to specify (object[])null to avoid DefaultParameters
                parameters = options.DefaultParameters;
            }

            parameters ??= new object[] { };

            if (options.AlwaysUseParameters != null)
            {
                parameters = parameters.Concat(options.AlwaysUseParameters).ToArray();
            }

            return parameters;
        }
    }

}
