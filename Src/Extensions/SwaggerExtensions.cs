﻿using APIGateway.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace APIGateway.Extensions;

public static class SwaggerExtensions
{
    /// <summary>
    /// Add Swagger services.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen();
        services.AddSwaggerForOcelot(configuration);
    }

    /// <summary>
    /// Enable middleware to serve Swagger.
    /// </summary>
    /// <param name="app"></param>
    public static void UseOcelotSwagger(this IApplicationBuilder app, IConfiguration configuration)
    {
        // Get gateway configuration
        var gatewayConfig = new GatewayConfiguration();
        configuration.GetSection(nameof(GatewayConfiguration)).Bind(gatewayConfig);

        // Enable middleware to serve generated Swagger as a JSON endpoint
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "docs/{documentname}/swagger.json";
        });

        // Add Swagger generator for downstream services
        app.UseSwaggerForOcelotUI(opt =>
        {
            // Default
            opt.DocExpansion(DocExpansion.None);
            opt.EnableFilter();
            opt.DisplayRequestDuration();
            if (!string.IsNullOrEmpty(gatewayConfig.OAuthClientId))
                opt.OAuthClientId(gatewayConfig.OAuthClientId);
            if (!string.IsNullOrEmpty(gatewayConfig.OAuthClientSecret))
                opt.OAuthClientSecret(gatewayConfig.OAuthClientSecret);
            opt.OAuthUsePkce();

            // Ocelot
            opt.RoutePrefix = "docs";
            opt.PathToSwaggerGenerator = "/docs/swagger";
            opt.DownstreamSwaggerEndPointBasePath = "/docs/swagger";
        });
    }
}