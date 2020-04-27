using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace NetPro.Checker
{
    public class AppEnvironment
    {
        public int ProcessId { get; private set; }

        public DateTime ProcessStartTime { get; private set; }

        public string Hostname { get; private set; }

        public Dictionary<string, string> EnvironmentVariables { get; } = new Dictionary<string, string>();

        public static AppEnvironment GetAppEnvironment()
        {
            var env = new AppEnvironment
            {
                ProcessId = Process.GetCurrentProcess().Id,
                ProcessStartTime = Process.GetCurrentProcess().StartTime.ToUniversalTime(),
                Hostname = Dns.GetHostName()
            };

            var envVars = Environment.GetEnvironmentVariables();
            foreach (var envVarKey in envVars.Keys)
            {
                env.EnvironmentVariables.Add(envVarKey.ToString(), envVars[envVarKey].ToString());
            }

            return env;
        }
    }
}