using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ffxiv.Common
{
    public class PolicyHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Policies.GetResiliencyStrategy().ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
        }
    }
}