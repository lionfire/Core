using LionFire.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Vos
{


    public interface IVohac<T> : INotifyingList<VobHandle<T>>
        where T : class, new()
    {
        VobHandle<T> this[string name] { get; }
        void RefreshCollection();
    }

    public interface IVoc
    {
        Vob Vob { get; set; }

        bool? AutoLoad { get; set; }
        bool? AutoSave { get; set; }

        #region Move / Delete Methods

        void Move(Vob vobDestination);
        void Rename(string newName);
        void Delete();

        #endregion
    }


    public abstract class VocBase : IVoc
    {

        #region Vob

        /// <summary>
        /// User can only set this once directly.  It may be modified by Move, Rename or Delete
        /// </summary>
        public Vob Vob
        {
            get { return vob; }
            set
            {
                if (vob == value) return;
                if (vob != default(Vob)) throw new AlreadySetException();
                vob = value;
                //OnVobChanged();

                if (EffectiveAutoLoad)
                {
                    //Clear();
                    TryRetrieve();
                }

                var ev = VobChangedFor; if (ev != null) ev(this);
            }
        } private Vob vob;

        public event Action<IVoc> VobChangedFor;

        //protected void OnVobChanged()
        //{
        //    if (EffectiveAutoLoad)
        //    {
        //        Clear();
        //        TryRetrieve();
        //    }
        //}

        #endregion

        #region Parameters

        #region AutoLoad

        public static bool DefaultAutoLoad = true;

        public bool EffectiveAutoLoad { get { return AutoLoad.HasValue ? AutoLoad.Value : DefaultAutoLoad; } }

        public bool? AutoLoad
        {
            get { return autoLoad; }
            set { autoLoad = value; }
        } private bool? autoLoad;

        #endregion

        public bool? AutoSave { get; set; }

        #endregion

        #region CRUD

        #region Retrieve

        public abstract bool TryRetrieve();

        #endregion

        #region Move / Delete Methods

        public abstract void Move(Vob vobDestination);
        public abstract void Rename(string newName);
        public abstract void Delete();

        #endregion

        #endregion

        #region List

        public void Clear()
        {
            if (Vob == null) return;
            VobFilter filter = null;
            Vob.DeleteChildren(filter);
        }

        public abstract int Count { get; }

        #endregion

        #region Misc

        public string ToStringStatus
        {
            get
            {
                if (Vob == null) return "null";
                //if (Count > 0)
                return Count.ToString() + " items";
            }
        }

        public override string ToString()
        {
            return "[Voc: " + Vob.ToString() + " (" + ToStringStatus + ")]";
        }

        #endregion
    }

}
