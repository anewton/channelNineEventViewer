﻿CREATE TABLE IF NOT EXISTS `SpeakerVideo` (
    `Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `Created` DATETIME NULL,
    `LastUpdated` DATETIME NULL,
    `SpeakerId` INTEGER NOT NULL,
    `VideoId` INTEGER NOT NULL
);

