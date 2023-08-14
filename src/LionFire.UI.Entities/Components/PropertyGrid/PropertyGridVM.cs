﻿using LionFire.Mvvm;
using ReactiveUI;
using LionFire.UI.Components.PropertyGrid;
using LionFire.Inspection;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.UI.Components;

public class PropertyGridVM : ReactiveObject, IObjectEditorVM
{
    #region Dependencies

    public ObjectInspectorService ObjectInspectorService { get; }

    #endregion

    [Reactive]
    public object? SourceObject { get; set; }

    [Reactive]
    public InspectedObjectVM? InspectedObject { get; set; }

    public bool ReadOnly { get; set; }

    public int MaxDepth { get; set; } = 4; // TEMP TODO: change to something like 15, maybe

    //public EditApproach EditApproach { get; set; } = EditApproach.InPlace;

    public RelevanceFlags ReadRelevance { get; set; } = RelevanceFlags.DefaultForUser;
    public RelevanceFlags WriteRelevance { get; set; } = RelevanceFlags.DefaultForUser;

    #region Derived

    public TypeInteractionModel? TypeModel => TypeInteractionModels.Get(SourceObject?.GetType());

    #endregion

    #region Lifecycle

    public PropertyGridVM(IViewModelProvider viewModelProvider, IServiceProvider serviceProvider, ObjectInspectorService objectInspectorService)
    {
        ViewModelProvider = viewModelProvider;
        ServiceProvider = serviceProvider;
        ObjectInspectorService = objectInspectorService;

        this.WhenAnyValue<PropertyGridVM, object>(t => t.SourceObject)
            .Subscribe((Action<object>)(o =>
            {
                if (o is InspectedObjectVM x) InspectedObject = x;
                else
                {
                    InspectedObject = o == null ? null : ActivatorUtilities.CreateInstance<InspectedObjectVM>(ServiceProvider, o);
                }

                Title = o?.GetType().Name.ToDisplayString() ?? "???";
            }));
    }

    #endregion

    #region Title

    public string Title { get; set; } = "Properties";
    public bool ShowTitle { get; set; } = true;

    #endregion

    #region Item Filters

    [Reactive]
    public bool ShowFilterTypes { get; set; }

    [Reactive]
    public PropertyGridItemType VisibleItemTypes { get; set; } = PropertyGridItemType.Data;

    public bool ShowDataMembers
    {
        get => VisibleItemTypes.HasFlag(PropertyGridItemType.Data);
        set
        {
            if (value) VisibleItemTypes |= PropertyGridItemType.Data; else VisibleItemTypes &= ~PropertyGridItemType.Data;
        }
    }
    public bool ShowEvents
    {
        get => VisibleItemTypes.HasFlag(PropertyGridItemType.Events);
        set
        {
            if (value) VisibleItemTypes |= PropertyGridItemType.Events; else VisibleItemTypes &= ~PropertyGridItemType.Events;
        }
    }
    public bool ShowMethods
    {
        get => VisibleItemTypes.HasFlag(PropertyGridItemType.Methods);
        set
        {
            if (value) VisibleItemTypes |= PropertyGridItemType.Methods; else VisibleItemTypes &= ~PropertyGridItemType.Methods;
        }
    }

    #endregion

    #region Items

    

    public IViewModelProvider ViewModelProvider { get; }
    public IServiceProvider ServiceProvider { get; }

    public bool CanRead(ReflectionMemberVM member)
        => member.Info.ReadRelevance.HasFlag(ReadRelevance);

    public bool CanWrite(ReflectionMemberVM member)
        => member.Info.WriteRelevance.HasFlag(WriteRelevance);

    #endregion
}
