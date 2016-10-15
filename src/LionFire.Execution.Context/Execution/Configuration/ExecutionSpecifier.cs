using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Execution.Configuration
{
    public static class ExecutionSpecifier
    {
        // TODO: Use a better parsing library such as Sprache

        #region Parsing

        public static string TokenPrefixes = "!@";
        public static string ToSpace = "?&";

        public static string DictMiddle = "=";
        public static string UriMiddle = ":";

        private const char TypeDelimiter = '[';
        private const char ParametersDelimiter = '(';
        private const char WordDelimiter = ' ';

        private static IEnumerable<char> TokenStarters {
            get {
                yield return TypeDelimiter;
                yield return ParametersDelimiter;
                yield return WordDelimiter;
            }
        }

        #endregion

        public static void ParseSpecifier(this ExecutionConfig ec, string val)
        {
            foreach (var token in Tokenize(val, TokenStarters))
            {
                if (token.StartsWith("!"))
                {
                    ec.Runtime = token.Substring(1);
                    continue;
                }

                if (token.StartsWith("@"))
                {
                    ec.ExecutionLocation = token.Substring(1);
                    continue;
                }

                if (token.Contains("="))
                {
                    var token2 = token;
                    ec.Parameters.Add(SplitOne(ref token2, '='), token2);
                    continue;
                }

                if (token.Contains("^"))
                {
                    ec.ConfigName = token.TrimStart('^');
                    continue;
                }

                if (token.Contains("["))
                {
                    var token2 = token;
                    ec.TypeName = SplitOne(ref token2, '[', returnFirst: false).TrimEnd(']');
                    continue;
                }

                if (token.Contains("("))
                {
                    var args = token.TrimStart('(').TrimEnd(')').Split(','); // TODO: Escape doublequote
                    ec.Arguments = args;
                    continue;
                }

                if (token.Contains(":"))
                {
                    var token2 = token;
                    ec.SourceUriScheme = SplitOne(ref token2, ':');
                    ec.SourceUriBody = token2;
                    continue;
                }
                else
                {
                    // Default: AssemblyName (Assembly may need to be retrieved by a package of the same name)
                    ec.SourceUriBody = token;
                }
            }
        }

        private static List<string> Tokenize(string val, IEnumerable<char> delimiterChars)
        {
            List<string> tokens = new List<string>();

            StringBuilder sb = new StringBuilder();

            bool isInMultiWordToken = false;

            var delimiters = new string(delimiterChars.ToArray());


            for (int i = 0; i < val.Length; i++)
            {
                char cur = val[i];
                if (ToSpace.Contains(cur))
                {
                    cur = ' ';
                }

                bool isEscaped = i > 0 && val[i - 1] == '\\' && (i < 2 || val[i - 2] != '\\');
                if (!isEscaped)
                {
                    if (sb.Length == 0 && cur == '(')
                    {
                        isInMultiWordToken = true;
                        sb.Append(cur);
                        continue;
                    }
                    if (isInMultiWordToken && cur == ')')
                    {
                        isInMultiWordToken = false;
                        tokens.Add(sb.ToString());
                        sb = new StringBuilder();
                    }

                    if (!isInMultiWordToken && delimiters.Contains(cur))
                    {
                        if (sb.Length > 0)
                        {
                            tokens.Add(sb.ToString());
                            sb = new StringBuilder();
                        }
                    }

                    if (cur != ' ')
                    {
                        sb.Append(cur);
                    }
                }
            }
            if (sb.Length > 0)
            {
                tokens.Add(sb.ToString());
            }
            return tokens;
        }

        private static string SplitOne(ref string str, char separator, bool returnFirst = true)
        {
            if (!str.Contains(separator.ToString())) return null;

            var split = str.Split(new char[] { separator }, 2);

            str = returnFirst ? split[1] : split[0];
            return returnFirst ? split[0] : split[1];
        }
    }
}
