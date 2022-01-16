using RabbitMQ.Client;
using System;
using System.Text;

namespace MQMiddleware.AuthUtil
{
    public class AliyunMechanism : AuthMechanism
    {
        public byte[] handleChallenge(byte[] challenge, IConnectionFactory factory)
        {
            if (factory is ConnectionFactory)
            {
                ConnectionFactory cf = factory as ConnectionFactory;
                return Encoding.UTF8.GetBytes("\0" + getUserName(cf) + "\0" + AliyunUtils.getPassword(cf.Password));
            }
            else
            {
                throw new InvalidCastException("need ConnectionFactory");
            }
        }

        private string getUserName(ConnectionFactory cf)
        {
            string ownerResourceId;
            try
            {
                string[] sArray = cf.HostName.Split('.');
                ownerResourceId = sArray[0];
            }
            catch (Exception e)
            {
                throw new InvalidProgramException("hostName invalid");
            }
            // Console.WriteLine(ownerResourceId);
            return AliyunUtils.getUserName(cf.UserName, ownerResourceId);
        }
    }
}
