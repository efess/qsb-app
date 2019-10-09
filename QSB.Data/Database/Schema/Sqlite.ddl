CREATE TABLE [GameServer] (
[ServerId] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[GameId] INTEGER  NULL,
[CustomName] VARCHAR(255)  NULL,
[AntiWallHack] INTEGER  NULL,
[Port] INTEGER  NULL,
[DNS] VARCHAR(255)  NULL,
[PublicSiteUrl] TEXT  NULL,
[MapDownloadUrl] TEXT  NULL,
[Location] VARCHAR(255)  NULL,
[QueryInterval] INTEGER  NULL,
[LastQuery] DateTime  NULL,
[QueryResult] INTEGER  NULL,
[Region] varchar(50)  NULL,
[NextQuery] DATETIME  NULL,
[LastQuerySuccess] DATETIME  NULL,
[FailedQueryAttempts] INTEGER  NULL,
[ModificationCode] VARCHAR(50)  NULL,
[Active] BOOLEAN  NULL,
[CustomNameShort] VARCHAR(50)  NULL,
[Category] VARCHAR(50)  NULL,
[CustomModificationName] VARCHAR(50) NULL
)

CREATE INDEX [IDX_GAMESERVER_] ON [GameServer](
[ServerId]  ASC,
[GameId]  ASC,
[Region]  ASC
)

CREATE TABLE [HistoricalHourlyLog] (
[HistoricalId] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[Serverid] INTEGER  NULL,
[PlayerId] INTEGER  NULL,
[Hour0] INTEGER  NULL,
[Hour1] INTEGER  NULL,
[Hour2] INTEGER  NULL,
[Hour3] INTEGER  NULL,
[Hour4] INTEGER  NULL,
[Hour5] INTEGER  NULL,
[Hour6] INTEGER  NULL,
[Hour7] INTEGER  NULL,
[Hour8] INTEGER  NULL,
[Hour9] INTEGER  NULL,
[Hour10] INTEGER  NULL,
[Hour11] INTEGER  NULL,
[Hour12] INTEGER  NULL,
[Hour13] INTEGER  NULL,
[Hour14] INTEGER  NULL,
[Hour15] INTEGER  NULL,
[Hour16] INTEGER  NULL,
[Hour17] INTEGER  NULL,
[Hour18] INTEGER  NULL,
[Hour19] INTEGER  NULL,
[Hour20] INTEGER  NULL,
[Hour21] INTEGER  NULL,
[Hour22] INTEGER  NULL,
[Hour23] INTEGER  NULL,
[TotalHours] INTEGER  NULL,
[HistoricalDate] DATE  NULL
)

CREATE INDEX [IDX_HISTORICALHOURLYLOG_] ON [HistoricalHourlyLog](
[Serverid]  ASC,
[PlayerId]  ASC,
[HistoricalDate]  ASC
)

CREATE TABLE [Player] (
[AliasId] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[PlayerId] INTEGER  NULL,
[Alias] VARCHAR(50)  NULL,
[AliasBytes] BLOB  NULL,
[IPAddress] VARCHAR(21)  NULL,
[LastPing] INTEGER  NULL,
[PlayerNumber] INTEGER  NULL,
[GameId] INTEGER  NULL,
[IdentifyIPAddress] INTEGER NULL,
[RestrictUserOption] INTEGER NULL
)

CREATE INDEX [IDX_PLAYER_] ON [Player](
[PlayerId]  ASC,
[AliasId]  ASC
)

CREATE TABLE [PlayerMatch] (
[PlayerMatchId] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
[PlayerId] intEGER  NULL,
[AliasId] INTEGER  NULL,
[PantColor] INTEGER  NULL,
[ShirtColor] INTEGER  NULL,
[Skin] VARCHAR(50)  NULL,
[Model] VARCHAR(50)  NULL,
[Latency] intEGER  NULL,
[Frags] intEGER  NULL,
[PlayerMatchStart] DateTime  NULL,
[PlayerMatchEnd] DATETIME  NULL,
[ServerMatchId] INTEGER  NULL
)

CREATE INDEX [IDX_PLAYERMATCH_] ON [PlayerMatch](
[PlayerMatchStart]  ASC,
[PlayerMatchEnd]  ASC,
[ServerMatchId]  ASC,
[AliasId]  ASC
)

