using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using BetterConsoles.Colors.Extensions;
using BetterConsoles.Tables;
using BetterConsoles.Tables.Builders;
using BetterConsoles.Tables.Configuration;
using BetterConsoles.Tables.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            var dynamicObject = new ExpandoObject() as IDictionary<string, Object>;

            builder.ConfigureAppConfiguration((config, builder) =>
              {
                  var env = config.HostingEnvironment.EnvironmentName; //只要代码写HostingEnvironment就报未实现，但是debug又能去取到数据
                  Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] loading json files");
                  builder.SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json", true, true)
                              .AddJsonFile($"appsettings.{env}.json", true, true)
                              //.AddJsonFile("startup.json", true, true)
                              .AddEnvironmentVariables();
                  _configuration = builder.Build();
              });

            builder.ConfigureServices((context, services) =>
            {
                //Inject the file lookup component
                var option = _configuration.GetSection(nameof(TypeFinderOption)).Get<TypeFinderOption>();
                services.AddFileProcessService(option);
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
                                if (jsonObj.Where(s => s.Key.ToLower() == startupName.ToLower()).Any())
                                {
                                    instance.NetProStartupImplement.Order = jsonObj.Where(s => s.Key.ToLower() == startupName.ToLower()).FirstOrDefault().Value;
                                    jsonObj.Remove(startupName);
                                }
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
                else
                {
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
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Service injection sequence：", System.Drawing.Color.FromArgb(1, 212, 1));

                CellFormat headerFormat = new CellFormat()
                {
                    Alignment = Alignment.Center,
                    ForegroundColor = Color.Magenta
                };

                string FormatData(string text)
                {
                    //高亮原生中间件，方便识别中间件顺序
                    if ("RoutingStartup200-EndpointsStartup1000-StaticFilesStartup100-ErrorHandlerStartup0".Contains(text))
                    {
                        return $"{text}(default)".ForegroundColor(Color.FromArgb(0, 255, 0));
                    }
                    return text;
                }

                Table table = new TableBuilder(headerFormat)
              .AddColumn("Order", rowsFormat: new CellFormat(foregroundColor: Color.FromArgb(128, 129, 126)))
              .AddColumn("StartupClassName").RowFormatter<string>((x) => FormatData(x)).RowsFormat().ForegroundColor(Color.FromArgb(128, 129, 126))
              .AddColumn("Path").RowsFormat().ForegroundColor(Color.FromArgb(128, 129, 126))
              .AddColumn("Assembly").RowFormatter<string>((x) =>
                {
                    if (!x.Contains("NetPro"))
                    {
                        return $"{x}(custom)".ForegroundColor(Color.FromArgb(255, 215, 0));
                    }
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

                Console.WriteLine(table.ToString());
                Console.ResetColor();

                //Inject the static object engine
                var engine = EngineContext.Create();
                engine.ConfigureServices(services);

                //run startup tasks
                RunStartupTasks(_typeFinder);

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

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //check for assembly already loaded
            //var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            var assembly = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(a => a.FullName== args.Name); 
            if (assembly != null)
                return assembly;
            return null;
        }
    }
}
