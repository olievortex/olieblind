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

//CREATE TABLE olieblind_dev.StormEventsDatabases(
//    Id varchar(36) NOT NULL,
//	  `Year` INT NOT NULL,
//	  BlobName varchar(320) NOT NULL,
//    RowCount INT NOT NULL,
//	  `Timestamp` DATETIME NOT NULL,
//	  IsActive BOOL NOT NULL,
//  CONSTRAINT StormEventsDatabases_PK PRIMARY KEY(Id,`Year`)
//)
//ENGINE=InnoDB
//DEFAULT CHARSET=utf8mb4
//COLLATE = utf8mb4_0900_ai_ci;
