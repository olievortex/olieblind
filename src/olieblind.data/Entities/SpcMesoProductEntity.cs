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
    [MaxLength(8000)] public string Html { get; init; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

//CREATE TABLE olieblind_dev.SpcMesoProducts(
//  Id INT NOT NULL,
//	EffectiveDate varchar(32) NOT NULL,
//  EffectiveTime DATETIME NOT NULL,
//	AreasAffected varchar(300) NOT NULL,
//  Concerning varchar(300) NOT NULL,
//  GraphicUrl varchar(320) NULL,
//	Narrative TEXT NOT NULL,
//  Html TEXT NOT NULL,
//	`Timestamp` DATETIME NOT NULL,
//	CONSTRAINT SpcMesoProducts_PK PRIMARY KEY(Id, EffectiveDate)
//)
//ENGINE=InnoDB
//DEFAULT CHARSET=utf8mb4
//COLLATE = utf8mb4_0900_ai_ci;
