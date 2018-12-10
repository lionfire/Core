using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.ObjectBus;
using LionFire.Referencing;
using LionFire.Structures;

namespace LionFire.Vos
{
    public class VosOBus : OBusBase, IOBus
    {
        #region (Static) Singleton

        public static VosOBus Instance => ManualSingleton<VosOBus>.GuaranteedInstance;

        #endregion

        #region Ontology

        public override IOBase SingleOBase => singleVBase;

        public VBase SingleVBase => singleVBase;
        private VBase singleVBase = new VBase();

        public VBase DefaultVBase => SingleVBase;
        
        #endregion


        public override IEnumerable<Type> HandleTypes
        {
            get
            {
                yield return typeof(VobHandle<>);
            }
        }

        public override IEnumerable<Type> ReferenceTypes
        {
            get
            {
                yield return typeof(VosReference);
            }
        }

        public override IEnumerable<string> UriSchemes => VosReference.UriSchemes;

        public override IOBase TryGetOBase(IReference reference) => ManualSingleton<VBase>.GuaranteedInstance;

        public override IReference TryGetReference(string referenceString, bool strictMode)
        {
            return VosReference.TryGetFromString(referenceString);
        }

        //public H<T> GetHandle<T>(IReference reference, T handleObject = default(T)) => throw new NotImplementedException();
        //public R<T> GetReadHandle<T>(IReference reference) => throw new NotImplementedException();
        //public bool IsCompatibleWith(string obj) => throw new NotImplementedException();
        //public bool IsCompatibleWith(IReference obj) => throw new NotImplementedException();
        //public bool IsValid(IReference reference) => throw new NotImplementedException();
        //public IOBase TryGetOBase(IReference reference) => throw new NotImplementedException();
        //public IReference TryGetReference(string uri, bool strictMode = false) => throw new NotImplementedException();


        //#region Uri Scheme

        //public string[] UriSchemes
        //{
        //    get { return VosReference.UriSchemes; }
        //}

        //public string DefaultUriScheme
        //{
        //    get { return VosReference.UriSchemeDefault; }
        //}

        //#endregion

        //#region OBases

        //public IOBase DefaultOBase
        //{
        //    get { return VBase.Default; }
        //}
        //public VBase DefaultVos
        //{
        //    get { return VBase.Default; }
        //}

        //public IEnumerable<IOBase> OBases
        //{
        //    get { yield return DefaultOBase; }
        //}

        //public IOBase GetOBaseForConnectionString(string connectionString)
        //{
        //    if (String.IsNullOrEmpty(connectionString)
        //        || String.Equals("default", connectionString.ToLowerInvariant())
        //        )
        //    {
        //        return DefaultOBase;
        //    }
        //    else
        //    {
        //        throw new NotImplementedException("Non-defuault VosBases not supported yet");
        //    }
        //}

        //public IEnumerable<IOBase> GetOBases(IReference reference)
        //{
        //    VosReference vref = reference as VosReference;
        //    if (vref == null) yield break;

        //    yield return DefaultOBase;
        //}

        //#endregion

        //#region IReferenceFactory

        //public IReference ToReference(string uri)
        //{
        //    int colonIndex = uri.IndexOf(':');
        //    if (colonIndex < 0)
        //    {
        //        throw new ArgumentException("Scheme missing");
        //    }

        //    #region // TODO: Verify scheme is supported!
        //    if (!uri.StartsWith(VosReference.UriSchemeDefault)) throw new ArgumentException("Unsupported scheme");
        //    #endregion

        //    int slashIndex = VosReference.UriSchemeDefault.Length;
        //    for (int i = 3; i > 0; i--)
        //    {
        //        slashIndex = uri.IndexOf('/', slashIndex);
        //        if (slashIndex < 0) throw new ArgumentException("Must be in the format scheme:///path");
        //    }

        //    string path = uri.Substring(slashIndex); // UNTESTED
        //    return new VosReference(path);
        //}

        //#endregion

        //#region Persistence

        //public void Set(IReference reference, object obj)
        //{
        //    VosReference vref = (VosReference)reference;

        //    //DefaultOBase.Set(reference, obj); - why was this here? duplicates!

        //    foreach (IOBase obase in  (IEnumerable)OBaseProviderBroker.GetOBaseProvider(reference).GetOBases(reference))
        //    {
        //        obase.Set(reference, obj);
        //    }
        //}

        //#endregion

    }
}
