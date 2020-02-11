using LionFire.Referencing;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Instantiating
{
    public class InstantiationReference<TInstance> : TypedReferenceBase<InstantiationReference<TInstance>>
    {
        public override IEnumerable<string> AllowedSchemes => throw new NotImplementedException();

        public IReference ResolvedReference { get; set; }

        public static implicit operator InstantiationReference<TInstance>(string str)
        {
            return new InstantiationReference<TInstance>();
        }

        public override string Scheme => throw new NotImplementedException();

        //public override string Host { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public override string Port { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Key { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override string Path { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    
}
