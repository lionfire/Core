namespace LionFire.Composables
{
    public interface IAdding
    {
        /// <summary>
        /// Return true if object should be added, false if not (in which case side effects typically have accomplished something useful during the OnAdding method call)
        /// </summary>
        /// <param name="parent">The object (IComposable) being added to</param>
        /// <returns></returns>
        bool OnAdding<T>(IComposable<T> parent);
    }
}
