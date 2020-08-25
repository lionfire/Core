#if IDEA // Overkill?
using LionFire.Copying;
using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Structures;
using System;
using System.Collections.Generic;

namespace LionFire.Assets
{

    [TreatAs(typeof(IAssetReference))]
    public class PersisterReference<TValue> : ReferenceBase<PersisterReference<TValue>>, ITypedReference<TValue, PersisterReference<TValue>>
        where TValue : IKeyed<string>
    {
        public Type Type => typeof(TValue);

#region Scheme

        public const string UriSchemeDefault = "persister-key";
        public const string UriPrefixDefault = "persister-key:";
        public static readonly string[] UriSchemes = new string[] { UriSchemeDefault };

        public override IEnumerable<string> AllowedSchemes { get { yield return UriSchemeDefault; } }

        public override string Scheme => UriSchemeDefault;

        [Ignore]
        public override string Key
        {
            get => $"{UriPrefixDefault}({(Persister == null ? "$keyed/" : Persister + "/")}{typeof(TValue)}){Path}";
            protected set => throw new NotImplementedException();/* Path = value;*/
        }

#region Path

        /// <summary>
        /// Can be null if object is not intended to be persisted (perhaps because it is contained by a parent object that itself will be persisted.)
        /// </summary>
        [SetOnce]
        [Assignment(AssignmentMode.Ignore)]
        public override string Path
        {
            get => path;
            set
            {
                if (path == value) return;
                if (path != default) throw new AlreadySetException();
                path = value;
            }
        }
        private string path;

#endregion

#endregion

#region Construction

        public PersisterReference()
        {
            //this.Path = string.Empty;
            this.Path = default;
        }
        public PersisterReference(string assetPath = "")
        {
            if (assetPath?.Contains(":") == true) throw new ArgumentException($"{nameof(assetPath)} may not contain :");
            this.Path = assetPath;
        }
        public PersisterReference(string assetPath, string assetChannel)
        {
            this.Path = assetPath;
            this.Channel = assetChannel;
        }

        public static PersisterReference<TValue> ForChannel(string assetChannel, string assetPath = "")
            => new PersisterReference<TValue>(assetPath, assetChannel);

#endregion

        public static implicit operator PersisterReference<TValue>(string str) => ParseKey(str) ?? new PersisterReference<TValue>(str);

        public static PersisterReference<TValue> ParseKey(string referenceKey)
        {
            if (referenceKey == null) return null;
            if (!referenceKey.StartsWith(UriSchemeDefault)) return null;

            throw new NotImplementedException();
        }
       
        public override string ToString() => Key;

#region Persister

        [SetOnce]
        public override string Persister
        {
            get => persister;
            set
            {
                if (persister == value) return;
                if (persister != default) throw new AlreadySetException();
                persister = value;
            }
        }
        private string persister;

#endregion
    }
}
#endif
