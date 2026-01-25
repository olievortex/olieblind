using Microsoft.EntityFrameworkCore;
using olieblind.data.Enums;
using System.ComponentModel.DataAnnotations;

namespace olieblind.data.Entities;

[PrimaryKey(nameof(Id), nameof(EffectiveDate))]
public class SatelliteAwsProductEntity
{
    [MaxLength(100)] public string Id { get; init; } = string.Empty; // AWS Key
    [MaxLength(32)] public string EffectiveDate { get; init; } = string.Empty; // Partition Key cannot be DateTime

    [MaxLength(50)] public string BucketName { get; init; } = string.Empty;
    public int Channel { get; init; } // 2 = Red Visible (HiRes)
    public DayPartsEnum DayPart { get; init; } // 3 = Afternoon
    [MaxLength(320)] public string? Path1080 { get; init; }
    [MaxLength(320)] public string? PathPoster { get; set; }
    [MaxLength(320)] public string? PathSource { get; set; }
    [MaxLength(320)] public string? PathLocal { get; set; }
    public DateTime ScanTime { get; init; }
    public DateTime Timestamp { get; set; }
    public int TimeTaken1080 { get; init; }
    public int TimeTakenDownload { get; set; }
    public int TimeTakenPoster { get; set; }
}

//-- olieblind.SatelliteAwsProducts definition

//CREATE TABLE "SatelliteAwsProducts" (
//  "Id" varchar(100) NOT NULL,
//  "EffectiveDate" varchar(32) NOT NULL,
//  "BucketName" varchar(50) NOT NULL,
//  "Channel" int NOT NULL,
//  "DayPart" int NOT NULL,
//  "Path1080" varchar(320) DEFAULT NULL,
//  "PathPoster" varchar(320) DEFAULT NULL,
//  "PathSource" varchar(320) DEFAULT NULL,
//  "PathLocal" varchar(320) DEFAULT NULL,
//  "ScanTime" datetime NOT NULL,
//  "Timestamp" datetime NOT NULL,
//  "TimeTaken1080" int NOT NULL,
//  "TimeTakenDownload" int NOT NULL,
//  "TimeTakenPoster" int NOT NULL,
//  PRIMARY KEY("Id","EffectiveDate")
//);