using Microsoft.Extensions.DependencyInjection;
using olieblind.lib.DroughtMonitor;
using olieblind.lib.ForecastModels;
using olieblind.lib.Maintenance;
using olieblind.lib.Processes;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Radar;
using olieblind.lib.Radar.Interfaces;
using olieblind.lib.Satellite;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Services;
using olieblind.lib.Services.Speech;
using olieblind.lib.StormEvents;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.StormPredictionCenter.Interfaces;
using olieblind.lib.StormPredictionCenter.Mesos;
using olieblind.lib.StormPredictionCenter.Outlooks;
using olieblind.lib.Video;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace olieblind.lib;

public static class OlieCommon
{
    public static string CreateLocalTmpPath(string extension)
    {
        var tmpPath = Path.GetTempPath();
        var guid = Guid.NewGuid().ToString();

        return $"{tmpPath}{guid}{extension}";
    }

    public static void DeleteTempFiles(IOlieWebService ows, List<string> fileList, params string[] additionalFiles)
    {
        var combined = fileList.Select(s => s).ToList();
        combined.AddRange(additionalFiles);

        foreach (var path in combined) ows.FileDelete(path);
    }

    public static int GetCurrentTimeZoneOffset(DateTime effectiveDate)
    {
        return IsDaylightSavingsTime(effectiveDate) ? -5 : -6;
    }

    public static bool IsDaylightSavingsTime(DateTime effectiveDate)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        return tz.IsDaylightSavingTime(effectiveDate);
    }

    public static DateTime ParseSpcEffectiveDate(string line)
    {
        // 0101 PM CDT Fri Mar 14 2025
        if (string.IsNullOrWhiteSpace(line)) throw new ArgumentNullException(nameof(line));

        var parts = line.Split(' ');
        var hour = int.Parse(parts[0][..2]);
        var minute = int.Parse(parts[0][2..]);
        var year = int.Parse(parts[6]);
        var month = DateTime.ParseExact(parts[4], "MMM", CultureInfo.CurrentCulture).Month;
        var day = int.Parse(parts[5]);
        var tzOffset = TimeZoneToOffset(parts[2]);
        var pmOffset = parts[1] == "PM" ? 12 : 0;

        if (hour == 12 && parts[1] == "AM") hour = 0;
        if (hour == 12 && parts[1] == "PM") pmOffset = 0;

        var result = new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc);
        result = result.AddHours(-tzOffset);
        result = result.AddHours(pmOffset);

        return result;
    }

    public static int TimeZoneToOffset(string tz)
    {
        return tz.ToUpper() switch
        {
            "UTC" => 0,
            "EDT" => -4,
            "EST" => -5,
            "CDT" => -5,
            "CST" => -6,
            "MDT" => -6,
            "MST" => -7,
            "PDT" => -7,
            "PST" => -8,
            _ => throw new ApplicationException($"Unknown time zone: {tz}")
        };
    }

    [ExcludeFromCodeCoverage]
    public static void AddOlieLibScopes(this ServiceCollection services)
    {
        #region Services

        services.AddScoped<IOlieConfig, OlieConfig>();
        services.AddScoped<IOlieWebService, OlieWebService>();
        services.AddScoped<IOlieImageService, OlieImageService>();
        services.AddScoped<IOlieSpeechService, GoogleSpeechService>();
        services.AddScoped<ICommonProcess, CommonProcess>();

        #endregion

        #region Business Dependencies

        services.AddScoped<IDailySummaryBusiness, DailySummaryBusiness>();
        services.AddScoped<IDatabaseBusiness, DatabaseBusiness>();
        services.AddScoped<IDatabaseProcess, DatabaseProcess>();
        services.AddScoped<IDroughtMonitor, DroughtMonitor.DroughtMonitor>();
        services.AddScoped<IDroughtMonitorScripting, DroughtMonitorScripting>();
        services.AddScoped<IMesoProductParsing, MesoProductParsing>();
        services.AddScoped<IMesoProductProcess, MesoProductProcess>();
        services.AddScoped<IMesoProductSource, MesoProductSource>();
        services.AddScoped<IMySqlMaintenance, MySqlMaintenance>();
        services.AddScoped<INorthAmericanMesoscale, NorthAmericanMesoscale>();
        services.AddScoped<IOutlookProduct, OutlookProduct>();
        services.AddScoped<IOutlookProductParsing, OutlookProductParsing>();
        services.AddScoped<IOutlookProductScript, OutlookProductScript>();
        services.AddScoped<IRadarBusiness, RadarBusiness>();
        services.AddScoped<IRadarSource, RadarSource>();
        services.AddScoped<ISatelliteIemBusiness, SatelliteIemBusiness>();
        services.AddScoped<ISatelliteAwsBusiness, SatelliteAwsBusiness>();
        services.AddScoped<ISatelliteAwsSource, SatelliteAwsSource>();
        services.AddScoped<ISatelliteProcess, SatelliteProcess>();
        services.AddScoped<ISatelliteSource, SatelliteSource>();
        services.AddScoped<ISpcBusiness, SpcBusiness>();
        services.AddScoped<ISpcProcess, SpcProcess>();
        services.AddScoped<ISpcSource, SpcSource>();

        #endregion

        #region Processes

        services.AddScoped<ICreateDayOneMapsProcess, CreateDayOneMapsProcess>();
        services.AddScoped<ICreateDroughtMonitorVideoProcess, CreateDroughtMonitorVideoProcess>();
        services.AddScoped<ICreateSpcOutlookVideoProcess, CreateSpcOutlookVideoProcess>();
        services.AddScoped<IDeleteOldContentProcess, DeleteOldContentProcess>();
        services.AddScoped<IImportStormEventsDatabaseProcess, ImportStormEventsDatabaseProcess>();
        services.AddScoped<IImportStormEventsSpcProcess, ImportStormEventsSpcProcess>();
        services.AddScoped<ISatelliteInventoryProcess, SatelliteInventoryProcess>();
        services.AddScoped<ISpcMesosProcess, SpcMesosProcess>();

        #endregion
    }
}