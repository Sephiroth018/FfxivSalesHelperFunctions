// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System.Linq;
using System.Threading.Tasks;
using Ffxiv.Services;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

namespace Ffxiv
{
    public class ItemAdded
    {
        private readonly ItemService _itemService;
        private readonly ILogger<ItemAdded> _logger;

        public ItemAdded(ItemService itemService, ILogger<ItemAdded> logger)
        {
            _itemService = itemService;
            _logger = logger;
        }

        [FunctionName("ItemAdded")]
        public async Task Run([EventGridTrigger] EventGridEvent eventGridEvent)
        {
            _logger.LogInformation(eventGridEvent.Data.ToString());

            dynamic data = eventGridEvent.Data;
            string url = data.url.ToString();
            var id = url.Split('/').Last();
            _logger.LogInformation($"Adding item with id {id}");

            await _itemService.AddItemToDatabase(id, _logger);
        }
    }
}