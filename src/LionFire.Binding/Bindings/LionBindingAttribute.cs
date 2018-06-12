using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace LionFire.Bindings
{

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class LionBindingAttribute : Attribute
    {
        readonly string sourcePath;

        public LionBindingAttribute(string sourcePath, LionBindingFlags flags)
        {
            this.sourcePath = sourcePath;
            this.TargetPath = String.Empty;
            this.Flags = flags;
        }
        public LionBindingAttribute(string sourcePath, string targetPath = "")
        {
            this.sourcePath = sourcePath;
            this.TargetPath = targetPath;
            this.Flags = null;
        }
        public LionBindingAttribute(string sourcePath, string targetPath, LionBindingFlags flags)
        {
            this.sourcePath = sourcePath;
            this.TargetPath = targetPath;
            this.Flags = flags;
        }

        public string SourcePath
        {
            get { return sourcePath; }
        }
        public string TargetPath
        {
            get;
            set;
        }

        public LionBindingFlags? Flags
        {
            get;
            set;
        }

        public void CreateBinding(object sourceObject, object targetObject = null)
        {
            var lionBinding = new LionBinding(sourceObject, sourcePath, targetObject ?? sourceObject,
TargetPath
, flags: Flags);

        }

        public static void CreateBindings(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            foreach (var pi in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty))
            {
				foreach (LionBindingAttribute attr in (IEnumerable) pi.GetCustomAttributes(typeof(LionBindingAttribute), false).OfType<LionBindingAttribute>())
                {
                    if (String.IsNullOrEmpty(attr.TargetPath))
                    {
                        attr.TargetPath = pi.Name;
                    }
                    attr.CreateBinding(obj);
                }
            }
        }
    }

}
