using Microsoft.EntityFrameworkCore;
using olieblind.data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace olieblind.data.Entities;

[PrimaryKey(nameof(Id), nameof(EffectiveDate), nameof(Channel), nameof(DayPart))]
[Table("SatelliteAwsInventories")]
public class SatelliteInventoryEntity
{
    [MaxLength(36)] public string Id { get; init; } = string.Empty; // AWS Bucket
    [MaxLength(32)] public string EffectiveDate { get; init; } = string.Empty; // Partition Key cannot be DateTime
    public int Channel { get; init; }
    public DayPartsEnum DayPart { get; init; }

    public DateTime Timestamp { get; init; }
}

//-- olieblind.SatelliteAwsInventories definition

//CREATE TABLE "SatelliteAwsInventories" (
//  "Id" varchar(36) NOT NULL,
//  "EffectiveDate" varchar(32) NOT NULL,
//  "Channel" int NOT NULL,
//  "DayPart" int NOT NULL,
//  "Timestamp" datetime NOT NULL,
//  PRIMARY KEY("Id","EffectiveDate","Channel","DayPart")
//);