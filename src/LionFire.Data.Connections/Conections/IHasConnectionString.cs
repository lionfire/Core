namespace LionFire.Data
{

    public interface IHasConnectionString
    {
        string ConnectionString { get; }
    }

    public interface IHasConnectionStringRW : IHasConnectionString
    {
        new string ConnectionString { get; set; }
    }
}
