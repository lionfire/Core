using System;
using System.Collections.Generic;

namespace LionFire.Referencing
{
    public class NamedReference : LocalReferenceBase
    {
        public override IEnumerable<string> AllowedSchemes => UriSchemes;
        public static string[] UriSchemes = new string[] { "object" };
        public override string Scheme => "object";

        #region Construction

        public NamedReference() { }
        public NamedReference(string key)
        {
            this.key = key;
        }

        #endregion

        public override string Key => Key;
        private string key;

        public void SetKey(string key)
        {
            if (this.key != null)
            {
                throw new AlreadySetException();
            }
            this.key = key;
        }

        public override string Path { get => Key; protected set => SetKey(value); }
    }
}
