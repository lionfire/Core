#nullable enable

namespace LionFire.Ontology;

public interface IInjectable<T> : IHas<T>, IDependsOn<T> { }