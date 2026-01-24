using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace olieblind.data.Entities;

[PrimaryKey(nameof(Id), nameof(DateFk), nameof(SourceFk))]
public class StormEventsDailyDetailEntity
{
    [MaxLength(36)] public string Id { get; init; } = string.Empty;
    [MaxLength(32)] public string DateFk { get; init; } = string.Empty;
    [MaxLength(32)] public string SourceFk { get; init; } = string.Empty;

    public DateTime EffectiveTime { get; init; }
    [MaxLength(50)] public string State { get; init; } = string.Empty;
    [MaxLength(50)] public string County { get; init; } = string.Empty;
    [MaxLength(50)] public string City { get; init; } = string.Empty;
    [MaxLength(25)] public string EventType { get; init; } = string.Empty;
    [MaxLength(5)] public string ForecastOffice { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    [MaxLength(8)] public string Magnitude { get; init; } = string.Empty;
    public float Latitude { get; init; }
    public float Longitude { get; init; }
    [MaxLength(65535)] public string Narrative { get; init; } = string.Empty;
    [MaxLength(16)] public string ClosestRadar { get; init; } = string.Empty;
}

//-- olieblind.StormEventsDailyDetails definition

//CREATE TABLE "StormEventsDailyDetails" (
//  "Id" varchar(36) NOT NULL,
//  "DateFk" varchar(32) NOT NULL,
//  "SourceFk" varchar(32) NOT NULL,
//  "EffectiveTime" datetime NOT NULL,
//  "State" varchar(50) NOT NULL,
//  "County" varchar(50) NOT NULL,
//  "City" varchar(50) NOT NULL,
//  "EventType" varchar(25) NOT NULL,
//  "ForecastOffice" varchar(5) NOT NULL,
//  "Timestamp" datetime NOT NULL,
//  "Magnitude" varchar(8) NOT NULL,
//  "Latitude" float NOT NULL,
//  "Longitude" float NOT NULL,
//  "Narrative" text NOT NULL,
//  "ClosestRadar" varchar(16) NOT NULL,
//  PRIMARY KEY("Id","DateFk","SourceFk")
//);