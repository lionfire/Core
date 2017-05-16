namespace LionFire.Queues
{
    public class NullReadHandle<T> : IReadHandle<T>
    {
        public T Object => default(T);
        public bool HasObject => false;
    }
    
}
