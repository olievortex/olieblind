using System.ComponentModel.DataAnnotations;

namespace olieblind.data.Entities;

public class RadarInventoryEntity
{
    [MaxLength(16)] public string Id { get; init; } = string.Empty; // Radar Id
    [MaxLength(32)] public string EffectiveDate { get; init; } = string.Empty;
    [MaxLength(50)] public string BucketName { get; init; } = string.Empty;

    [MaxLength(1000)] public string FileList { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
}

//CREATE TABLE olieblind_dev.RadarInventories(
//    Id varchar(16) NOT NULL,
//    EffectiveDate varchar(32) NOT NULL,
//    BucketName varchar(50) NOT NULL,
//    FileList VARCHAR(1000) NOT NULL,
//	  `Timestamp` DATETIME NOT NULL,
//	CONSTRAINT RadarInventories_PK PRIMARY KEY(Id, EffectiveDate, BucketName)
//)
//ENGINE=InnoDB
//DEFAULT CHARSET=utf8mb4
//COLLATE = utf8mb4_0900_ai_ci;
