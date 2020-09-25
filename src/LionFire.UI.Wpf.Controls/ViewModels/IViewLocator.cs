using Caliburn.Micro;
#if NOESIS
using Noesis;
#else
using System.Windows;
#endif
using System;

namespace LionFire.UI.Wpf
{
    public interface IViewLocator
    {
#if false
        // Summary:
        //     Used to transform names.
        NameTransformer NameTransformer;
        // Summary:
        //     Separator used when resolving View names for context instances.
        string ContextSeparator;
        // Summary:
        //     Retrieves the view from the IoC container or tries to create it if not found.
        //
        // Remarks:
        //     Pass the type of view as a parameter and recieve an instance of the view.
        Func<Type, UIElement> GetOrCreateViewType;
        // Summary:
        //     Modifies the name of the type to be used at design time.
        Func<string, string> ModifyModelTypeAtDesignTime;
        // Summary:
        //     Transforms a ViewModel type name into all of its possible View type names. Optionally
        //     accepts an instance of context object
        //
        // Returns:
        //     Enumeration of transformed names
        //
        // Remarks:
        //     Arguments: typeName = The name of the ViewModel type being resolved to its companion
        //     View. context = An instance of the context or null.
        Func<string, object, IEnumerable<string>> TransformName;
#endif

        // Summary:
        //     Locates the view type based on the specified model type.
        //
        // Returns:
        //     The view.
        //
        // Remarks:
        //     Pass the model type, display location (or null) and the context instance (or
        //     null) as parameters and receive a view type.
        Type LocateTypeForModelType(Type type, DependencyObject dependencyObject, object context);

        // Summary:
        //     Locates the view for the specified model type.
        //
        // Returns:
        //     The view.
        //
        // Remarks:
        //     Pass the model type, display location (or null) and the context instance (or
        //     null) as parameters and receive a view instance.
         UIElement LocateForModelType(Type type, DependencyObject dependencyObject, object context);

        // Summary:
        //     Locates the view for the specified model instance.
        //
        // Returns:
        //     The view.
        //
        // Remarks:
        //     Pass the model instance, display location (or null) and the context (or null)
        //     as parameters and receive a view instance.
         UIElement LocateForModel(object model, DependencyObject dependencyObject, object context);

#if false
        //// Summary:
        ////     Transforms a view type into a pack uri.
        //string DeterminePackUriFromType(Type a, Type b);

        //// Summary:
        ////     Adds a default type mapping using the standard namespace mapping convention
        ////
        //// Parameters:
        ////   viewSuffix:
        ////     Suffix for type name. Should be "View" or synonym of "View". (Optional)
        //void AddDefaultTypeMapping(string viewSuffix = "View");

        // Summary:
        //     Adds a standard type mapping based on simple namespace mapping
        //
        // Parameters:
        //   nsSource:
        //     Namespace of source type
        //
        //   nsTargets:
        //     Namespaces of target type as an array
        //
        //   viewSuffix:
        //     Suffix for type name. Should be "View" or synonym of "View". (Optional)
        void AddNamespaceMapping(string nsSource, string[] nsTargets, string viewSuffix = "View");

        // Summary:
        //     Adds a standard type mapping based on simple namespace mapping
        //
        // Parameters:
        //   nsSource:
        //     Namespace of source type
        //
        //   nsTarget:
        //     Namespace of target type
        //
        //   viewSuffix:
        //     Suffix for type name. Should be "View" or synonym of "View". (Optional)
        void AddNamespaceMapping(string nsSource, string nsTarget, string viewSuffix = "View");

        // Summary:
        //     Adds a standard type mapping by substituting one subnamespace for another
        //
        // Parameters:
        //   nsSource:
        //     Subnamespace of source type
        //
        //   nsTarget:
        //     Subnamespace of target type
        //
        //   viewSuffix:
        //     Suffix for type name. Should be "View" or synonym of "View". (Optional)
        void AddSubNamespaceMapping(string nsSource, string nsTarget, string viewSuffix = "View");

        // Summary:
        //     Adds a standard type mapping by substituting one subnamespace for another
        //
        // Parameters:
        //   nsSource:
        //     Subnamespace of source type
        //
        //   nsTargets:
        //     Subnamespaces of target type as an array
        //
        //   viewSuffix:
        //     Suffix for type name. Should be "View" or synonym of "View". (Optional)
        void AddSubNamespaceMapping(string nsSource, string[] nsTargets, string viewSuffix = "View");

        // Summary:
        //     Adds a standard type mapping based on namespace RegEx replace and filter patterns
        //
        // Parameters:
        //   nsSourceReplaceRegEx:
        //     RegEx replace pattern for source namespace
        //
        //   nsSourceFilterRegEx:
        //     RegEx filter pattern for source namespace
        //
        //   nsTargetsRegEx:
        //     Array of RegEx replace values for target namespaces
        //
        //   viewSuffix:
        //     Suffix for type name. Should be "View" or synonym of "View". (Optional)
        void AddTypeMapping(string nsSourceReplaceRegEx, string nsSourceFilterRegEx, string[] nsTargetsRegEx, string viewSuffix = "View");

        // Summary:
        //     Adds a standard type mapping based on namespace RegEx replace and filter patterns
        //
        // Parameters:
        //   nsSourceReplaceRegEx:
        //     RegEx replace pattern for source namespace
        //
        //   nsSourceFilterRegEx:
        //     RegEx filter pattern for source namespace
        //
        //   nsTargetRegEx:
        //     RegEx replace value for target namespace
        //
        //   viewSuffix:
        //     Suffix for type name. Should be "View" or synonym of "View". (Optional)
        void AddTypeMapping(string nsSourceReplaceRegEx, string nsSourceFilterRegEx, string nsTargetRegEx, string viewSuffix = "View");

        // Summary:
        //     Specifies how type mappings are created, including default type mappings. Calling
        //     this method will clear all existing name transformation rules and create new
        //     default type mappings according to the configuration.
        //
        // Parameters:
        //   config:
        //     An instance of TypeMappingConfiguration that provides the settings for configuration
        void ConfigureTypeMappings(TypeMappingConfiguration config);

        // Summary:
        //     When a view does not contain a code-behind file, we need to automatically call
        //     InitializeCompoent.
        //
        // Parameters:
        //   element:
        //     The element to initialize
        void InitializeComponent(object element);

        // Summary:
        //     This method registers a View suffix or synonym so that View Context resolution
        //     works properly. It is automatically called internally when calling AddNamespaceMapping(),
        //     AddDefaultTypeMapping(), or AddTypeMapping(). It should not need to be called
        //     explicitly unless a rule that handles synonyms is added directly through the
        //     NameTransformer.
        //
        // Parameters:
        //   viewSuffix:
        //     Suffix for type name. Should be "View" or synonym of "View".
        void RegisterViewSuffix(string viewSuffix);
#endif
    }
}
