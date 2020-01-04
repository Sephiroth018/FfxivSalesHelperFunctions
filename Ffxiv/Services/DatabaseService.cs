using System.Threading.Tasks;
using Ffxiv.Common;
using Ffxiv.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
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
            var container = GetItemContainer();

            await container.UpsertItemAsync(item);
        }

        private Container GetItemContainer()
        {
            var container = _cosmosClient.GetContainer(_config.DatabaseId, _config.ContainerId);
            return container;
        }

        public async Task RemoveAll()
        {
            var container = GetItemContainer();
            //var iterator = _cosmosClient.GetDatabaseQueryIterator<Item>("SELECT * from items");

            var iterator = container.GetItemLinqQueryable<Item>().ToFeedIterator();

            while (iterator.HasMoreResults)
            {
                foreach (var doc in await iterator.ReadNextAsync())
                {
                    await container.DeleteItemAsync<Item>(doc.Id.ToString(), new PartitionKey(doc.ItemKind.Name));
                }
            }
        }
    }
}