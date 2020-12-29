#nullable enable

using LionFire.Copying;
using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Structures;
using LionFire.Typing;
using System;
using System.Collections.Generic;

namespace LionFire.Data.Id
{

    /// <summary>
    /// Reference an object of type TValue by its Id.  An IIdAdapter service should be available from the DI mechanism as well as IIdMappingStrategy classes that can resolve the Id from your objects (for example, by an Id or Key or Name property.)
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [TreatAs(typeof(IIdReference))]
    public class IdReference<TValue> : ReferenceBase<IdReference<TValue>>, ITypedReference<TValue, IdReference<TValue>>, IIdReference
    {
        IdReference<TValue> ITypedReference<TValue, IdReference<TValue>>.Reference => this;
        public Type Type => typeof(TValue);

        #region Scheme

        public const string UriSchemeDefault = "id";
        public const string UriPrefixDefault = UriSchemeDefault + ":";
        public static readonly string[] UriSchemes = new string[] { UriSchemeDefault };

        public override IEnumerable<string> AllowedSchemes { get { yield return UriSchemeDefault; } }

        public override string Scheme => UriSchemeDefault;

        #endregion

        [Ignore]
        public override string Key
        {
            get => $"{UriPrefixDefault}({("$id/")}{TypeNameResolver.ToName(typeof(TValue))}){Path}";
            protected set => throw new NotImplementedException();/* Path = value;*/
        }

        public string Id => Path;

        #region Path

        /// <summary>
        /// For IdReference, this is the Id for the object that is resolved from IIdAdapter.
        /// 
        /// It could be null if object is not intended to be persisted directly (perhaps because it is contained by a parent object that itself will be persisted), though ideally, no IdReferences should be created for such objects
        /// </summary>
        [SetOnce]
        [Assignment(AssignmentMode.Ignore)]
        public override string? Path
        {
            get => path;
            protected set
            {
                if (path == value) return;
                if (path != default) throw new AlreadySetException();
                path = value;
            }
        }
        private string? path;
        protected override void InternalSetPath(string path) => this.path = path;

        #endregion

        #region Construction

        //public IdReference()
        //{
        //    this.Path = default;
        //}
        public IdReference(string? id = null)
        {
            if (id?.Contains(":") == true) throw new ArgumentException($"{nameof(id)} may not contain :");
            this.path = id;
        }
        
        #endregion

        #region Static Conversion

        public static implicit operator IdReference<TValue>(string str) => ParseKey(str) ?? new IdReference<TValue>(str);

        public static IdReference<TValue>? ParseKey(string referenceKey)
        {
            if (referenceKey == null) return null;
            if (!referenceKey.StartsWith(UriSchemeDefault)) return null;

            throw new NotImplementedException();
        }

        #endregion

        public override string ToString() => Key;
    }
}
