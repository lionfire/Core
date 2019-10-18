using LionFire.Referencing;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Instantiating
{
    public class InstantiationReference<TInstance> : TypedReferenceBase<IInstantiation>
    {
        public override IEnumerable<string> AllowedSchemes => throw new NotImplementedException();

        public IReference ResolvedReference { get; set; }

        public static implicit operator InstantiationReference<TInstance>(string str)
        {
            return new InstantiationReference<TInstance>();
        }

        public static implicit operator InstantiationReference<TInstance> (string str)
        {
            //new Resolvable<string, >
        }
    }

    
}
