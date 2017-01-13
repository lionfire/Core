/*
using LionFire.States;
using LionFire.Instantiating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    public static class AssetInstantiationExtensions
    {
        public static object GetInstantiation<T>(this T obj)
        {
            var objTemplateInstance = obj as ITemplateInstance;

            var tAsset = objTemplateInstance?.Template as IAsset;

            Instantiation inst = null;

            if (objTemplateInstance != null)
            {
                inst = new Instantiation();
                
                if(tAsset != null)
                {
                    inst.TemplateSubPath = tAsset.AssetSubPath;
                }
                else
                {
                    inst.Template = objTemplateInstance.Template;
                }
                inst.State = obj.GetState();
                return inst;
            }

            return obj;
        }
    }
}
*/