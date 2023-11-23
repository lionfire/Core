﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

#if OverlayProxies
using Castle.DynamicProxy;
#endif

namespace LionFire.Overlays
{
    public static class OverlayFactory<T>
        where T : class, new()
    {


        private static T autoCreatedDefaultObject;

        public static T Create(OverlayParameters nParameters = null, T defaultInstance = null)
        {
            //			if (!nParameters.HasValue)
            if (nParameters == null)
                nParameters = OverlayParameters.UnlockedTopDown;

            var parameters = nParameters
                    //				.Value;
                    ;

            if (parameters.DefaultObject == null && parameters.AutoCreateDefaultObject)
            {
                if (autoCreatedDefaultObject == null)
                {
                    autoCreatedDefaultObject = new T();

                    //					l.Warn("AOTTODO May not be safe for AOT in OverlayFactory.Create");
                    LionFire.Extensions.DefaultValues.DefaultValueUtils.ApplyDefaultValues<T>(autoCreatedDefaultObject); // AOTREVIEW GENERICMETHOD - works
                                                                                                                         //					l.Warn("AOTTODO May not be safe for AOT in OverlayFactory.Create... worked");
                }
                parameters.DefaultObject = autoCreatedDefaultObject;
            }

            T proxy;

#if OverlayProxies

            var options = new ProxyGenerationOptions(new OverlayProxyGenerator())
            {
            };
            options.AddMixinInstance(new OverlayMixin<T>(parameters, defaultInstance));

            ProxyGenerator generator = new ProxyGenerator();
            proxy = generator.CreateClassProxy<T>(options, new OverlayInterceptor());

#else
            if (defaultInstance != null)
			{
				l.Warn("STUB - OverlayFactory.Create - default " + typeof(T).Name);
				proxy = defaultInstance;
			}
			else
			{
//				l.Warn("STUB - OverlayFactory.Create - new " + typeof(TValue).Name);
				
				proxy = defaultInstance = new T();
//				var mixin = new OverlayMixin<TValue>(nParameters.HasValue?nParameters.Value : new OverlayParameters(), defaultInstance);

//				proxy = mixin.DefaultInstance;
			}
#endif
            return proxy;

        }

//
//        public static TValue Create(OverlayParameters? nParameters = null, TValue defaultInstance = null)
//        {
//            
//        }
//#endif

        private static readonly ILogger l = Log.Get();
		
    }

}

