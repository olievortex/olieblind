using System.ComponentModel.DataAnnotations;

namespace olieblind.data.Entities;

public class ProductMapEntity
{
    [Key]
    public int Id { get; set; }
    public string SourceUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime Effective { get; set; }
    public DateTime Timestamp { get; set; }
    public int ForecastHour { get; set; }
    public int ProductId { get; set; }
}

//CREATE TABLE `ProductMaps` (
//  `Id` int NOT NULL AUTO_INCREMENT,
//  `SourceUrl` varchar(1000) NOT NULL,
//  `IsActive` bit(1) NOT NULL,
//  `Effective` datetime NOT NULL,
//  `Timestamp` datetime NOT NULL,
//  `ForecastHour` int NOT NULL,
//  `ProductId` int NOT NULL,
//  PRIMARY KEY(`Id`)
//) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE = utf8mb4_0900_ai_ci;