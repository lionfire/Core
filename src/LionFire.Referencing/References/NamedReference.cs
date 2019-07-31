using System;
using System.Collections.Generic;

namespace LionFire.Referencing
{
    public class NamedReference : LocalReferenceBase<NamedReference>
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

        [SetOnce]
        public override string Key
        {
            get => Key;
            protected set
            {
                if (this.key != null)
                {
                    throw new AlreadySetException();
                }
                this.key = value;
            }
        }

        private string key;

        [SetOnce]
        public override string Path { get => Key; set => Key = value; }
    }
}
