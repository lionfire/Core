using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LionFire.Vos.Api
{
    public class VosController
    {
        [HttpGet]
        public Task<object> Get(string mountName, string path)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public Task<object> Set(string mountName, string path, object obj)
        {

            throw new NotImplementedException();
        }
    }
}
