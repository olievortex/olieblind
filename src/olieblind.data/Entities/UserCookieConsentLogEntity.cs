using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace olieblind.data.Entities;

public class UserCookieConsentLogEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string Cookie { get; init; } = string.Empty;
    public string SourceIp { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string UserAgent { get; init; } = string.Empty;
}

//-- olieblind.UserCookieConsentLogs definition

//CREATE TABLE "UserCookieConsentLogs" (
//  "Id" varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
//  "Cookie" varchar(1000) NOT NULL,
//  "UserAgent" varchar(1000) NOT NULL,
//  "SourceIp" varchar(100) NOT NULL,
//  "Status" varchar(100) NOT NULL,
//  "Timestamp" datetime NOT NULL,
//  PRIMARY KEY("Id")
//);