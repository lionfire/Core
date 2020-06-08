using System;

namespace LionFire.UI
{
    public class RootInterfaceOptions
    {
        /// <summary>
        /// Must be a UI type that the ILionFireShell is capable of showing, or else a IHostedService
        /// </summary>
        public Type RootViewType { get; set; }

        /// <summary>
        /// Must be a UI object that the ILionFireShell is capable of showing, or else a IHostedService
        /// </summary>
        public object RootView { get; set; }

        public Action RootViewAction { get; set; }

        public Type RootViewModelType { get; set; }

        public object RootViewModel { get; set; }
    }
}
