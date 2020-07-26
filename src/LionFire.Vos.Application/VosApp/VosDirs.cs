//#define ASSETS_SUBPATH // Prefer this off?  TODO - make sure this works for packages
using LionFire.Assets;
using LionFire.Persistence;
using LionFire.Vos.Services;
using Microsoft.Extensions.Logging;

namespace LionFire.Vos.VosApp
{
    public class VosDirs
    {
        #region Ontology

        public IRootVob Root { get; }

        #endregion

        public VosAppOptions Options => options ??= Root.GetService<VosAppOptions>();
        private VosAppOptions options;

        #region Construction

        public VosDirs(IRootVob root)
        {
            this.Root = root;
        }

        #endregion

        #region ActiveData

        public bool HasActiveData => activeData != null;

        /// <summary>
        /// The primary place to store application data.  Multiple locations may be mounted at this point to create an overlay effect.  Data that is written typically goes to the OS-specific location for variable program data (such as /var or C:\ProgramData\MyApp)
        /// </summary>
        public IVob ActiveData => activeData ??= Root[VosPaths.ActiveDataPath];
        private IVob activeData;

        #endregion

        public IVob Settings => settings ??= Root[VosPaths.Settings];
        private IVob settings;

        //public IVob DBs => dbs ??= Root[VosPaths.DBsPath];
        //private IVob dbs;
        //public IVob AppData => settings ??= Root["AppData"];
        //private IVob appData;

    }
}
