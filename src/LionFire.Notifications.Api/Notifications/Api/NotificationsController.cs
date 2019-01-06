using System;
using Microsoft.AspNetCore.Mvc;

namespace LionFire.Notifications.Api
{
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {

        [HttpGet]
        public string Test()
        {
            return "Hello world";
        }
    }
}
