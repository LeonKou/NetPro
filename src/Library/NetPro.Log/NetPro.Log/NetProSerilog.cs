using NetPro.Log.NetPro.Log;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using System;

namespace NetPro.Logging
{
    /// <summary>
    /// serilog日志封装
    /// </summary>
    public class NetProSerilog
    {
        /// <summary>
        /// 初始化Serilog.支持常见接收器
        /// </summary>
        /// <param name="param">初始化参数</param>
        public static void InitSerilog(NetProSerilogOptions param)
        {
            var configuraton = new LoggerConfiguration()
                .Enrich.FromLogContext()
                 .Enrich.WithExceptionDetails();

            if (param.Configuration != null)
            {
                configuraton = configuraton.ReadFrom.Configuration(param.Configuration);
            }

            //配置日志级别
            switch (param.MinimumLevel)
            {
                case LogEventLevel.Debug:
                    configuraton = configuraton.MinimumLevel.Debug();
                    break;
                case LogEventLevel.Error:
                    configuraton = configuraton.MinimumLevel.Error();
                    break;
                case LogEventLevel.Fatal:
                    configuraton = configuraton.MinimumLevel.Fatal();
                    break;
                case LogEventLevel.Information:
                    configuraton = configuraton.MinimumLevel.Information();
                    break;
                case LogEventLevel.Verbose:
                    configuraton = configuraton.MinimumLevel.Verbose();
                    break;
                case LogEventLevel.Warning:
                    configuraton = configuraton.MinimumLevel.Warning();
                    break;
            }
            if (!string.IsNullOrWhiteSpace(param.ApplicationName))
            {
                configuraton = configuraton.Enrich.WithProperty("Application", param.ApplicationName);
            }

            var sinks = param.Sinks;
            if (sinks != null)
            {
                sinks = sinks.ToLower();

                if (sinks.Contains(SerilogSink.Console))
                {
                    configuraton = configuraton.WriteTo.Async(s => s.Console());
                }
                if (sinks.Contains(SerilogSink.Debug))
                {
                    configuraton = configuraton.WriteTo.Async(s => s.Debug());
                }
                if (sinks.Contains(SerilogSink.File))
                {
                    if (param.IsSeparateLogFile)
                    {
                        configuraton = configuraton
                        .WriteTo.Async(lc => lc.Logger(lc => lc.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Error))
                        .WriteTo.Async(a => a.File("logs/log_error.txt",
                         rollingInterval: RollingInterval.Day,
                         rollOnFileSizeLimit: true)))
                          .WriteTo.Async(lc => lc.Logger(lc => lc.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Warning))
                          .WriteTo.Async(a => a.File("logs/log_warning.txt",
                         rollingInterval: RollingInterval.Day,
                         rollOnFileSizeLimit: true)))
                            .WriteTo.Async(lc => lc.Logger(lc => lc.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Fatal))
                           .WriteTo.Async(a => a.File("logs/log_fatal.txt",
                         rollingInterval: RollingInterval.Day,
                         rollOnFileSizeLimit: true)))
                          .WriteTo.Async(lc => lc.Logger(lc => lc.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Information))
                          .WriteTo.Async(a => a.File("logs/log_info.txt",
                         rollingInterval: RollingInterval.Day,
                         rollOnFileSizeLimit: true)))
                           .WriteTo.Async(lc => lc.Logger(lc => lc.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Verbose))
                           .WriteTo.Async(a => a.File("logs/log_verbose.txt",
                         rollingInterval: RollingInterval.Day,
                         rollOnFileSizeLimit: true)))
                          .WriteTo.Async(lc => lc.Logger(lc => lc.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Debug))
                          .WriteTo.Async(a => a.File("logs/log_debug.txt",
                         rollingInterval: RollingInterval.Day,
                         rollOnFileSizeLimit: true)));
                    }
                    else
                    {
                        configuraton = configuraton.WriteTo.Async(a => a.File("logs/log.txt",
                       rollingInterval: RollingInterval.Day,
                       rollOnFileSizeLimit: true));
                    }
                }
                if (sinks.Contains(SerilogSink.Exceptionless))
                {
                    var elSetting = param.ExceptionLessOptions;
                    if (elSetting == null)
                    {
                        throw new ArgumentNullException("ExceptionLessSetting参数不能为空");
                    }
                    var apiKey = elSetting.ApiKey;
                    var serverUrl = elSetting.ServerUrl;
                    var minLevel = elSetting.MinimumLevel < param.MinimumLevel ? param.MinimumLevel : elSetting.MinimumLevel;
                    if (string.IsNullOrWhiteSpace(apiKey))
                    {
                        throw new ArgumentNullException("ExceptionLessSetting apiKey参数不能为空");
                    }
                    if (string.IsNullOrWhiteSpace(serverUrl))
                    {
                        throw new ArgumentNullException("ExceptionLessSetting serverUrl参数不能为空");
                    }
                    configuraton = configuraton.WriteTo.Async(s => s.Exceptionless(apiKey, serverUrl: serverUrl, restrictedToMinimumLevel: minLevel));
                }
                if (sinks.Contains(SerilogSink.Elasticsearch))
                {
                    var elSetting = param.ElasticsearchOptions;
                    if (elSetting == null)
                    {
                        throw new ArgumentNullException("ElasticsearchOptions参数不能为空");
                    }
                    configuraton = configuraton.WriteTo.Async(s => s.Exceptionless(elSetting.ApiKey, serverUrl: elSetting.ServerUrl, restrictedToMinimumLevel: elSetting.MinimumLevel));
                }
                if (sinks.Contains(SerilogSink.Sentry))
                {
                    var options = param.SentryOptions;  //待优化成实体
                    if (options == null)
                    {
                        throw new ArgumentNullException("SentryOptions参数不能为空");
                    }
                    configuraton = configuraton.WriteTo.Async(s => s.Sentry(options));
                }
            }
            Serilog.Log.Logger = configuraton.CreateLogger();
        }
    }
}
