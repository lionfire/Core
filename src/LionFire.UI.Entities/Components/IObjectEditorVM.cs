namespace LionFire.UI.Components;

public interface IObjectEditorVM
{
    object SourceObject { get; }

    bool ReadOnly { get; }

    RelevanceFlags ReadRelevance { get; set; }
    RelevanceFlags WriteRelevance { get; set; }
}
