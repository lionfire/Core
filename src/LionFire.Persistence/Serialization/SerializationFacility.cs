using LionFire.Dependencies;

namespace LionFire.Serialization
{
    public static class SerializationFacility
    {
        public static IResolvesSerializationStrategies Default
        {
            get
            {
                IResolvesSerializationStrategies serializationResolver = ServiceLocator.TryGet<ISerializationProvider>();

                if (serializationResolver == null)
                {
                    throw new HasUnresolvedDependenciesException($"No {typeof(ISerializationProvider).Name} available");
                }
                return serializationResolver;
            }
        }
    }
}
