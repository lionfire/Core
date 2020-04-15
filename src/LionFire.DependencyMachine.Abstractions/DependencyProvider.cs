namespace LionFire.DependencyMachine
{
    public class DependencyProvider : DependencyProvider<string>
    {
        public override string Key => key;
        private string key;

        public DependencyProvider(string name)
        {
            this.key = name;
        }
    }
}
