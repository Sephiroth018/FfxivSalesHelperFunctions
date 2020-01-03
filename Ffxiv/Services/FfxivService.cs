using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Flurl;
using Flurl.Http;
using Polly;
using Polly.Wrap;

namespace Ffxiv.Services
{
    public class FfxivService
    {
        private const string ItemPath = "Item";
        private const string RecipePath = "Recipe";
        private const string ApiKeyParamName = "private_key";

        private readonly IConfig _config;
        private readonly AsyncPolicyWrap<HttpResponseMessage> _retryStrategy;

        public FfxivService(IConfig config)
        {
            _config = config;
            _retryStrategy = GetResiliencyStrategy();
        }

        private Url GetBaseUrl()
        {
            return _config.FfxivBaseUrl.SetQueryParam(ApiKeyParamName, _config.FfxivApiKey);
        }

        private async Task<dynamic> GetData(string type, string id)
        {
            var response = await _retryStrategy.ExecuteAsync(() => GetBaseUrl().AppendPathSegments(type, id).AllowAnyHttpStatus().GetAsync());
            return await Task.FromResult(response).ReceiveJson();
        }

        public async Task<dynamic> GetItem(string id)
        {
            return await GetData(ItemPath, id);
        }

        public async Task<dynamic> GetRecipe(string id)
        {
            return await GetData(RecipePath, id);
        }

        /// <summary>
        ///     Creates a Polly-based resiliency strategy that helps deal with transient faults when communicating
        ///     with the external (downstream) Computer Vision API service.
        /// </summary>
        /// <returns></returns>
        private AsyncPolicyWrap<HttpResponseMessage> GetResiliencyStrategy()
        {
            // Retry when these status codes are encountered.
            HttpStatusCode[] httpStatusCodesWorthRetrying =
            {
                HttpStatusCode.InternalServerError, // 500
                HttpStatusCode.BadGateway, // 502
                HttpStatusCode.GatewayTimeout // 504
            };

            // Define our waitAndRetry policy: retry n times with an exponential backoff in case the Computer Vision API throttles us for too many requests.
            var waitAndRetryPolicy = Policy
                                     .HandleResult<HttpResponseMessage>(e => e.StatusCode == HttpStatusCode.ServiceUnavailable || e.StatusCode == HttpStatusCode.TooManyRequests)
                                     .WaitAndRetryAsync(10, // Retry 10 times with a delay between retries before ultimately giving up
                                                        //attempt => TimeSpan.FromSeconds(0.25 * Math.Pow(2, attempt)) // Back off!  2, 4, 8, 16 etc times 1/4-second
                                                        attempt => TimeSpan.FromSeconds(1) // Wait 1 seconds between retries
                                                       );

            // Define our first CircuitBreaker policy: Break if the action fails 4 times in a row.
            // This is designed to handle Exceptions from the Computer Vision API, as well as
            // a number of recoverable status messages, such as 500, 502, and 504.
            var circuitBreakerPolicyForRecoverable = Policy
                                                     .Handle<HttpResponseException>()
                                                     .OrResult<HttpResponseMessage>(r => httpStatusCodesWorthRetrying.Contains(r.StatusCode))
                                                     .CircuitBreakerAsync(
                                                                          3,
                                                                          TimeSpan.FromSeconds(3)
                                                                         );

            // Combine the waitAndRetryPolicy and circuit breaker policy into a PolicyWrap. This defines our resiliency strategy.
            return Policy.WrapAsync(waitAndRetryPolicy, circuitBreakerPolicyForRecoverable);
        }
    }
}