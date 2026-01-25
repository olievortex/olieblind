using System.ComponentModel.DataAnnotations;

namespace olieblind.data.Entities;

public class ProductVideoEntity
{
    [Key]
    public int Id { get; init; }
    public string Category { get; init; } = string.Empty;
    public string PosterUrl { get; init; } = string.Empty;
    public string PosterLocalPath { get; init; } = string.Empty;
    public string VideoUrl { get; init; } = string.Empty;
    public string VideoLocalPath { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Transcript { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public bool IsActive { get; set; }
}

//-- olieblind.ProductVideos definition

//CREATE TABLE "ProductVideos" (
//  "Id" int NOT NULL AUTO_INCREMENT,
//  "Category" varchar(100) NOT NULL,
//  "PosterUrl" varchar(1000) NOT NULL,
//  "VideoUrl" varchar(1000) NOT NULL,
//  "Title" varchar(1000) NOT NULL,
//  "Transcript" text NOT NULL,
//  "Timestamp" datetime NOT NULL,
//  "PosterLocalPath" varchar(1000) NOT NULL,
//  "VideoLocalPath" varchar(1000) NOT NULL,
//  "IsActive" bit(1) NOT NULL,
//  PRIMARY KEY("Id")
//);