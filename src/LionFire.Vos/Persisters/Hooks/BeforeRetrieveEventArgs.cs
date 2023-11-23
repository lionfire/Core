#nullable enable
using LionFire.Referencing;
using LionFire.Vos;

namespace LionFire.Persistence.Persisters.Vos;

public class BeforeRetrieveEventArgs : BeforeReadEventArgs
{
    //public bool AfterNotFound  { get; set; }
}

public class AfterNotFoundEventArgs : BeforeReadEventArgs
{
    public bool FoundMore { get; set; }
}
