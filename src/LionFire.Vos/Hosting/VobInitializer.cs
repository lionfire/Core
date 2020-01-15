using System;
using System.Collections.Generic;
using LionFire.Referencing;
using LionFire.Vos;

namespace LionFire.Services
{
    public class VobInitializer
    {
        public IVosReference Reference { get; }
        public Action<IServiceProvider, IVob> InitializationAction { get; set; }

        #region Construction

        /// <summary>
        /// Targets Root Vob of default Root
        /// </summary>
        /// <param name="initializationAction"></param>
        public VobInitializer(IVosReference reference, Action<IServiceProvider, IVob> initializationAction)
        {
            Reference = reference;
            InitializationAction = initializationAction;
        }

        public VobInitializer(IVosReference reference, Action<IVob> initializationAction)
        {
            Reference = reference;
            InitializationAction = (_, v) => initializationAction(v);
        }

        ///// <summary>
        ///// Targets default Root
        ///// </summary>
        ///// <param name="path"></param>
        ///// <param name="initializationAction"></param>
        //public VobInitializer(string path, Action<IServiceProvider, IVob> initializationAction)
        //{
        //    Reference = new VosReference(path);
        //    InitializationAction = initializationAction;
        //}
        
        #endregion
    }
}


