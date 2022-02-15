using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MMLib.Ocelot.Provider.AppConfiguration;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Polly;

namespace APIGateway.Extensions;

public static class OcelotExtensions
{
    public static void AddOcelot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOcelot()
            .AddAppConfiguration()
            .AddPolly();
    }
}