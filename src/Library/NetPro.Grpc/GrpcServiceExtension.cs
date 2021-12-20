using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NetPro.Grpc
{
    public static class GrpcServiceExtension
    {
        public static void AddGrpcServices(this IEndpointRouteBuilder builder, string[] assemblyNames)
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            foreach (var item in ServicesHelper.GetGrpcServices(assemblyNames))
            {
                Type mytype = assembly.GetType(item.Value + "." + item.Key);
                var method = typeof(GrpcEndpointRouteBuilderExtensions).GetMethod("MapGrpcService").MakeGenericMethod(mytype);
                method.Invoke(null, new[] { builder });
            };
        }

        public static void UseNetProGrpcServices(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                AddGrpcServices(endpoints, new string[] { "Grpc.Server" });
            });
        }
    }

    public static class ServicesHelper
    {
        public static Dictionary<string, string> GetGrpcServices(string[] assemblyNames)
        {
            var result = new Dictionary<string, string>();
            foreach (var assemblyName in assemblyNames)
            {
                if (!string.IsNullOrEmpty(assemblyName))
                {
                    Assembly assembly = Assembly.Load(assemblyName);
                    List<Type> ts = assembly.GetTypes().ToList();

                    foreach (var item in ts.Where(u => u.CustomAttributes.Any(a => a.AttributeType == typeof(GrpcServiceAttribute))))
                    {
                        result.Add(item.Name, item.Namespace);
                    }
                }
            }
            return result;
        }
    }
}
