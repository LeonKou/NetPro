using StackExchange.Redis.Profiling;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Authentication;

namespace StackExchange.Redis.Extensions.Core.Configuration
{
    /// <summary>
    /// The redis configuration
    /// </summary>
    public class RedisConfiguration
    {
        private ConfigurationOptions options;
        private bool enabled;
        private string keyPrefix;
        private string password;
        private bool allowAdmin;
        private bool ssl;
        private int connectTimeout = 5000;
        private int syncTimeout = 1000;
        private bool abortOnConnectFail;
        private int database = 0;
        private RedisHost[] hosts;
        private ServerEnumerationStrategy serverEnumerationStrategy;
        private uint maxValueLength;
        private int poolSize = 5;
        private string[] excludeCommands;
        private string configurationChannel = null;
        private string connectionString = null;
        private string serviceName = null;
        private SslProtocols? sslProtocols = null;
        private Func<ProfilingSession> profilingSessionProvider;

        /// <summary>
        /// 一个RemoteCertificateValidationCallback代表，负责验证远程方提供的证书；注意不能在配置字符串中指定。
        /// </summary>
        public event RemoteCertificateValidationCallback CertificateValidation;

        /// <summary>
        /// 是否启用Redis;false实例化NullCache
        /// </summary>
        public bool? Enabled
        {
            get => enabled;
            set
            {
                if (value.HasValue)
                    enabled = value.Value;
                else
                    enabled = true;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// Gets or sets the servicename used in case of Sentinel.
        /// </summary>
        public string ServiceName
        {
            get => serviceName;
            set
            {
                serviceName = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// 获取一个值，表示是否获取一个布尔值，该值表示集群是否被配置为哨兵。
        /// </summary>
        public bool IsSentinelCluster => !string.IsNullOrEmpty(ServiceName);

        /// <summary>
        /// G
        /// </summary>
        public SslProtocols? SslProtocols
        {
            get => sslProtocols;
            set
            {
                sslProtocols = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// 获取或设置连接字符串
        /// </summary>
        public string ConnectionString
        {
            get => connectionString;
            set
            {
                connectionString = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// 获取或设置用于广播和监听配置更改通知的通道名称
        /// </summary>
        public string ConfigurationChannel
        {
            get => configurationChannel;
            set
            {
                configurationChannel = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// 获取或设置所有缓存项的key隔离前缀。
        /// </summary>
        public string KeyPrefix
        {
            get => keyPrefix;
            set
            {
                keyPrefix = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// 获取或设置redis密码。
        /// </summary>
        public string Password
        {
            get => password;
            set
            {
                password = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// 获取或设置一个值，表示是否允许进行获取或设置管理操作。
        /// </summary>
        public bool AllowAdmin
        {
            get => allowAdmin;
            set
            {
                allowAdmin = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// 获取或设置一个值，表示是否指定是否要对连接进行加密。
        /// </summary>
        public bool Ssl
        {
            get => ssl;
            set
            {
                ssl = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// 获取或设置允许连接的时间，以毫秒为单位（默认为5秒，除非SyncTimeout更高）。
        /// </summary>
        public int ConnectTimeout
        {
            get => connectTimeout;
            set
            {
                connectTimeout = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// 获取或设置系统允许同步操作的时间，单位为毫秒（默认值为5秒）
        /// </summary>
        public int SyncTimeout
        {
            get => syncTimeout;
            set
            {
                syncTimeout = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// 获取或设置一个值，表示是否获取或设置是否应该通过TimeoutException显式通知连接/配置超时。
        /// </summary>
        public bool AbortOnConnectFail
        {
            get => abortOnConnectFail;
            set
            {
                abortOnConnectFail = value;//始终false,超时不报错
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// 获取或设置数据库Id。
        /// </summary>
        public int Database
        {
            get => database;
            set
            {
                database = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// 获取或设置Redis服务器的主机（ips或名称）
        /// </summary>
        public RedisHost[] Hosts
        {
            get => hosts;
            set
            {
                hosts = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// 获取或设置执行服务器范围内的命令时要使用的策略。
        /// </summary>
        public ServerEnumerationStrategy ServerEnumerationStrategy
        {
            get => serverEnumerationStrategy;
            set
            {
                serverEnumerationStrategy = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        ///获取或设置Redis数据库中待存储的值最大长度
        /// </summary>
        public uint MaxValueLength
        {
            get => maxValueLength;
            set
            {
                maxValueLength = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// 获取或设置redis连接池的大小
        /// </summary>
        public int PoolSize
        {
            get => poolSize;
            set
            {
                poolSize = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// 获取或设置redis conn获取或设置excludecommands.ecctions池大小
        /// </summary>
        public string[] ExcludeCommands
        {
            get => excludeCommands;
            set
            {
                excludeCommands = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// Gets or sets redis Profiler to attach to ConnectionMultiplexer.
        /// </summary>
        public Func<ProfilingSession> ProfilingSessionProvider
        {
            get => profilingSessionProvider;
            set
            {
                profilingSessionProvider = value;
                ResetConfigurationOptions();
            }
        }

        /// <summary>
        /// Gets the Redis configuration options
        /// </summary>
        /// <value>An instanfe of <see cref="ConfigurationOptions" />.</value>
        public ConfigurationOptions ConfigurationOptions
        {
            get
            {
                if (options == null)
                {
                    if (!string.IsNullOrEmpty(ConnectionString))
                    {
                        options = ConfigurationOptions.Parse(ConnectionString);
                    }
                    else
                    {
                        options = new ConfigurationOptions
                        {
                            KeepAlive = 180,
                            Ssl = Ssl,
                            AllowAdmin = AllowAdmin,
                            Password = Password,
                            ConnectTimeout = ConnectTimeout,
                            SyncTimeout = SyncTimeout,
                            AbortOnConnectFail = AbortOnConnectFail,
                            ConfigurationChannel = ConfigurationChannel,
                            SslProtocols = sslProtocols,
                            ChannelPrefix = KeyPrefix,
                        };

                        if (IsSentinelCluster)
                        {
                            options.ServiceName = ServiceName;
                            options.CommandMap = CommandMap.Sentinel;
                        }

                        foreach (var redisHost in Hosts)
                            options.EndPoints.Add(redisHost.Host, redisHost.Port);
                    }

                    if (ExcludeCommands != null)
                    {
                        options.CommandMap = CommandMap.Create(
                            new HashSet<string>(ExcludeCommands),
                            available: false);
                    }

                    options.CertificateValidation += CertificateValidation;
                }

                return options;
            }
        }

        private void ResetConfigurationOptions()
        {
            // this is needed in order to cover this scenario
            // https://github.com/imperugo/StackExchange.Redis.Extensions/issues/165
            options = null;
        }
    }
}
