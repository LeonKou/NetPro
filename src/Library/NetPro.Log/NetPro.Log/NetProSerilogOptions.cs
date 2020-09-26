using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace NetPro.Log.NetPro.Log
{
    public class NetProSerilogOptions
    {
        public string Sinks { get; set; }

        public string ApplicationName { get; set; }
        public LogEventLevel MinimumLevel { get; set; }
        public IConfiguration Configuration { get; set; }

        public bool IsSeparateLogFile { get; set; }

        public ExceptionLessOptions ExceptionLessOptions { get; set; }

        public ElasticsearchOptions ElasticsearchOptions { get; set; }

        public string SentryOptions { get; set; }  //dsn
    }

    public class ExceptionLessOptions
    {
        public string ApiKey { get; set; }
        public string ServerUrl { get; set; }
        public LogEventLevel MinimumLevel { get; set; }
    }

    public class ElasticsearchOptions
    {
        public string ApiKey { get; set; }
        public string ServerUrl { get; set; }
        public LogEventLevel MinimumLevel { get; set; }
    }

    public class SerilogSink
    {
        public const string Console = "console";
        public const string Debug = "debug";
        public const string File = "file";
        public const string Exceptionless = "exceptionless";
        public const string Sentry = "sentry";
        public const string Elasticsearch = "es";
    }
}
