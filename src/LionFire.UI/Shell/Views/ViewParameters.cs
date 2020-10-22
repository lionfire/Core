#nullable enable

namespace LionFire.Shell
{
    public class ViewParameters
    {
        /// <summary>
        /// Not applicable to external Web Browsers
        /// </summary>
        public bool Modal { get; set; }

        public NavigateHistoryMode Mode { get; set; } = NavigateHistoryMode.Replace;
    }
}
