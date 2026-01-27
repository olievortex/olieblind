using Microsoft.EntityFrameworkCore;
using olieblind.data.Enums;
using System.ComponentModel.DataAnnotations;

namespace olieblind.data.Entities;

[PrimaryKey(nameof(Id), nameof(Timestamp))]
public class UserSatelliteAdHocLogEntity
{
    [MaxLength(320)]
    public string Id { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }

    [MaxLength(32)]
    public string EffectiveDate { get; init; } = string.Empty;
    public int Channel { get; init; }
    public DayPartsEnum DayPart { get; init; }
    public bool IsFree { get; init; }
}

//-- olieblind.UserSatelliteAdHocLogs definition

//CREATE TABLE "UserSatelliteAdHocLogs" (
//  "Id" varchar(320) NOT NULL,
//  "Timestamp" datetime NOT NULL,
//  "EffectiveDate" varchar(32) NOT NULL,
//  "Channel" int NOT NULL,
//  "DayPart" int NOT NULL,
//  "IsFree" tinyint(1) NOT NULL,
//  PRIMARY KEY("Id","Timestamp")
//);