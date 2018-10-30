namespace LionFire.Referencing
{
    public static class UriExtensions
    {
        public static string GetUriScheme(this string str)
        {
            if (str == null)
            {
                return null;
            }

            var indexOf = str.IndexOf(':');
            if (indexOf == -1)
            {
                return null;
            }

            return str.Substring(0, indexOf).Trim();
        }
    }
}
