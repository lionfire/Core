#nullable enable

namespace LionFire.UI
{
    public class ViewParameters
    {
        /// <summary>
        /// Not applicable to external Web Browsers
        /// </summary>
        public bool Modal { get; set; }

        public NavigateHistoryMode Mode { get; set; } = NavigateHistoryMode.Replace;

        public string Path { get; set; }

        public string? ViewName { get; set; }


        /// <summary>
        /// Automatically close if no descendants have the PreventsAutoClose flag set to true.
        /// </summary>
        public bool AutoClose { get; set; }

        /// <summary>
        /// Prevents ancestors with AutoClose set to true from automatically closing
        /// </summary>
        public bool PreventsAutoClose { get; set; }
    }
}
