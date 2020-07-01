//#define DISABLE_SHAREDRD
//#define TRACE_FILE
// License: ?
// Retrieved from http://www.wpftutorial.net/MergedDictionaryPerformance.html on 11.01.22


// Also see:

// TODO: A better approach?? 
// http://stackoverflow.com/questions/8712228/wpf-sharedresourcedictionary

// Defining and Using Shared Resources in a Custom Control Library
// http://blogs.msdn.com/b/wpfsdk/archive/2007/06/08/defining-and-using-shared-resources-in-a-custom-control-library.aspx - doesn't work in Cider?

// LIVING WITHOUT APP.XAML (…AND STILL BEING ABLE TO USE BLEND!)
// http://blogs.msdn.com/b/expression/archive/2008/04/09/creating-a-wpf-blend-project-that-loads-resources-in-code.aspx

// Best Practices: How to handle shared resources in a modular application?
// http://compositeextensions.codeplex.com/discussions/42919?ProjectName=compositeextensions

// http://drwpf.com/blog/2007/10/05/managing-application-resources-when-wpf-is-hosted/
// http://leecampbell.blogspot.ca/2010/05/mergeddictionaries-performance-problems.html

// Creating and Consuming Resource Dictionaries in WPF and Silverlight
// http://blogs.msdn.com/b/wpfsldesigner/archive/2010/06/03/creating-and-consuming-resource-dictionaries-in-wpf-and-silverlight.aspx

// ComponentResourceKey as DynamicResource problem
// http://stackoverflow.com/questions/1610090/componentresourcekey-as-dynamicresource-problem

// Managing Application Resources when WPF is Hosted
// http://drwpf.com/blog/2007/10/05/managing-application-resources-when-wpf-is-hosted/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;
using System.ComponentModel;

namespace LionFire.Avalon
{
    // TODO MEMORYOPTIMIZE - Try using this everywhere and compare memory usage.  See if it works in VS 11 Designer.

    //TODO: In order to overcome the designer issue I used a Powershell script in a Pre and Post Build event to change ResourceDictionary to SharedResourceDictionary at build time.


    //[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "WPFTutorial.Utils")]
    /// <summary>
    /// The shared resource dictionary is a specialized resource dictionary
    /// that loads it content only once. If a second instance with the same source
    /// is created, it only merges the resources from the cache.
    /// </summary>
    public class SharedResourceDictionary : ResourceDictionary
    {
#if !DISABLE_SHAREDRD

#if TRACE_FILE
        private static readonly ILogger l = Log.Get();
#endif

        /// <summary>
        /// Internal cache of loaded dictionaries 
        /// </summary>
        public static Dictionary<Uri, ResourceDictionary> _sharedDictionaries =
            new Dictionary<Uri, ResourceDictionary>();
        private static object dictLock = new object();




        //protected override void OnGettingValue(object key, ref object value, out bool canCache)
        //{
        //    if (cachedResourceDictionary != null)
        //    {
        //        cachedResourceDictionary.OnGettingValue(key, ref value, out canCache);
        //    }
        //    else
        //    {
        //        base.OnGettingValue(key, ref value, out canCache);
        //    }
        //}

        ResourceDictionary cachedResourceDictionary;


#if true // Experiment from http://stackoverflow.com/questions/8712228/wpf-sharedresourcedictionary
        public new Uri Source
        {
            get { return _sourceUri; }
            set
            {
                _sourceUri = value;
                if (!_sharedDictionaries.ContainsKey(value))
                {
                    try
                    {
                        //If the dictionary is not yet loaded, load it by setting
                        //the source of the base class
                        base.Source = value;
                    }
                    catch (Exception )
                    {
                        //only throw exception @runtime to avoid "Exception has been 
                        //thrown by the target of an invocation."-Error@DesignTime
                        if (!IsInDesignMode)
                            throw;
                    }
                    // add it to the cache
                    _sharedDictionaries.Add(value, this);
                }
                else
                {
                    // If the dictionary is already loaded, get it from the cache 
                    MergedDictionaries.Add(_sharedDictionaries[value]);
                }
            }
        }
#else
        /// <summary>
        /// Gets or sets the uniform resource identifier (URI) to load resources from.
        /// </summary>
        public new Uri Source
        {
            get
            {
                if (IsInDesignMode)
                {
                    return base.Source;
                }

                return _sourceUri;
            }
            set
            {
                //And this put in front of the old setter-code : 

#if true // Use normal approach for designer
                if (IsInDesignMode)
                {
#if TRACE_FILE
                    l.Trace("SharedResourceDictionary IsInDesignMode: Falling back to normal approach = " + value);
#endif
#if true
                    base.Source = value;
#else
                    MergedDictionaries.Add(new ResourceDictionary { Source = value }); // Takes up too much memory?  Too intensive for Xdesproc?
                    this.Add(Guid.NewGuid().ToString(), null); // TEST - does it need to contain a dummy item?
#endif
                    return;
                }
#endif

                try
                {
                    _sourceUri = new Uri(value.OriginalString); // Allows garbage collection, according to comment.  
                }

                catch (Exception
#if TRACE_FILE
                    ex
#endif
)
                {
#if TRACE_FILE
                    l.Trace("SharedResourceDictionary new Uri(value.OriginalString) threw exception for  " + value + " (falling back to normal setter): " + ex);
#endif
                    _sourceUri = value;
                }

                //l.Trace("SharedResourceDictionary.Source = " + value);

                lock (dictLock)
                {
                    if (!_sharedDictionaries.ContainsKey(value))
                    {
                        try
                        {
                            //if (cacheMissCount++ % 25 == 0)
                            //{
                            //    l.Debug("SharedResourceDictionary loading  " + value + " (Count: " + cacheMissCount + ")");
                            //}

                            // If the dictionary is not yet loaded, load it by setting
                            // the source of the base class
                            base.Source = value;
                        }
                        catch (Exception)
                        {
                            // EMPTYCATCH
                        }

                        // add it to the cache
                        _sharedDictionaries.Add(value, this);
                    }
                    else
                    {
                        this.cachedResourceDictionary = _sharedDictionaries[value];

                        // If the dictionary is already loaded, get it from the cache
                        if (!MergedDictionaries.Contains(cachedResourceDictionary))
                        {
                            MergedDictionaries.Add(cachedResourceDictionary);
#if TRACE_FILE
                            //if(cacheHitCount++ % 100 == 0)
                            //{
                            //    l.Trace("SharedResourceDictionary using existing:  " + value   + " (Count: " + cacheHitCount + ")");
                            //}
#endif
                            this.Add(Guid.NewGuid().ToString(), null); // TEST - does it need to contain a dummy item?

                            //base.Source = value;
                        }
                        //else
                        //{
                        //    throw new Exception("Caught duplicate!!!!"); // TEMP
                        //}
                    }
                }
            }
        } 
#endif
        private Uri _sourceUri;

#if TRACE_FILE
        private static int cacheHitCount = 0;
        private static int cacheMissCount = 0;
#endif

        #region Addition from comment to permit designer use (but high memory use in designer)

        private static bool IsInDesignMode
        {
            get
            {
                var result = (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty,
                typeof(DependencyObject)).Metadata.DefaultValue;
                
#if TRACE_FILE
                l.Trace("SharedRD.IsInDesignMode: " + result);

#endif
                return result;
            }
        }

        #endregion
#endif
    }
}
