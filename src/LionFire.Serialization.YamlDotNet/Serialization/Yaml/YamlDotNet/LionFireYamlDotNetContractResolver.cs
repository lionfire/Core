#if false
// Based on https://blog.rsuter.com/advanced-newtonsoft-yaml-dynamically-rename-or-ignore-properties-without-changing-the-serialized-class/ which says to be based on an NSwag implementation, license MIT  https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md
using Microsoft.Extensions.Configuration;
using LionFire.FlexObjects;
using LionFire.Applications.Hosting;
using YamlDotNet;
using Newtonsoft.Yaml.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace LionFire.Serialization.Yaml.Newtonsoft
{
    public class LionFireYamlDotNetContractResolver : DefaultContractResolver
    {
        #region Parameters

        public LionSerializeContext SerializationPurpose { get; set; }

        #endregion

        #region Construction

        public LionFireYamlDotNetContractResolver()
        {
        }

        #endregion

        #region (Public) Methods

        public void IgnoreProperty(Type type, params string[] yamlPropertyNames)
        {
            if (!_ignores.ContainsKey(type))
                _ignores[type] = new HashSet<string>();

            foreach (var prop in yamlPropertyNames)
                _ignores[type].Add(prop);
        }

        public void RenameProperty(Type type, string propertyName, string newYamlPropertyName)
        {
            if (!_renames.ContainsKey(type))
                _renames[type] = new Dictionary<string, string>();

            _renames[type][propertyName] = newYamlPropertyName;
        }

        #endregion

        #region (protected) IContractResolver Implementation

        protected override YamlProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (IsIgnored(property.DeclaringType, property.PropertyName))
            {
                property.ShouldSerialize = i => false;
                property.Ignored = true;
            }

            if (IsRenamed(property.DeclaringType, property.PropertyName, out var newYamlPropertyName))
                property.PropertyName = newYamlPropertyName;

            return property;
        }

        #endregion

        #region (Private) Implementation

        private readonly Dictionary<Type, HashSet<string>> _ignores = new Dictionary<Type, HashSet<string>>();
        private readonly Dictionary<Type, Dictionary<string, string>> _renames = new Dictionary<Type, Dictionary<string, string>>();

        private bool IsIgnored(Type type, string yamlPropertyName)
        {
            var propertyInfo = type.GetProperties().SingleOrDefault(p => p.Name == yamlPropertyName && p.DeclaringType == type);
            if (propertyInfo?.ShouldSerialize(SerializationPurpose) == false) return true;

            if (!_ignores.ContainsKey(type))
                return false;

            return _ignores[type].Contains(yamlPropertyName);
        }

        private bool IsRenamed(Type type, string yamlPropertyName, out string newYamlPropertyName)
        {
            Dictionary<string, string> renames;

            if (!_renames.TryGetValue(type, out renames) || !renames.TryGetValue(yamlPropertyName, out newYamlPropertyName))
            {
                newYamlPropertyName = null;
                return false;
            }

            return true;
        }

        #endregion

    }
}
#endif