// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using Ffxiv.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

namespace Ffxiv
{
    public class ItemAdded
    {
        private readonly FfxivService _ffxivService;
        private readonly DatabaseService _databaseService;

        public ItemAdded(FfxivService ffxivService, DatabaseService databaseService)
        {
            _ffxivService = ffxivService;
            _databaseService = databaseService;
        }

        [FunctionName("ItemAdded")]
        public void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());
        }
    }
}
