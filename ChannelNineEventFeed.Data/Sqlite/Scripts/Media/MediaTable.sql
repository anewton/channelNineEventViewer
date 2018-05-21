CREATE TABLE IF NOT EXISTS `Media` (
    `Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `Created` DATETIME NULL,
    `LastUpdated` DATETIME NULL,
    `SessionId` INTEGER NOT NULL,
    `MediaType` NVARCHAR(8000) NULL,
    `IsDownloaded` BIT NULL,
    `IsDownloadInProgress` BIT NULL,
    `IsPlayableInMediaElement` BIT NULL,
    `DownloadLink` NVARCHAR(8000) NULL,
    `PublishDate` DATETIME NULL
);


