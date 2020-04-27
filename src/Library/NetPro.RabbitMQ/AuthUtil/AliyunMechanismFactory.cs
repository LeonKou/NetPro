using RabbitMQ.Client;

namespace MQMiddleware.AuthUtil
{
    public class AliyunMechanismFactory : AuthMechanismFactory
    {
        /// <summary>
        /// The name of the authentication mechanism, as negotiated on the wire.
        /// </summary>
        public string Name
        {
            get { return "PLAIN"; }
        }

        /// <summary>
        /// Return a new authentication mechanism implementation.
        /// </summary>
        public AuthMechanism GetInstance()
        {
            return new AliyunMechanism();
        }
    }
}
