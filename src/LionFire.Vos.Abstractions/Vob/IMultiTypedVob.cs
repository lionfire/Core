using LionFire.MultiTyping;
using LionFire.Structures;
using System;

namespace LionFire.Vos
{
    public interface IMultiTypedVob : IVob, IMultiTyped
    {
        
    }


    public static class IMultiTypedVobExtensions
    {
        public static IMultiTyped GetMultiTyped(this IVob vob)
        {
            if (vob is IMultiTypedVob mtv) return mtv;
            if (vob is IMultiTypable mt) return mt.MultiTyped;

            if(vob is IReplaceable<IVob> replaceable)
            {
                //replaceable.ReplaceWith(new Vob());

                throw new NotImplementedException("TODO: Replace Vob with more functional version");
            }

            throw new NotSupportedException("Vob does not support IMultiTyped and cannot be upgraded to do so.");
        }
    }
}
