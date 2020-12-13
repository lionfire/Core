namespace LionFire.UI
{
    /// <summary>
    /// Parent of a single child
    /// </summary>
    public interface IUIParent : IUIObject
    {
        IUIObject Child { get; set; }

        /// <summary>
        /// Will set Child to null if child is the same object reference
        /// </summary>
        /// <param name="child"></param>
        bool Remove(IUIObject child);
    }
}
