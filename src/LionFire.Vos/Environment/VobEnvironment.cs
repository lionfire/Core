using LionFire.ExtensionMethods;
using LionFire.FlexObjects;
using LionFire.MultiTyping;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LionFire.Vos.Environment
{
    // TODO
    // If needed (not envisioned)
    //  - Overlay
    // Not envisioned to be needed:
    //  - fall-through inheritance from an IFlex from one VobEnvironment to a parent node.

    // FUTURE: For planned inheritance/overlay/stacking behavior within a key, see IFlexOverlay


    /// <summary>
    /// VobEnvironment is a dictionary of IFlex objects acquired by descendant Vobs
    /// </summary>
    public class VobEnvironment : VobNodeBase<VobEnvironment>, IHasFlexDictionary<string>, IFlexOwner, IFlexOverlayOwner
    {
        public VobEnvironment(Vob vob) : base(vob)
        {
        }

        #region Settings

        /// <summary>
        /// Default: true
        /// </summary>
        [DefaultValue(true)]
        public bool Inherit { get; set; } = true;

        #endregion

        #region State

        public FlexDictionary<string> FlexDictionary { get; } = new FlexDictionary<string>();
        //public ConcurrentDictionary<string, IFlex> Values { get; } = new ConcurrentDictionary<string, IFlex>();
        
        #endregion

        IFlexOwner IFlexOverlayOwner.ParentFlex => Inherit ? this.NextAncestor()?.Value : null;

        public T TryGet<T>(string key) => FlexDictionary.QueryFlex(key).Get<T>();
        
        public object this[string key]
        {
            get => FlexDictionary.QueryFlex(key)?.SingleValueOrDefault();
            set => FlexDictionary.GetFlex(key)?.Set(value);
        }

        //public void Add<TValue>(string key, TValue value)
        //    => Values.AddOrThrow<string, TValue>(key, value);

        //public TValue Get<TValue>(string key)
        //{
        //    if (Values.ContainsKey(key)) return (TValue)Values[key];
        //    if (Inherit)
        //    {
        //        var parent = this.ParentValue;
        //        if (parent != null) return parent.Get<TValue>(key);
        //    }
        //    return default;
        //}
    }

   
}
