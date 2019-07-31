// REVIEW Consider moving to LionFire.Referencing.dll
using System;

namespace LionFire.Referencing
{
    public static class LionUri
    {
        #region Scheme

        public static string TryGetUriScheme(string uri, bool validateScheme = true)
        {
            var split = uri.Split(new char[] { ':' }, 2, StringSplitOptions.None);
            if (split.Length < 2)
            {
                return null;
            }
            if (validateScheme && !IsValidScheme(split[0])) return null;
            return split[0];
        }
        public static string GetUriScheme(string uri, bool validateScheme = true)
        {
            var split = uri.Split(new char[] { ':' }, 2, StringSplitOptions.None);
            if (split.Length < 2)
            {
                throw new ArgumentException($"No URI scheme present (no ':' in {nameof(uri)})");
            }
            if (validateScheme) ValidateScheme(split[0]);
            return split[0];
        }

        public static void ValidateScheme(string scheme)
        {
            if (!IsValidScheme(scheme)) {
                throw new ArgumentException($@"Scheme ""{scheme}"" is not in a valid format.  (Format: ALPHA *( ALPHA / DIGIT / "" + "" / "" - "" / ""."" ))");
            }
        }

        public static bool IsValidScheme(string scheme)
        {
            // ALPHA *( ALPHA / DIGIT / "+" / "-" / "." )
            // https://tools.ietf.org/html/rfc3986#page-17

            if (scheme.Length == 0) return false;
            if (!char.IsLetter(scheme[0])) return false;
            for(var i = 1; i < scheme.Length; i++)
            {
                if (!char.IsLetterOrDigit(scheme[i]) &&
                    scheme[i] != '+' &&
                    scheme[i] != '-' &&
                    scheme[i] != '.')
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

    }
}
