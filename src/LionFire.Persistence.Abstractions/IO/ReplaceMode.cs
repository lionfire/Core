using System;
using FileMode = System.IO.FileMode;

namespace LionFire.IO
{
    public enum ReplaceMode
    {
        Unspecified = 0,
        Create = 1 << 0,
        Update = 1 << 1,
        Upsert = Create | Update,
    }

    public static class ReplaceModeExtensions
    {
        public static bool RequireOverwrite(this ReplaceMode replaceMode) => replaceMode == ReplaceMode.Update;
        public static bool AllowOverwrite(this ReplaceMode replaceMode) => replaceMode.HasFlag(ReplaceMode.Update);

        public static FileMode ToFileMode(this ReplaceMode replaceMode)
        {
            switch (replaceMode)
            {
                case ReplaceMode.Create:
                    return FileMode.Create;
                case ReplaceMode.Update:
                    return FileMode.Open;
                case ReplaceMode.Upsert:
                    return FileMode.OpenOrCreate;
                case ReplaceMode.Unspecified:
                default:
                    throw new ArgumentException(nameof(replaceMode));
            }
        }

        public static string DescriptionString(this ReplaceMode replaceMode)
        {
            switch (replaceMode)
            {
                case ReplaceMode.Create:
                case ReplaceMode.Update:
                    return replaceMode.ToString();
                case ReplaceMode.Upsert:
                    return "Upsert";
                default:
                    return "(Unknown ReplaceMode)";
            }
        }

        public static string ToArrow(this ReplaceMode replaceMode)
            => replaceMode switch
            {
                ReplaceMode.Create => "+>",
                ReplaceMode.Update => "=>",
                ReplaceMode.Upsert => "+=>",
                _ => "(Unknown ReplaceMode)",
            };
    }
    
}
