using System.Net.Http;
using Flurl.Http.Configuration;

namespace Ffxiv.Common
{
    public class PollyHttpClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new PolicyHandler
                   {
                       InnerHandler = base.CreateMessageHandler()
                   };
        }
    }
}