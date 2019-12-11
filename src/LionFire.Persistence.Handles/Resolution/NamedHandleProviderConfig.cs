using LionFire;
using System;

namespace LionFire.Persistence.Handles
{
    public class NamedHandleProviderConfig : INamedHandleProviderConfig
    {
        public Type ProviderType { get; set; }

        #region Name

        [SetOnce]
        public string Name
        {
            get => name;
            set
            {
                if (name == value) return;
                if (name != default) throw new AlreadySetException();
                name = value;
            }
        }
        private string name;

        #endregion

        public NamedHandleProviderConfig() { }
        protected NamedHandleProviderConfig(string name)
        {
            this.name = name;
        }
        public virtual string ConnectionString { get { throw new NotSupportedException(); } set => throw new NotSupportedException(); }
    }
}
