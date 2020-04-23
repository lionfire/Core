namespace LionFire.DependencyMachines
{
    public class Placeholder : Participant
    {
        public Placeholder(string name, params string[] dependencies)
        {
            Key = name;
            Contributes = new string[] { name };
            Dependencies = dependencies;
            Flags |= ParticipantFlags.StageEnder;
        }

        public override string Key { get; }


    }
}
