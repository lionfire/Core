using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using LionFire.Assets;
using LionFire.Copying;
using LionFire.ObjectBus;
using LionFire.Persistence;
using Microsoft.Extensions.Logging;

namespace LionFire.Instantiating
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DefaultParentKeyAttribute : Attribute
	{
		readonly string defaultParent;

		public DefaultParentKeyAttribute(string defaultParentKey)
		{
			this.defaultParent = defaultParentKey;
		}

		public string DefaultParentKey {
			get { return defaultParent; }
		}
	}

	public class TemplateParametersBase<TemplateType> : AssetInstantiation, IParentedTemplateParameters
        , ITemplateParameters<TemplateType>
        where TemplateType : class
        //, IAsset
        , ITemplateAsset
	{

        #region Construction

		public TemplateParametersBase()
		{
		}
        //public TemplateParametersBase(AssetReference<ITemplate> template)
        //    : base(template)
        //{
        //}

#if true //!AOT
        public TemplateParametersBase(HAsset<TemplateType> template, string parentKey = null)
            : base(
#if AOT
                        (IReadHandle) // For Mono on Mac BACKPORTED
#else
                        (RH<TemplateType>) // For Mono on Mac BACKPORTED
#endif
						template)
        {
            //Log.Info("ZX TemplateParametersBase.ctor");
            //Log.Info("ZX TemplateParametersBase.ctor " + (template == null ? "" : template.Reference.ToString()));

            if (parentKey == null)
            {
                var attr = typeof(TemplateType).GetCustomAttribute<DefaultParentKeyAttribute>();
                if (attr != null)
                {
                    parentKey = attr.DefaultParentKey;
                }
            }
            this.ParentKey = parentKey;
                    //GC.Collect();
            //Log.Info("ZX TemplateParametersBase.ctor end " + template.Reference);
					
        }
#endif

        //public TemplateParametersBase(string templateAssetName)
        //    : base((HAsset<TemplateType>)templateAssetName) // Do you believe in magic?
        //{
        //}

		public TemplateParametersBase(ITemplateAsset template)
		{
            var hasHA = template as IHasHAsset;
            if (hasHA != null && hasHA.HAsset != null)
            {
                base.TemplateAsset =
                    (IReadHandle
                    #if !AOT && !UNITY // Unity crashes with contravariant IReadHandle
            < ITemplateAsset >
#endif
            )
                    hasHA.HAsset;
            }
            else
            {
                //base.Template = new AssetReference<ITemplate>(template);
                base.TemplateAsset = new HAsset
#if !AOT
                        <ITemplateAsset>
#endif
                            (template);
            }
		}

#if AOT
        //public TemplateParametersBase(HAsset hAsset)
        //{
        //    base.Template = hAsset;
        //}
#endif

#endregion

        #region Parameters

		[Ignore]
		[SerializeDefaultValue(false)]
		[Assignment(AssignmentMode.Ignore)]
		public override ITemplateParameters Parameters { get => this; set { if (object.ReferenceEquals(this, value)) { return; } throw new NotSupportedException("Parameters == this and cannot be changed."); } }
        #endregion

        #region Template

        public TemplateType OTemplate {
#if AOT
			get { if(TemplateAsset==null)return null; 
						object obj = TemplateAsset.Object;
						if(obj==null)return null;
						if(obj.GetType() == typeof(string))
						{
							l.Error("AOTERROR - " + this.ToString() + " Template.Object has unexpected type.  Template type: " + TemplateAsset.ToTypeFullNameSafe());
							var x = (IReadHandle)TemplateObj;
							obj = x.Object;
							if(obj.GetType() == typeof(string))
							{
                                l.Error("AOTERROR - " + this.ToString() + " Template.Object has unexpected type.  Template type: " + TemplateAsset.ToTypeFullNameSafe());
							}
						}
						TemplateType result = TemplateAsset.Object as TemplateType; 
						return result; }
#else
					get { TemplateType result = Template; return result; }
#endif
        }

        ///// <summary>
        ///// Intended for use only by serializer (TODO: Make private)
        ///// </summary>
        //[SerializeDefaultValue(false)]
        //public TemplateType TemplateObject
        //{
        //    get
        //    {
        //        if (this.Template != null)
        //        {
        //            if (string.IsNullOrEmpty(Template.AssetPath) && Template.HasObject)
        //            {
        //                return Template.Object;
        //            }
        //        }
        //        return null;
        //    }
        //    set
        //    {
        //        if (Template != null)
        //        {
        //            l.Warn("Template != null when setting TemplateObject");
        //        }
        //        else
        //        {
        //            if (value != null)
        //            {
        //                Template = value;
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Intended for use only by serializer (TODO: Make private)
        ///// </summary>
        //[SerializeDefaultValue(false)]
        //public string TemplatePath
        //{
        //    get
        //    {
        //        if (this.Template != null) return Template.AssetPath;
        //        return null;
        //    }
        //    set
        //    {
        //        if (Template != null)
        //        {
        //            l.Warn("Template != null when setting TemplateObject");
        //        }
        //        else
        //        {
        //            if (value != null)
        //            {
        //                Template = value;
        //            }
        //        }
        //    }
        //}

				#if !AOT  // public new HAsset<TemplateType> Template
            [Ignore]
        [SerializeDefaultValue(false)]
        [Assignment(AssignmentMode.Assign)]
        public new HAsset<TemplateType> Template
        {
            get
            {
                return (HAsset<TemplateType>)base.TemplateAsset;
            }
            set
            {
                base.TemplateAsset = value.ToRegistered();
            }
        }
#endif
        //// TODO: Replace with generic base classes?
        //public new AssetReference<TemplateType> Template
        //{
        //    get
        //    {
        //         var ar = new AssetReference<TemplateType>(base.Template.ID);
        //        if (base.Template.HasAsset) ar.Asset =  (TemplateType) base.Template.Asset;
        //        return ar;
        //    }
        //    set
        //    {
        //       base.Template = new AssetReference<ITemplate>(value.ID);
        //       if (value.HasAsset)
        //       {
        //           base.Template.Asset = value.Asset;
        //       }
        //    }
        //}

        #endregion

        

        //#region Pid

        //public short Pid
        //{
        //    get { return pid; }
        //    set
        //    {
        //        if (pid == value) return;
        //        if (pid != 0) throw new NotSupportedException("Pid can only be set once.");
        //        pid = value;
        //    }
        //} private short pid;

        //#endregion


        #region Misc

        private static ILogger l = Log.Get();

        #endregion

    }
}

//public interface ITemplateParameterization<TemplateType, InstanceType>
////where InstanceType : class, new()
//{
//    TemplateType Template { get; set; }
//}

//public class ParameterBase<TemplateType, InstanceType> : ITemplateParameterization<TemplateType, InstanceType>
//{
//    public TemplateType Template { get; set; }
//    public InstanceType Create
//}
