﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.Instantiating;

/// <summary>
/// REVIEW: REFACTOR - If this is just a marker interface, drop it as a constraint on generic Template classes
/// </summary>
public interface ITemplateInstance
{
    // Overkill?  If you want to get the template, search for ITemplateInstance<>'s -- there should only be one, or at least one that has a null value in Template.
    //ITemplate Template { get; set; }
}

public interface IParameterizedTemplateInstance
{
    ITemplateParameters Parameters { get; set; }
}
//public interface IParameterizedTemplateInstance<out TParameters>
//{
//    ITemplateParameters Parameters { get; set; }
//}
//public interface IParameterizedTemplateInstance<out TParameters, out TTemplate>
//{
//    ITemplateParameters<TTemplate> Parameters { get; set; }
//}

public interface ITemplateInstance<TTemplate> : ITemplateInstance, IInstanceForSettable<TTemplate>
    where TTemplate : ITemplate
{
     new TTemplate Template { get; set; }
}

public static class ITemplateInstanceExtensions
{
    private static PropertyInfo GetTemplateProperty(ITemplateInstance obj)
    {
        foreach (var pi in obj.GetType().GetProperties().Where(p => p.Name == "Template"))
        {
            if (pi.PropertyType == typeof(ITemplate) || typeof(ITemplate).IsAssignableFrom(pi.PropertyType)) return pi;
        }
        throw new Exception("Template property that returns ITemplate or derived type not found on object of type " + obj.GetType().FullName);
    }
    public static ITemplate GetTemplate(this ITemplateInstance templateInstance)
    {
        if (templateInstance == null) return null;
        return (ITemplate)GetTemplateProperty(templateInstance).GetValue(templateInstance);
    }
    public static void SetTemplate(this ITemplateInstance templateInstance, ITemplate template)
    {
        GetTemplateProperty(templateInstance).SetValue(templateInstance, template);
    }
}
