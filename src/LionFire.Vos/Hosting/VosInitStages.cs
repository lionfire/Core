using LionFire.Vos;

namespace LionFire.Services
{
    public static class VosInitStages
    {
        public static string RootNameSuffix(this string rootName) => string.IsNullOrEmpty(rootName) ? "" : $"-{rootName}";
        public static string NamedRootStage(string stageName, string rootName = null) => stageName + RootNameSuffix(rootName);

        public static string RootStage(string rootName = null) => "vos:" + VosPath.GetRoot(rootName);

        public static string RootMountStage(string rootName ) => NamedRootStage("Mounts", rootName);
    }

}

