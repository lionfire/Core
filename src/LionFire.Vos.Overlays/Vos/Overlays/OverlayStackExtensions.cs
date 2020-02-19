using LionFire.MultiTyping;
using Microsoft.Extensions.DependencyInjection;
using System;
using LionFire.Vos.Services;
using LionFire.Vos.Mounts;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Referencing;
using LionFire.Persistence;

namespace LionFire.Vos.Overlays
{
    public static class OverlayStackExtensions
    {
        public static IVob AddOverlayStack(this IVob vob, OverlayStackOptions options = null)
        {
            //vob.GetMultiTyped().AddType(ActivatorUtilities.CreateInstance<VosPackageManager>(serviceProvider, v, options ?? new VosPackageManagerOptions())); // OLD

            vob.GetRequiredService<ServiceDirectory>().RegisterService<OverlayStack, string>(vob.Path, name: options?.Name);

            vob.GetMultiTyped().AddType(new OverlayStack(vob, options));

            return vob;
        }

        public static OverlayStack AsOverlayStack(this IVob vob)
        {
            return vob.GetMultiTyped().AsType<OverlayStack>();
        }

        public static async Task AddExistingOverlaySources(this IRootVob root, string overlayStackName, OverlayStackOptions overlayStackOptions = null, MountOptions mountOptions = null, bool? requiredExistResult = true)
        {
            if (requiredExistResult == false) throw new ArgumentException($"{nameof(requiredExistResult)} cannot be false.");

            overlayStackOptions ??= new OverlayStackOptions();
            mountOptions ??= overlayStackOptions?.DefaultMountOptions;

            foreach (var mount in root["$stores"].AcquireOwn<VobMounts>().ReadMounts.Select(kvp => kvp.Value))
            {
                var storeName = mount.MountPoint.Name;

                if (overlayStackOptions.StoreNameBlacklist != null && overlayStackOptions.StoreNameBlacklist.Contains(storeName)) continue; // TOTEST
                if (overlayStackOptions.StoreNameWhitelist != null && !overlayStackOptions.StoreNameWhitelist.Contains(storeName)) continue; // TOTEST

                var potentialOverlaySourceTarget = mount.Target.GetChild(overlayStackName);  // Example: "file:c:\Program Files (x86)\Org\App\Base"

                if (await potentialOverlaySourceTarget.GetReadHandle<IDirectory>().Exists().ConfigureAwait(false) != requiredExistResult) continue;

                var availableVob = root[VosOverlayDirs.GetOverlayStackPath(overlayStackName)].AsOverlayStack()?.AvailableRoot; // Example: /overlays/base/available
                if (availableVob == null) throw new NotFoundException("Mount point not found"); // TODO: Allow silent fail?
                availableVob.Mount(root[$"$stores/{storeName}/{overlayStackName}"], mountOptions);
            }
        }

    }
}
