using Azure.Storage.Blobs;
using Ffxiv;
using Ffxiv.Common;
using Ffxiv.Services;
using Flurl.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Ffxiv
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<Config>()
                   .Configure<IConfiguration>((settings, configuration) => { configuration.Bind(settings); });
            builder.Services.AddSingleton<IFfxivService, FfxivService>();
            builder.Services.AddSingleton(s => new CosmosClient(s.GetService<IOptions<Config>>().Value.DatabaseConnection));
            builder.Services.AddTransient(s => new BlobServiceClient(s.GetService<IOptions<Config>>().Value.BlobConnection));
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddTransient<ItemService>();
            builder.Services.AddTransient<StorageService>();

            FlurlHttp.Configure(settings => settings.HttpClientFactory = new PollyHttpClientFactory());
        }
    }
}