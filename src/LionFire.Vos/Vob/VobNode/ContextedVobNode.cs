namespace LionFire.Vos
{
    public struct ContextedVobNode<TInterface>
         where TInterface : class
    {
        public ContextedVobNode(IVob context, VobNode<TInterface> vobNode)
        {
            Context = context;
            VobNode = vobNode;
        }

        public IVob Context { get; set; }
        public VobNode<TInterface> VobNode { get; set; }
    }

}
