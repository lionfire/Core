using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Referencing;

namespace LionFire.ObjectBus.Resolution
{
    public class ReferenceUriParsing
    {
        public static string PathOnlyFromUri(string uri, IEnumerable<string> allowedSchemes) // RENAME PathUriFromInput
        {
            var scheme = LionUri.GetUriScheme(uri, false);

            #region Verify scheme is supported

            if (!allowedSchemes.Contains(scheme))
            {
                return null;
                //throw new ArgumentException("Unsupported scheme");
            }

            #endregion

            #region Eat up 3 slashes after file: to find the start of the path.

            #endregion
            int slashIndex = scheme.Length;
            for (int i = 3; i > 0; i--)
            {
                slashIndex = uri.IndexOf('/', slashIndex + 1);
                if (slashIndex < 0)
                {
                    // FUTURE: Relative paths?
                    return null;
                    //throw new ArgumentException("Must be in the format scheme:///path");
                }
            }

            string path = uri.Substring(slashIndex + 1);
            return path;
        }
    }
}

