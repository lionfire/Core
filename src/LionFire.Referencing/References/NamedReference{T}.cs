using System;
using System.Collections.Generic;

namespace LionFire.Referencing
{
    public class NamedReference<TValue> : LocalReferenceBase<NamedReference<TValue>>
    {
        public override IEnumerable<string> AllowedSchemes => UriSchemes;
        public static string[] UriSchemes = new string[] { TypeNameForScheme(typeof(TValue)) };
        public override string Scheme =>  TypeNameForScheme(typeof(TValue));

        // FUTURE: Use FullName or a type alias directory to prevent conflicts
        //public static Func<Type, string> TypeNameForScheme = t => "object-" + t.Name; 
        public static Func<Type, string> TypeNameForScheme = t => t.FullName;

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
