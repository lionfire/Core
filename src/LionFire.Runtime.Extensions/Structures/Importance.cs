using LionFire.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LionFire
{
    public enum Importance
    {
        Unspecified,

        [Code("A")]
        Supercritical,

        [Code("B")]
        Critical,
        [Code("C")]
        Error,
        [Code("D")]
        Warning,
        [Code("E")]
        Message,
        [Code("F")]
        Caution,
        [Code("G")]
        Info,
        [Code("H")]
        Debug,
        [Code("I")]
        Trace,
    }

    public static class ImportanceExtensions
    {
        public static string ToCode(this Importance imp)
        {
            var attr = typeof(Importance).GetField(imp.ToString()).GetCustomAttribute<CodeAttribute>();
            return attr?.Code;
        }
    }
}
