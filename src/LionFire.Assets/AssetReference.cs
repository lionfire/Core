﻿using LionFire.Ontology;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Assets
{
    // REVIEW - RENAME this DLL to just LionFire.Assets?  Or move this AssetReference<T> to that DLL?

    [TreatAs(typeof(IAssetReference))]
    public class AssetReference<TValue> : ReferenceBase<AssetReference<TValue>>, IAssetReference//, ITypedReference<TValue, AssetReference<TValue>>
    {
        public Type Type => typeof(TValue);

        #region Scheme

        public const string UriSchemeDefault = "asset";
        public const string UriPrefixDefault = "asset:";

        public override IEnumerable<string> AllowedSchemes
        {
            get { yield return UriSchemeDefault; }
        }

        public override string Scheme => UriSchemeDefault;

        public override string Key { get => Path; protected set => Path = value; }

        #region Path

        [SetOnce]
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

        public static readonly string[] UriSchemes = new string[] { UriSchemeDefault };

        #endregion

        #region Construction

        public AssetReference(string assetPath)
        {
            this.Path = assetPath;
        }

        #endregion

        public static implicit operator AssetReference<TValue>(string assetPath) => new AssetReference<TValue>(assetPath);
    }
}