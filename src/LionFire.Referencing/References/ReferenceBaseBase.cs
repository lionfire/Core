using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    public abstract class ReferenceBaseBase
    {
        protected virtual void CopyFrom(IReference other, string newPath = null)
        {
            this.Path = newPath;
        }

        public abstract string Path { get; set; }
        
        #region Children

        public virtual IReference GetChild(string subPath)
        {
            // Use ctor instead? Or reference factory?

            var result = (ReferenceBase)Activator.CreateInstance(this.GetType());
            result.CopyFrom((IReference)this, this.Path + String.Concat(ReferenceConstants.PathSeparator, subPath));
            return result;
        }

        //public IReference GetChildSubpath(params string[] subpath)
        public IReference GetChildSubpath(IEnumerable<string> subpath)
        {
            var sb = new StringBuilder();
            bool isFirst = true;
            foreach (var subpathChunk in subpath)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else { sb.Append("/"); }
                sb.Append(subpathChunk);
            }
            return GetChild(sb.ToString());
        }

        #endregion

    }
}
