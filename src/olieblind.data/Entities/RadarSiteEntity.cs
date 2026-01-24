using System.ComponentModel.DataAnnotations;

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

//-- olieblind.RadarSites definition

//CREATE TABLE "RadarSites" (
//  "Id" varchar(16) NOT NULL,
//  "Name" varchar(32) NOT NULL,
//  "State" varchar(8) NOT NULL,
//  "Latitude" double NOT NULL,
//  "Longitude" double NOT NULL,
//  "Timestamp" datetime NOT NULL,
//  PRIMARY KEY("Id")
//);