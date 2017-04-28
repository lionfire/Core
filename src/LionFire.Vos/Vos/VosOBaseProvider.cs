using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LionFire.ObjectBus
{
    public class VosOBaseProvider : IOBaseProvider, IReferenceFactory
    {
        #region (Static) Singleton

        public static VosOBaseProvider Instance { get { return Singleton<VosOBaseProvider>.Instance; } }

        #endregion

        #region Construction

        public VosOBaseProvider()
        {
        }

        #endregion

        #region Uri Scheme

        public string[] UriSchemes
        {
            get { return VosReference.UriSchemes; }
        }

        public string UriSchemeDefault
        {
            get { return VosReference.UriSchemeDefault; }
        }

        #endregion

        #region OBases

        public IOBase DefaultOBase
        {
            get { return Vos.Default; }
        }
        public Vos DefaultVos
        {
            get { return Vos.Default; }
        }

        public IEnumerable<IOBase> OBases
        {
            get { yield return DefaultOBase; }
        }

        public IOBase GetOBase(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString)
                || String.Equals("default", connectionString.ToLowerInvariant())
                )
            {
                return DefaultOBase;
            }
            else
            {
                throw new NotImplementedException("Non-defuault VosBases not supported yet");
            }
        }

        public IEnumerable<IOBase> GetOBases(IReference reference)
        {
            VosReference vref = reference as VosReference;
            if (vref == null) yield break;

            yield return DefaultOBase;
        }

        #endregion

        #region IReferenceFactory

        public IReference ToReference(string uri)
        {
            int colonIndex = uri.IndexOf(':');
            if (colonIndex < 0)
            {
                throw new ArgumentException("Scheme missing");
            }

            #region // TODO: Verify scheme is supported!
            if (!uri.StartsWith(VosReference.UriSchemeDefault)) throw new ArgumentException("Unsupported scheme");
            #endregion

            int slashIndex = VosReference.UriSchemeDefault.Length;
            for (int i = 3; i > 0; i--)
            {
                slashIndex = uri.IndexOf('/', slashIndex);
                if (slashIndex < 0) throw new ArgumentException("Must be in the format scheme:///path");
            }

            string path = uri.Substring(slashIndex); // UNTESTED
            return new VosReference(path);
        }

        #endregion

        #region Persistence

        public void Set(IReference reference, object obj)
        {
            VosReference vref = (VosReference)reference;

            //DefaultOBase.Set(reference, obj); - why was this here? duplicates!

            foreach (IOBase obase in  (IEnumerable)OBaseProviderBroker.GetOBaseProvider(reference).GetOBases(reference))
            {
                obase.Set(reference, obj);
            }
        }

        #endregion

    }
}
