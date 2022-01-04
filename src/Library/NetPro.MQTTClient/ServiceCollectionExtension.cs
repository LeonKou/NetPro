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
            if (!mqtttClientOptions.Enabled)
            {
                return services;
            }
            services.AddSingleton(mqtttClientOptions);
            var idleBus = new IdleBus<IMqttClient>(TimeSpan.FromMinutes(10));
            foreach (var item in mqtttClientOptions.ConnectionString)
            {
                idleBus.Register(item.Key, () =>
                {
                    try
                    {
                        var mqttClient = new MqttFactory().CreateMqttClient() as MqttClient;
                        var clientid = _GetItemValueFromConnectionString(item.Value, "clientid");
                        var username = _GetItemValueFromConnectionString(item.Value, "username");
                        var password = _GetItemValueFromConnectionString(item.Value, "password");
                        var host = _GetItemValueFromConnectionString(item.Value, "host");
                        var protString = _GetItemValueFromConnectionString(item.Value, "port");
                        var succeedPort = int.TryParse(protString, out int port);

                        if (string.IsNullOrWhiteSpace(clientid) || string.IsNullOrWhiteSpace(host) || !succeedPort)
                        {
                            throw new ArgumentException($"mqttclient配置信息缺失;clientid={clientid};host={host};Port={protString}");
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
                            System.Console.WriteLine("Client Connected");
                        });

                        mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(arg =>
                        {
                            System.Console.WriteLine("Client disconnected, ClientWasConnected=" + arg.ClientWasConnected.ToString());
                        });

                        //mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(arg =>
                        //{
                        //    string payload = System.Text.Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                        //    System.Console.WriteLine("Message received, topic [" + arg.ApplicationMessage.Topic + "], payload [" + payload + "]");
                        //});

                        mqttClient.ConnectAsync(option.Build());//.GetAwaiter().GetResult();
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
