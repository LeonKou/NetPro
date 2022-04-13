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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using NetPro.MQTTClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.NetPro;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

public interface IMqttClientMulti
{
    /// <summary>
    /// 根据key标识获取连接对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public IMqttClient Get(string key);

    /// <summary>
    /// 根据key标识获取连接对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public IMqttClient this[string key]
    {
        get;
    }
}

namespace NetPro.MQTTClient
{
    public class MqttClientMulti : IMqttClientMulti
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

        /// <summary>
        /// 
        /// https://github.com/dotnet/MQTTnet/wiki/Client
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private IMqttClient CreateInstanceByKey(string key)
        {
            if (MqttClients.TryGetValue(key, out IMqttClient client))
            {
                if (!client.IsConnected)
                {
                    var succeed = MqttClients.TryAdd(key, client);
                    return _(key);
                }
                return client;
            }
            else
            {
                return _(key);
            }

            IMqttClient _(string key)
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
                    var timeoutString = _GetItemValueFromConnectionString(value, "timeout");
                    var keepaliveString = _GetItemValueFromConnectionString(value, "keepalive");
                    var cleansessionString = _GetItemValueFromConnectionString(value, "cleansession");
                    var succeedtimeout = int.TryParse(timeoutString, out int timeout);
                    var succeedCleansession = bool.TryParse(cleansessionString, out bool cleansession);
                    var succeedKeepalive = int.TryParse(keepaliveString, out int keepalive);
                    
                    if (string.IsNullOrWhiteSpace(clientid) || string.IsNullOrWhiteSpace(host))
                    {
                        //clientid不存在将自动生成
                        clientid = $"{Assembly.GetEntryAssembly().GetName().Name}-{Guid.NewGuid().ToString("N")}";
                    }
                    var option = new MqttClientOptionsBuilder()
                    .WithClientId(clientid);
                    if (Uri.IsWellFormedUriString(host, UriKind.RelativeOrAbsolute))
                    {
                        var mqttUri = new Uri(host);
                        //mqttUri.Scheme //mqtt;mqtts;ws;wss
                        option.WithTcpServer(mqttUri.Host, mqttUri.Port);
                    }
                    else
                    {
                        throw new ArgumentException($"[mqttclient]host配置有误;host={host}");
                    }

                    if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                    {
                        option.WithCredentials(username, password);
                    }
                    if (succeedtimeout)
                    {
                        //通信超时配置，服务器一般默认15秒
                        option.WithCommunicationTimeout(TimeSpan.FromSeconds(timeout));
                    }
                    if (succeedKeepalive)
                    {
                        //https://support.huaweicloud.com/devg-iothub/iot_02_2132.html
                        option.WithKeepAlivePeriod(TimeSpan.FromSeconds(keepalive));
                    }
                    if (succeedCleansession)
                    {
                        option.WithCleanSession(cleansession);
                    }

                    //https://github.com/chkr1011/MQTTnet/issues/929
                    mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(e =>
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        System.Console.WriteLine("[mqttclient]Client Connected");
                        Console.ResetColor();
                    });

                    mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(async arg =>
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        System.Console.WriteLine($"[mqttclient]Client disconnected; messsage={arg.Exception?.Message};Reason={arg.Reason}");
                        Console.ResetColor();
                        //重新订阅方案：
                        //1、调用ReconnectAsync()重新连接后，重新调用订阅主题方法SubscribeAsync()，在clientid保持不变的前提下，会重新接收订阅消息
                        //2、设置客户端WithCleanSession(false);在调用ReconnectAsync()重新连接后，在clientid保持不变的前提下，会重新接收订阅消息

                        //只是重连，但是消息需要重新订阅;也可设置CleanSession为false，重连依旧启用之前的订阅。
                        //推荐方案1，减少服务器session存储。每次连接都是新session
                        var reconnectResult = await mqttClient.ReconnectAsync();
                        Console.WriteLine("[mqttclient] reconnectResult=" + JsonSerializer.Serialize(reconnectResult, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
                        }));
                        Console.WriteLine($"[mqttclient]Client reconnected");
                    });
                    //https://www.cnblogs.com/ccsharppython/archive/2019/07/28/11261069.html
                    //await will get a successful connection
                    //var build = option.WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500).Build();
                    //mqttClient.ConnectAsync(build).Wait();
                    //var version= build.ProtocolVersion;
                    //Console.WriteLine(version);
                    mqttClient.ConnectAsync(option.Build()).ConfigureAwait(false).GetAwaiter().GetResult();
                    MqttClients.TryRemove(key, out IMqttClient _outMqttClient);
                    var succeed = MqttClients.TryAdd(key, mqttClient);
                    return mqttClient;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(@$"MQTT Client初始化有误;
                                                    建议检查连接串是否符合规则；clientid是否有相同;标准格式实例---> clientid=netpro;host=mqtt://localhost:1883;username=netpro;password=netpro;timeout=5000;cleansession=true;");
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
        /// <param name="connectionFactory"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddMQTTClient(this IServiceCollection services, Func<IServiceProvider, IList<ConnectionString>> connectionFactory)
        {
            services.Replace(ServiceDescriptor.Singleton(sp =>
            {
                var connection = connectionFactory.Invoke(sp);
                var config = sp.GetRequiredService<IConfiguration>();
                var option = new MQTTClientOption(config);
                option!.ConnectionString = connection.ToList();
                return option;
            }));
            return services;
        }

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

            services.TryAddSingleton(mqtttClientOptions);
            services.TryAddSingleton<IMqttClientMulti>(MqttClientMulti.Instance);
            return services;
        }
    }
}
