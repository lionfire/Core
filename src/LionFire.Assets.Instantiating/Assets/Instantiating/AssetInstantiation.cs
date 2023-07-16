using LionFire.Instantiating;
using System;

namespace LionFire.Assets
{
    public class AssetInstantiation<TTemplate> : InstantiationBase<TTemplate, RAsset<TTemplate>, object>, IInstantiation
        where TTemplate : class, ITemplate, IAsset<TTemplate>
    {
        public override string Key { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AssetInstantiation(RAsset<TTemplate> template) : base(template) { }
    }

    public static class AssetInstantiationExtensions
    {
        //public static AssetInstantiation<TTemplate> CreateAssetInstantiation<TTemplate>(this TTemplate template)
        //    where TTemplate : ITemplate, IAsset<TTemplate>
        //    => new AssetInstantiation<TTemplate>(template);
        public static AssetInstantiation<TTemplate> CreateAssetInstantiation<TTemplate>(this RAsset<TTemplate> template)
            where TTemplate : class, ITemplate, IAsset<TTemplate>
            => new AssetInstantiation<TTemplate>(template);
    }

    // TODO: Nicer alternernative for serialized InstantiationCollections?
    public class AssetInstantiationCollection : InstantiationCollection
    {

    }
}
