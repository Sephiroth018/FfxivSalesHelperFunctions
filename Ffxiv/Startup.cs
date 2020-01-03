using Ffxiv.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ffxiv
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<Config>()
                   .Configure<IConfiguration>((settings, configuration) => { configuration.Bind(settings); });
            builder.Services.AddTransient((s) => new FfxivService(s.GetService<Config>()));
            builder.Services.AddTransient(s => new DatabaseService());
        }
    }
}