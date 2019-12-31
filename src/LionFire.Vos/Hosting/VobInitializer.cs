using System;
using System.Collections.Generic;
using LionFire.Referencing;
using LionFire.Vos;

namespace LionFire.Services
{
    public class VobInitializer
    {
        public string VobRootName { get; set; } = VosConstants.DefaultRootName;

        public string VobPath { get; set; }

        public IEnumerable<string> VobPathChunks { get => LionPath.ToPathArray(VobPath); set => VobPath = LionPath.FromPathArray(value); }

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


