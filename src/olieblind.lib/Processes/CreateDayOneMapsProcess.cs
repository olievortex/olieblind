using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.ForecastModels;
using olieblind.lib.Processes.Interfaces;
using olieblind.lib.Services;

namespace olieblind.lib.Processes;

public class CreateDayOneMapsProcess(
    IMyRepository repo,
    INorthAmericanMesoscale nam,
    IOlieConfig config,
    IOlieWebService ows) : ICreateDayOneMapsProcess
{
    private const string GribExtension = ".grib2";
    private const string PythonScript = "prold2.py";
    public DateOnly EffectiveDateOnly { get; set; }
    public int EffectiveHour { get; set; }
    public int ForecastHour { get; set; }
    public string DestinationBaseUrl => nam.GetFolder(EffectiveDateOnly, config.BaseVideoUrl);
    public DateTime EffectiveDateTime => new(EffectiveDateOnly, new TimeOnly(EffectiveHour, 0));
    public string FilePrefix => $"nam{EffectiveHour:00}{ForecastHour:00}";
    public string ImageFolder => nam.GetFolder(EffectiveDateOnly, config.ModelForecastPath);
    public string SourceUrl => nam.GetNcepUrl(EffectiveDateOnly, EffectiveHour, ForecastHour);

    public async Task Run(DateOnly effectiveDateOnly, int effectiveHour, int forecastHour, CancellationToken ct)
    {
        EffectiveDateOnly = effectiveDateOnly;
        EffectiveHour = effectiveHour;
        ForecastHour = forecastHour;

        await Do(ct);
    }

    public async Task Do(CancellationToken ct)
    {
        // Acquire a product
        var grib = OlieCommon.CreateLocalTmpPath(GribExtension);
        await ows.ApiDownloadBytes(grib, SourceUrl, ct);

        // Shell out to Purple
        _ = await ows.Shell(config, config.BrownCmdPath, $"{PythonScript} \"{grib}\" \"{ImageFolder}\" {FilePrefix}", ct);

        // Update database
        await AddToDatabaseAsync(ct);

        // Cleanup
        OlieCommon.DeleteTempFiles(ows, [grib]);
    }

    public async Task AddToDatabaseAsync(CancellationToken ct)
    {
        var map = new ProductMapEntity
        {
            Effective = EffectiveDateTime,
            ForecastHour = ForecastHour,
            IsActive = true,
            Timestamp = DateTime.UtcNow,
            ProductId = 1,
            SourceUrl = SourceUrl
        };

        await repo.ProductMapCreate(map, ct);

        foreach (var product in Products.StandardProducts)
        {
            var filename = $"{FilePrefix}_{product.File}_1.png";

            var item = new ProductMapItemEntity
            {
                GeographyId = 1,
                IsActive = true,
                LocalPath = $"{ImageFolder}/{filename}",
                ParameterId = product.Id,
                ProductMapId = map.Id,
                Timestamp = DateTime.UtcNow,
                Title = product.Title,
                Url = $"{DestinationBaseUrl}/{filename}"
            };

            await repo.ProductMapItemCreate(item, ct);
        }
    }
}