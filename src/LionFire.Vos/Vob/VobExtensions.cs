using System;

namespace LionFire.Vos
{
    public static class VobExtensions
    {
        public static bool IsAncestorOf(this Vob potentialAncestor, Vob potentialChild)
        {
            for (IVob vobParent = potentialChild.Parent; vobParent != null; vobParent = vobParent.Parent)
            {
                if (Object.ReferenceEquals(vobParent, potentialAncestor))
                {
                    return true;
                }
            }
            return false;
        }
    }
}