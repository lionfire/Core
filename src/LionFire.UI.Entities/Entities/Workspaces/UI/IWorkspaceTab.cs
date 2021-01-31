namespace LionFire.UI.Workspaces
{

    public interface IWorkspaceTab : IUIObject
    {
        IWorkspaceItem WorkspaceItem { get; set; }

        IUIObject ContentPane { get; set; }

        object Contents { get; set; }

        string Title { get; }
        string Tooltip { get; set; }

        void OnClosing();

        /// <summary>
        /// Set to true when the nav item is focused
        /// </summary>
        bool IsFocused { get; set; }

        /// <summary>
        /// Set to false if the user or application logic decides to hide 
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Set to true by WorkspaceConductor when the pane(s) for this WorkspaceItem has focus
        /// </summary>
        bool IsTabPaneFocused { get; set; }

        /// <summary>
        /// Set to true by WorkspaceConductor when the pane(s) for this WorkspaceItem are visible
        /// </summary>
        bool IsTabPaneVisible { get; set; }
    }
}
