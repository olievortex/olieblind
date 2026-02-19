using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using olieblind.data;
using olieblind.lib;
using olieblind.lib.Services;
using System.Runtime.InteropServices;

namespace olieblind.cli;

public class Program
{
    private static readonly InMemoryChannel _channel = new();
    private static ServiceProvider _serviceProvider = new ServiceCollection().BuildServiceProvider();

    private static async Task<int> Main(string[] args)
    {
        AddServices();

        var exitCode = 0;
        var logger = CreateLogger<Program>();
        var cts = new CancellationTokenSource();

        PosixSignalRegistration.Create(PosixSignal.SIGINT, signalContext =>
        {
            cts.Cancel();
            Console.WriteLine($"{DateTime.UtcNow:u} olieblind.cli - SIGINT detected.");
            signalContext.Cancel = true;
        });

        PosixSignalRegistration.Create(PosixSignal.SIGTERM, signalContext =>
        {
            cts.Cancel();
            Console.WriteLine($"{DateTime.UtcNow:u} olieblind.cli - SIGTERM detected.");
            signalContext.Cancel = true;
        });

        try
        {
            Console.WriteLine($"{DateTime.UtcNow:u} olieblind.cli");
            logger.LogInformation("{timeStamp} olieblind.cli", DateTime.UtcNow.ToString("u"));

            exitCode = await MainAsync(args, cts.Token);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            logger.LogError(ex, "{timeStamp} olieblind.cli error: {error}", DateTime.UtcNow.ToString("u"), ex);
            exitCode = 1;
        }
        finally
        {
            Console.WriteLine($"{DateTime.UtcNow:u} olieblind.cli exit: {exitCode}");
            logger.LogInformation("{timeStamp} olieblind.cli exit: {exitCode}", DateTime.UtcNow.ToString("u"), exitCode);

            FlushChannel();
        }

        return exitCode;
    }

    private static async Task<int> MainAsync(string[] args, CancellationToken ct)
    {
        var olieArgs = new OlieArgs(args);

        return olieArgs.Command switch
        {
            OlieArgs.CommandsEnum.DayOneMaps => await CreateService<CommandDayOneMaps>().Run(ct),
            OlieArgs.CommandsEnum.DeleteOldContent => await CreateService<CommandDeleteOldContent>().Run(ct),
            OlieArgs.CommandsEnum.DroughtMonitorVideo => await CreateService<CommandDroughtMonitorVideo>().Run(ct),
            OlieArgs.CommandsEnum.EventsDatabase => await CreateService<CommandEventsDatabase>().Run(olieArgs, ct),
            OlieArgs.CommandsEnum.EventsSpc => await CreateService<CommandEventsSpc>().Run(olieArgs, ct),
            OlieArgs.CommandsEnum.SatelliteInventory => await CreateService<CommandSatelliteInventory>().Run(olieArgs, ct),
            OlieArgs.CommandsEnum.SatelliteMarquee => await CreateService<CommandSatelliteMarquee>().Run(olieArgs, ct),
            OlieArgs.CommandsEnum.SatelliteRequest => await CreateService<CommandSatelliteRequest>().Run(ct),
            OlieArgs.CommandsEnum.SpcDayOneVideo => await CreateService<CommandSpcDayOneVideo>().Run(ct),
            OlieArgs.CommandsEnum.SpcDayTwoVideo => await CreateService<CommandSpcDayTwoVideo>().Run(ct),
            OlieArgs.CommandsEnum.SpcDayThreeVideo => await CreateService<CommandSpcDayThreeVideo>().Run(ct),
            OlieArgs.CommandsEnum.SpcMesos => await CreateService<CommandSpcMesos>().Run(olieArgs, ct),
            OlieArgs.CommandsEnum.ListVoices => await CommandListVoices.Run(ct),
            OlieArgs.CommandsEnum.LoadRadars => await CreateService<CommandLoadRadars>().Run(ct),
            _ => throw new ArgumentException($"The command {olieArgs.Command} is not implemented yet."),
        };
    }

    private static ILogger<T> CreateLogger<T>()
    {
        return _serviceProvider.GetRequiredService<ILogger<T>>();
    }

    private static T CreateService<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    private static void AddServices()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
           .AddEnvironmentVariables()
           .AddUserSecrets<Program>()
           .Build();
        var config = new OlieConfig(configuration);
        var host = new OlieHost
        {
            ServiceScopeFactory = CreateHostServices(config).Services.GetRequiredService<IServiceScopeFactory>()
        };

        services.AddLogging(builder =>
        {
            var connectionString = config.ApplicationInsightsConnectionString;

            // Only Application Insights is registered as a logger provider
            builder.AddApplicationInsights(
                configureTelemetryConfiguration: (config) => config.ConnectionString = connectionString,
                configureApplicationInsightsLoggerOptions: (options) => { }
            );
        });
        services.Configure<TelemetryConfiguration>(config => config.TelemetryChannel = _channel);
        services.AddScoped(_ => (IConfiguration)configuration);
        services.AddSingleton(_ => host);
        services.AddSingleton<IOlieConfig, OlieConfig>();
        services.AddScoped<IOlieWebService, OlieWebService>();

        #region Commands

        services.AddScoped<CommandDayOneMaps>();
        services.AddScoped<CommandDeleteOldContent>();
        services.AddScoped<CommandDroughtMonitorVideo>();
        services.AddScoped<CommandEventsDatabase>();
        services.AddScoped<CommandEventsSpc>();
        services.AddScoped<CommandSatelliteInventory>();
        services.AddScoped<CommandSatelliteMarquee>();
        services.AddScoped<CommandSatelliteRequest>();
        services.AddScoped<CommandSpcDayOneVideo>();
        services.AddScoped<CommandSpcDayTwoVideo>();
        services.AddScoped<CommandSpcDayThreeVideo>();
        services.AddScoped<CommandSpcMesos>();
        services.AddScoped<CommandListVoices>();
        services.AddScoped<CommandLoadRadars>();

        #endregion

        _serviceProvider = services.BuildServiceProvider();
    }

    private static IHost CreateHostServices(IOlieConfig config)
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                #region Entity Framework

                services.AddDbContext<MyContext>(options =>
                {
                    var connectionString = config.MySqlConnection;

                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                }, ServiceLifetime.Scoped);

                #endregion

                #region Miscellaneous

                services.AddHttpClient();
                services.AddOlieLibScopes();
                services.AddScoped<IMyRepository, MyRepository>();

                #endregion
            })
            .Build();

        return host;
    }

    private static void FlushChannel()
    {
        // Explicitly call Flush() followed by Delay, as required in console apps.
        // This ensures that even if the application terminates, telemetry is sent to the back end.
        _channel.Flush();

        var t = Task.Delay(TimeSpan.FromMilliseconds(1000));
        t.Wait();
    }
}
