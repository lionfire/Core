using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Resources;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using Microsoft.Extensions.Logging;

namespace LionFire.UI.Wpf
{

    public static class WpfResourceUtils
    {
        [Conditional("DEBUG")]
        public static void DumpResourcePaths(Assembly assembly)
        {
            var resourceUris = (assembly ?? throw new ArgumentNullException(nameof(assembly)))
               .GetCustomAttributes(typeof(AssemblyAssociatedContentFileAttribute), true)
               .Cast<AssemblyAssociatedContentFileAttribute>()
               .Select(attr => new Uri(attr.RelativeContentFilePath));


            //foreach (var a in System.Windows.Media.Fonts.GetFontFamilies("pack://application:,,,/LionFire.Valor.Avalon.Fonts"))
            foreach (var a in GetResourceNames(assembly))
            {
                l.Debug(a.ToString());
            }
            l.Trace("----");
            foreach (var a in resourceUris)
            {
                l.Trace(a.ToString());
            }
        }

        public static string[] GetResourceNames(Assembly assembly)
        {
            string resName = (assembly ?? throw new ArgumentNullException(nameof(assembly))).GetName().Name + ".g.resources";
            using (var stream = assembly.GetManifestResourceStream(resName))
            using (var reader = new System.Resources.ResourceReader(stream))
            {
                return reader.Cast<DictionaryEntry>().Select(entry => (string)entry.Key).ToArray();
            }
        }

        private static ILogger l = Log.Get();

    }
}
