namespace LionFire.Assets
{
#if AOT
	public interface ITemplate<InstanceType> 
		where InstanceType : ITemplateAssetInstance, new()
	{
//		ITemplateInstance ConstructInstance();
//
//		Type InstanceType { get; }
//		Type InstantiationType { get; }
//		IInstantiation CreateInstantiation();
	}
#else

    public interface ITemplateAsset<InstanceType> : ITemplateAsset
        where InstanceType : ITemplateAssetInstance//, new()
    {
        //IInstantiation<InstanceType> CreateInstantiation();
    }
#endif
}
