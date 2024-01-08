using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace LionFire.Applications;

public static class ApplicationAutoDetection
{
    public static Func<string?, string, string> AutoDetectAppName = (appId, orgNamespace) =>
    {
        appId ??= Assembly.GetEntryAssembly()?.GetName().Name ?? "(null)";

        #region Trim leading OrgId

        var prefix = orgNamespace + ".";
        if (appId.StartsWith(prefix) && appId.Length > prefix.Length) { appId = appId.Substring(prefix.Length); }

        #endregion

        return appId;
    };

    public static Func<string, string> AutoDetectOrgNamespace = (orgName) =>
    {
        // TODO: Get AppInfo from .NET Assembly attributes
        //Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyName>

        if (orgName == null)
        {
            orgName = Assembly.GetEntryAssembly().FullName;
        }
        var chunks = orgName.Split(new char[] { '.' }, 2, StringSplitOptions.RemoveEmptyEntries);
        if (chunks.Length > 1)
        {
            orgName = chunks[0];
        }
        return orgName;
    };
    public static string FindCustomAppRoot(string appId)
    {
        if (appId == null) throw new ArgumentNullException(nameof(appId));

        var assembly = Assembly.GetEntryAssembly();
        if (assembly?.Location == null) return null;

        var dir = Path.GetDirectoryName(assembly.Location);

        bool foundIt = false;
        for (; dir != null; dir = Directory.GetParent(dir)?.FullName)
        {
            var path = Path.Combine(dir, "application.json");
            if (File.Exists(path))
            {
                try
                {
                    var appInfo = System.Text.Json.JsonSerializer.Deserialize<AppMarker>(File.ReadAllText(path));
                    if (appInfo != null)
                    {
                        if (appInfo.AppId != null && appInfo.AppId != appId) continue;
                        foundIt = true;
                        //if (appInfo.AppId == null || appInfo.AppId == appId) { return dir; }
                        if (appInfo.Dir != null)
                        {
                            dir = Path.Combine(dir, appInfo.Dir);
                        }
                    }
                }
                catch
                {
                    // TOLOG - unexpected / corrupt application.json file
                    // EMPTYCATCH
                }
            }
            if (foundIt) break;
        }

        if (dir != null && Path.GetFileName(dir).ToLowerInvariant().Contains(appId.ToLowerInvariant()))
        {
            foundIt = true;
        }

        if (foundIt)
        {
            return dir;
        }
        return null;
    }
}
