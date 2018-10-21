using System.Collections.Generic;
using LionFire.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.AccessPanel
{
    /// <summary>
    /// DependencyInjection
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DIController : Controller
    {
        [HttpGet("services")]
        public IEnumerable<string> Services()
        {
                var list = new List<string>();
                var sp = InjectionContext.Default.ServiceProvider as ServiceProvider;
            //foreach (var service in InjectionContext.Default.ServiceProvider)
            //{
            //    yield return service.GetType().Name;
            //}
            list.Add("test");
            return list;
        }
    }
}