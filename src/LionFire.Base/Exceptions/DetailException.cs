using System;

namespace LionFire
{
    [Serializable]
    public class DetailException : Exception
    {
        /// <summary>
        /// Object containing fault information
        /// </summary>
        public object? Detail { get; set; }

        public DetailException() { }
        public DetailException(object detail) { Detail = detail; }
        public DetailException(string message) : base(message) { }
        public DetailException(object fault, string message) : base(message) { Detail = fault; }
        public DetailException(string message, Exception inner) : base(message, inner) { }
        protected DetailException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
