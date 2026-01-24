use olieblind_dev;

CREATE TABLE "ProductMapItems" (
  "Id" int NOT NULL AUTO_INCREMENT,
  "Url" varchar(1000) NOT NULL,
  "Title" varchar(1000) NOT NULL,
  "Timestamp" datetime NOT NULL,
  "ParameterId" int NOT NULL,
  "GeographyId" int NOT NULL,
  "LocalPath" varchar(1000) NOT NULL,
  "IsActive" bit(1) NOT NULL,
  "ProductMapId" int NOT NULL,
  PRIMARY KEY ("Id")
);

CREATE TABLE "ProductMaps" (
  "Id" int NOT NULL AUTO_INCREMENT,
  "SourceUrl" varchar(1000) NOT NULL,
  "IsActive" bit(1) NOT NULL,
  "Effective" datetime NOT NULL,
  "Timestamp" datetime NOT NULL,
  "ForecastHour" int NOT NULL,
  "ProductId" int NOT NULL,
  PRIMARY KEY ("Id")
);

CREATE TABLE "ProductVideos" (
  "Id" int NOT NULL AUTO_INCREMENT,
  "Category" varchar(100) NOT NULL,
  "PosterUrl" varchar(1000) NOT NULL,
  "VideoUrl" varchar(1000) NOT NULL,
  "Title" varchar(1000) NOT NULL,
  "Transcript" text NOT NULL,
  "Timestamp" datetime NOT NULL,
  "PosterLocalPath" varchar(1000) NOT NULL,
  "VideoLocalPath" varchar(1000) NOT NULL,
  "IsActive" bit(1) NOT NULL,
  PRIMARY KEY ("Id")
);

CREATE TABLE "RadarInventories" (
  "Id" varchar(16) NOT NULL,
  "EffectiveDate" varchar(32) NOT NULL,
  "BucketName" varchar(50) NOT NULL,
  "FileList" json NOT NULL,
  "Timestamp" datetime NOT NULL,
  PRIMARY KEY ("Id","EffectiveDate","BucketName")
);

CREATE TABLE "RadarSites" (
  "Id" varchar(16) NOT NULL,
  "Name" varchar(32) NOT NULL,
  "State" varchar(8) NOT NULL,
  "Latitude" double NOT NULL,
  "Longitude" double NOT NULL,
  "Timestamp" datetime NOT NULL,
  PRIMARY KEY ("Id")
);

CREATE TABLE "SatelliteAwsInventories" (
  "Id" varchar(36) NOT NULL,
  "EffectiveDate" varchar(32) NOT NULL,
  "Channel" int NOT NULL,
  "DayPart" int NOT NULL,
  "Timestamp" datetime NOT NULL,
  PRIMARY KEY ("Id","EffectiveDate","Channel","DayPart")
);

CREATE TABLE "SatelliteAwsProducts" (
  "Id" varchar(100) NOT NULL,
  "EffectiveDate" varchar(32) NOT NULL,
  "BucketName" varchar(50) NOT NULL,
  "Channel" int NOT NULL,
  "DayPart" int NOT NULL,
  "Path1080" varchar(320) DEFAULT NULL,
  "PathPoster" varchar(320) DEFAULT NULL,
  "PathSource" varchar(320) DEFAULT NULL,
  "PathLocal" varchar(320) DEFAULT NULL,
  "ScanTime" datetime NOT NULL,
  "Timestamp" datetime NOT NULL,
  "TimeTaken1080" int NOT NULL,
  "TimeTakenDownload" int NOT NULL,
  "TimeTakenPoster" int NOT NULL,
  PRIMARY KEY ("Id","EffectiveDate")
);

