using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Exceptions
{
    
    public class PermanentException : Exception, IPotentiallyTemporaryError
    {
        public PotentiallyTemporaryErrorKind IsTemporaryError => PotentiallyTemporaryErrorKind.KnownPermanent;

        public PermanentException() { }
        public PermanentException(string message) : base(message) { }
        public PermanentException(string message, Exception inner) : base(message, inner) { }
        public PermanentException(Exception inner) : base(inner?.Message, inner) { }
    }
}
