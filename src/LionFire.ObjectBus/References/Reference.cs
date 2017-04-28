using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    /// <summary>
    /// A generic reference class
    /// </summary>
    public class Reference : ReferenceBase
    {
        public Reference()
        {
        }

        //public new string Scheme { get;  set; }
        //public new string Scheme { set; }

        //public string Host { get; set; }
        //public string Port { get; set; }
        //public string Path { get; set; }
        //public string Layer { get; set; }
        //public string TypeName { get; set; }

        //public Type Type { get; set; }

        public override string Scheme
        {
            get;
            set; // TODO: Set once
        }

        public override string Key
        {
            get
            {
                return this.ToString();
            }
            set
            {
                throw new NotImplementedException("set_Key");
            }
        }
    }

    public static class ReferenceConstants
    {
        public const string LayerSeparator = "|";
        public const string LocationSeparator = "^";
        public const string PortSeparator = ":";
        public const string TypeNameSeparator = "%";
    }

    public static class ReferenceUtils
    {
        public static string ToUriString(IReference reference)
        {
            return String.Format("{0}://{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}",
    reference.Scheme, reference.Host,
    /*{2}*/reference.Port == null ? "" : ReferenceConstants.PortSeparator, (reference.Port ?? ""),
   /*{4}*/ reference.Package == null ? "" : ReferenceConstants.LayerSeparator, (reference.Package ?? ""),
       /*{6}*/           reference.Location == null ? "" : ReferenceConstants.LocationSeparator, (reference.Location ?? ""),
                /*{8}*/reference.Path,
    reference.TypeName == null ? "" : ReferenceConstants.TypeNameSeparator, (reference.TypeName ?? "")
    );
        }

        #region Host

        public static readonly string[] LocalhostStrings = new string[]
        {
            "localhost",
            "127.0.0.1",
            //"::1",
        };

        #endregion
    }

}
