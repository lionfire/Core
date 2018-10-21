// FUTURE?
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace LionFire.Linq
//{
//    public static class CaseInsensitiveLinqExtensions
//    {
// From https://dotnetthoughts.net/how-to-make-string-contains-case-insensitive/
//        public static bool CaseInsensitiveContains(this string text, string value,
//            StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
//        {
//            return text.IndexOf(value, stringComparison) >= 0;
//        }

//        public static bool CaseInsensitiveContains(this IEnumerable<string> strings, string value,
//            StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
//        {
//            return strings.Where(str=> str.IndexOf(value, stringComparison) >= 0).Any();
//        }
//    }

