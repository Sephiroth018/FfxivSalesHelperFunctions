// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

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
        private readonly IFfxivService _ffxivService;
        private readonly ILogger<ItemAdded> _logger;

        public ItemAdded(IFfxivService ffxivService, ILogger<ItemAdded> logger)
        {
            _ffxivService = ffxivService;
            _logger = logger;
        }

        [FunctionName("ItemAdded")]
        public async Task Run([EventGridTrigger] EventGridEvent eventGridEvent)
        {
            _logger.LogInformation(eventGridEvent.Data.ToString());
            var item = await _ffxivService.GetItem("28471");
            _logger.LogInformation($"loaded item {item.ID} {item.Name}");
        }
    }
}