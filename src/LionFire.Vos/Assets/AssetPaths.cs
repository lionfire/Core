// REVIEW - MOVE to LionFire.Assets?

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LionFire.Instantiating;
using LionFire.ObjectBus;
using LionFire.Referencing;
using LionFire.Vos;
using Microsoft.Extensions.Logging;

namespace LionFire.Assets
{

    /// <summary>
    /// Nomenclature with examples:
    ///  - Path: The full VOS Path: /$/App/Assets/Loadout/MyExperiments/ABC
    ///  - AssetPath: The subpath for the asset, located within the app's asset location: Loadout/MyExperiments/ABC
    ///  - AssetTypePath: The subpath for the type: MyExperiments/ABC
    ///  - AssetName: The "filename" of the asset.  Last chunk in the path: ABC
    /// </summary>
    public static class AssetPaths
    {
        private static Dictionary<Type, string> assetPaths = new Dictionary<Type, string>();
        private static readonly object assetPathsLock = new object();

        #region Path

        public static string AssetPathFromAssetTypePath<AssetType>(this string subpath) => LionPath.Combine(AssetPaths.GetAssetTypeFolder(typeof(AssetType)), subpath);
        public static string AssetTypePathFromAssetPath<AssetType>(this string path)
        {
            var dir = AssetPaths.GetAssetTypeFolder(typeof(AssetType));
            if (path.StartsWith(dir))
            {
                return path.Substring(dir.Length);
            }
            throw new ArgumentException("Specified path '" + path + "' for type <" + typeof(AssetType).Name + "> does not match expected asset directory: " + dir);
        }

        [AotReplacement]
        public static string AssetPathFromAssetTypePath(string assetTypePath, Type type) => LionPath.Combine(AssetPaths.GetAssetTypeFolder(type), assetTypePath);



        //        private static string GetAssetTypeSubpath(Type type)
        //        //where TModuleType : IModuleType
        //        {
        //#if !OLD_M
        //            return AssetPaths.GetDefaultDirectory(type);
        //#else // OLD
        //            string typeName = type.Name;
        //            if (typeName.StartsWith("T") && typeName.Length > 1 && Char.IsUpper(typeName[1]))
        //            {
        //                typeName = typeName.Substring(1);
        //            }
        //            else if (typeName.EndsWith("Type"))
        //            {
        //                typeName = typeName.Substring(0, typeName.LastIndexOf("Type"));
        //            }
        //            return typeName;
        //            //return typeName + "s";
        //#endif
        //        }

        public static string AssetPathFromAssetTypePath(this IAsset asset, string assetTypePath) => LionPath.Combine(AssetPaths.GetAssetTypeFolder(asset), assetTypePath);
        public static string GetAssetPath(Type type, string assetTypePath) // REVIEW - deduplicate?
=> LionPath.Combine(AssetPaths.GetAssetTypeFolder(type), assetTypePath);

        #endregion

        #region Directory

        public static string GetAssetTypeFolder(IAsset asset)
        {
            if (asset == null)
            {
                throw new ArgumentNullException();
            }

            Type type = asset.Type;

            return GetAssetTypeFolder(type);
        }


        /// <summary>
        /// Get reasonable paths for holding instances of a particular type.
        /// Format the name nicely: strip I for interfaces, 
        /// Can be set manually if you don't like the automated naming scheme.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetAssetTypeFolder(Type type, bool alwaysAppendLastBase = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException();
            }

