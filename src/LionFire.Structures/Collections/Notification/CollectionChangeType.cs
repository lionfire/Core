namespace LionFire.Collections;

// TODO REVIEW: Migrate to Dynamic-Data's classes for this?  Or CySharp.ObservableCollection?
public enum CollectionChangeType
{
    Unspecified = 0,
    Added = 1,
    Removed = 2,
    Changed = 3,
}