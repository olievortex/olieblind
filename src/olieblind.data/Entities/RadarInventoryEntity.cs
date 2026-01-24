using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace olieblind.data.Entities;

[PrimaryKey(nameof(Id), nameof(EffectiveDate), nameof(BucketName))]
public class RadarInventoryEntity
{
    [MaxLength(16)] public string Id { get; init; } = string.Empty; // Radar Id
    [MaxLength(32)] public string EffectiveDate { get; init; } = string.Empty;
    [MaxLength(50)] public string BucketName { get; init; } = string.Empty;

    public List<string> FileList { get; init; } = [];
    public DateTime Timestamp { get; init; }
}

//-- olieblind.RadarInventories definition

//CREATE TABLE "RadarInventories" (
//  "Id" varchar(16) NOT NULL,
//  "EffectiveDate" varchar(32) NOT NULL,
//  "BucketName" varchar(50) NOT NULL,
//  "FileList" json NOT NULL,
//  "Timestamp" datetime NOT NULL,
//  PRIMARY KEY("Id","EffectiveDate","BucketName")
//);