using System.Collections.Specialized;
using System.Configuration;
using System.Runtime.Remoting;
using omg.org.CosNaming;

namespace Chat.Model
{
    public sealed class ConnNaming
    {
        public static readonly ConnNaming Instance;

        public static NamingContext Service { get; set; }

        static ConnNaming()
        {
            Instance = new ConnNaming();

            NameValueCollection serverConfig = ConfigurationManager.AppSettings;

            string nameServiceUrl = "corbaloc::" + serverConfig.Get("NameServerHost") +
                                    ":" + serverConfig.Get("NameServerPort") + "/NameService";

            Service = (NamingContext)RemotingServices.Connect(typeof(NamingContext), nameServiceUrl);
        }
    }
}
