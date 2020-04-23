#nullable enable
using LionFire.Dependencies;
using LionFire.Types;
using System;
using System.Collections.Generic;

namespace LionFire.Referencing
{
    //public class NamedReference : NamedReference<object>
    //{
    //    #region Construction

    //    public NamedReference() { }
    //    public NamedReference(string key) : base(key)
    //    {
    //    }

    //    #endregion
    //}

    public class NamedObjectReference : LocalReferenceBase<NamedObjectReference>
    {
        public override IEnumerable<string> AllowedSchemes => UriSchemes;
        public static string[] UriSchemes => new string[] { "object" };
        public override string Scheme => "object";

        public static Func<Type, string> TypeNameForType = t => t.FullName;

        #region Construction

        public NamedObjectReference() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="path">The name of the object</param>
        public NamedObjectReference(Type type, string path) {
            TypeName = type.FullName;
            Path = path;
        }

        #endregion

        [SetOnce]
        public override string Key
        {
            get => $"{Path}:{TypeNameForType(Type)}";
            protected set
            {
                var split = value.Split(':');
                Path = split[0];
                if(split.Length > 1) TypeName = split[1];
            }
        }

        #region TypeName

        [SetOnce]
        public string TypeName
        {
            get => typeName;
            set
            {
                if (typeName == value) return;
                if (typeName != default) throw new AlreadySetException();
                typeName = value;
            }
        }
        private string typeName = null;

        #endregion

        public Type Type => DependencyContext.Current.GetService<ITypeResolver>().Resolve(TypeName);
    }
}
