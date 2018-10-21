using System;

namespace LionFire.Referencing
{
    public class NamedReference : LocalReferenceBase
    {
        public NamedReference() { }
        public NamedReference(string key)
        {
            this.key = key;
        }

        public override string Scheme => "named";

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
