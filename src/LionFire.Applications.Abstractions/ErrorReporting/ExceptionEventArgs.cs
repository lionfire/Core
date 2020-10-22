using System;

namespace LionFire.ErrorReporting
{
    public class ExceptionEventArgs
    {
        public ExceptionEventArgs(Exception exception, object context = null)
        {
            Exception = exception;
            Context = context;
        }

        public object Context { get; }
        public Exception Exception { get; }

        public bool Handled { get; set; }

        public static implicit operator ExceptionEventArgs(Exception ex) => new ExceptionEventArgs(ex);
    }
}
