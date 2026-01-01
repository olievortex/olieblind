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
    [MaxLength(5)] public string Magnitude { get; init; } = string.Empty;
    public float Latitude { get; init; }
    public float Longitude { get; init; }
    [MaxLength(1000)] public string Narrative { get; init; } = string.Empty;
    [MaxLength(16)] public string ClosestRadar { get; init; } = string.Empty;
}

// CREATE TABLE olieblind_dev.StormEventsDailyDetails (
//	  Id varchar(36) NOT NULL,
//    DateFk varchar(32) NOT NULL,
//    SourceFk varchar(32) NOT NULL,
//    EffectiveTime DATETIME NOT NULL,
//	  State varchar(50) NOT NULL,
//    County varchar(50) NOT NULL,
//    City varchar(50) NOT NULL,
//    EventType varchar(25) NOT NULL,
//    ForecastOffice varchar(5) NOT NULL,
//	  `Timestamp` DATETIME NOT NULL,
//	  Magnitude varchar(5) NOT NULL,
//    Latitude FLOAT NOT NULL,
//	  Longitude FLOAT NOT NULL,
//    Narrative varchar(1000) NOT NULL,
//    ClosestRadar varchar(16) NOT NULL,
//  CONSTRAINT StormEventsDailyDetails_PK PRIMARY KEY(Id, DateFk, SourceFk)
//)
//ENGINE=InnoDB
//DEFAULT CHARSET=utf8mb4
//COLLATE = utf8mb4_0900_ai_ci;
