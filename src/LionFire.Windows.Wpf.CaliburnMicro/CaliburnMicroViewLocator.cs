using Caliburn.Micro;
using System;
using System.Windows;

namespace LionFire.UI.Wpf.Controls.CaliburnMicro
{
    public class CaliburnMicroViewLocator : IViewLocator
    {

        public void AddDefaultTypeMapping(string viewSuffix = "View") 
            => ViewLocator.AddDefaultTypeMapping(viewSuffix);

        public void AddNamespaceMapping(string nsSource, string[] nsTargets, string viewSuffix = "View") 
            => ViewLocator.AddNamespaceMapping(nsSource, nsTargets, viewSuffix);
        
        public void AddNamespaceMapping(string nsSource, string nsTarget, string viewSuffix = "View") 
            => ViewLocator.AddNamespaceMapping(nsSource, nsTarget, viewSuffix);
        
        public void AddSubNamespaceMapping(string nsSource, string nsTarget, string viewSuffix = "View") 
            => ViewLocator.AddSubNamespaceMapping(nsSource, nsTarget, viewSuffix);
        
        public void AddSubNamespaceMapping(string nsSource, string[] nsTargets, string viewSuffix = "View") 
            => ViewLocator.AddSubNamespaceMapping(nsSource, nsTargets, viewSuffix);
        
        public void AddTypeMapping(string nsSourceReplaceRegEx, string nsSourceFilterRegEx, string[] nsTargetsRegEx, string viewSuffix = "View") 
            => ViewLocator.AddTypeMapping(nsSourceReplaceRegEx, nsSourceFilterRegEx, nsTargetsRegEx, viewSuffix);
        
        public void AddTypeMapping(string nsSourceReplaceRegEx, string nsSourceFilterRegEx, string nsTargetRegEx, string viewSuffix = "View") 
            => ViewLocator.AddTypeMapping(nsSourceReplaceRegEx, nsSourceFilterRegEx, nsTargetRegEx, viewSuffix);
        
        public void ConfigureTypeMappings(TypeMappingConfiguration config) 
            => ViewLocator.ConfigureTypeMappings(config);
        
        public void InitializeComponent(object element) 
            => ViewLocator.InitializeComponent(element);
        
        public UIElement LocateForModel(object model, DependencyObject dependencyObject, object context) 
            => ViewLocator.LocateForModel(model, dependencyObject, context);
        
        public UIElement LocateForModelType(Type type, DependencyObject dependencyObject, object context) 
            => ViewLocator.LocateForModelType(type, dependencyObject, context);
        
        public Type LocateTypeForModelType(Type type, DependencyObject dependencyObject, object context) 
            => ViewLocator.LocateTypeForModelType(type, dependencyObject, context);
        
        public void RegisterViewSuffix(string viewSuffix) => ViewLocator.RegisterViewSuffix(viewSuffix);
    }
}