            lock (assetPathsLock)
            {
                if (assetPaths.TryGetValue(type, out string defaultPath))
                {
                    return defaultPath;
                }

                var types = GetBaseTypesInOrder(type).Concat(GetInterfaceTypesInOrder(type)).ToList();

                var usedTypes = new HashSet<Type>();
                var sb = new StringBuilder();

                bool gotConcreteType = false;
                string lastConcreteType = null;
                for (int index = 0; index < types.Count; index++)
                {
                    var typeItem = types[index];

                    if (usedTypes.Contains(types[index]))
                    {
                        continue;
                    }

                    usedTypes.Add(types[index]);

                    AssetAttribute attr = typeItem.GetCustomAttribute<AssetAttribute>();

                    if (attr != null)
                    {
                        if (attr.DefaultPath != null)
                        {
                            sb.Append(attr.DefaultPath);
                            sb.Append(LionPath.SeparatorChar);
                            if (!attr.IsAbstract) { gotConcreteType = true; }
                            continue;
                        }
                        if (attr.IsAbstract)
                        {
                            continue;
                        }
                    }

                    string typeName = typeItem.Name;

                    bool appendToPath = true;

                    if (typeItem.IsInterface)
                    {
                        if (attr == null)
                        {
                            appendToPath = false;
                        }

                        if (typeName.Length > 2 && typeof(ITemplateAsset).IsAssignableFrom(typeItem))
                        {
                            if (typeName[0] == 'I' && char.IsUpper(typeName[1]))
                            {
                                typeName = typeName.Substring(1);
                            }
                        }
                    }

                    if (!gotConcreteType /*&& alwaysAppendLastBase*/ && !typeItem.IsAbstract && appendToPath)
                    {

                        #region Strip prefix naming convention from ITemplates

                        if (typeName.Length > 1 && (typeof(ITemplateAsset).IsAssignableFrom(typeItem))
                            )
                        {
                            char[] prefixChars = new char[] { 'T' };

                            if (prefixChars.Contains(typeName[0]) && char.IsUpper(typeName[1]))
                            {
                                typeName = typeName.Substring(1);
                            }
                        }

                        #endregion

                        #region Strip prefix naming convention from ITemplateParameters

#if ITemplateParameters
                        if (typeName.Length > 1 &&  typeof(ITemplateParameters).IsAssignableFrom(typeItem)))
                        {
                            char[] prefixChars = new char[] { 'P' };

                            if (prefixChars.Contains(typeName[0]) && char.IsUpper(typeName[1]))
                            {
                                typeName = typeName.Substring(1);
                            }
                        }
#endif
                        #endregion

                        lastConcreteType = typeName;

                        sb.Append(typeName);
                        sb.Append(LionPath.SeparatorChar);
                        //defaultPath = type.Name + "s/";
                    }
                }
                if (alwaysAppendLastBase && lastConcreteType != null)
                {
                    sb.Append(lastConcreteType);
                    sb.Append(LionPath.SeparatorChar);
                }

                defaultPath = sb.ToString();
                defaultPath = defaultPath.Replace("//", "/");
                while (defaultPath.EndsWith("//"))
                {
                    defaultPath = defaultPath.Remove(defaultPath.Length - 2);
                }
                assetPaths.Add(type, defaultPath);
                if (String.IsNullOrWhiteSpace(defaultPath) || defaultPath.Contains("`") || type.Name.Contains("OrbatItem"))
                {
                    l.Warn($"assetPaths.Add({type}, {defaultPath}) - Empty path, (or OrbatItem)");
                }
#if SanityChecks
                if (defaultPath.Contains(LionPath.SeparatorChar.ToString() + LionPath.SeparatorChar))
                {
                    l.Warn("AssetPath.GetDefaultDirectory - Incorrect path detected: " + defaultPath);
                }
#endif
                //l.Trace(type + " -> " + defaultPath);

                //l.Fatal("DEFAULT PATH for " + type + " is " + defaultPath.ToString());
                return defaultPath;
            }
        }

        #region Misc

        private static ILogger l = Log.Get();

        #endregion

        private static List<Type> GetBaseTypesInOrder(Type type)
        {
            List<Type> baseTypes = new List<Type>();

            while (type != null && type != typeof(object))
            {
                baseTypes.Add(type);
                type = type.BaseType;
            }

            baseTypes.Reverse();
            return baseTypes;
        }
        private static List<Type> GetInterfaceTypesInOrder(Type type)
        {
            List<Type> baseTypes = GetBaseTypesInOrder(type);
            List<Type> interfaceTypes = new List<Type>();

            foreach (var baseType in baseTypes)
            {
                interfaceTypes.AddRange(baseType.GetInterfaces());
            }
            return interfaceTypes;
        }

        #endregion

        // semi-Duplicate, From AssetReferenceResolver
        //        public static string AssetTypePathToAssetPath<AssetType>(this string name, Type concreteType = null)
        //            where AssetType : class
        //        {
        //            var assetTypePath = AssetPaths.GetAssetTypeFolder(concreteType ?? typeof(AssetType));

        //            if (String.IsNullOrWhiteSpace(assetTypePath) && !name.Contains("/"))
        //            {
        //                l.Warn($"AssetNameToDefaultSubpath: String.IsNullOrWhiteSpace(defaultAssetPath) && !name.Contains(\" / \") for AssetNameToDefaultSubpath<{typeof(AssetType).Name}>({name},{concreteType})");
        //            }

        //            string path = LionPath.Combine(assetTypePath, name);
        //#if SanityChecks
        //            if (name.StartsWith(assetTypePath))
        //            {
        //                l.Warn($"AssetNameToDefaultSubpath: name{{{name}}}.StartsWith(defaultAssetPath{{{assetTypePath}}})");
        //            }
        //#endif
        //            return path;
        //        }

    }
}

