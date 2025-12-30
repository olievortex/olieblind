use olieblind_dev;

CREATE TABLE `ProductMaps` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `SourceUrl` varchar(1000) NOT NULL,
  `IsActive` bit(1) NOT NULL,
  `Effective` datetime NOT NULL,
  `Timestamp` datetime NOT NULL,
  `ForecastHour` int NOT NULL,
  `ProductId` int NOT NULL,
  PRIMARY KEY(`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE = utf8mb4_0900_ai_ci;

CREATE TABLE `ProductMapItems` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Url` varchar(1000) NOT NULL,
  `Title` varchar(1000) NOT NULL,
  `Timestamp` datetime NOT NULL,
  `ParameterId` int NOT NULL,
  `GeographyId` int NOT NULL,
  `LocalPath` varchar(1000) NOT NULL,
  `IsActive` bit(1) NOT NULL,
  `ProductMapId` int NOT NULL,
  PRIMARY KEY(`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE = utf8mb4_0900_ai_ci;

CREATE TABLE `ProductVideos` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Category` varchar(100) NOT NULL,
  `PosterUrl` varchar(1000) NOT NULL,
  `VideoUrl` varchar(1000) NOT NULL,
  `Title` varchar(1000) NOT NULL,
  `Transcript` text NOT NULL,
  `Timestamp` datetime NOT NULL,
  `PosterLocalPath` varchar(1000) NOT NULL,
  `VideoLocalPath` varchar(1000) NOT NULL,
  `IsActive` bit(1) NOT NULL,
  PRIMARY KEY(`Id`)
) ENGINE=InnoDB DEFAULT CHARSET = utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `UserCookieConsentLogs` (
  `Id` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Cookie` varchar(1000) NOT NULL,
  `UserAgent` varchar(1000) NOT NULL,
  `SourceIp` varchar(100) NOT NULL,
  `Status` varchar(100) NOT NULL,
  `Timestamp` datetime NOT NULL,
  PRIMARY KEY(`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE = utf8mb4_0900_ai_ci;
