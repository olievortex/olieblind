using Azure.Monitor.OpenTelemetry.AspNetCore;
using olieblind.lib.CookieConsent;
using olieblind.lib.Services;
using olieblind.web.Components;

namespace olieblind.web;

public static class Program
{
    private const string OlieBlue = "blue";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var config = builder.AddConfiguration();
        builder.AddDependencyInjection();
        builder.AddHttpClient(config);
        builder.Services.AddRazorPages();
        builder.Services.AddOpenTelemetry().UseAzureMonitor();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();

        app.Run();
    }

    private static void AddDependencyInjection(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IOlieConfig, OlieConfig>();
        builder.Services.AddScoped<ICookieConsentFrontEnd, CookieConsentFrontEnd>();
    }

    private static OlieConfig AddConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .AddEnvironmentVariables()
            .AddUserSecrets<VideoPageHomeViewComponent>()
            .Build();

        return new OlieConfig(builder.Configuration);
    }

    public static void AddHttpClient(this WebApplicationBuilder builder, IOlieConfig config)
    {
        builder.Services.AddHttpClient(OlieBlue, httpClient =>
        {
            httpClient.BaseAddress = new Uri(config.BlueUrl);
        })
            .ConfigurePrimaryHttpMessageHandler(config => new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.Brotli | System.Net.DecompressionMethods.GZip
            });
    }

    public static HttpClient GetOlieBlue(IHttpClientFactory httpClientFactory)
    {
        return httpClientFactory.CreateClient(OlieBlue);
    }
}
