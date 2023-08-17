﻿using LionFire.Structures;

public class InspectorGroupInfo : IKeyable<string>
{

    #region Relationships

    public string? Parent { get; set; }

    #region Derived

    /// <summary>
    /// How many parents this group has.  0 means it is a root group.  The higher the number, the more likely it is that the group is a more sophisticated and tailored way to inspect the object.
    /// </summary>
    public int Depth
    {
        get
        {
            int depth = 0;
            var p = Parent;
            while (p != null)
            {
                depth++;
                p = Parent;
            }
            return depth;
        }
    }

    #endregion

    #endregion

    #region Identity

    public string Key { get; set; }

    #endregion

    #region Lifecycle

    public InspectorGroupInfo(string key)
    {
        Key = key;
    }

    #endregion

    #region Properties

    public string Key { get; set; }
    public string DisplayName { get; set; }

    public float Order { get; set; }

    #endregion
}