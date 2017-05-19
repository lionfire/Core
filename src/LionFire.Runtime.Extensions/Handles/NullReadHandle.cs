namespace LionFire.Handles
{
    public class NullReadHandle<T> : IReadHandle<T>
    {
        public T Object => default(T);
        public bool HasObject => false;
    }

}
