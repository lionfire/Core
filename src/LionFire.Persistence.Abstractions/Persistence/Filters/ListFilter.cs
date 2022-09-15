#nullable enable

namespace LionFire.Persistence
{
    public class ListFilter
    {
        /// <summary>
        /// If None, Default will be used
        /// </summary>
        public ItemFlags Flags { get; set; }
        public Predicate<string> FilenameFilter { get; set; }
        //public string SearchPattern { get; set; } // ENH
    }
}
