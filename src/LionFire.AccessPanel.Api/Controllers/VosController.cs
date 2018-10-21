using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LionFire.AccessPanel.Api.Controllers
{
    public class VosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}