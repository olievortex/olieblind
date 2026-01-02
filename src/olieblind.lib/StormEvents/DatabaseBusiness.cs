using Azure.Storage.Blobs;
using CsvHelper;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Mapping;
using olieblind.lib.Services;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.StormEvents.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace olieblind.lib.StormEvents;

public partial class DatabaseBusiness(IOlieWebService ows, IMyRepository repo) : IDatabaseBusiness
{
    private const string StormEventsUrl = "https://www.ncei.noaa.gov/pub/data/swdi/stormevents/csvfiles/";

    #region Detail

    public async Task AddDailyDetailToCosmos(List<DailyDetailModel> models, string sourceFk, CancellationToken ct)
    {
        var entities = EntityMapping.ToStormEventsDailyDetail(models, sourceFk);
        await repo.StormEventsDailyDetailCreate(entities, ct);
    }

    public async Task CompareDetailCount(string dateFk, string sourceFk, int count, CancellationToken ct)
    {
        var actual = await repo.StormEventsDailyDetailCount(dateFk, sourceFk, ct);

        if (actual != count)
            throw new InvalidOperationException($"dateFk: {dateFk}, sourceFk: {sourceFk}, count: {count}");
    }

    #endregion

    #region Summary

    public async Task ActivateSummary(StormEventsDailySummaryEntity entity, CancellationToken ct)
    {
        entity.IsCurrent = true;
        entity.Timestamp = DateTime.UtcNow;

        await repo.StormEventsDailySummaryUpdate(entity, ct);
    }

    public async Task AddDailySummaryToCosmos(DailySummaryModel model, string sourceFk, CancellationToken ct)
    {
        var entity = EntityMapping.ToStormEventsDailySummary([model], sourceFk)[0];

        await repo.StormEventsDailySummaryCreate(entity, ct);
    }

    #endregion

    #region Database

    public async Task DatabaseDownload(BlobContainerClient client, List<DatabaseFileModel> model, CancellationToken ct)
    {
        var inventory = await repo.StormEventsDatabaseGetAll(ct);

        foreach (var csv in model)
        {
            if (csv.Year < 2010) continue;

            var entity = inventory
                .SingleOrDefault(w => w.Id == csv.Updated && w.Year == csv.Year);
            if (entity is not null) continue;

            var localFileName = OlieCommon.CreateLocalTmpPath(".csv.gz");
            var fileName = $"bronze/storm-events/{csv.Name}";
            var content = await ows.ApiGetBytes($"{StormEventsUrl}/{csv.Name}", ct);
            await ows.FileWriteAllBytes(localFileName, content, ct);
            await ows.BlobUploadFile(client, fileName, localFileName, ct);

            entity = new StormEventsDatabaseEntity
            {
                Id = csv.Updated,
                Year = csv.Year,
                BlobName = fileName,
                Timestamp = DateTime.UtcNow
            };

            await repo.StormEventsDatabaseCreate(entity, ct);
            OlieCommon.DeleteTempFiles(ows, [localFileName]);
        }
    }

    public async Task<List<DailyDetailModel>> DatabaseLoad(BlobContainerClient blobClient, StormEventsDatabaseEntity eventsDatabase, CancellationToken ct)
    {
        var localFileName = OlieCommon.CreateLocalTmpPath(".csv.gz");
        await ows.BlobDownloadFile(blobClient, eventsDatabase.BlobName, localFileName, ct);
        var csv = await ows.FileReadAllTextFromGz(localFileName, ct);
        using var sr = new StringReader(csv);

        var result = DatabaseParse(sr);

        OlieCommon.DeleteTempFiles(ows, [localFileName]);

        return result;
    }

    public static List<DailyDetailModel> DatabaseParse(TextReader textReader)
    {
        var result = new List<DailyDetailModel>();
        using var reader = new CsvReader(textReader, CultureInfo.InvariantCulture, true);
        var records = reader.GetRecords<StormEventRowModel>();

        foreach (var record in records)
        {
            string magnitude;

            switch (record.EventType)
            {
                case "Hail":
                    magnitude = $"{record.Magnitude:0.00}";
                    break;
                case "Thunderstorm Wind":
                    magnitude = $"{record.Magnitude}";
                    break;
                case "Tornado":
                    magnitude = record.TornadoFScale;
                    break;
                default:
                    continue;
            }

            var offset = int.Parse(StripTimeZoneRegex().Replace(record.TimeZone, string.Empty));
            var effective = DateTime
                .ParseExact(record.Effective, "dd-MMM-yy HH:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal)
                .AddHours(-offset);

            var model = new DailyDetailModel
            {
                City = record.City,
                County = record.County,
                Effective = effective,
                EventType = record.EventType,
                ForecastOffice = record.ForecastOffice,
                Latitude = record.Latitude ?? float.NaN,
                Longitude = record.Longitude ?? float.NaN,
                Magnitude = magnitude,
                Narrative = record.Narrative,
                State = record.State
            };
            result.Add(model);
        }

        return result;
    }

    public async Task DatabaseUpdateActive(StormEventsDatabaseEntity entity, CancellationToken ct)
    {
        entity.IsActive = true;
        entity.Timestamp = DateTime.UtcNow;

        await repo.StormEventsDatabaseInventoryUpdate(entity, ct);
    }

    public async Task DatabaseUpdateRowCount(StormEventsDatabaseEntity entity, int rowCount, CancellationToken ct)
    {
        entity.RowCount = rowCount;
        entity.Timestamp = DateTime.UtcNow;

        await repo.StormEventsDatabaseInventoryUpdate(entity, ct);
    }

    public async Task<List<DatabaseFileModel>> DatabaseList(CancellationToken ct)
    {
        var results = await ows.ApiGetString(StormEventsUrl, ct);
        var matches = MatchCsvFileRegex().Matches(results);

        return [.. matches.Select(s => new DatabaseFileModel
        {
            Name = s.Value
        })];
    }

    #endregion

    #region Regex

    [GeneratedRegex("[^0-9-]+")]
    private static partial Regex StripTimeZoneRegex();

    [GeneratedRegex(@"StormEvents_details-ftp_v1\.0_d\d{4}_c\d{8}\.csv\.gz(?=\"")")]
    private static partial Regex MatchCsvFileRegex();

    #endregion
}