CREATE TABLE "SpcMesoProducts" (
  "Id" int NOT NULL,
  "EffectiveDate" varchar(32) NOT NULL,
  "EffectiveTime" datetime NOT NULL,
  "AreasAffected" varchar(300) NOT NULL,
  "Concerning" varchar(300) NOT NULL,
  "GraphicUrl" varchar(320) DEFAULT NULL,
  "Narrative" text NOT NULL,
  "Html" varchar(320) NOT NULL,
  "Timestamp" datetime NOT NULL,
  PRIMARY KEY ("Id","EffectiveDate"),
  KEY "SpcMesoProducts_EffectiveDate_IDX" ("EffectiveDate")
);

CREATE TABLE "StormEventsDailyDetails" (
  "Id" varchar(36) NOT NULL,
  "DateFk" varchar(32) NOT NULL,
  "SourceFk" varchar(32) NOT NULL,
  "EffectiveTime" datetime NOT NULL,
  "State" varchar(50) NOT NULL,
  "County" varchar(50) NOT NULL,
  "City" varchar(50) NOT NULL,
  "EventType" varchar(25) NOT NULL,
  "ForecastOffice" varchar(5) NOT NULL,
  "Timestamp" datetime NOT NULL,
  "Magnitude" varchar(8) NOT NULL,
  "Latitude" float NOT NULL,
  "Longitude" float NOT NULL,
  "Narrative" text NOT NULL,
  "ClosestRadar" varchar(16) NOT NULL,
  PRIMARY KEY ("Id","DateFk","SourceFk"),
  KEY "StormEventsDailyDetails_DateFk_IDX" ("DateFk","SourceFk")
);

CREATE TABLE "StormEventsDailySummaries" (
  "Id" varchar(36) NOT NULL,
  "Year" int NOT NULL,
  "SourceFk" varchar(36) NOT NULL,
  "HeadlineEventTime" datetime DEFAULT NULL,
  "SatellitePathPoster" varchar(320) DEFAULT NULL,
  "SatellitePath1080" varchar(320) DEFAULT NULL,
  "Hail" int NOT NULL,
  "Wind" int NOT NULL,
  "F5" int NOT NULL,
  "F4" int NOT NULL,
  "F3" int NOT NULL,
  "F2" int NOT NULL,
  "F1" int NOT NULL,
  "RowCount" int NOT NULL,
  "Timestamp" datetime NOT NULL,
  "IsCurrent" tinyint(1) NOT NULL,
  PRIMARY KEY ("Id","Year","SourceFk")
);

CREATE TABLE "StormEventsDatabases" (
  "Id" varchar(36) NOT NULL,
  "Year" int NOT NULL,
  "BlobName" varchar(320) NOT NULL,
  "RowCount" int NOT NULL,
  "Timestamp" datetime NOT NULL,
  "IsActive" tinyint(1) NOT NULL,
  PRIMARY KEY ("Id","Year")
);

CREATE TABLE "StormEventsReports" (
  "Id" varchar(320) NOT NULL,
  "EffectiveDate" varchar(32) NOT NULL,
  "Rows" json NOT NULL,
  "Timestamp" datetime NOT NULL,
  "IsDailySummaryComplete" tinyint(1) NOT NULL,
  "IsDailyDetailComplete" tinyint(1) NOT NULL,
  "IsTornadoDay" tinyint(1) NOT NULL,
  PRIMARY KEY ("Id","EffectiveDate")
);

CREATE TABLE "UserCookieConsentLogs" (
  "Id" varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  "Cookie" varchar(1000) NOT NULL,
  "UserAgent" varchar(1000) NOT NULL,
  "SourceIp" varchar(100) NOT NULL,
  "Status" varchar(100) NOT NULL,
  "Timestamp" datetime NOT NULL,
  PRIMARY KEY ("Id")
);

CREATE TABLE "UserSatelliteAdHocLogs" (
  "Id" varchar(320) NOT NULL,
  "Timestamp" datetime NOT NULL,
  "EffectiveDate" varchar(32) NOT NULL,
  "Channel" int NOT NULL,
  "DayPart" int NOT NULL,
  PRIMARY KEY ("Id","Timestamp")
);