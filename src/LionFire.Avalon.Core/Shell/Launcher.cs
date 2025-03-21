using LionFire.Applications;
using LionFire.Assemblies;
using LionFire.Shell;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

//namespace LionFire.Launcher.Avalon
//{
//    public static class AvalonApplicationOpener
//    {
//        public static bool CanOpen(Type type)
//        {
//            return typeof(FrameworkElement).IsAssignableFrom(type);
//        }

//        public static void Open(Type type)
//        {
//            if(!CanOpen(type))
//            {
//                throw new NotSupportedException("Can't open specified parameter");
//            }
//            LionFireShell.Run(type);
//        }
//    }
//}

namespace LionFire.Launcher
{
    //public interface ILauncher
    //{
    //    void Launch(Type type);
    //}

    public static class Launcher
    {
        public static void Launch(string[] args)
        {
            var a = Assembly.GetEntryAssembly();

            if (a == null)
            {
                MessageBox.Show("Launcher: Failed to get entry Assembly");
                return;
            }

            if (String.IsNullOrWhiteSpace(a.Location))
            {
                MessageBox.Show("Launcher: Failed to get entry Assembly location");
                return;
            }

            var launchName = System.IO.Path.GetFileNameWithoutExtension(a.Location);

            if (launchName == "LionFire.Launcher")
            {
                // For debugging on VS Express
                //launchName = "Workbench";
                //launchName = "VosExplorer"; 
                //launchName = "ExpandoVobEditor";
                launchName = "Vob"; 
                
            }

            if (String.IsNullOrWhiteSpace(launchName))
            {
                MessageBox.Show("Launcher: Failed to get entry Assembly name");
                return;
            }

            bool secondPass = false;
            //bool unresolvedType = false;
            TypeMetaData launcherTypeMetaData = null;
            tryAgain:
            foreach (var typeMetaData in MetaDataManager.FindAll<ILauncher>())
            {
                var metaAttr = typeMetaData.MetaAttributes.TryGetValue("LaunchName");
                if(metaAttr == launchName ||
                     typeMetaData.Name == launchName
                    )
                {
                    launcherTypeMetaData = typeMetaData;
                    break;
                }
            }

            if (launcherTypeMetaData == null && !secondPass)
            {
                secondPass = true;

                MetaDataManager.RefreshExportDerivatives();
                goto tryAgain;  
            }
            
            if (launcherTypeMetaData == null)
            {
                //l.Info("Launcher launching: " + launchName + " using " + typeMetaData.FullName);

                string msg = "Failed to find launcher named \"" + launchName + "\".  Rename the executable to the name of the launcher you want to launch and make sure it is in the same directory, or some directory that is set in LionFire.Environment.MetaDataPath.";
                l.Info(msg);
                MessageBox.Show(msg);
            }
            else
            {
                //var assembly = Assembly.LoadFrom(launcherTypeMetaData.AssemblyMetaData.Path);
                Type type;
                try
                {
                    type = launcherTypeMetaData.ResolveType();
                    if (type == null && !secondPass)
                    {
                        MetaDataManager.RefreshExportDerivatives();
                        secondPass = true;
                        goto tryAgain;
                    }
                }
                catch(Exception ex)
                {
                    l.Error("Failed to resolve type: " + ex);
                    throw;
                }
                if (type == null)
                {
                    MessageBox.Show("Failed to resolve type: " + launcherTypeMetaData.FullName);
                }
                else
                {
                    ILauncher launcher = (ILauncher)Activator.CreateInstance(type);
                    launcher.Launch(args);
                }
                //LionFire.Launcher.Avalon.AvalonApplicationOpener.Open(type);

            }


            l.Info("[" + launchName + "] Launcher finished.");
        }

        #region Misc

        private static readonly ILogger l = Log.Get();
    
        #endregion

    }

}
