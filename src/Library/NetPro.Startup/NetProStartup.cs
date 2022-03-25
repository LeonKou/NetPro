﻿/*
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

using BetterConsoles.Colors.Extensions;
using BetterConsoles.Tables;
using BetterConsoles.Tables.Builders;
using BetterConsoles.Tables.Configuration;
using BetterConsoles.Tables.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.NetPro.Startup._;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Threading.Tasks;

[assembly: HostingStartup(typeof(Startup))]
namespace System.NetPro.Startup._
{
    internal class _
    {
        public INetProStartup NetProStartupImplement { get; set; }

        public Type Type { get; set; }
    }
    internal class Startup : IHostingStartup
    {
        /// <summary>
        /// Gets or sets service provider
        /// </summary>
        private IServiceProvider _serviceProvider { get; set; }

        public void Configure(IWebHostBuilder builder)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            var frameworkName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            Console.WriteLine(Figgle.FiggleFonts.Varsity.Render(frameworkName.Substring(0, frameworkName.IndexOf('.'))));
            Console.ResetColor();
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] dotnet process id:{Process.GetCurrentProcess().Id}");

            Console.WriteLine("The enhanced service has started");

            IConfiguration _configuration = null;
            List<_> instancesByOrder = null;
            List<_> instances = null;
            List<string> defaultStartupNames = null;
            var dynamicObject = new ExpandoObject() as IDictionary<string, Object>;

            builder.ConfigureAppConfiguration((config, builder) =>
            {
                //reset ApplicationName
                config.HostingEnvironment.ApplicationName = Assembly.GetEntryAssembly().GetName().Name;

                var env = config.HostingEnvironment.EnvironmentName; //只要代码写HostingEnvironment就报未实现，但是debug又能去取到数据

                //custome json files
                //if (!File.Exists($"customeconfig/custom.{env}.json"))
                //{
                //    Directory.CreateDirectory("customeconfig");
                //    using (var writer = File.CreateText($"customeconfig/custom.{env}.json"))
                //    {
                //        writer.WriteLine("{}");
                //    }
                //}

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] loading json files");
                builder.SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", true, true)//base config
                            .AddJsonFile($"appsettings.{env}.json", true, true); //inherit base config

                _configuration = builder.Build();

                var jsonFilePath = _configuration.GetValue<string>("ConfigPath", ".");

                //Load all jSON-formatted files as configuration
                foreach (var file in Directory.GetFiles(jsonFilePath, $"*.json"))
                {
                    if (file.Contains("runtimeconfig.template.json") || file.Contains($"appsettings."))
                        continue;

                    builder.AddJsonFile(file, true, true);
                    Console.WriteLine(file);
                }

                builder.AddEnvironmentVariables();
                _configuration = builder.Build();
            });

            builder.ConfigureServices(async (context, services) =>
            {
                //Inject the file lookup component
                var option = _configuration.GetSection(nameof(TypeFinderOption)).Get<TypeFinderOption>();
                services.AddFileProcessService(option);
                ITypeFinder _typeFinder = services.BuildServiceProvider().GetRequiredService<ITypeFinder>();
                var startupConfigurations = _typeFinder.FindClassesOfType<INetProStartup>();
                var defaultStartup = _typeFinder.FindClassesOfType<System.NetPro.Startup.__._>().ToList();

                defaultStartupNames = defaultStartup.Select(s => s.Name).ToList();

                //create and sort instances of startup configurations
                instances = startupConfigurations
                  .Select(startup => new _ { NetProStartupImplement = (INetProStartup)Activator.CreateInstance(startup), Type = startup })
                  .OrderBy(startup => startup.NetProStartupImplement.Order)
                  .ToList();

                //try to read startup.jsonfile
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), $"StartupConfig/startup.json");
                if (File.Exists(jsonPath))
                {
                    var startupJson = File.ReadAllText(jsonPath);
                    //dynamic jsonObj = new System.Dynamic.ExpandoObject();
                    try
                    {
                        if (string.IsNullOrWhiteSpace(startupJson))
                        {
                            uint tempInt = 1;
                            foreach (var instance in instances)
                            {
                                if (dynamicObject.ContainsKey(instance.Type.Name))
                                {
                                    //instance.Type.Name = $"{instance.Type.Name}-{tempInt}";
                                    dynamicObject.Add($"{instance.Type.Name}-{tempInt}", instance.NetProStartupImplement.Order);
                                    tempInt++;
                                }
                                else
                                {
                                    dynamicObject.Add(instance.Type.Name, instance.NetProStartupImplement.Order);
                                }
                            }

                            var jsonString = JsonSerializer.Serialize(dynamicObject, new JsonSerializerOptions { WriteIndented = true });
                            using (StreamWriter file = new StreamWriter(jsonPath, true))
                            {
                                file.WriteLine(jsonString);
                            }
                        }
                        else
                        {
                            var jsonObj = JsonSerializer.Deserialize<Dictionary<string, double>>(startupJson);

                            foreach (var instance in instances)
                            {
                                var startupName = instance.NetProStartupImplement.GetType().Name;
                                // startup file is as the criterion
                                if (jsonObj.Where(s => s.Key.ToLower() == startupName.ToLower()).Any())
                                {
                                    instance.NetProStartupImplement.Order = jsonObj.Where(s => s.Key.ToLower() == startupName.ToLower()).FirstOrDefault().Value;
                                    //jsonObj.Remove(startupName);
                                }
                                else
                                {
                                    jsonObj.Add(startupName, instance.NetProStartupImplement.Order);
                                }
                            }

                            using (var writer = File.CreateText(jsonPath))
                            {
                                var jsonString = JsonSerializer.Serialize(jsonObj.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value), new JsonSerializerOptions { WriteIndented = true });
                                writer.WriteLine(jsonString);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"StartupConfig/startup.json exception: {ex.Message}");
                        goto nofile;
                    }

                    instancesByOrder = instances.OrderBy(startup => startup.NetProStartupImplement.Order).ToList();
                }
                else
                {
                    Directory.CreateDirectory("StartupConfig");

                    using (var writer = File.CreateText(jsonPath))
                    {
                        foreach (var instance in instances)
                        {
                            dynamicObject.Add(instance.Type.Name, instance.NetProStartupImplement.Order);
                        }

                        var jsonString = JsonSerializer.Serialize(dynamicObject, new JsonSerializerOptions { WriteIndented = true });
                        writer.WriteLine(jsonString);
                    }
                }
            //configure services
            nofile:
                Console.WriteLine($"Service injection sequence", System.Drawing.Color.FromArgb(1, 212, 1));

                CellFormat headerFormat = new CellFormat()
                {
                    Alignment = Alignment.Center,
                    ForegroundColor = Color.Magenta
                };

                string FormatData(string text)
                {
                    //高亮原生中间件，方便识别中间件顺序
                    if (defaultStartupNames.Where(s => s == text).Any())
                    {
                        return $"{text}(default)".ForegroundColor(Color.FromArgb(0, 255, 0));
                    }
                    else
                    {
                        return $"{text}(custom)".ForegroundColor(Color.FromArgb(255, 215, 0));
                    }

                }

                Table table = new TableBuilder(headerFormat)
              .AddColumn("Order", rowsFormat: new CellFormat(foregroundColor: Color.FromArgb(128, 129, 126)))
              .AddColumn("StartupClassName").RowFormatter<string>((x) => FormatData(x)).RowsFormat().ForegroundColor(Color.FromArgb(128, 129, 126))
              .AddColumn("Path").RowsFormat().ForegroundColor(Color.FromArgb(128, 129, 126))
              .AddColumn("Assembly").RowFormatter<string>((x) =>
                {
                    return x;
                }).RowsFormat().ForegroundColor(Color.FromArgb(128, 129, 126)).Alignment(Alignment.Left)
             .AddColumn("Version").RowsFormat().ForegroundColor(Color.FromArgb(128, 129, 126)).Alignment(Alignment.Left)
             .Build();

                table.Config = TableConfig.Default();

                var tempList = new List<string>();
                uint tempstartupClassNameInt = 1;
                foreach (var instance in instancesByOrder ?? instances)
                {
                    instance.NetProStartupImplement.ConfigureServices(services, _configuration, _typeFinder);
                    var assemblyName = instance.Type.Assembly.GetName();
                    string startupClassName;

                    if (tempList.Where(s => s == instance.Type.Name).Any())
                    {
                        startupClassName = $"{instance.Type.Name}-{tempstartupClassNameInt}";
                        tempstartupClassNameInt++;
                    }
                    else
                    {
                        startupClassName = instance.Type.Name;
                    }
                    table.AddRow(instance.NetProStartupImplement.Order, startupClassName, instance.NetProStartupImplement, $"{assemblyName.Name} ", $" {assemblyName.Version}");
                    tempList.Add(instance.Type.Name);
                }
                Console.WriteLine($"instancesByOrder={instancesByOrder?.Count()};instances={instances.Count()}");
                Console.WriteLine(table.ToString());
                Console.ResetColor();

                //Inject the static object engine
                var engine = EngineContext.Create();
                engine.ConfigureServices(services);

                //run startup tasks
                RunStartupTasks(_typeFinder);

                await RunStartupTasksAsync(_typeFinder);

                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            });

            builder.Configure((context, app) =>
            {
                //var hostEnvironment = context.HostingEnvironment;
                //if (hostEnvironment.EnvironmentName == Environments.Development)
                _serviceProvider = app.ApplicationServices;

                var typeFinder = _serviceProvider.GetRequiredService<ITypeFinder>();
                var startupConfigurations = typeFinder.FindClassesOfType<INetProStartup>();

                //configure request pipeline
                foreach (var instance in instancesByOrder ?? instances)
                {
                    instance.NetProStartupImplement.Configure(app, context.HostingEnvironment);
                }
            });

        }

        /// <summary>
        /// Run startup tasks
        /// </summary>
        /// <param name="typeFinder">Type finder</param>
        protected virtual void RunStartupTasks(ITypeFinder typeFinder)
        {
            //find startup tasks provided by other assemblies
            var startupTasks = typeFinder.FindClassesOfType<IStartupTask>();

            var instances = startupTasks
                .Select(startupTask => (IStartupTask)Activator.CreateInstance(startupTask))
                .OrderBy(startupTask => startupTask.Order);

            foreach (var task in instances)
                task.Execute();
        }

        /// <summary>
        /// Run startupAsync tasks
        /// </summary>
        /// <param name="typeFinder">Type finder</param>
        protected virtual async Task RunStartupTasksAsync(ITypeFinder typeFinder)
        {
            //***IStartupTaskAsync***
            var startupAsyncTasks = typeFinder.FindClassesOfType<IStartupTaskAsync>();

            var instancesAsync = startupAsyncTasks
                .Select(startupTask => (IStartupTaskAsync)Activator.CreateInstance(startupTask))
                .OrderBy(startupTask => startupTask.Order);

            foreach (var task in instancesAsync)
                await task.ExecuteAsync();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //check for assembly already loaded
            //var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            var assembly = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
                return assembly;
            return null;
        }
    }
}
