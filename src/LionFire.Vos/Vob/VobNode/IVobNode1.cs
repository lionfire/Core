namespace LionFire.Vos
{
    public interface IVobNode<out TInterface> : IVobNode
    {
        new TInterface Value
        {
            get;
        }

        IVobNode<TInterface> ParentVobNode { get; }
    }

}
