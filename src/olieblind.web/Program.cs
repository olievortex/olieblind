using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using olieblind.lib.CookieConsent;
using olieblind.lib.Services;
using olieblind.web.Components;
using System.Net;

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
        builder.AddOidcAuthentication();
        builder.AddReverseProxySupport();

        var app = builder.Build();
        app.UseForwardedHeaders();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseRouting();
        app.UseAuthentication();
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

    private static void AddOidcAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.Scope.Add("profile");
                options.Scope.Add("email");
            });
    }

    private static void AddReverseProxySupport(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
            options.KnownProxies.Add(IPAddress.Parse("::1"));
        });
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
