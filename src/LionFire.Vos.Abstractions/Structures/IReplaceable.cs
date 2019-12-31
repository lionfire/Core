namespace LionFire.Structures
{
    public interface IReplaceable<T>
    {
        T Replacement{ get; }
        void ReplaceWith(T vob);
    }
}
