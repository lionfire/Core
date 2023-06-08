namespace LionFire.UI.Components;

public interface IObjectEditorVM
{
    object Object { get; }

    bool ReadOnly { get; }

    RelevanceFlags ReadRelevance { get; set; }
    RelevanceFlags WriteRelevance { get; set; }
}
