using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LionFire.ExtensionMethods;
using System.Threading.Tasks;

namespace LionFire.CommandLine.Arguments
{
    public class CliArgumentDefinition
    {
        #region (Public) Properties

        public string LongForm { get; set; }
        public char ShortForm { get; set; }

        public string Description { get; set; }

        public string Usage { get; set; }

        public bool IsExclusive { get; set; }

        /// <summary>
        /// If true, the prefix (e.g. '-' for short form and "--") must be prepended to the short form and long form arguments.  If false, they must not be present.
        /// </summary>
        public bool UsesOptionPrefix { get; set; }

        /// <summary>
        /// Optional parameters: string[] args, CommandLineArgs, object context
        /// TODO: ArgInfo parameter which shows subparameters and other info
        /// </summary>
        public MethodInfo Handler { get; set; }

        /// <summary>
        /// Can be null if Handler is a static method
        /// </summary>
        public object HandlerInstance { get; set; }

        #endregion

        #region Static

        public static List<CliArgumentDefinition> GetForType(object instance = null, Type type = null)

        {
            if (type == null) type = instance.GetType();

            // FUTURE: Allow multiple attributes to decorate the method, in which case a key may be helpful

            Dictionary<string, CliArgumentDefinition> dict = new Dictionary<string, CliArgumentDefinition>();
            foreach (var mi in type.GetMethods())
            {
                if (instance == null && !mi.IsStatic) continue;

                foreach (var cliAttr in mi.GetCustomAttributes().OfType<CliArgumentAttribute>())
                {
                    string key = null;

                    if (!string.IsNullOrWhiteSpace(cliAttr.LongForm))
                    {
                        key = cliAttr.LongForm;
                    }
                    else if (cliAttr.LongForm == string.Empty)
                    {
                        key = "___" + mi.Name.ToLower();
                    }
                    else
                    {
                        key = cliAttr.LongForm ?? mi.Name.ToLower();
                    }

                    var def = new CliArgumentDefinition
                    {
                        Handler = mi,
                        HandlerInstance = instance,
                    };
                    cliAttr.AssignPropertiesTo(def);

                    if (def.LongForm == null) def.LongForm = mi.Name.ToLower();

                    dict.Add(key, def);
                }
            }
            return dict.Values.ToList();
        }

        #endregion

    }

    public static class CliargumentDefinitionExtensions
    {
        public static void Validate(this List<CliArgumentDefinition> args)
        {
            // TODO: make sure no duplicates exist, or else throw
        }
    }
}
