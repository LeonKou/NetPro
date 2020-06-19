using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.Web.Api.Infrastructure.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace NetPro.Web.Api
{
    /// <summary>
    /// swagger 启动配置,默认包含 api,viewmodel,servicemodel对应的xml文件，如果需要增加可以继承改类重写GetXmlComments方法
    /// </summary>
    public class SwaggerStartup : INetProStartup
    {
        public int Order => 999;

        public void Configure(IApplicationBuilder application)
        {
            var openApiInfo = application.ApplicationServices.GetService<OpenApiInfo>();
            var configuration = application.ApplicationServices.GetService<IConfiguration>();
            //var config = EngineContext.Current.Resolve<NetProOption>();
            if (!configuration.GetValue<bool>("SwaggerOption:Enable", false)) return;

            application.UseSwagger(c =>
            {
                c.RouteTemplate = "docs/{documentName}/docs.json";//使中间件服务生成Swagger作为JSON端点
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Info.Description = httpReq.Path);//请求过滤处理
            });

            application.UseSwaggerUI(c =>
            {
                c.RoutePrefix = $"{configuration.GetValue<string>("SwaggerOption:RoutePrefix")}";//设置文档首页根路径
                c.SwaggerEndpoint("/docs/v1/docs.json", "V1");//此处配置要和UseSwagger的RouteTemplate匹配
                c.SwaggerEndpoint("https://petstore.swagger.io/v2/swagger.json", "petstore.swagger");//远程swagger示例
                                                                                                     //c.InjectStylesheet("NetPro.Web.Api.Infrastructure.Swagger.custom.css");//注入style文件
                c.InjectStylesheet("/custom.css");//注入style文件
                if (configuration.GetValue<bool>("SwaggerOption:MiniProfilerEnabled", false))
                {
                    var stream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream("NetPro.Web.Api.Infrastructure.Swagger.SwaggerProfiler.html");
                    c.IndexStream = () => stream;
                }
            });
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            if (!configuration.GetValue<bool>("SwaggerOption:Enable", false))
                return;

            services
                .Configure<OpenApiInfo>(configuration.GetSection("SwaggerOption"));

            var info = services.BuildServiceProvider().GetService<IOptions<OpenApiInfo>>().Value;

            services.AddSwaggerGen(c =>
            {
                c.DescribeAllParametersInCamelCase();//请求参数转小写
                c.OperationFilter<SwaggerFileUploadFilter>();//add file fifter component
                c.OperationFilter<SwaggerDefaultValueFilter>();//add webapi  default value of parameter
                c.OperationFilter<CustomerHeaderParameter>();//add default header

                var securityRequirement = new OpenApiSecurityRequirement();
                securityRequirement.Add(new OpenApiSecurityScheme { Name = "Bearer" }, new string[] { });
                c.AddSecurityRequirement(securityRequirement);

                //batch find xml file of swagger
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;//get app root path
                List<string> xmlComments = GetXmlComments();
                xmlComments.ForEach(r =>
                {
                    string filePath = Path.Combine(basePath, r);
                    if (File.Exists(filePath))
                    {
                        c.IncludeXmlComments(filePath);
                    }
                });

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = info.Title,
                    Version = info.Version,
                    Description = info.Description,
                    //TermsOfService = "None",
                    Contact = info.Contact,// new OpenApiContact { Email = "Email", Name = "Name", Url = new Uri("http://www.github.com") },
                    License = info.License,//new OpenApiLicense { Url = new Uri("http://www.github.com"), Name = "LicenseName" },
                });
                c.IgnoreObsoleteActions();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Authority certification(The data is transferred in the request header) structure of the parameters : \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });
        }

        /// <summary>
        /// 所有xml默认当作swagger文档注入swagger
        /// </summary>
        /// <returns></returns>
        protected virtual List<string> GetXmlComments()
        {
            //var pattern = $"^{netProOption.ProjectPrefix}.*({netProOption.ProjectSuffix}|Domain)$";
            //List<string> assemblyNames = ReflectionHelper.GetAssemblyNames(pattern);
            List<string> assemblyNames = AppDomain.CurrentDomain.GetAssemblies().Select(s => s.GetName().Name).ToList();
            List<string> xmlFiles = new List<string>();
            assemblyNames.ForEach(r =>
            {
                string fileName = $"{r}.xml";
                xmlFiles.Add(fileName);
            });
            return xmlFiles;
        }

    }
}
