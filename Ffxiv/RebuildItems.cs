using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Ffxiv
{
    public class RebuildItems
    {
        private readonly ILogger<RebuildItems> _logger;

        public RebuildItems(ILogger<RebuildItems> logger)
        {
            _logger = logger;
        }

        [FunctionName("RebuildItems")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                       ? (ActionResult)new OkObjectResult($"Hello, {name}")
                       : new BadRequestObjectResult("Please pass a name in the request body");
        }
    }
}