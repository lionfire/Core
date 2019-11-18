using LionFire.Dependencies;
using LionFire.Structures;
using System;

namespace LionFire.Activation
{
    [Flags]
    public enum ActivationConfigStrategy
    {
        DefaultStrategy = 0,
        DependencyContext = 1 << 0,
        Null = 1 << 1,
        ManualSingleton = 1 << 2,
    }


    public static class TypeActivationConfiguration<T>
    {
        #region Strategy

        [SetOnce]
        public static ActivationConfigStrategy? Strategy
        {
            get => strategy;
            set
            {
                if (strategy == value) return;
                if (strategy != default) throw new AlreadySetException();
                strategy = value;
            }
        }
        private static ActivationConfigStrategy? strategy;

        #endregion

        public static Type ActivationType
        {
            get
            {
                var strategy = Strategy ?? ActivationConfigStrategy.DefaultStrategy;

                switch (strategy)
                {
                    case ActivationConfigStrategy.DefaultStrategy:
                        return typeof(T);
                    case ActivationConfigStrategy.DependencyContext:
                        {
                            var provider = DependencyContext.Current.GetService<ITypeActivationTypeProvider<T>>();
                            if (provider == null) throw new HasUnresolvedDependenciesException($"DependencyContext.Current is missing service ITypeActivationTypeProvider<{typeof(T).FullName}>");
                            return provider.Type;
                        }
                    case ActivationConfigStrategy.ManualSingleton:
                        {
                            var provider = ManualSingleton<ITypeActivationTypeProvider<T>>.Instance;
                            if (provider == null) throw new HasUnresolvedDependenciesException($"ManualSingleton<> is missing instance for ITypeActivationTypeProvider<{typeof(T).FullName}>");
                            return provider.Type;
                        }
                    case ActivationConfigStrategy.Null:
                        return null;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(Strategy));
                }
            }
        }
    }
}
