/*
 *  MIT License
 *  
 *  Copyright (c) 2021 LeonKou
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.NetPro;
using System.Reflection;

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
