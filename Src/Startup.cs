using APIGateway.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.Middleware;

namespace APIGateway;

public class Startup
{
    /// <summary>
    /// Represents a set of key/value application configuration properties.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Startup
    /// </summary>
    /// <param name="configuration">Configuration</param>
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <summary>
    /// Configuration service.
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        // Configure HSTS
        // The default HSTS value is 90 days.
        // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        // https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?WT.mc_id=DT-MVP-5003978#http-strict-transport-security-protocol-hsts
        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Strict-Transport-Security
        services.AddHsts();

        // API Gateway
        services.AddOcelot(Configuration);

        // Frameworks
        services.AddControllers();
        services.AddSwagger(Configuration);
    }

    /// <summary>
    /// Configure application.
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <param name="env">Web host environment</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Checks if the current host environment name is development.
        if (env.IsDevelopment() || env.EnvironmentName == "LocalDevelopment")
            app.UseDeveloperExceptionPage();
        else
            app.UseHsts();

        // Enables routing capabilities.
        app.UseRouting();

        // Redirect to swagger documentation
        var option = new RewriteOptions();
        option.AddRedirect("^$", "docs");
        app.UseRewriter(option);

        // Gateway
        app.UseOcelotSwagger(Configuration);
        app.UseOcelot().Wait();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}