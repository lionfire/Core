//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using LionFire.Collections;
//using LionFire.Vos.Filesystem;

//namespace LionFire.Vos
//{
    
//    public class VosMountManager
//    {

//        #region Construction

//        private readonly Vos vos;
//        public VosMountManager(Vos vos)
//        {
//            if (vos == null) throw new ArgumentNullException("vos");
//            this.vos = vos;
//        }

//        #endregion

//        public int MountStateVersion
//        {
//            get;
//            private set;
//        }


//        //public IEnumerable<Mount> GetMountsForPath(string vosPath)
//        //{
//        //    List<Mount> mounts = new List<Mount>();
//        //    foreach (var mount in Mounts.Where(m => vosPath.StartsWith(m.MountPath)))
//        //    {
//        //        mounts.Add(mount);
//        //    }
//        //    return mounts;
//        //}

//        #region Mount / Unmount

//        public void Mount(string layerName, string vosPath, IReference root = null, MountOptions? mountOptions = null)
//        {
//            Mount(layerName, vosPath, root.ToHandle(), mountOptions);
//        }

//        public void Mount(string layerName, string vosPath, IHandle root = null, MountOptions? mountOptions = null)
//        {
//            var mount = new Mount()
//            {
//                MountName = layerName,
//                MountPath = vosPath,
//                RootHandle = root,
//                MountOptions = mountOptions.HasValue ? mountOptions.Value : (MountOptions?)null,
//            };

//            Vob vob = vos[vosPath];
//            vob.Mounts.Add(mount);

//            MountStateVersion++;
//        }

//        public int Unmount(string path)
//        {
//            int count = 0;
//            foreach (var mount in mounts.ToArray().Where(m => path == m.MountPath))
//            {
//                mounts.Remove(mount);
//                count++;
//            }
//            MountStateVersion++;
//            return count;
//        }

//        #endregion

//    }
//}
