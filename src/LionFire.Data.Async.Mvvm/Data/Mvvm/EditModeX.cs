namespace LionFire.Data.Mvvm;

public static class EditModeX
{
    public static bool CanCreate(this EditMode editMode) => editMode.HasFlag(EditMode.Create);
    public static bool CanRename(this EditMode editMode) => editMode.HasFlag(EditMode.Rename);
    public static bool CanDelete(this EditMode editMode) => editMode.HasFlag(EditMode.Delete);
    public static bool CanUpdate(this EditMode editMode) => editMode.HasFlag(EditMode.Update);

}