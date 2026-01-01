using System;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace olieblind.data.Entities;

public class RadarSiteEntity
{
    [MaxLength(16)] public string Id { get; init; } = string.Empty;

    [MaxLength(32)] public string Name { get; init; } = string.Empty;
    [MaxLength(8)] public string State { get; init; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public DateTime Timestamp { get; init; }
}

//CREATE TABLE olieblind_dev.RadarSites(
//    Id varchar(16) NOT NULL,
//    Name varchar(32) NOT NULL,
//    State varchar(8) NOT NULL,
//    Latitude DOUBLE NOT NULL,
//	  Longitude double NOT NULL,
//	  `Timestamp` DATETIME NOT NULL,
//	CONSTRAINT RadarSite_PK PRIMARY KEY(Id)
//)
//ENGINE=InnoDB
//DEFAULT CHARSET=utf8mb4
//COLLATE = utf8mb4_0900_ai_ci;
