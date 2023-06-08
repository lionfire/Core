namespace LionFire.Blazor.Components.MudBlazor_;

public interface IViewTypeProvider
{
    bool HasView(Type modelType);
    Type? GetViewType(Type modelType);
}