CREATE TABLE [PlayerSession] (
[SessionId] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[ServerId] INTEGER  NULL,
[PlayerId] INTEGER  NULL,
[LastAliasId] INTEGER  NULL,
[Map] VARCHAR(50)  NULL,
[SessionStart] DateTime  NULL,
[SessionEnd] DateTime  NULL,
[FragCount] INTEGER  NULL,
[ShirtColor] VARCHAR(50)  NULL,
[PantColor] VARCHAR(50)  NULL,
[Latency] INTEGER  NULL,
[CurrentFrags] INTEGER  NULL,
[SessionDate] DATE  NULL
)

CREATE INDEX [player_sessinon_playerid] ON [PlayerSession](
[PlayerId]  ASC,
[ServerId]  ASC,
[LastAliasId]  ASC,
[SessionDate]  ASC
)

CREATE INDEX [serverid_sessionstart_sessionend] ON [PlayerSession](
[ServerId]  ASC,
[SessionStart]  ASC,
[SessionEnd]  ASC
)

CREATE INDEX [session_stats] ON [PlayerSession](
[Map]  ASC,
[SessionStart]  ASC,
[SessionEnd]  ASC,
[FragCount]  ASC
)

CREATE TABLE [ServerData] (
[ServerId] INTEGER  NOT NULL,
[TimeStamp] DATETIME  NOT NULL,
[Map] varchar(50)  NULL,
[Modification] varchar(50)  NULL,
[PlayerData] TEXT  NULL,
[ServerDataId] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[Name] VARCHAR(255)  NULL,
[MaxPlayers] INTEGER  NULL,
[ServerSettings] TEXT  NULL,
[IpAddress] VARCHAR(50)  NULL
)

CREATE INDEX [IDX_SERVERDATA_] ON [ServerData](
[ServerId]  ASC
)

CREATE TABLE [ServerMatch] (
[ServerMatchId] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[Map] varchar(50)  NULL,
[Modification] varchar(50)  NULL,
[MatchStart] DATETIME  NULL,
[MatchEnd] DATETIME  NULL,
[ServerId] INTEGER  NULL
)

CREATE INDEX [IDX_SERVERMATCH_] ON [ServerMatch](
[ServerId]  ASC,
[ServerMatchId]  ASC
)

CREATE TABLE [UserAccess] (
[UserId] INTEGER  PRIMARY KEY NULL,
[Name] VARCHAR(50)  NULL,
[Password] VARCHAR(50)  NULL,
[AccessLevel] INTEGER  NULL,
[LastIpAddress] VARCHAR(50)  NULL,
[LastUrl] TEXT  NULL,
[LastAccessed] DateTime NULL
)

CREATE VIEW [vMatchDetail] AS 
select
GameServer.GameId,
GameServer.DNS as HostName,
GameServer.Port as Port,
ServerData.Name as ServerName,
ServerMatch.ServerMatchId as MatchId,
ServerMatch.Map as Map,
ServerMatch.Modification as Modification,
ServerMatch.MatchStart as MatchStart,
ServerMatch.MatchEnd as MatchEnd,
player.Alias as Alias,
player.AliasBytes as AliasBytes,
player.PlayerId as PlayerId,
playermatch.PlayerMatchId as PlayerMatchId,
playermatch.Frags as Frags,
playermatch.PantColor as PantColor,
playermatch.ShirtColor as ShirtColor,
playermatch.Skin as Skin,
playermatch.Model as Model,
PlayerMatch.PlayerMatchStart as PlayerMatchStart,
PlayerMatch.PlayerMatchEnd as PlayerMatchEnd,
strftime('%s',ifnull(PlayerMatch.PlayerMatchEnd,datetime('now')))-strftime('%s',PlayerMatch.PlayerMatchStart) as PlayerStayDuration,
CASE WHEN Frags > 0 THEN
round(cast(playermatch.Frags as float)/cast((strftime('%s',PlayerMatch.PlayerMatchEnd)-strftime('%s',PlayerMatch.PlayerMatchStart)) / 60 as float),2)
ELSE 0

END as FPM
FROM PlayerMatch
inner join player on (playermatch.aliasid = player.aliasid)
inner join ServerMatch on (playermatch.ServerMatchId = ServerMatch.ServerMatchId)
inner join GameServer on (ServerMatch.ServerId = gameserver.serverid)
inner join ServerData on (ServerMatch.ServerId = ServerData.serverid)

