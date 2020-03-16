using System;
using System.Collections.Generic;
using System.Diagnostics;
using LionFire.Referencing;
using LionFire.Structures;

namespace LionFire.ObjectBus.ExtensionlessFs
{
    public abstract class RelativeReferenceBase : IReference
    {
        public IReference OverlayTarget { get; }

        public string Persister => OverlayTarget.Persister;

        public RelativeReferenceBase(IReference reference)
        {
            this.OverlayTarget = reference;
        }

        public string Key
        {
            get
            {
                if (OverlayTarget is IKeyGenerator<string, IReference> x)
                {
                    return x.GetKey(this);
                }
                else
                {
                    // REVIEW - may want to further document and/or warn about this fallback:
                    Debug.WriteLine($"WARNING - {OverlayTarget.GetType().FullName} does not implement IKeyGenerator<string, IReference>.  Falling back to path replacement method in RelativeReferenceBase.Key");
                    return OverlayTarget.Key.Replace(OverlayTarget.Path, Path);
                }
            }
        }

        public virtual string Scheme => OverlayTarget.Scheme;

        public virtual string Host => (OverlayTarget as IHostReference)?.Host;

        public virtual string Port => (OverlayTarget as IHostReference)?.Port;
        public virtual string Path => OverlayTarget.Path;

        public IReference GetChild(string subPath) => throw new NotImplementedException();
        //public IReference GetChildSubpath(IEnumerable<string> subpath) => throw new NotImplementedException();
        public bool IsCompatibleWith(string obj) => throw new NotImplementedException();
    }

    //public class ReferenceResolver
    //{
    //    public List<ReferenceResolutionStrategy> Strategies { get; private set; }

    //    public IEnumerable<R<T>> Resolve<T>(IReference r, ResolveOptions options = null)
    //    {
    //        //DependencyContext
    //    }

    //    public Task<T> Get(IReference r)
    //    {

    //    }

    //    public Task<bool> Exists<T>(IReference r)
    //    {
    //    }


    //}
}
