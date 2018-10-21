using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;

namespace LionFire.ObjectBus
{
    public interface IOBus
    {
        IEnumerable<Type> SupportedReferenceTypes { get; }

        bool IsValid(IReference reference);

        // REVIEW: Move this?
        T InstantiateObject<T>(Func<T> createDefault = null); // MIGRATION: Was private _Create<T>
    }

    public abstract class OBusBase : IOBus
    {
        public abstract IEnumerable<Type> SupportedReferenceTypes { get; }
        public abstract bool IsValid(IReference reference);

        public virtual T InstantiateObject<T>(Func<T> createDefault = null)
        {
            T result;

            if (createDefault != null)
            {
                result = createDefault();
            }
            else
            {
                result = (T)Activator.CreateInstance(typeof(T));
            }

            return result;
        }
    }
}

namespace LionFire.ObjectBus.Filesystem
{
    public class FsOBus : IOBus
    {
        public override IEnumerable<Type> SupportedReferenceTypes
        {
            get
            {
                yield return typeof(LocalFileReference);
            }
        }
        //public bool IsValid(IReference reference) => reference.GetOBases().Any();
        public abstract bool IsValid(IReference reference) => throw new NotImplementedException();
    }
}
namespace LionFire.Vos
{
    public class VosOBus : IOBus
    {
        public IEnumerable<Type> SupportedReferenceTypes
        {
            get
            {
                yield return typeof(VosReference);
            }
        }
        public bool IsValid(IReference reference) => throw new NotImplementedException();
    }
}
