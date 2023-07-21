using Pulsar.Client.Api;

namespace NetPro.Pulsar
{
    /// <summary>
    /// Basic认证
    /// </summary>
    public class AuthenticationBasic : Authentication
    {
        public class BasicAuthenticationDataProvider : AuthenticationDataProvider
        {
            private readonly string _username;

            private readonly string _password;

            public BasicAuthenticationDataProvider(string username, string password)
            {
                _username = username;
                _password = password;
            }

            public override bool HasDataFromCommand()
            {
                return true;
            }

            public override string GetCommandData()
            {
                return _username + ":" + _password;
            }
        }

        private readonly string _username;

        private readonly string _password;

        public AuthenticationBasic(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public override AuthenticationDataProvider GetAuthData()
        {
            BasicAuthenticationDataProvider authenticationDataProvider = new BasicAuthenticationDataProvider(_username, _password) { };
            return authenticationDataProvider;
        }

        public override string GetAuthMethodName()
        {
            return "basic";
        }
    }
}
