﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using BetterConsoles.Colors.Extensions;
using BetterConsoles.Tables;
using BetterConsoles.Tables.Builders;
using BetterConsoles.Tables.Configuration;
using BetterConsoles.Tables.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetPro.Startup;
using NetPro.TypeFinder;

[assembly: HostingStartup(typeof(Startup))]
namespace NetPro.Startup
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

            builder.ConfigureAppConfiguration((config, builder) =>
              {
                  var env = config.HostingEnvironment.EnvironmentName; //只要代码写HostingEnvironment就报未实现，但是debug又能去取到数据
                  Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] loading json files");
                  builder.SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json", true, true)
                              .AddJsonFile($"appsettings.{env}.json", true, true)
                              .AddJsonFile("startup.json", true, true)
                              .AddEnvironmentVariables();
                  _configuration = builder.Build();
              });

            builder.ConfigureServices((context, services) =>
            {
                //Inject the file lookup component
                services.AddFileProcessService();
                ITypeFinder _typeFinder = services.BuildServiceProvider().GetRequiredService<ITypeFinder>();
                var startupConfigurations = _typeFinder.FindClassesOfType<INetProStartup>();

                //create and sort instances of startup configurations                 
                instances = startupConfigurations
                  .Select(startup => new _ { NetProStartupImplement = (INetProStartup)Activator.CreateInstance(startup), Type = startup })
                  .OrderBy(startup => startup.NetProStartupImplement.Order)
                  .ToList();

                //try to read startup.jsonfile
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), $"startup.json");
                if (File.Exists(jsonPath))
                {
                    var folderDetails = jsonPath;
                    var startupJson = File.ReadAllText(folderDetails);
                    //dynamic jsonObj = new System.Dynamic.ExpandoObject();
                    try
                    {
                        var jsonObj = JsonSerializer.Deserialize<Dictionary<string, double>>(startupJson);

                        foreach (var instance in instances)
                        {
                            var startupName = instance.NetProStartupImplement.GetType().Name;
                            if (jsonObj.Where(s => s.Key.ToLower() == startupName.ToLower()).Any())
                            {
                                instance.NetProStartupImplement.Order = jsonObj.Where(s => s.Key.ToLower() == startupName.ToLower()).FirstOrDefault().Value;
                                jsonObj.Remove(startupName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"startup.json exception: {ex.Message}");
                        goto nofile;
                    }

                    instancesByOrder = instances.OrderBy(startup => startup.NetProStartupImplement.Order).ToList();
                }
            //else
            //{
            //    using (var writer = File.CreateText(jsonPath))
            //    {
            //        writer.WriteLine("log message");
            //    }
            //}
            //configure services
            nofile:
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Service injection sequence：", System.Drawing.Color.FromArgb(1, 212, 1));

                //var table = new Table("Order", "StartupClassName", "Path", "Assembly");

                CellFormat headerFormat = new CellFormat()
                {
                    Alignment = Alignment.Center,
                    ForegroundColor = Color.Magenta
                };

                string FormatMoney(string text)
                {
                    if ("RoutingStartup200-EndpointsStartup1000-StaticFilesStartup100-ErrorHandlerStartup0".Contains(text))
                    {
                        return text.ForegroundColor(Color.FromArgb(152, 168, 75));
                    }
                    return text;
                }

                Table table = new TableBuilder(headerFormat)
              .AddColumn("Order", rowsFormat: new CellFormat(foregroundColor: Color.FromArgb(128, 129, 126)))
              .AddColumn("StartupClassName")
              .RowFormatter<string>((x) => FormatMoney(x))
                  .RowsFormat()
                      .ForegroundColor(Color.FromArgb(128, 129, 126))
              .AddColumn("Path")
                  .RowsFormat()
                      .ForegroundColor(Color.FromArgb(128, 129, 126))
              .AddColumn("Assembly")
                  .RowsFormat()
                      .ForegroundColor(Color.FromArgb(128, 129, 126))
                      .Alignment(Alignment.Left)
             .AddColumn("Version")
                  .RowsFormat()
                      .ForegroundColor(Color.FromArgb(128, 129, 126))
                      .Alignment(Alignment.Left)
              .Build();

                table.Config = TableConfig.Unicode();

                foreach (var instance in instancesByOrder ?? instances)
                {
                    instance.NetProStartupImplement.ConfigureServices(services, _configuration, _typeFinder);
                    var assemblyName = instance.Type.Assembly.GetName();
                    table.AddRow(instance.NetProStartupImplement.Order, instance.Type.Name, instance.NetProStartupImplement, $"{assemblyName.Name} ",$" {assemblyName.Version}");
                }

                Console.WriteLine(table.ToString());
                Console.ResetColor();

                //run startup tasks
                RunStartupTasks(_typeFinder);

                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            });

            builder.Configure((context, app) =>
            {

                //var hostEnvironment = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
                //var hostEnvironment = context.HostingEnvironment;
                //if (hostEnvironment.EnvironmentName == Environments.Development)
                _serviceProvider = app.ApplicationServices;

                var typeFinder = _serviceProvider.GetRequiredService<ITypeFinder>();
                var startupConfigurations = typeFinder.FindClassesOfType<INetProStartup>();

                //configure request pipeline
                foreach (var instance in instancesByOrder ?? instances)
                {
                    instance.NetProStartupImplement.Configure(app);
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

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //check for assembly already loaded
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
                return assembly;
            return null;
        }
    }
}
