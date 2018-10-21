using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace LionFire.AccessPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : Controller
    {
        [HttpGet("components")]
        public IEnumerable<string> Components()
        {
            var list = new List<string>();
            foreach(var component in AppHost.MainApp.Children)
            {
                list.Add(component.GetType().Name);
            }
            return list;
        }
    }
}
