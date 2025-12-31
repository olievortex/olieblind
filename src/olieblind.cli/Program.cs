using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using olieblind.data;
using olieblind.lib.DroughtMonitor;
using olieblind.lib.ForecastModels;
using olieblind.lib.Maintenance;
using olieblind.lib.Processes;
using olieblind.lib.Services;
using olieblind.lib.Services.Speech;
using olieblind.lib.StormPredictionCenter.Outlooks;
using olieblind.lib.Video;

namespace olieblind.cli;

public class Program
{
    private static readonly InMemoryChannel _channel = new();
    private static ServiceProvider _serviceProvider = new ServiceCollection().BuildServiceProvider();

    private static int Main(string[] args)
    {
        var olieConfig = AddConfiguration();
        CreateService(olieConfig);

        var exitCode = 0;
        var logger = CreateLogger<Program>();
        var ct = CancellationToken.None;

        try
        {
            Console.WriteLine($"{DateTime.UtcNow:u} olieblind.cli");
            logger.LogInformation("{timeStamp} olieblind.cli", DateTime.UtcNow.ToString("u"));

            var t = MainAsync(args, ct);
            t.Wait();
            exitCode = t.Result;
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
        var configService = CreateService<IOlieConfig>();
        var droughtProcess = CreateService<ICreateDroughtMonitorVideoProcess>();
        var spcProcess = CreateService<ICreateSpcOutlookVideoProcess>();
        var mapProcess = CreateService<ICreateDayOneMapsProcess>();
        var deleteProcess = CreateService<IDeleteOldContentProcess>();

        return olieArgs.Command switch
        {
            OlieArgs.CommandsEnum.DayOneMaps => await new CommandDayOneMaps(
                mapProcess, CreateLogger<CommandDayOneMaps>()).Run(ct),
            OlieArgs.CommandsEnum.DeleteOldContent => await new CommandDeleteOldContent(
                deleteProcess, configService, CreateLogger<CommandDeleteOldContent>()).Run(ct),
            OlieArgs.CommandsEnum.DroughtMonitorVideo => await new CommandDroughtMonitorVideo(
                droughtProcess, configService, CreateLogger<CommandDroughtMonitorVideo>()).Run(ct),
            OlieArgs.CommandsEnum.SpcDayOneVideo => await new CommandSpcDayOneVideo(
                spcProcess, configService, CreateLogger<CommandSpcDayOneVideo>()).Run(ct),
            OlieArgs.CommandsEnum.SpcDayTwoVideo => await new CommandSpcDayTwoVideo(
                spcProcess, configService, CreateLogger<CommandSpcDayTwoVideo>()).Run(ct),
            OlieArgs.CommandsEnum.SpcDayThreeVideo => await new CommandSpcDayThreeVideo(
                spcProcess, configService, CreateLogger<CommandSpcDayThreeVideo>()).Run(ct),
            OlieArgs.CommandsEnum.ListVoices => await CommandListVoices.Run(ct),
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

    private static IConfigurationRoot AddConfiguration()
    {
        return new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>()
            .Build();
    }

    private static void CreateService(IConfiguration configuration)
    {
        var olieConfig = new OlieConfig(configuration);
        var services = new ServiceCollection();

        #region Logging

        services.AddLogging(builder =>
        {
            var connectionString = olieConfig.ApplicationInsightsConnectionString;

            // Only Application Insights is registered as a logger provider
            builder.AddApplicationInsights(
                configureTelemetryConfiguration: (config) => config.ConnectionString = connectionString,
                configureApplicationInsightsLoggerOptions: (options) => { }
            );
        });

        #endregion

        #region Entity Framework

        services.AddDbContext<MyContext>(options =>
        {
            var connectionString = olieConfig.MySqlConnection;

            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }, ServiceLifetime.Scoped);

        #endregion

        #region Miscellaneous

        services.Configure<TelemetryConfiguration>(config => config.TelemetryChannel = _channel);
        services.AddScoped(_ => configuration);
        services.AddHttpClient();

        #endregion

        #region Common Dependencies

        services.AddScoped<IOlieConfig, OlieConfig>();
        services.AddScoped<IMyRepository, MyRepository>();
        services.AddScoped<IOlieWebService, OlieWebService>();
        services.AddScoped<IOlieImageService, OlieImageService>();
        services.AddScoped<IOlieSpeechService, GoogleSpeechService>();
        services.AddScoped<ICommonProcess, CommonProcess>();

        #endregion

        #region CreateMaps Dependencies

        services.AddScoped<ICreateDayOneMapsProcess, CreateDayOneMapsProcess>();
        services.AddScoped<INorthAmericanMesoscale, NorthAmericanMesoscale>();

        #endregion

        #region SpcVideo Dependencies

        services.AddScoped<ICreateSpcOutlookVideoProcess, CreateSpcOutlookVideoProcess>();
        services.AddScoped<IOutlookProduct, OutlookProduct>();
        services.AddScoped<IOutlookProductParsing, OutlookProductParsing>();
        services.AddScoped<IOutlookProductScript, OutlookProductScript>();

        #endregion

        #region DroughtMonitor Dependencies

        services.AddScoped<ICreateDroughtMonitorVideoProcess, CreateDroughtMonitorVideoProcess>();
        services.AddScoped<IDroughtMonitor, DroughtMonitor>();
        services.AddScoped<IDroughtMonitorScripting, DroughtMonitorScripting>();

        #endregion

        #region DeleteContent Dependencies

        services.AddScoped<IDeleteOldContentProcess, DeleteOldContentProcess>();
        services.AddScoped<IMySqlMaintenance, MySqlMaintenance>();

        #endregion

        _serviceProvider = services.BuildServiceProvider();
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
