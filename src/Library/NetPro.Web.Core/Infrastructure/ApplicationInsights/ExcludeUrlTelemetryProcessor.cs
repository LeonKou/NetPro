using NetPro.Core.Configuration;
using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPro.Web.Core.Infrastructure.ApplicationInsights
{

    public class ExcludeUrlTelemetryProcessorFactory : ITelemetryProcessorFactory
    {
        private readonly string[] _excludeUrls;

        public ExcludeUrlTelemetryProcessorFactory(string excludeUrls)
        {
            _excludeUrls = excludeUrls?.Split(',');
            if(_excludeUrls!=null&&_excludeUrls.Length>0)
            {
                _excludeUrls = _excludeUrls.Select(r => r.ToLower().Trim()).ToArray();
            }
        }

        public ITelemetryProcessor Create(ITelemetryProcessor next)
        {
            return new ExcludeUrlTelemetryProcessor(next,_excludeUrls);
        }
    }
    public class ExcludeUrlTelemetryProcessor : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        string[] _excludeUrls;

        // Link processors to each other in a chain.
        public ExcludeUrlTelemetryProcessor(ITelemetryProcessor next, string[] excludeUrls)
        {
            this.Next = next;
             _excludeUrls = excludeUrls;
        }
        public void Process(ITelemetry item)
        {
            if (!Filter(item)) { return; }

            this.Next.Process(item);
        }

        private bool Filter(ITelemetry item)
        {
            if (_excludeUrls == null||(_excludeUrls!=null&&_excludeUrls.Length==0)) return true;
           
            if (item is RequestTelemetry)
            {
                var request = item as RequestTelemetry;
                if (_excludeUrls.Any(n => request.Name.ToLower().Contains(n)))
                {
                    return false;
                }
            }
            else if (item is DependencyTelemetry)
            {
                var dep = item as DependencyTelemetry;

                if (_excludeUrls.Any(n => dep.Data.ToLower().Contains(n)))
                {
                    return false;
                }
            }
            return true;

        }
    }
}
