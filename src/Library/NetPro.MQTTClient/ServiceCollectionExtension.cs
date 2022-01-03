using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetPro.MQTTClient
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// MQTT客户端，支持多个host
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddMQTTClient(this IServiceCollection services, IConfiguration configuration)
        {
            var mqtttClientOptions = new MQTTClientOption(configuration);
            services.AddSingleton(mqtttClientOptions);
            var idleBus = new IdleBus<IMqttClient>(TimeSpan.FromMinutes(10));
            foreach (var item in mqtttClientOptions.ConnectionString)
            {
                idleBus.Register(item.Key, () =>
                {
                    try
                    {
                        var mqttClient = new MqttFactory().CreateMqttClient() as IMqttClient; ;
                        var option = new MqttClientOptionsBuilder()
                        .WithClientId(_GetItemValueFromConnectionString(item.Value, "clientid"))
                        .WithCredentials(_GetItemValueFromConnectionString(item.Value, "username"), _GetItemValueFromConnectionString(item.Value, "password"))
                        .WithTcpServer(_GetItemValueFromConnectionString(item.Value, "host"), int.Parse(_GetItemValueFromConnectionString(item.Value, "port")))
                        .Build();
                        mqttClient.ConnectAsync(option);
                        return mqttClient;
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException($"mqttclient初始化有误{ex}");
                    }
                });
            }
            services.AddSingleton(idleBus);
            //https://www.cnblogs.com/ccsharppython/archive/2019/07/28/11261069.html
            return services;

            string _GetItemValueFromConnectionString(string connectionString, string itemName)
            {
                if (!connectionString.EndsWith(";"))
                    connectionString += ";";

                // \s* 匹配0个或多个空白字符
                // .*? 匹配0个或多个除 "\n" 之外的任何字符(?指尽可能少重复)
                string regexStr = itemName + @"\s*=\s*(?<key>.*?);";
                Regex r = new Regex(regexStr, RegexOptions.IgnoreCase);
                Match mc = r.Match(connectionString);
                return mc.Groups["key"].Value;
            }
        }
    }
}
