using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Metadata;

[Flags]
public enum RelevanceFlags
{
    Unspecified = 0,
    User = 1 << 0,

    Id = 1 << 1,

    /// <summary>
    /// The value has no meaning -- it is just a token whose contents should not be interpreted
    /// </summary>
    Opaque = 1 << 2,

    /// <summary>
    /// Not User-relevant, but visible to end-users, such as an identifier in a URL
    /// </summary>
    VisibleToUser = 1 << 3,

    Internal = 1 << 4,
    Technical = 1 << 5,
    Intermediate = 1 << 6,

    Admin = 1 << 7,
    TechAdmin = 1 << 8,
    AuthAdmin = 1 << 9,

    //[ShortCode("diag")] // TODO
    Diagnostic = 1<< 12,

    //[ShortCode("dev")] // TODO
    Developer = 1<<13,

    #region Roll-ups

    DefaultForUser = User | VisibleToUser,
    DefaultForAdmin = DefaultForUser | Admin,
    DefaultForTechAdmin = DefaultForUser | TechAdmin,

    All = User | VisibleToUser | Id | Opaque | Internal | Technical | Intermediate | Admin | TechAdmin,

    #endregion

}
