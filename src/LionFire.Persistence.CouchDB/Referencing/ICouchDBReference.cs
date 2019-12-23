using LionFire.Referencing;

namespace LionFire.Persistence.CouchDB
{
    public interface ICouchDBReference : IReference
    {
        string Id { get; }
        string Database { get; }

        //(string, string) FieldId { get; }
    }
}
