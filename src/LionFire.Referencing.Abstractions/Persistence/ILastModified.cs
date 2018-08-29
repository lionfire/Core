using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence
{
    public interface ILastModified
    {
        DateTime? LastModified { get; set; }
    }
}
