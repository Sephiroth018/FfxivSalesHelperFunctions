using System.Threading.Tasks;
using Ffxiv.Common;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace Ffxiv.Services
{
    public class FfxivService : IFfxivService
    {
        private const string ItemPath = "Item";
        private const string RecipePath = "Recipe";
        private const string ApiKeyParamName = "private_key";

        private readonly Config _config;

        public FfxivService(IOptions<Config> config)
        {
            _config = config.Value;
        }

        public async Task<dynamic> GetItem(string id)
        {
            return await GetData(ItemPath, id);
        }

        public async Task<dynamic> GetRecipe(string id)
        {
            return await GetData(RecipePath, id);
        }

        private Url GetBaseUrl()
        {
            return _config.FfxivBaseUrl.SetQueryParam(ApiKeyParamName, _config.FfxivApiKey);
        }

        private async Task<dynamic> GetData(string type, string id)
        {
            return await GetBaseUrl().AppendPathSegments(type, id).GetJsonAsync();
        }
    }
}