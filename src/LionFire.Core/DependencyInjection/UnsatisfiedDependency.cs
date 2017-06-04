using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.DependencyInjection
{
    public class UnsatisfiedDependency
    {
        #region Construction

        public UnsatisfiedDependency() { }
        public UnsatisfiedDependency(string description)
        {
            this.Description = description;
        }
        public UnsatisfiedDependency(PropertyInfo propertyInfo)
        {
            this.Description = $"Property: {propertyInfo.Name} ({propertyInfo.PropertyType})";
        }
        public static implicit operator UnsatisfiedDependency(string desc)
        {
            return new UnsatisfiedDependency(desc);
        }
        
        #endregion

        /// <summary>
        /// Optional
        /// </summary>
        //public int Id { get; set; }

        public string Description { get; set; }
    }
}
