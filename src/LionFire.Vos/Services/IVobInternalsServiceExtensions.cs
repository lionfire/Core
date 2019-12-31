using LionFire.Structures;
using LionFire.Vos.Internals;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Services
{
    public static class IVobInternalsServiceExtensions
    {
        public static IServiceCollection OwnServiceCollection(this IVob vob)
        {
            return vob.GetOrAddOwn<IServiceCollection>();
            //var vobI = 
            //var vobNode = vobI.GetOrAddOwnVobNode<IServiceCollection>(vobNode =>
            //{
            //    var factory = vob.GetService<IFactory<IServiceCollection>>();
            //    if (factory == null)
            //    {
            //        throw new NotSupportedException("No IFactory<IServiceCollection> registered -- cannot create IServiceCollection.");

            //    }
            //    return factory.Create();
            //});
            //return vobNode.Value;
        }
    }
}
