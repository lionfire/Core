#nullable enable
using System;
using System.Collections.Generic;
using LionFire.DependencyMachine;
using LionFire.Referencing;
using LionFire.Vos;

namespace LionFire.Services
{

    public class VobInitializer
    {
        public IVosReference Reference { get; }
        public Func<IServiceProvider, IVob, object?> InitializationAction { get; set; }

        public IDependencyMachineParticipant Reactor { get; set; }

        #region Construction

        #region IVob

        #region Primary

        /// <summary>
        /// Targets Root Vob of default Root
        /// </summary>
        /// <param name="initializationAction"></param>
        public VobInitializer(IVosReference reference, Func<IServiceProvider, IVob, object?> initializationAction)
        {
            Reference = reference;
            InitializationAction = initializationAction;
        }

        #endregion

        /// <summary>
        /// Targets Root Vob of default Root
        /// </summary>
        /// <param name="initializationAction"></param>
        public VobInitializer(IVosReference reference, Action<IServiceProvider, IVob> initializationAction)
            : this(reference, (serviceProvider, vob) =>
            {
                initializationAction(serviceProvider, vob);
                return null;
            })
        {
        }

        public VobInitializer(IVosReference reference, Action<IVob> initializationAction) : this(reference, (_, vob) =>
        {
            initializationAction(vob);
            return null;
        })
        {
        }

        #endregion

        #region IRootVob

        #region Primary

        public VobInitializer(IVosReference reference, Func<IServiceProvider, IRootVob, object> initializationAction)
     : this(reference, (serviceProvider, vob) => initializationAction(serviceProvider, (IRootVob)vob))
        {
        }

        #endregion

        public VobInitializer(IVosReference reference, Action<IServiceProvider, IRootVob> initializationAction)
                 : this(reference, (serviceProvider, vob) =>
                 {
                     initializationAction(serviceProvider, (IRootVob)vob);
                     return null;
                 })
        {
        }
        public VobInitializer(IVosReference reference, Action<IRootVob> initializationAction) : this(reference, (_, vob) =>
        {
            initializationAction((IRootVob)vob);
            return null;
        })
        {
        }

        #endregion

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


