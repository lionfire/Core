#nullable enable

namespace LionFire.DependencyMachines
{
    /// <summary>
    /// Initializers run as soon as possible
    /// </summary>
    public class Initializer : Participant
    {
        public Initializer(string key, string? runAfter = null)
        {
            Key = key;
            RunAfter = runAfter;
        }

        public override string Key { get; }
        public string? RunAfter { get; }
    }
}
