using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace olieblind.data.Entities;

[PrimaryKey(nameof(Id), nameof(Year), nameof(SourceFk))]
public class StormEventsDailySummaryEntity
{
    [MaxLength(36)] public string Id { get; init; } = string.Empty;
    public int Year { get; init; }
    [MaxLength(36)] public string SourceFk { get; init; } = string.Empty;

    public DateTime? HeadlineEventTime { get; init; }
    [MaxLength(320)] public string? SatellitePathPoster { get; set; }
    [MaxLength(320)] public string? SatellitePath1080 { get; set; }
    public int Hail { get; init; }
    public int Wind { get; init; }
    public int F5 { get; init; }
    public int F4 { get; init; }
    public int F3 { get; init; }
    public int F2 { get; init; }
    public int F1 { get; init; }
    public int RowCount { get; init; }
    public DateTime Timestamp { get; set; }
    public bool IsCurrent { get; set; }

    public override string ToString()
    {
        return Id;
    }
}

//-- olieblind.StormEventsDailySummaries definition

//CREATE TABLE "StormEventsDailySummaries" (
//  "Id" varchar(36) NOT NULL,
//  "Year" int NOT NULL,
//  "SourceFk" varchar(36) NOT NULL,
//  "HeadlineEventTime" datetime DEFAULT NULL,
//  "SatellitePathPoster" varchar(320) DEFAULT NULL,
//  "SatellitePath1080" varchar(320) DEFAULT NULL,
//  "Hail" int NOT NULL,
//  "Wind" int NOT NULL,
//  "F5" int NOT NULL,
//  "F4" int NOT NULL,
//  "F3" int NOT NULL,
//  "F2" int NOT NULL,
//  "F1" int NOT NULL,
//  "RowCount" int NOT NULL,
//  "Timestamp" datetime NOT NULL,
//  "IsCurrent" tinyint(1) NOT NULL,
//  PRIMARY KEY("Id","Year","SourceFk")
//);