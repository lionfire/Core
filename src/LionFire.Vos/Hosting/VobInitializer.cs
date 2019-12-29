using System;
using LionFire.Vos;

namespace LionFire.Services
{
    public class VobInitializer
    {
        public string VobRootName { get; set; } = VosConstants.DefaultRootName;

        public string VobPath { get; set; }

        /// <summary>
        /// Targets Root Vob of default Root
        /// </summary>
        /// <param name="initializationAction"></param>
        public VobInitializer(Action<IVob> initializationAction)
        {
            InitializationAction = initializationAction;
        }

        /// <summary>
        /// Targets default Root
        /// </summary>
        /// <param name="path"></param>
        /// <param name="initializationAction"></param>
        public VobInitializer(string path, Action<IVob> initializationAction)
        {
            VobPath = path;
            InitializationAction = initializationAction;
        }
        public Action<IVob> InitializationAction { get; set; }
    }
}


