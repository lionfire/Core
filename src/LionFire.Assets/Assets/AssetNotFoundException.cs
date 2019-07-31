
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Assets
{
#if TODO
    public interface IProblem
    {
        string Message { get; }
        string Target { get; }
        object TargetObject { get; }
        Exception Exception { get; }
    }
    public static class IProblemExtensions
    {
        public static string GetString(this IProblem problem);
    }
#endif
    public class AssetNotFoundException : AssetException
        #if TODO
        , IProblem
#endif
    {
        public AssetNotFoundException() { }
        //public AssetNotFoundException(object obj) { this.TargetObject = obj; }
        public AssetNotFoundException(string message) : base(message) { }
        public AssetNotFoundException(string message, Exception inner) : base(message, inner) { }
        
#if TODO
        public new string Message
        {
            get
            {
                return "Asset not found";
            }
        }

        public string Target
        {
            get
            {
                return target ?? TargetObject.ToStringSafe();
            }
            set
            {
                target = value;
            }
        } private string target;

        public object TargetObject
        {
            get;
            set;
        }

        public Exception Exception
        {
            get;set;
        }
#endif
    }
}
