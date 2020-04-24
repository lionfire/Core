#if OLD
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LionFire.DependencyMachines;
using LionFire.Referencing;
using LionFire.Structures;
using LionFire.Vos;

namespace LionFire.Services
{


    public class VobInitializer : StartableParticipant<VobInitializer>, IKeyed
    {
        public override string DefaultKey => Vob.Reference.Key;
        private IVob Vob { get; }

        #region Construction

        #region IVob

        #region Primary

        /// <summary>
        /// Targets Root Vob of default Root
        /// </summary>
        /// <param name="initializationAction"></param>
        public VobInitializer(IServiceProvider serviceProvider, IVosReference reference, IVos rootManager, Func<IServiceProvider, IVob, object?> initializationAction)
        {
            Vob = rootManager.GetVob(reference) ?? throw new Exception("Failed to get Vob '" + reference +"'.  Is the root name specified in VosOptions?");

            //InitializationAction = initializationAction;

            StartAction = (ctx, ct) =>
            {
                initializationAction(serviceProvider, ct, Vob);
            };
            
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


#endif