CREATE VIEW [vPlayerDetail] AS 
SELECT
master_p.GameId as GameId,
alias_p.AliasBytes as AliasBytes,
alias_p.playerid as AliasPlayerId,
alias_p.alias as Alias,
master_p.PlayerId as PlayerId,
(select ifnull(SessionEnd,datetime('now'))
FROM playersession
where master_p.aliasid = LastAliasId order by sessionstart desc limit 1) as AliasLastSeen,
(SELECT GameServer.DNS
FROM playersession
INNER JOIN GameServer ON (GameServer.ServerId = PlayerSession.ServerId)
WHERE playerid = master_p.playerid order by sessionstart desc limit 1) as LastServer,

(select ifnull(SessionEnd,datetime('now'))
FROM playersession
where playerid = master_p.playerid order by sessionstart desc limit 1) as LastSeen,

(select Map
FROM playersession
where playerid = master_p.playerid order by sessionstart desc limit 1) as LastMap,

(select sum(strftime('%s',ifnull(SessionEnd,datetime('now')))-strftime('%s',sessionstart))
FROM playersession
WHERE playerid = master_p.playerId AND sessionstart >= date('now','start of year')) AS year_playtime_sum  ,

(select sum(fragcount) + sum(currentfrags)
FROM playersession
WHERE playerid = master_p.playerId AND sessionstart >= date('now','start of year')) AS year_frags_sum      ,

(select round(cast(sum(fragcount) + sum(currentfrags) as float)/cast((sum(strftime('%s',ifnull(SessionEnd,datetime('now')))-strftime('%s',sessionstart)) / 60) as float),2)
FROM playersession
WHERE playerid = master_p.playerId AND sessionstart >= date('now','start of year')) AS year_FPM      ,

(select sum(strftime('%s',ifnull(SessionEnd,datetime('now')))-strftime('%s',sessionstart))
FROM playersession
WHERE playerid = master_p.playerId AND sessionstart >= date('now','start of month')) AS month_playtime_sum  ,

(select sum(fragcount) + sum(currentfrags)
FROM playersession
WHERE playerid = master_p.playerId AND sessionstart >= date('now','start of month')) AS month_frags_sum,

(select round(cast(sum(fragcount) + sum(currentfrags) as float)/cast((sum(strftime('%s',ifnull(SessionEnd,datetime('now')))-strftime('%s',sessionstart)) / 60) as float),2)
FROM playersession
WHERE playerid = master_p.playerId AND sessionstart >= date('now','start of month')) AS month_FPM      ,

(select sum(strftime('%s',ifnull(SessionEnd,datetime('now')))-strftime('%s',sessionstart))
FROM playersession
WHERE playerid = master_p.playerId AND sessionstart >= date('now','weekday 0','-7 day')) AS week_playtime_sum,

(select sum(fragcount) + sum(currentfrags)
FROM playersession
WHERE playerid = master_p.playerId AND sessionstart >= date('now','weekday 0','-7 day')) AS week_frags_sum,

(select round(cast(sum(fragcount) + sum(currentfrags) as float)/cast((sum(strftime('%s',ifnull(SessionEnd,datetime('now')))-strftime('%s',sessionstart)) / 60) as float),2)
FROM playersession
WHERE playerid = master_p.playerId AND sessionstart >= date('now','weekday 0','-7 day')) AS week_FPM      ,

(select sum(strftime('%s',ifnull(SessionEnd,datetime('now')))-strftime('%s',sessionstart))
FROM playersession
WHERE playerid = master_p.playerId AND sessionstart >= date('now','start of day')) AS day_playtime_sum,

(select sum(fragcount) + sum(currentfrags)
FROM playersession
WHERE playerid = master_p.playerId AND sessionstart >= date('now','start of day')) AS day_frags_sum,

(select round(cast(sum(fragcount) + sum(currentfrags) as float)/cast((sum(strftime('%s',ifnull(SessionEnd,datetime('now')))-strftime('%s',sessionstart)) / 60) as float),2)
FROM playersession
WHERE playerid = master_p.playerId AND sessionstart >= date('now','start of day')) AS day_FPM      ,

(select sum(strftime('%s', ifnull(SessionEnd,datetime('now')))-strftime('%s',sessionstart))
FROM playersession
WHERE playerid = master_p.playerId ) AS playtime_sum,

