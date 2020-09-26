using System.Collections.Generic;

namespace TestConsole
{
    public class OptionTest
    {
        public List<string> subSettings { get; set; }

        public List<Endpoints> Endpoint { get; set; }
    }

    public class Endpoints
    {
        public int Port { get; set; }

        public string Host { get; set; }
    }
}
