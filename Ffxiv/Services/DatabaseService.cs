using System.Threading.Tasks;
using Ffxiv.Common;
using Ffxiv.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Ffxiv.Services
{
    public class DatabaseService
    {
        private readonly Config _config;
        private readonly CosmosClient _cosmosClient;

        public DatabaseService(IOptions<Config> options, CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            _config = options.Value;
        }

        public async Task UpsertItem(Item item)
        {
            var container = _cosmosClient.GetContainer(_config.DatabaseId, _config.ContainerId);

            await container.UpsertItemAsync(item);
        }
    }
}