namespace LionFire.Orleans_;

public interface IHasGrainId
{
    string GrainPrimaryKey { get; }
}

