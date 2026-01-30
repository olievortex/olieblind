using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.EntityFrameworkCore;
using olieblind.api.Endpoints;
using olieblind.data;
using olieblind.lib.CookieConsent;
using olieblind.lib.ForecastModels;
using olieblind.lib.Models;
using olieblind.lib.Satellite;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Services;
using olieblind.lib.StormEvents;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.Video;
using OpenTelemetry.Trace;

namespace olieblind.api;

public static class Program
{
    private const string AllowedCors = "_allowedCors";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var config = builder.AddConfiguration();
        builder.AddResponseMiddleware();
        builder.AddCors(config);
        builder.Services.AddAuthorization();
        builder.Services.AddHttpClient();
        builder.AddDependencyInjection();
        builder.AddEntityFramework(config);
        builder.Services.AddOpenTelemetry().UseAzureMonitor().WithTracing(builder =>
        {
            builder.AddSqlClientInstrumentation();
        });

        var app = builder.Build();
        app.UseCors(AllowedCors);
        app.UseOutputCache();
        app.UseAuthorization();
        app.UseResponseCompression();
        app.MapEndpoints();
        app.Run();
    }

    private static void AddDependencyInjection(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IOlieConfig, OlieConfig>();
        builder.Services.AddScoped<IOlieWebService, OlieWebService>();
        builder.Services.AddScoped<IVideoBusiness, VideoBusiness>();
        builder.Services.AddScoped<IModelForecastBusiness, ModelForecastBusiness>();
        builder.Services.AddScoped<ICookieConsentBusiness, CookieConsentBusiness>();
        builder.Services.AddScoped<IStormEventsBusiness, StormEventsBusiness>();
        builder.Services.AddScoped<IStormEventsSource, StormEventsSource>();
        builder.Services.AddScoped<ISatelliteRequestBusiness, SatelliteRequestBusiness>();
        builder.Services.AddScoped<ISatelliteRequestSource, SatelliteRequestSource>();
    }

    private static void MapEndpoints(this WebApplication app)
    {
        app.MapVideoEndpoints();
        app.MapUserEndpoints();
        app.MapModelForecastEndpoints();
        app.MapEventsEndpoints();
        app.MapSatelliteEndpoints();
    }

    private static void AddEntityFramework(this WebApplicationBuilder builder, IOlieConfig config)
    {
        var serverVersion = ServerVersion.AutoDetect(config.MySqlConnection);
        void mySqlOptions(DbContextOptionsBuilder options)
        {
            options.UseMySql(config.MySqlConnection, serverVersion);
        }

        builder.Services.AddDbContext<MyContext>(mySqlOptions);
        builder.Services.AddScoped<IMyRepository, MyRepository>();
    }

    private static OlieConfig AddConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .AddEnvironmentVariables()
            .AddUserSecrets<ProductVideoModel>()
            .Build();

        return new OlieConfig(builder.Configuration);
    }

    private static void AddResponseMiddleware(this WebApplicationBuilder builder)
    {
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.MimeTypes = ["text/css", "text/javascript", "text/html", "application/json"];
        });
        builder.Services.AddOutputCache(options =>
        {
            options.AddBasePolicy(b =>
                b.Expire(TimeSpan.FromSeconds(120)));
        });
    }

    private static void AddCors(this WebApplicationBuilder builder, IOlieConfig config)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: AllowedCors,
                              policy =>
                              {
                                  policy.WithOrigins(config.BlueCors);
                                  policy.WithMethods(["OPTIONS", "HEAD", "GET", "POST"]);
                                  policy.WithHeaders(["content-type"]);
                              });
        });
    }

    public static ServiceBusAdministrationClient ServiceBusAdministrationClient(this IOlieConfig config)
    {
        return new ServiceBusAdministrationClient(config.AwsServiceBus, config.Credential);
    }

    public static ServiceBusSender ServiceBusSender(this IOlieConfig config)
    {
        var client = new Azure.Messaging.ServiceBus.ServiceBusClient(config.AwsServiceBus, config.Credential);
        return client.CreateSender(config.SatelliteRequestQueueName);
    }
}
