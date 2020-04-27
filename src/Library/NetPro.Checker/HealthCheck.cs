using System;

namespace NetPro.Checker
{
    public class HealthCheck
    {
        public struct Result
        {
            public readonly string Name;
            public readonly HealthResponse Check;

            public Result(string name, HealthResponse check)
            {
                Name = name;
                Check = check;
            }
        }

        private readonly Func<HealthResponse> _checkFunc;
        public string Name { get; private set; }

        public HealthCheck(string name, Action check)
           : this(name, () => { check(); return string.Empty; })
        { }

        public HealthCheck(string name, Func<string> check)
            : this(name, () => HealthResponse.Healthy(check()))
        { }

        public HealthCheck(string name, Func<HealthResponse> check)
        {
            Name = name;
            _checkFunc = check;
        }

        protected virtual HealthResponse Check()
        {
            return _checkFunc();
        }

        public Result Execute()
        {
            try
            {
                return new Result(Name, Check());
            }
            catch (Exception x)
            {
                return new Result(Name, HealthResponse.Unhealthy(x));
            }
        }

    }
}
