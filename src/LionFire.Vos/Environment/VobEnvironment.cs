using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Environment
{
    public class VobEnvironment : VobNodeBase<VobEnvironment>
    {
        public VobEnvironment(Vob vob) : base(vob)
        {
        }

        public bool Inherit { get; set; }

        public Dictionary<string, object> OwnVariables { get; } = new Dictionary<string, object>();

        public void Add<T>(string key, T value) 
            => OwnVariables.Add(key, value);

        public T Get<T>(string key)
        {
            if (OwnVariables.ContainsKey(key)) return (T) OwnVariables[key];
            if (Inherit)
            {
                var parent = this.ParentValue;
                if (parent != null) return parent.Get<T>(key);
            }
            return default;
        }
    }

}
