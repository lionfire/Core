using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.Referencing
{
    public abstract class ReferenceBase<ConcreteType> : ReferenceBaseBase<ConcreteType>, IReference
        where ConcreteType : ReferenceBase<ConcreteType>
    {
        public bool IsCompatibleWith(string stringUrl) => AllowedSchemes.Contains(stringUrl.GetUriScheme());
        public abstract IEnumerable<string> AllowedSchemes { get; }

        #region Scheme

        public abstract string Scheme
        {
            get;
        }

        public virtual bool CanSetScheme { get { return false; } }

        public static bool VerifyScheme = true;

        public virtual void ValidateScheme(string scheme)
        {
            if (scheme != Scheme)
            {
                throw new ArgumentException("Scheme '"
                    + (scheme ?? "null")
                    + "'not valid for this reference type: " + this.GetType().Name);
            }
        }

        #endregion

        public abstract string Host { get; set; }
        public abstract string Port { get; set; }

        // OLD
        //#region Construction

        //#region Copy From

        //protected override void CopyFrom(T other)
        //{
        //    this.Key = other.Key;
        //    //this.Host = other.Host;
        //    //this.Port = other.Port;
        //    //this.Path = newPath ?? other.Path;
        //}

        //#endregion

        //#endregion


        #region Misc

        #region Object Overrides

        public override bool Equals(object obj)
        {
            var other = obj as IReference;
            if (other == null)
            {
                return false;
            }

            return this.Key == other.Key;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        #endregion

        #endregion

    }
}
