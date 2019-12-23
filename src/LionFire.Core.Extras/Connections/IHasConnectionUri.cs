using System;

namespace LionFire.Data
{
    public interface IHasConnectionUri
    {
        Uri ConnectionUri { get; set; }
    }
}
