using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace olieblind.data.Entities;

[PrimaryKey(nameof(Id), nameof(Year))]
public class StormEventsDatabaseEntity
{
    [MaxLength(36)] public string Id { get; init; } = string.Empty;
    public int Year { get; init; }

    [MaxLength(320)] public string BlobName { get; init; } = string.Empty;
    public int RowCount { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsActive { get; set; }
}

//-- olieblind.StormEventsDatabases definition

//CREATE TABLE "StormEventsDatabases" (
//  "Id" varchar(36) NOT NULL,
//  "Year" int NOT NULL,
//  "BlobName" varchar(320) NOT NULL,
//  "RowCount" int NOT NULL,
//  "Timestamp" datetime NOT NULL,
//  "IsActive" tinyint(1) NOT NULL,
//  PRIMARY KEY("Id","Year")
//);