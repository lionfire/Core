using Microsoft.AspNetCore.Mvc;
//using LionFire.Instantiating;

namespace LionFire.Machine.Sentinel.Controllers
{
    [Route("api/[controller]")]
    public class DiagController : Controller
    {
        [HttpGet("host")]
        public string Host()
        {
            return this.Request.Host.Host;
        }
    }
}
