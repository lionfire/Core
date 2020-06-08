namespace LionFire.Shell
{
    public interface IKeyboardShell
    {
        #region State

        /// <summary>
        /// Set to true by applications when keyboard is receiving text input.  This can be useful in determining whether certain keyboard events and hotkeys should be disabled while the user is typing.
        /// </summary>
        bool IsEditingText { get; set; }

        #endregion
    }
}
