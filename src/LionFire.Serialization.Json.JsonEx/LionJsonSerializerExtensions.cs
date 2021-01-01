// OLD?

using System;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace LionFire.Serialization
{
    public static class LionJsonSerializerExtensions
    {
        public static void InitializeAliases(this JsonExSerializer.Serializer serializer, Assembly a)
        {
#if !AOT
            //var timing = Timing.StartNow("InitializeAliases");
            foreach (var type in a.GetTypes())
            {
                if (type.IsInterface) continue;
                if (!type.IsPublic) continue;
                if (type.IsAbstract) continue;
                if (type.IsGenericTypeDefinition) continue; // TODO: JsonEx Parser doesn't parse apostrophe into the identifier yet
                try
                {
                    serializer.Config.TypeAliases.Add(type, type.Name);
                }
                catch (Exception ex)
                {
                    l.Warn("Failed to add type alias for: " + type.FullName + ".  Exception: " + ex);
                }
            }
            //timing.StopAndRecord();
#endif
        }

        private static readonly ILogger l = Log.Get();
		
    }


}