using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;
using LionFire.Types;

namespace LionFire.ObjectBus
{
    //internal class CachingVosDirectory : VosDirectory
    //{
    //    public override VosDirectory TryGetDirectory(string subpath)
    //    {
    //        return base.TryGetDirectory(subpath);
    //    }
    //}



    public class VosDirectory
    {
        private MultiBindableDictionary<string, IHandle> objects = new MultiBindableDictionary<string, IHandle>();

        VosDirectory parent;

        public VosDirectory(VosDirectory parent)
        {
            this.parent = parent;
        }

        //public virtual VosDirectory TryGetDirectory(string subpath)
        //{
            
        //}

        public IHandle this[string subpath, bool autoCreateDirectory = false]
        {
            get
            {
                throw new NotImplementedException();
                //var subPaths = subpath.Split(VosPath.Separator);
                //int index = 0;

                //IHandle child = objects.TryGetValue(subPaths[index]);

                //if(child)
                //{
                //}

                //if(objects.subPaths[index]
            }
        }

        public IHandle this[string[] subpaths, int startIndex]
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
