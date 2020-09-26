namespace NetPro.Authentication
{
    public class AuthenticationOption
    {
        public bool Enabled { get; set; }

        public string Secret { get; set; }

        public bool Issuer { get; set; }

        public int AccessTokenExpired { get; set; }

        public int RefreshTokenExpired { get; set; }
    }
}