(select sum(fragcount) + sum(currentfrags)
FROM playersession
WHERE playerid = master_p.playerId) AS frags_sum   ,

(select round(cast(sum(fragcount) + sum(currentfrags) as float)/cast((sum(strftime('%s',ifnull(SessionEnd,datetime('now')))-strftime('%s',sessionstart)) / 60) as float),2)
FROM playersession
WHERE playerid = master_p.playerId) AS FPM
FROM Player as master_p
LEFT OUTER JOIN Player as alias_p ON master_p.AliasId = alias_p.AliasId and (master_p.playerid = alias_p.playerid
OR (master_p.IpAddress = alias_p.IpAddress AND master_p.IpAddress <> '' AND master_p.IpAddress <> 'private'))
ORDER BY AliasLastSeen DESC

CREATE VIEW [vPlayerMatches] AS 
select
ServerMatch.ServerMatchId as MatchId,
ServerMatch.MatchStart as MatchStart,
GameServer.GameId as GameId,
GameServer.DNS as HostName,
GameServer.Port as Port,
ServerData.Name as ServerName,
GameServer.ServerId as ServerId,
ServerMatch.Map as Map,
ServerMatch.Modification as Modification,
Player.Alias as Alias,
Player.PlayerId as PlayerId,
playermatch.frags as Frags,
playermatch.PantColor as PantColor,
playermatch.ShirtColor as ShirtColor,
playermatch.Skin as Skin,
playermatch.Model as Model,
playermatch.playermatchstart as PlayerJoinTime,
strftime('%s',ifnull(PlayerMatch.PlayerMatchEnd,datetime('now')))-strftime('%s',PlayerMatch.PlayerMatchStart) as PlayerStayDuration
FROM PlayerMatch
inner join player on (playermatch.aliasid = player.aliasid)
inner join ServerMatch on (playermatch.ServerMatchId = ServerMatch.ServerMatchId)
inner join GameServer on (ServerMatch.ServerId = GameServer.serverid)
inner join ServerData on (ServerMatch.ServerId = ServerData.serverid)

