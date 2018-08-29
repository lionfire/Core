using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{


    

    public class VobTypeRegistrar
    {
        #region Static Accessor

        public static VobTypeSettings Instance { get { return Singleton<VobTypeSettings>.Instance; } }

        #endregion

        #region Fields

        private Dictionary<Type, VobTypeSettings> settingsByType = new Dictionary<Type, VobTypeSettings>();

        #endregion

        #region Get Settings

        //public VobTypeSettings SettingsForType(Vob vob)
        //{
        //}

        public VobTypeSettings SettingsForType(Type type)
        {
            var s = settingsByType.GetOrAddNewKeyed(type);
            return s;
        }

        #endregion


        public object GetOrAssignKey(Vob vob)
        {
            //if (vob.Name != null) return vob.Name;


            //var h = vob.UnitypeHandle;
            //if (h == null)
            //{

            //}

            //// Empty case -- get key generator from parent
            //if (vob.UnitypeHandle.HasObject)
            //{

            //}


            ////if(vob.UnitypeHandle

            ////var s = SettingsForType(vob);

            throw new NotImplementedException();
        }
    }
}
