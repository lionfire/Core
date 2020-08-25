using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Exceptions
{
    public enum PotentiallyTemporaryErrorKind
    {
        Unknown = 0,
        KnownPermanent = 1,
        PotentiallyTemporary = 2,
        KnownTemporary = 3,
    }

    public interface IPotentiallyTemporaryError
    {
        PotentiallyTemporaryErrorKind IsTemporaryError { get; }
    }
}
