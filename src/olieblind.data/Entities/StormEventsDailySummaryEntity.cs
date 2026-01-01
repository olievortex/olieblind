using System.ComponentModel.DataAnnotations;

namespace olieblind.data.Entities;

public class StormEventsDailySummaryEntity
{
    [MaxLength(36)] public string Id { get; init; } = string.Empty;
    public int Year { get; init; }
    [MaxLength(36)] public string SourceFk { get; init; } = string.Empty;

    public DateTime? HeadlineEventTime { get; init; }
    [MaxLength(320)] public string? SatellitePathPoster { get; set; }
    [MaxLength(320)] public string? SatellitePath1080 { get; set; }
    public int Hail { get; init; }
    public int Wind { get; init; }
    public int F5 { get; init; }
    public int F4 { get; init; }
    public int F3 { get; init; }
    public int F2 { get; init; }
    public int F1 { get; init; }
    public int RowCount { get; init; }
    public DateTime Timestamp { get; set; }
    public bool IsCurrent { get; set; }

    public override string ToString()
    {
        return Id;
    }
}

//CREATE TABLE olieblind_dev.StormEventDailySummaries (
//	  Id varchar(36) NOT NULL,
//	  `Year` INT NOT NULL,
//	  SourceFk varchar(36) NOT NULL,
//    HeadlineEventTime DATETIME NULL,
//    SatellitePathPoster varchar(320) NULL,
//	  SatellitePath1080 varchar(320) NULL,
//	  Hail INT NOT NULL,
//    Wind INT NOT NULL,
//	  F5 INT NOT NULL,
//    F4 INT NOT NULL,
//    F3 INT NOT NULL,
//    F2 INT NOT NULL,
//	  F1 INT NOT NULL,
//    RowCount INT NOT NULL,
//	  `Timestamp` DATETIME NOT NULL,
//	  IsCurrent BOOL NOT NULL,
//  CONSTRAINT StormEventDailySummaries_PK PRIMARY KEY(Id,`Year`, SourceFk)
//)
//ENGINE=InnoDB
//DEFAULT CHARSET=utf8mb4
//COLLATE = utf8mb4_0900_ai_ci;
