using Microsoft.Extensions.Hosting;
using Serilog;

namespace APIGateway.Extensions;

public static class LoggerExtensions
{
    /// <summary>
    /// Add serilog with custom implementation.
    /// </summary>
    /// <param name="hostBuilder"></param>
    public static void UseCustomSerilog(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((hostContext, services, loggerConfig) =>
        {
            loggerConfig.ConfigureSerilog(hostContext);
        });
    }

    /// <summary>
    /// Add serilog with custom implementation.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="hostContext"></param>
    public static void ConfigureSerilog(
        this LoggerConfiguration logger,
        HostBuilderContext hostContext)
    {
        // Add user configuration.
        var config = logger.ReadFrom.Configuration(hostContext.Configuration);

        // Add console configuration.
        config.WriteTo.Async(opt => opt.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss.fff}][{Level:u4}][{ThreadId}][{SourceContext}] {Message}{NewLine}{Exception}"),
            bufferSize: 10000,
            blockWhenFull: true);

        // Add file configuration.
        config.WriteTo.File(
            path: "Logs\\log-.txt",
            fileSizeLimitBytes: 5242880,
            rollingInterval: RollingInterval.Day,
            rollOnFileSizeLimit: true,
            retainedFileCountLimit: 30);
    }
}