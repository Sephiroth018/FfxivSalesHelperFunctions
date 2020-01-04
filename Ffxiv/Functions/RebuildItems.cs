using System;
using System.Threading.Tasks;
using System.Web.Http;
using Ffxiv.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Ffxiv.Functions
{
    public class RebuildItems
    {
        private readonly ItemService _itemService;
        private readonly ILogger<RebuildItems> _logger;
        private readonly StorageService _storageService;

        public RebuildItems(ILogger<RebuildItems> logger, ItemService itemService, StorageService storageService)
        {
            _logger = logger;
            _itemService = itemService;
            _storageService = storageService;
        }

        [FunctionName("RebuildItems")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req)
        {
            try
            {
                var itemIds = _storageService.GetAllItemBlobNames();

                await _itemService.RebuildData(itemIds, _logger);

                _logger.LogInformation(string.Join(',', itemIds));

                return new OkObjectResult(string.Empty);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new ExceptionResult(e, false);
            }
        }
    }
}