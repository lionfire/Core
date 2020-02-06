using LionFire.Referencing;
using System;

namespace LionFire.Persistence
{
    public class PersistenceConventions : IPersistenceConventions
    {
        public Func<string, bool> IsDirectory = d => d.EndsWith(LionPath.Separator);
        public Func<string, bool> IsFile = d => !d.EndsWith(LionPath.Separator);
        public Func<string, bool> IsHidden = d => d.StartsWith(".");
        public Func<string, bool> IsMeta = d => d.StartsWith("^");
        public Func<string, bool> IsSpecial = d => d.StartsWith("_");

        public ItemFlags ResolveItemFlags(string name)
        {
            ItemFlags flags = ItemFlags.None;

            if (IsDirectory(name)) flags |= ItemFlags.Directory;
            if (IsFile(name)) flags |= ItemFlags.File;
            if (IsHidden(name)) flags |= ItemFlags.Hidden;
            if (IsMeta(name)) flags |= ItemFlags.Meta;
            if (IsSpecial(name)) flags |= ItemFlags.Special;

            return flags;
        }
    }
}
