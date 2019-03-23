using LionFire.Referencing;

namespace LionFire.ObjectBus.Sql
{
    public class SqlPath
    {
        public static char SeparatorChar = ':';
        public static string Separator = ":";

        public static string PathToSqlPath(string path) => path.TrimStart(LionPath.SeparatorChar).Replace(LionPath.SeparatorChar, SqlPath.SeparatorChar);
    }
}
