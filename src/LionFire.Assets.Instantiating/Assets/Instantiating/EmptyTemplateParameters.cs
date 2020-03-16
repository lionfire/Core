using LionFire.Assets;
using LionFire.Persistence;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Instantiating
{
    public class EmptyTemplateParameters : ITemplateParameters
    {
        public string Key { get; set; }
        //#if AOT
        //object IKeyed<object>.Key => Key;
        //#endif

        public IReadHandle<ITemplateAsset> TemplateAsset { get; set; }
        public ParameterOverlayMode OverlayMode { get => ParameterOverlayMode.None; set => throw new NotSupportedException(); }

        [Ignore]
        public object OverlayParent { get => null; set => throw new NotSupportedException(); }

        public IEnumerable<IEnumerable<IAssetInstantiation>> OverlayTargets => Enumerable.Empty<IEnumerable<IAssetInstantiation>>();

        public IReadHandleBase<ITemplate> RTemplate => throw new NotImplementedException();
    }
}
