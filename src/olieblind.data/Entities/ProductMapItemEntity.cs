using System.ComponentModel.DataAnnotations;

namespace olieblind.data.Entities;

public class ProductMapItemEntity
{
    [Key]
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int ParameterId { get; set; }
    public int GeographyId { get; set; }
    public string LocalPath { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int ProductMapId { get; set; }
}

//CREATE TABLE `ProductMapItems` (
//  `Id` int NOT NULL AUTO_INCREMENT,
//  `Url` varchar(1000) NOT NULL,
//  `Title` varchar(1000) NOT NULL,
//  `Timestamp` datetime NOT NULL,
//  `ParameterId` int NOT NULL,
//  `GeographyId` int NOT NULL,
//  `LocalPath` varchar(1000) NOT NULL,
//  `IsActive` bit(1) NOT NULL,
//  `ProductMapId` int NOT NULL,
//  PRIMARY KEY(`Id`)
//) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE = utf8mb4_0900_ai_ci;