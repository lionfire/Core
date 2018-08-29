
namespace LionFire
{
    public interface IValidatable // MOVE
    {
        /// <summary>
        /// Validate the object, throwing an exception if validation fails.
        /// </summary>
        /// <param name="throwMany">If an error is detected, attempt to keep detecting more errors, and throw all errors within an AggregateException</param>
        void Validate(bool throwMany = true);
    }
}
