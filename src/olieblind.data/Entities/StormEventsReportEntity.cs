using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace olieblind.data.Entities;

[PrimaryKey(nameof(Id), nameof(EffectiveDate))]
public class StormEventsReportEntity
{
    [MaxLength(320)] public string Id { get; init; } = string.Empty;
    [MaxLength(32)] public string EffectiveDate { get; init; } = string.Empty; // Partition Key cannot be DateTime

    public string[] Rows { get; init; } = [];
    public DateTime Timestamp { get; set; }
    public bool IsDailySummaryComplete { get; set; }
    public bool IsDailyDetailComplete { get; set; }
    public bool IsTornadoDay { get; set; }

    public static StormEventsReportEntity FromValues(DateTime effectiveDate, string body, string etag)
    {
        return new StormEventsReportEntity
        {
            Id = etag,
            EffectiveDate = effectiveDate.ToString("yyyy-MM-dd"),
            IsDailySummaryComplete = false,
            IsTornadoDay = false,
            Rows = body.ReplaceLineEndings("\n").Split("\n"),
            Timestamp = DateTime.UtcNow
        };
    }

    public DateTime DecodeEffectiveDate()
    {
        return DateTime.ParseExact(EffectiveDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}

//-- olieblind.StormEventsReports definition

//CREATE TABLE "StormEventsReports" (
//  "Id" varchar(320) NOT NULL,
//  "EffectiveDate" varchar(32) NOT NULL,
//  "Rows" json NOT NULL,
//  "Timestamp" datetime NOT NULL,
//  "IsDailySummaryComplete" tinyint(1) NOT NULL,
//  "IsDailyDetailComplete" tinyint(1) NOT NULL,
//  "IsTornadoDay" tinyint(1) NOT NULL,
//  PRIMARY KEY("Id","EffectiveDate")
//);