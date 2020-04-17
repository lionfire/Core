namespace LionFire.DependencyMachine
{
    public class DependencyStagePlaceholder : ResolvableDependency
    {
        public DependencyStagePlaceholder(string name, params string[] dependencies)
        {
            Key = name;
            Contributes = new string[] { name };
            Dependencies = dependencies;
        }

        public override string Key { get; }


    }
}
