using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Aliases
{
    public class VobAliases
    {
        public Dictionary<string, IVob> Aliases { get; private set; }
        public Dictionary<string, string> StringAliases { get; private set; }

        public VobAliases()
        {
        }

        public void Set(string name, string target, bool allowReplace = false)
        {
            if (StringAliases == null) StringAliases = new Dictionary<string, string>();
            if (allowReplace)
            {
                StringAliases[name] = target;
            }
            else
            {
                StringAliases.Add(name, target);
            }
        }
        public void Set(string name, IVob target, bool allowReplace = false)
        {
            if (Aliases == null) Aliases = new Dictionary<string, IVob>();
            if (allowReplace)
            {
                Aliases[name] = target;
            }
            else
            {
                Aliases.Add(name, target);
            }
        }
    }
}

