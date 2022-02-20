using APIGateway;
using MicroAutomation.Log.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MMLib.SwaggerForOcelot.DependencyInjection;
using System;
using System.IO;
using System.Reflection;

namespace ApiGateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = CreateHostBuilder(args);
        var host = builder.Build();
        host.Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        //
        var builder = Host.CreateDefaultBuilder(args);
        IConfiguration configuration = null;

        // Set current directory
        var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
        var pathContextRoot = new FileInfo(location.AbsolutePath).Directory.FullName;
        builder.UseContentRoot(pathContextRoot);
        Directory.SetCurrentDirectory(pathContextRoot);

        // Add serilog implementation.
        builder.UseCustomSerilog();

        //
        builder.ConfigureAppConfiguration((hostContext, config) =>
        {
            // Retrieve the name of the environment.
            var aspnetcore = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var dotnetcore = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            var environmentName = string.IsNullOrWhiteSpace(aspnetcore) ? dotnetcore : aspnetcore;
            if (string.IsNullOrWhiteSpace(environmentName))
                environmentName = "Production";

            // Define the configuration builder.
            config.SetBasePath(pathContextRoot);
            config.AddJsonFile("appsettings.json", optional: false);
            config.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
            config.AddEnvironmentVariables();
            config.AddCommandLine(args);

            // Add ocelot configuration
            config.AddOcelotWithSwaggerSupport((o) =>
            {
                o.Folder = "Configurations";
            });

            configuration = config.Build();
        });

        builder.ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });

        return builder;
    }
}