CREATE VIEW [vPlayerSessionHourly] AS 
select ServerId as ServerId,
PlayerId as PlayerId,
Date(SessionDate) as SessionDate,
case
when (strftime('%s',datetime(date(sessionstart), '+0 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+0 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+1 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+1 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+0 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+1 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_zero,
case
when (strftime('%s',datetime(date(sessionstart), '+1 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+1 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+2 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+2 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+1 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+2 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_one,
case
when (strftime('%s',datetime(date(sessionstart), '+2 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+2 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+3 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+3 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+2 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+3 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_two,
case
when (strftime('%s',datetime(date(sessionstart), '+3 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+3 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+4 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+4 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+3 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+4 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_three,
case
when (strftime('%s',datetime(date(sessionstart), '+4 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+4 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+5 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+5 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+4 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+5 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_four,
case
when (strftime('%s',datetime(date(sessionstart), '+5 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+5 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+6 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+6 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+5 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+6 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_five,
case
when (strftime('%s',datetime(date(sessionstart), '+6 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+6 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+7 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+7 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+6 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+7 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_six,
case
when (strftime('%s',datetime(date(sessionstart), '+7 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+7 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+8 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+8 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+7 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+8 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_seven,
case
when (strftime('%s',datetime(date(sessionstart), '+8 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+8 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+9 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+9 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+8 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+9 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_eight,
case
when (strftime('%s',datetime(date(sessionstart), '+9 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+9 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+10 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+10 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+9 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+10 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_nine,
case
when (strftime('%s',datetime(date(sessionstart), '+10 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+10 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+11 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+11 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+10 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+11 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_ten,
case
when (strftime('%s',datetime(date(sessionstart), '+11 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+11 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+12 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+12 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+11 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+12 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_eleven,
case
when (strftime('%s',datetime(date(sessionstart), '+12 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+12 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+13 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+13 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+12 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+13 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_twelve,
case
when (strftime('%s',datetime(date(sessionstart), '+13 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+13 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+14 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+14 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+13 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+14 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_thirteen,
case
when (strftime('%s',datetime(date(sessionstart), '+14 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+14 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+15 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+15 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+14 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+15 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_fourteen,
case
when (strftime('%s',datetime(date(sessionstart), '+15 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+15 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+16 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+16 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+15 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+16 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_fifteen,
case
when (strftime('%s',datetime(date(sessionstart), '+16 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+16 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+17 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+17 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+16 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+17 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_sixteen,
case
when (strftime('%s',datetime(date(sessionstart), '+17 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+17 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+18 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+18 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+17 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+18 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_seventeen,
case
when (strftime('%s',datetime(date(sessionstart), '+18 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+18 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+19 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+19 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+18 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+19 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_eighteen,
case
when (strftime('%s',datetime(date(sessionstart), '+19 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+19 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+20 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+20 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+19 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+20 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_nineteen,
case
when (strftime('%s',datetime(date(sessionstart), '+20 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+20 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+21 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+21 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+20 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+21 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_twenty,
case
when (strftime('%s',datetime(date(sessionstart), '+21 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+21 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+22 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+22 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+21 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+22 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_twentyone,
case
when (strftime('%s',datetime(date(sessionstart), '+22 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+22 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+23 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+23 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+22 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+23 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_twentytwo,
case
when (strftime('%s',datetime(date(sessionstart), '+23 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+23 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+24 hours')) > strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+24 hours')) < strftime('%s',sessionend))
OR (strftime('%s',datetime(date(sessionstart), '+23 hours')) < strftime('%s',sessionstart)
AND strftime('%s',datetime(date(sessionstart), '+24 hours')) > strftime('%s',sessionend))
then 1 else 0 end as hour_twentythree
from playersession

CREATE VIEW [vPlayerSessionHourlySummery] AS 
select
psView.SessionDate as SessionDate,
psView.PlayerId as PlayerId,
psView.ServerId as ServerId,
sum(psView.hour_zero) as HourZeroSum,
sum(psView.hour_one) as HourOneSum,
sum(psView.hour_two) as HourTwoSum,
sum(psView.hour_three) as HourThreeSum,
sum(psView.hour_four) as HourFourSum,
sum(psView.hour_five) as HourFiveSum,
sum(psView.hour_six) as HourSixSum,
sum(psView.hour_seven) as HourSevenSum,
sum(psView.hour_eight) as HourEightSum,
sum(psView.hour_nine) as HourNineSum,
sum(psView.hour_ten) as HourTenSum,
sum(psView.hour_eleven) as HourElevenSum,
sum(psView.hour_twelve) as HourTwelveSum,
sum(psView.hour_thirteen) as HourThirteenSum,
sum(psView.hour_fourteen) as HourFourteenSum,
sum(psView.hour_fifteen) as HourFifteenSum,
sum(psView.hour_sixteen) as HourSixteenSum,
sum(psView.hour_seventeen) as HourSeventeenSum,
sum(psView.hour_eighteen) as HourEighteenSum,
sum(psView.hour_nineteen) as HourNineteenSum,
sum(psView.hour_twenty) as HourTwentySum,
sum(psView.hour_twentyone) as HourTwentyOneSum,
sum(psView.hour_twentytwo) as HourTwentyTwoSum,
sum(psView.hour_twentythree) as HourTwentyThreeSum
from vplayersessionhourly as psView
group by psView.SessionDate,psView.PlayerId,psView.ServerId



CREATE VIEW vServerDetail AS
SELECT
sd.ServerDataId as ServerDataId
,gs.ServerId as ServerId
,gs.CustomName as CustomName
,gs.CustomNameShort as CustomNameShort
,gs.DNS as DNS
,gs.Port as Port
,gs.GameId as GameId
,gs.PublicSiteUrl as PublicSiteUrl
,gs.MapDownloadUrl as MapDownloadUrl
,gs.Location as Location
,gs.QueryInterval as QueryInterval
,gs.FailedQueryAttempts as FailedQueryAttempts
,gs.Region as Region
,gs.ModificationCode as ModificationCode
,gs.Category as Category
,gs.QueryResult as CurrentStatus
,gs.CustomModification as CustomModificationName
,sd.Name as ServerName
,sd.Map as Map
,sd.ServerSettings as ServerSettings
,sd.Modification as Modification
,sd.PlayerData as PlayerData
,sd.Timestamp as Timestamp
,sd.MaxPlayers as MaxPlayers
,sd.IpAddress as IpAddress
from
GameServer gs
inner join ServerData  sd
ON gs.ServerId = sd.ServerId