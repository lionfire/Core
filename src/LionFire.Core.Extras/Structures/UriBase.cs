using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire
{
    public abstract class UriBase
    {
        //private System.Net.IPEndPoint ipEndPoint;

        public UriBase() { }
        public UriBase(string hostName)
        {
            this.HostName = hostName;
        }
        public UriBase(string hostName, int port) : this(hostName)
        {
            this.Port = port.ToString();
        }

        public UriBase(string hostName, PortRange portRange)
        {
            this.HostName = hostName;
            this.PortRange = portRange;
        }

        public UriBase(System.Net.IPEndPoint ipEndPoint)
        {
            this.HostName = ipEndPoint.Address.ToString();
            this.Port = ipEndPoint.Port.ToString();
        }

        public abstract string UriScheme { get; }

        public string UriPrefix { get { return UriScheme + ":"; } }

        public virtual bool HostSlashPrefix { get { return true; } }

        public string HostName { get; protected set; }
        public string Port { get; protected set; }
        public string Path { get; protected set; }

        public bool HasPath => !string.IsNullOrWhiteSpace(Path);
        public bool IsDefaultPort => string.IsNullOrWhiteSpace(Port) || Port.Equals("0") || Port.Equals("-1");
        public bool IsPortRange => Port.Contains('-');

        public PortRange PortRange
        {
            get
            {
                if (IsDefaultPort) return new PortRange(0,0);
                if (IsPortRange)
                {
                    string[] ports = Port.Split('-');
                    if (ports.Length != 2) return null;
                    // TODO - better fault tolerance
                    return new PortRange(Convert.ToInt32(ports[0]), Convert.ToInt32(ports[1]));
                }
                else
                {
                    return new PortRange(Convert.ToInt32(Port));
                }
            }
            protected set
            {
                if (value.MinPort == value.MaxPort) Port = value.MinPort.ToString();
                else Port = value.MinPort.ToString() + '-' + value.MaxPort.ToString();
            }
        }

        #region Uri

        public Uri Uri
        {
            get
            {
                string uriString = UriPrefix + (HostSlashPrefix ? "//" : "") + HostName +
                    (IsDefaultPort ? "" : (":" + Port.ToString())) +
                    (HasPath ? Path : "");
                return new Uri(uriString);
            }
            protected set
            {
                if (!value.Scheme.Equals(UriScheme)) throw new ArgumentException("Invalid uri scheme");

                this.HostName = value.Host;
                this.Port = value.IsDefaultPort ? null : value.Port.ToString();
                this.Path = value.PathAndQuery;
            }
        }

        public string UriString
        {
            get
            {
                return Uri.ToString();
            }
            protected set
            {
                if (value.StartsWith(UriScheme))
                {
                    throw new ArgumentException("Invalid uri scheme");
                }
                Uri uri = new Uri(value);
                this.Uri = uri;
            }
        }

        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //    return 71096143 
        //        * (HostName != null ? HostName.GetHashCode() : 1)
        //        * (PortRange != null ? HostName.GetHashCode() : 1) 
        //        ;
        //    }
        //}

        #endregion

        public override bool Equals(object obj)
        {
            UriBase other = obj as UriBase;
            if (other == null) return false;
            return UriString.Equals(other.UriString);
        }

        public override int GetHashCode()
        {
            return UriString.GetHashCode();
        }

        public override string ToString()
        {
            return UriString;            
        }
    }
}
