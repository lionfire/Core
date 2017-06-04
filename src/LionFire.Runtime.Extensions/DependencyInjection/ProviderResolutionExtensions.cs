using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LionFire.DependencyInjection
{
    public static class ProviderResolutionExtensions
    {

        public static IEnumerable<PropertyInfo> GetProvideAttributeProperties(this object obj)
        {
            return obj.GetType().GetProperties().Where(_ => _.GetCustomAttribute(typeof(ProvideAttribute)) != null);
        }

        // TODO: similar methods as DependencyResolutionExtensions

    }
}
