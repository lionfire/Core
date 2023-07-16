using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LionFire.ExtensionMethods;
using LionFire.Persistence.Assets;

namespace LionFire.Assets;

public class AssetPersistenceController<TAsset> : Controller
    where TAsset : class
{
    protected LiveAssetCollection<TAsset> assetObjects = LiveAssetCollection<TAsset>.Instance;

    [HttpGet]
    public IEnumerable<string> Get()
    {
        assetObjects.RefreshHandles().Wait();
        return assetObjects.Handles.Keys.ToArray();
    }

    [HttpGet("{key}")]
    public TAsset Get(string key)
    {
        var existing = assetObjects.Handles[key];
        if (existing != null && existing.Object != null)
        {
            return existing.Object;
        }
        return null;
    }

    [HttpPost("{key}")]
    public async Task Post(string key, [FromBody]TAsset value)
    {
        var existing = assetObjects.Handles[key];
        if (existing != null && existing.Object != null)
        {
            existing.Object.AssignPropertiesFrom(value); // TODO - only assign non-default properties
            await existing.Save();
        }
        else
        {
            await assetObjects.Add(value, key);
        }
    }

    //[HttpPatch]
    //public void Patch(string key, JsonPatchExtensions<HeartbeatMonitor> patch)
    //{
    //}

    [HttpPut("{key}")]
    public async Task Put(string key, [FromBody]TAsset value)
    {

        if (assetObjects.ContainsKey(key) && assetObjects.Handles[key].Object != null)
        {
            throw new AlreadyException("Already exists");
        }
        else
        {
            await assetObjects.Add(value, key);
        }
    }

    [HttpDelete("{key}")]
    public async Task Delete(string key)
    {
        if (assetObjects.Handles.ContainsKey(key))
        {
            await assetObjects.Remove(key);
            //fsObjects.Handles[key].Object = null;
        }
    }
}
