using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace olieblind.data.Entities;

[PrimaryKey(nameof(Id), nameof(EffectiveDate))]
public class SpcMesoProductEntity
{
    public int Id { get; init; }
    [MaxLength(32)] public string EffectiveDate { get; init; } = string.Empty;

    public DateTime EffectiveTime { get; init; }
    [MaxLength(300)] public string AreasAffected { get; set; } = string.Empty;
    [MaxLength(300)] public string Concerning { get; set; } = string.Empty;
    [MaxLength(320)] public string? GraphicUrl { get; set; }
    [MaxLength(8000)] public string Narrative { get; init; } = string.Empty;
    [MaxLength(8000)] public string Html { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

//-- olieblind.SpcMesoProducts definition

//CREATE TABLE "SpcMesoProducts" (
//  "Id" int NOT NULL,
//  "EffectiveDate" varchar(32) NOT NULL,
//  "EffectiveTime" datetime NOT NULL,
//  "AreasAffected" varchar(300) NOT NULL,
//  "Concerning" varchar(300) NOT NULL,
//  "GraphicUrl" varchar(320) DEFAULT NULL,
//  "Narrative" text NOT NULL,
//  "Html" varchar(320) NOT NULL,
//  "Timestamp" datetime NOT NULL,
//  PRIMARY KEY("Id","EffectiveDate"),
//  KEY "SpcMesoProducts_EffectiveDate_IDX" ("EffectiveDate")
//);