using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.NetPro;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetPro.MQTTClient
{
    public class MqttClientMulti
    {
        internal static MQTTClientOption MQTTClientOption;
        private MqttClientMulti()
        {

        }

        internal static MqttClientMulti Instance
        {
            get
            {
                MqttClients = new ConcurrentDictionary<string, IMqttClient>();
                return new MqttClientMulti();
            }
        }

        /// <summary>
        ///  通过key获取IMqttClient；连接对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IMqttClient Get(string key)
        {
            return CreateInstanceByKey(key);
        }

        /// <summary>
        /// 通过key获取IMqttClient；连接对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IMqttClient this[string key]
        {
            get
            {
                return CreateInstanceByKey(key);
            }
        }

        private IMqttClient CreateInstanceByKey(string key)
        {
            if (MqttClients.TryGetValue(key, out IMqttClient client))
            {
                return client;
            }
            else
            {
                if (MQTTClientOption == null)
                {
                    MQTTClientOption = EngineContext.Current.Resolve<MQTTClientOption>();
                }

                var value = MQTTClientOption.ConnectionString.Where(s => s.Key == key).FirstOrDefault()?.Value;

                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"Could not find connection string for key = {key}");
                }

                try
                {
                    var mqttClient = new MqttFactory().CreateMqttClient() as IMqttClient;
                    var clientid = _GetItemValueFromConnectionString(value, "clientid");
                    var username = _GetItemValueFromConnectionString(value, "username");
                    var password = _GetItemValueFromConnectionString(value, "password");
                    var host = _GetItemValueFromConnectionString(value, "host");
                    var protString = _GetItemValueFromConnectionString(value, "port");
                    var succeedPort = int.TryParse(protString, out int port);

                    if (string.IsNullOrWhiteSpace(clientid) || string.IsNullOrWhiteSpace(host) || !succeedPort)
                    {
                        throw new ArgumentException($"[mqttclient]mqttclient配置信息缺失;clientid={clientid};host={host};Port={protString}");
                    }
                    var option = new MqttClientOptionsBuilder()
                    .WithClientId(clientid)
                    .WithTcpServer(host, port);

                    if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                    {
                        option.WithCredentials(username, password);
                    }

                    //https://github.com/chkr1011/MQTTnet/issues/929
                    mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(e =>
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        System.Console.WriteLine("[mqttclient]Client Connected");
                        Console.ResetColor();
                    });

                    mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(arg =>
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        System.Console.WriteLine($"[mqttclient]Client disconnected, exception={arg.Exception}");
                        Console.ResetColor();
                    });
                    //https://www.cnblogs.com/ccsharppython/archive/2019/07/28/11261069.html
                    //await will get a successful connection
                    mqttClient.ConnectAsync(option.Build()).ConfigureAwait(false).GetAwaiter().GetResult();
                    MqttClients.TryAdd(key, mqttClient);
                    return mqttClient;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"[mqttclient]初始化有误{ex}");
                }
            }

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
        private static ConcurrentDictionary<string, IMqttClient> MqttClients { get; set; }
    }
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
            if (!mqtttClientOptions.Enabled)
            {
                return services;
            }
            services.AddSingleton(mqtttClientOptions);
            services.AddSingleton(MqttClientMulti.Instance);
            return services;
        }
    }
}
