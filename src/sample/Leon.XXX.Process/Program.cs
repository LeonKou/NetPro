using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NetPro.Utility.Helpers;
using NetPro.Web.Core.Infrastructure.Extensions;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Leon.XXX.Process
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddEnvironmentVariables("ASPNETCORE_");
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", true);
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);
                    configApp.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.ConfigureApplicationServices(hostContext.Configuration, hostContext.HostingEnvironment);
                })
                .Build();

            host.Run();


            //自动生成随机aes 密钥
            byte[] bytes = new byte[16];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);

            var key = Convert.ToBase64String(bytes);
            var me = EncryptHelper.AesEncrypt("88888", key);


            var resulat = EncryptHelper.AesDecrypt(me, key);
            Console.WriteLine(resulat);
        }
    }
}
