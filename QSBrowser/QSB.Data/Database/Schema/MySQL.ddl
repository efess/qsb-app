DROP DATABASE servers;
CREATE DATABASE servers;
USE servers;

CREATE TABLE GameServer (
ServerId INTEGER AUTO_INCREMENT PRIMARY KEY,
GameId INTEGER ,
CustomName VARCHAR(255) ,
AntiWallHack INTEGER,
Port INTEGER ,
DNS VARCHAR(255),
PublicSiteUrl TEXT ,
MapDownloadUrl TEXT ,
Location VARCHAR(255) ,
QueryInterval INTEGER ,
FailedQueryAttempts INTEGER,
LastQuery DateTime ,
QueryResult INTEGER ,
Region varchar(50) ,
NextQuery DATETIME ,
LastQuerySuccess DATETIME ,
CustomNameShort VARCHAR(50),
ModificationCode VARCHAR(50),
Modification VARCHAR(50),
Category VARCHAR(50),
Active BOOL,
CustomModificationName VARCHAR(50)
);

CREATE INDEX IDX_GAMESERVER_ ON GameServer(
ServerId  ASC,
GameId ASC,	
Region  ASC
);

CREATE TABLE HistoricalHourlyLog (
HistoricalId INTEGER  PRIMARY KEY AUTO_INCREMENT NOT NULL,
Serverid INTEGER  NULL,
PlayerId INTEGER  NULL,
Hour0 INTEGER  NULL,
Hour1 INTEGER  NULL,
Hour2 INTEGER  NULL,
Hour3 INTEGER  NULL,
Hour4 INTEGER  NULL,
Hour5 INTEGER  NULL,
Hour6 INTEGER  NULL,
Hour7 INTEGER  NULL,
Hour8 INTEGER  NULL,
Hour9 INTEGER  NULL,
Hour10 INTEGER  NULL,
Hour11 INTEGER  NULL,
Hour12 INTEGER  NULL,
Hour13 INTEGER  NULL,
Hour14 INTEGER  NULL,
Hour15 INTEGER  NULL,
Hour16 INTEGER  NULL,
Hour17 INTEGER  NULL,
Hour18 INTEGER  NULL,
Hour19 INTEGER  NULL,
Hour20 INTEGER  NULL,
Hour21 INTEGER  NULL,
Hour22 INTEGER  NULL,
Hour23 INTEGER  NULL,
TotalHours INTEGER  NULL,
HistoricalDate DATE  NULL
);

CREATE INDEX IDX_HISTORICALHOURLYLOG_ ON HistoricalHourlyLog(
Serverid  ASC,
PlayerId  ASC,
HistoricalDate  ASC
);


CREATE TABLE Player (
AliasId INTEGER  PRIMARY KEY AUTO_INCREMENT NOT NULL,
PlayerId INTEGER  NULL,
Alias VARCHAR(50)  NULL,
AliasBytes BLOB  NULL,
IPAddress VARCHAR(21)  NULL,
LastPing INTEGER  NULL,
PlayerNumber INTEGER  NULL,
GameId INTEGER  NULL,
IdentifyIPAddress INTEGER NULL,
RestrictUserOption INTEGER NULL
);

CREATE INDEX IDX_PLAYER_ ON Player(
PlayerId  ASC,
AliasId  ASC,
IPAddress ASC
);

CREATE TABLE PlayerMatch (
PlayerMatchId INTEGER  NOT NULL PRIMARY KEY AUTO_INCREMENT,
PlayerId intEGER  NULL,
AliasId INTEGER  NULL,
PantColor INTEGER  NULL,
ShirtColor INTEGER  NULL,
Skin VARCHAR(50)  NULL,
Model VARCHAR(50)  NULL,
Latency intEGER  NULL,
Frags intEGER  NULL,
PlayerMatchStart DateTime  NULL,
PlayerMatchEnd DATETIME  NULL,
ServerMatchId INTEGER  NULL
);

CREATE INDEX IDX_PLAYERMATCH_ ON PlayerMatch(
ServerMatchId,
PlayerMatchStart,
PlayerMatchEnd,
PlayerId
);

CREATE INDEX IDX_PLAYERMATCH_PLAYERID ON PlayerMatch(
PlayerId  ASC,
AliasId  ASC
);

CREATE TABLE PlayerSession (
SessionId INTEGER  PRIMARY KEY AUTO_INCREMENT NOT NULL,
ServerId INTEGER  NULL,
PlayerId INTEGER  NULL,
LastAliasId INTEGER  NULL,
Map VARCHAR(50)  NULL,
SessionStart DateTime  NULL,
SessionEnd DateTime  NULL,
FragCount INTEGER  NULL,
ShirtColor VARCHAR(50)  NULL,
PantColor VARCHAR(50)  NULL,
Latency INTEGER  NULL,
CurrentFrags INTEGER  NULL,
SessionDate DATE  NULL
);

CREATE INDEX player_sessinon_playerid ON PlayerSession(
PlayerId  ASC,
ServerId  ASC,
LastAliasId  ASC,
SessionDate  ASC
);

CREATE INDEX player_sessinon_serverid ON PlayerSession(
ServerId
);

CREATE INDEX serverid_sessionstart_sessionend ON PlayerSession(
ServerId  ASC,
SessionStart  ASC,
SessionEnd  ASC
);

CREATE INDEX session_stats ON PlayerSession(
Map  ASC,
SessionStart  ASC,
SessionEnd  ASC,
FragCount  ASC
);

CREATE INDEX IDX_PLAYERSESSION_LASTALIAS ON PlayerSession(
LastAliasId ASC,
SessionStart ASC
);

CREATE TABLE ServerMatch (
ServerMatchId INTEGER  NOT NULL PRIMARY KEY AUTO_INCREMENT,
Map varchar(50)  NULL,
Modification VARCHAR(50)  NULL,
MatchStart DATETIME  NULL,
MatchEnd DATETIME  NULL,
ServerId INTEGER  NULL
);

CREATE INDEX IDX_SERVERMATCH_ ON ServerMatch(
ServerId,
ServerMatchId,
MatchStart
);

CREATE TABLE ServerData (
ServerDataId INTEGER NOT NULL PRIMARY KEY AUTO_INCREMENT,
ServerId INTEGER  NOT NULL,
TimeStamp DATETIME NOT NULL,
Map varchar(50) NULL,
Modification varchar(50) NULL,
PlayerData TEXT NULL,
ServerSettings TEXT NULL,
MaxPlayers INTEGER NULL,
IpAddress VARCHAR(50) NULL,
Name VARCHAR(255) NULL
);

CREATE INDEX IDX_SERVERDATA_ ON ServerData(
ServerId  ASC
);

CREATE TABLE UserAccess (
UserId INTEGER  PRIMARY KEY NULL,
Name VARCHAR(50)  NULL,
Password VARCHAR(50)  NULL,
AccessLevel INTEGER  NULL,
LastIpAddress VARCHAR(50)  NULL,
LastUrl TEXT  NULL,
LastAccessed DateTime NULL
);



DROP PROCEDURE IF EXISTS spMatchDetail;

DELIMITER //

CREATE PROCEDURE spMatchDetail (
IN p_matchId INTEGER)
BEGIN
	select
	GameServer.GameId,
	GameServer.DNS as HostName,
	GameServer.Port as Port,
	ServerData.Name as ServerName,
	ServerMatch.ServerMatchId as MatchId,
	ServerMatch.Map as Map,
	ServerMatch.Modification as Modification,
	UNIX_TIMESTAMP(ServerMatch.MatchStart) as MatchStart,
	UNIX_TIMESTAMP(ServerMatch.MatchEnd) as MatchEnd,
	Player.Alias as Alias,
	Player.AliasBytes as AliasBytes,
	Player.PlayerId as PlayerId,
	PlayerMatch.PlayerMatchId as PlayerMatchId,
	PlayerMatch.Frags as Frags,
	PlayerMatch.PantColor as PantColor,
	PlayerMatch.ShirtColor as ShirtColor,
	PlayerMatch.Skin as Skin,
	PlayerMatch.Model as Model,
	UNIX_TIMESTAMP(PlayerMatch.PlayerMatchStart) as PlayerMatchStart,
	UNIX_TIMESTAMP(PlayerMatch.PlayerMatchEnd) as PlayerMatchEnd,
	TIMESTAMPDIFF(SECOND, PlayerMatch.PlayerMatchStart, ifnull(PlayerMatch.PlayerMatchEnd,UTC_TIMESTAMP())) as PlayerStayDuration,
	CASE WHEN Frags > 0 THEN
	round(cast(PlayerMatch.Frags as DECIMAL)/cast((TIMESTAMPDIFF(SECOND, PlayerMatch.PlayerMatchStart, PlayerMatch.PlayerMatchEnd)) / 60 as DECIMAL),2)
	ELSE 0
	END as FPM
	FROM PlayerMatch
	inner join Player on (PlayerMatch.AliasId = Player.AliasId)
	inner join ServerMatch on (PlayerMatch.ServerMatchId = ServerMatch.ServerMatchId)
	inner join GameServer on (ServerMatch.ServerId = GameServer.ServerId)
	inner join ServerData on (ServerMatch.ServerId = ServerData.ServerId)
	WHERE PlayerMatch.ServerMatchId = p_matchId;

END //
DELIMITER ;


DROP PROCEDURE IF EXISTS spPlayerDetail;

DELIMITER //
CREATE PROCEDURE spPlayerDetail (
IN p_playerId INTEGER)
BEGIN

	SELECT
	master_p.GameId as GameId,
	alias_p.AliasBytes as AliasBytes,
	alias_p.PlayerId as AliasPlayerId,
	alias_p.Alias as Alias,
	master_p.PlayerId as PlayerId,
	(select UNIX_TIMESTAMP(ifnull(SessionEnd,UTC_TIMESTAMP()))
	FROM PlayerSession

	where master_p.AliasId = LastAliasId order by SessionStart desc limit 1) as AliasLastSeen,
	(SELECT GameServer.DNS
	FROM PlayerSession
	INNER JOIN GameServer ON (GameServer.ServerId = PlayerSession.ServerId)
	WHERE PlayerId = master_p.PlayerId order by SessionStart desc limit 1) as LastServer,

	(select UNIX_TIMESTAMP(ifnull(SessionEnd,UTC_TIMESTAMP()))
	FROM PlayerSession
	where PlayerId = master_p.PlayerId order by SessionStart desc limit 1) as LastSeen,

	(select Map
	FROM PlayerSession
	where PlayerId = master_p.PlayerId order by SessionStart desc limit 1) as LastMap,

	(
		SELECT sum( TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP()))  )
		FROM PlayerSession
		WHERE
				PlayerId = master_p.PlayerId
			AND 	SessionStart >= Date_Add(Current_Date(),INTERVAL -(DayOfYear(Current_Date())-1 ) DAY)
	) AS year_playtime_sum  ,

	(select sum(FragCount) + sum(CurrentFrags)
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId AND SessionStart >= Date_Add(Current_Date(),INTERVAL -(DayOfYear(Current_Date())-1 ) DAY)) AS year_frags_sum      ,

	(select round(cast(sum(FragCount) + sum(CurrentFrags) as DECIMAL)/cast(( sum( TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP()))  ) / 60) as DECIMAL),2)
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId AND SessionStart >= Date_Add(Current_Date(),INTERVAL -(DayOfYear(Current_Date())-1 ) DAY)) AS year_FPM      ,

	(select sum( TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP())))
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId AND SessionStart >= Date_Add(Current_Date(),INTERVAL -(DayOfMonth(Current_Date())-1 ) DAY)) AS month_playtime_sum  ,

	(select sum(FragCount) + sum(CurrentFrags)
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId AND SessionStart >= Date_Add(Current_Date(),INTERVAL -(DayOfMonth(Current_Date())-1 ) DAY)) AS month_frags_sum,

	(select round(cast(sum(FragCount) + sum(CurrentFrags) as DECIMAL)/cast(( sum( TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP()))  ) / 60) as DECIMAL),2)
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId AND SessionStart >= Date_Add(Current_Date(),INTERVAL -(DayOfMonth(Current_Date())-1 ) DAY)) AS month_FPM      ,

	(select sum( TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP()))  )
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId AND SessionStart >= Date_Add(Current_Date(),INTERVAL -(DayOfWeek(Current_Date())-1 ) DAY)) AS week_playtime_sum,

	(select sum(FragCount) + sum(CurrentFrags)
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId AND SessionStart >= Date_Add(Current_Date(),INTERVAL -(DayOfWeek(Current_Date())-1 ) DAY)) AS week_frags_sum,

	(select round(cast(sum(FragCount) + sum(CurrentFrags) as DECIMAL)/cast(( sum( TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP()))  ) / 60) as DECIMAL),2)
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId AND SessionStart >= Date_Add(Current_Date(),INTERVAL -(DayOfWeek(Current_Date())-1 ) DAY)) AS week_FPM      ,

	(select  sum( TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP()))  )
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId AND SessionStart >= Current_Date()) AS day_playtime_sum,

	(select sum(FragCount) + sum(CurrentFrags)
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId AND SessionStart >= Current_Date()) AS day_frags_sum,

	(select round(cast(sum(FragCount) + sum(CurrentFrags) as DECIMAL)/cast(( sum( TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP()))  ) / 60) as DECIMAL),2)
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId AND SessionStart >= Current_Date()) AS day_FPM      ,

	(select  sum( TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP()))  )
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId ) AS playtime_sum,

	(select sum(FragCount) + sum(CurrentFrags)
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId) AS frags_sum   ,

	(select round(cast(sum(FragCount) + sum(CurrentFrags) as DECIMAL)/cast(( sum( TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP()))  ) / 60) as DECIMAL),2)
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId) AS FPM
	FROM Player as master_p
	LEFT OUTER JOIN Player as alias_p ON master_p.AliasId = alias_p.AliasId and (master_p.PlayerId = alias_p.PlayerId
	OR (master_p.IpAddress = alias_p.IpAddress AND master_p.IpAddress <> '' AND master_p.IpAddress <> 'private'))
	WHERE master_p.PlayerId = p_playerId
	ORDER BY AliasLastSeen DESC;


END //
DELIMITER ;



DROP PROCEDURE IF EXISTS spPlayerMatches;

DELIMITER //
CREATE PROCEDURE spPlayerMatches (
IN p_playerId INTEGER)
BEGIN
     SELECT
	ServerMatch.ServerMatchId as MatchId,
	UNIX_TIMESTAMP(ServerMatch.MatchStart) as MatchStart,
	GameServer.GameId as GameId,
	GameServer.DNS as HostName,
	GameServer.Port As Port,
	ServerData.Name as ServerName,
	GameServer.ServerId as ServerId,
	ServerMatch.Map as Map,
	ServerMatch.Modification as Modification,
	Player.Alias as Alias,
	Player.PlayerId as PlayerId,
	PlayerMatch.Frags as Frags,
	PlayerMatch.PantColor as PantColor,
	PlayerMatch.ShirtColor as ShirtColor,
	PlayerMatch.Skin as Skin,
	PlayerMatch.Model as Model,
	UNIX_TIMESTAMP(PlayerMatch.PlayerMatchStart) as PlayerJoinTime,
	TIMESTAMPDIFF(SECOND, PlayerMatch.PlayerMatchStart, ifnull(PlayerMatch.PlayerMatchEnd,UTC_TIMESTAMP())) as PlayerStayDuration
	FROM PlayerMatch
	inner join Player on (PlayerMatch.AliasId = Player.AliasId)
	inner join ServerMatch on (PlayerMatch.ServerMatchId = ServerMatch.ServerMatchId)
	inner join GameServer on (ServerMatch.ServerId = GameServer.ServerId)
	inner join ServerData on (ServerMatch.ServerId = ServerData.ServerId)
	WHERE PlayerMatch.PlayerId = p_playerId
	ORDER BY UNIX_TIMESTAMP(ServerMatch.MatchStart) DESC
	LIMIT 13;
END //
DELIMITER ;

DROP VIEW IF EXISTS vPlayerSessionHourly;

CREATE VIEW vPlayerSessionHourly AS 
select ServerId as ServerId,
PlayerId as PlayerId,
Date(SessionDate) as SessionDate,
case
when ( TIMESTAMPADD(HOUR, 0 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 0 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 1 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 1 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 0 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 1 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_zero,
case
when ( TIMESTAMPADD(HOUR, 1 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 1 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 2 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 2 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 1 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 2 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_one,
case
when ( TIMESTAMPADD(HOUR, 2 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 2 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 3 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 3 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 2 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 3 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_two,
case
when ( TIMESTAMPADD(HOUR, 3 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 3 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 4 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 4 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 3 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 4 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_three,
case
when ( TIMESTAMPADD(HOUR, 4 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 4 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 5 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 5 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 4 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 5 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_four,
case
when ( TIMESTAMPADD(HOUR, 5 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 5 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 6 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 6 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 5 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 6 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_five,
case
when ( TIMESTAMPADD(HOUR, 6 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 6 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 7 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 7 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 6 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 7 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_six,
case
when ( TIMESTAMPADD(HOUR, 7 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 7 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 8 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 8 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 7 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 8 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_seven,
case
when ( TIMESTAMPADD(HOUR, 8 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 8 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 9 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 9 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 8 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 9 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_eight,
case
when ( TIMESTAMPADD(HOUR, 9 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 9 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 10 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 10 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 9 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 10 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_nine,
case
when ( TIMESTAMPADD(HOUR, 10 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 10 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 11 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 11 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 10 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 11 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_ten,
case
when ( TIMESTAMPADD(HOUR, 11 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 11 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 12 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 12 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 11 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 12 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_eleven,
case
when ( TIMESTAMPADD(HOUR, 12 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 12 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 13 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 13 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 12 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 13 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_twelve,
case
when ( TIMESTAMPADD(HOUR, 13 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 13 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 14 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 14 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 13 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 14 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_thirteen,
case
when ( TIMESTAMPADD(HOUR, 14 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 14 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 15 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 15 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 14 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 15 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_fourteen,
case
when ( TIMESTAMPADD(HOUR, 15 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 15 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 16 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 16 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 15 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 16 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_fifteen,
case
when ( TIMESTAMPADD(HOUR, 16 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 16 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 17 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 17 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 16 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 17 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_sixteen,
case
when ( TIMESTAMPADD(HOUR, 17 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 17 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 18 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 18 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 17 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 18 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_seventeen,
case
when ( TIMESTAMPADD(HOUR, 18 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 18 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 19 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 19 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 18 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 19 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_eighteen,
case
when ( TIMESTAMPADD(HOUR, 19 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 19 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 20 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 20 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 19 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 20 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_nineteen,
case
when ( TIMESTAMPADD(HOUR, 20 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 20 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 21 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 21 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 20 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 21 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_twenty,
case
when ( TIMESTAMPADD(HOUR, 21 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 21 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 22 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 22 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 21 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 22 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_twentyone,
case 
when ( TIMESTAMPADD(HOUR, 22 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 22 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 23 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 23 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 22 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 23 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_twentytwo,
case
when ( TIMESTAMPADD(HOUR, 23 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 23 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 24 , DATE(SessionStart)) > SessionStart
AND  TIMESTAMPADD(HOUR, 24 , DATE(SessionStart)) < SessionEnd)
OR ( TIMESTAMPADD(HOUR, 23 , DATE(SessionStart)) < SessionStart
AND  TIMESTAMPADD(HOUR, 24 , DATE(SessionStart)) > SessionEnd)
then 1 else 0 end as hour_twentythree
from PlayerSession;

CREATE VIEW vPlayerSessionHourlySummery AS
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
from vPlayerSessionHourly as psView
group by 
	psView.SessionDate,
	psView.PlayerId,
	psView.ServerId;


DROP VIEW IF EXISTS vServerDetail;

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
,gs.CustomModificationName as CustomModificationName
,sd.Name as ServerName
,sd.Map as Map
,sd.ServerSettings as ServerSettings
,sd.Modification as Modification
,sd.PlayerData as PlayerData
,UNIX_TIMESTAMP(sd.Timestamp) as Timestamp
,sd.MaxPlayers as MaxPlayers
,sd.IpAddress as IpAddress
from
GameServer gs
inner join ServerData  sd
ON gs.ServerId = sd.ServerId;



DROP PROCEDURE IF EXISTS spAddUpdateGameServer;

DELIMITER //

CREATE PROCEDURE spAddUpdateGameServer (
IN p_serverId INTEGER,
IN p_gameId INTEGER,
IN p_customName VARCHAR(255),
IN p_antiWallHack INTEGER,
IN p_port INTEGER,
IN p_dns VARCHAR(255),
IN p_publicSiteUrl TEXT,
IN p_mapDownloadUrl TEXT,
IN p_location VARCHAR(255),
IN p_queryInterval INTEGER,
IN p_region VARCHAR(50),
IN p_nextQuery DATETIME,
IN p_customNameShort VARCHAR(50),
IN p_modificationCode VARCHAR(50),
IN p_category VARCHAR(50),
IN p_active BOOL,
IN p_customModificationName VARCHAR(50)
)
BEGIN

	IF p_serverId = -1 THEN
	  INSERT INTO GameServer (GameId, CustomName, AntiWallHack, Port, DNS, PublicSiteUrl, MapDownloadUrl, Location, QueryInterval, Region, CustomNameShort, ModificationCode, Category, Active, CustomModificationName)
	  VALUES (p_gameId, p_customName, p_antiWallHack, p_port, p_dns, p_publicSiteUrl, p_mapDownloadUrl, p_location, p_queryInterval, p_region, p_customNameShort, p_modificationCode, p_category, p_active, p_customModificationName);

	ELSE
	
	UPDATE GameServer SET GameId = p_gameId, CustomName = p_customName, AntiWallHack = p_antiWallHack, Port = p_port, DNS = p_dns, PublicSiteUrl = p_publicSiteUrl,
	  MapDownloadUrl = p_mapDownloadUrl, Location = p_location, QueryInterval = p_queryInterval, Region = p_region, CustomNameShort = p_customNameShort,
	  ModificationCode = p_modificationCode, Category = p_category, Active = p_active, CustomModificationName = p_customModificationName, NextQuery = p_nextQuery
	WHERE ServerId = p_serverId;

	END IF;
END //
DELIMITER ;  



DROP PROCEDURE IF EXISTS spRemoveGameServer;

DELIMITER //

CREATE PROCEDURE spRemoveGameServer (
IN p_serverId INTEGER
)
BEGIN

	DELETE FROM GameServer WHERE ServerId = p_serverId;

END //
DELIMITER ;  

DROP PROCEDURE IF EXISTS spPlayerHourlySummery;

DELIMITER //

CREATE PROCEDURE spPlayerHourlySummery (
	IN p_playerId INTEGER,
	IN p_fromDate DATE,
	IN p_toDate DATE)

BEGIN

	SELECT
	
	'ALL' as ServerId,
	'' as DNS,
	'' as Port,
	'' as CustomName,
	'' as ServerName,
	sum(Hour0) as hour0,
	sum(Hour1) as hour1,
	sum(Hour2) as hour2,
	sum(Hour3) as hour3,
	sum(Hour4) as hour4,
	sum(Hour5) as hour5,
	sum(Hour6) as hour6,
	sum(Hour7) as hour7,
	sum(Hour8) as hour8,
	sum(Hour9) as hour9,
	sum(Hour10) as hour10,
	sum(Hour11) as hour11,
	sum(Hour12) as hour12,
	sum(Hour13) as hour13,
	sum(Hour14) as hour14,
	sum(Hour15) as hour15,
	sum(Hour16) as hour16,
	sum(Hour17) as hour17,
	sum(Hour18) as hour18,
	sum(Hour19) as hour19,
	sum(Hour20) as hour20,
	sum(Hour21) as hour21,
	sum(Hour22) as hour22,
	sum(Hour23) as hour23,
	sum(TotalHours) as totalHours
	
	FROM HistoricalHourlyLog
	WHERE PlayerId = p_playerId
	AND HistoricalDate >= p_fromDate
	AND HistoricalDate <= p_toDate
	
	UNION
	
	SELECT
	
	GameServer.ServerId as ServerId,
	GameServer.DNS as DNS,
	GameServer.Port as Port,
	GameServer.CustomName as CustomName,
	ServerData.Name as ServerName,
	sum(Hour0) as hour0,
	sum(Hour1) as hour1,
	sum(Hour2) as hour2,
	sum(Hour3) as hour3,
	sum(Hour4) as hour4,
	sum(Hour5) as hour5,
	sum(Hour6) as hour6,
	sum(Hour7) as hour7,
	sum(Hour8) as hour8,
	sum(Hour9) as hour9,
	sum(Hour10) as hour10,
	sum(Hour11) as hour11,
	sum(Hour12) as hour12,
	sum(Hour13) as hour13,
	sum(Hour14) as hour14,
	sum(Hour15) as hour15,
	sum(Hour16) as hour16,
	sum(Hour17) as hour17,
	sum(Hour18) as hour18,
	sum(Hour19) as hour19,
	sum(Hour20) as hour20,
	sum(Hour21) as hour21,
	sum(Hour22) as hour22,
	sum(Hour23) as hour23,
	sum(TotalHours) as totalHours
	
	FROM HistoricalHourlyLog
		INNER JOIN GameServer on GameServer.ServerId = HistoricalHourlyLog.ServerId
		INNER JOIN ServerData on ServerData.ServerId = HistoricalHourlyLog.ServerId
	WHERE PlayerId = p_playerId
	AND HistoricalDate >= p_fromDate
	AND HistoricalDate <= p_toDate
	GROUP BY ServerId;

END //
DELIMITER ;


DROP PROCEDURE IF EXISTS spServerHourlySummery;

DELIMITER //

CREATE PROCEDURE spServerHourlySummery (
	IN p_serverId INTEGER,
	IN p_fromDate DATE,
	IN p_toDate DATE)

BEGIN
	SELECT
	
	GameServer.ServerId as ServerId,
	GameServer.DNS as DNS,
	GameServer.Port as Port,
	GameServer.CustomName as CustomName,
	ServerData.Name as ServerName,
	sum(Hour0) as hour0,
	sum(Hour1) as hour1,
	sum(Hour2) as hour2,
	sum(Hour3) as hour3,
	sum(Hour4) as hour4,
	sum(Hour5) as hour5,
	sum(Hour6) as hour6,
	sum(Hour7) as hour7,
	sum(Hour8) as hour8,
	sum(Hour9) as hour9,
	sum(Hour10) as hour10,
	sum(Hour11) as hour11,
	sum(Hour12) as hour12,
	sum(Hour13) as hour13,
	sum(Hour14) as hour14,
	sum(Hour15) as hour15,
	sum(Hour16) as hour16,
	sum(Hour17) as hour17,
	sum(Hour18) as hour18,
	sum(Hour19) as hour19,
	sum(Hour20) as hour20,
	sum(Hour21) as hour21,
	sum(Hour22) as hour22,
	sum(Hour23) as hour23,
	sum(TotalHours) as totalHours
	
	FROM HistoricalHourlyLog
		INNER JOIN GameServer on GameServer.ServerId = HistoricalHourlyLog.ServerId
		INNER JOIN ServerData on ServerData.ServerId = HistoricalHourlyLog.ServerId
	WHERE ServerId = p_serverId
	AND HistoricalDate >= p_fromDate
	AND HistoricalDate <= p_toDate;

END //
DELIMITER ;




DROP PROCEDURE IF EXISTS spPlayerLookup;

DELIMITER //

CREATE PROCEDURE spPlayerLookup (
	IN p_aliasPart TEXT)

BEGIN
	SELECT 	PlayerId,
		AliasBytes,
		Alias
	
	FROM 	Player
	WHERE	Alias like CONCAT(p_aliasPart, '%')
	LIMIT 	10;

END //
DELIMITER ;
DROP PROCEDURE IF EXISTS spPlayerLookup;

DELIMITER //

CREATE PROCEDURE spPlayerLookup (
	IN p_aliasPart TEXT)

BEGIN
	
	SELECT 	PlayerId,
		AliasBytes,
		Alias

	FROM 	Player
	WHERE	Alias = p_aliasPart	
	UNION
	SELECT 	PlayerId,
		AliasBytes,
		Alias
	
	FROM 	Player
	WHERE	Alias like CONCAT(p_aliasPart, '%')
	LIMIT 	10;

END //
DELIMITER ;


DROP TABLE IF EXISTS StatsServerMatchFrags;

CREATE TABLE StatsServerMatchFrags (
	StatsServerMatchFragsId INTEGER PRIMARY KEY AUTO_INCREMENT NOT NULL,
	Date DATETIME NULL,
	Span varchar(50) NULL,
	ServerId INTEGER NULL,
	PlayerId INTEGER NULL,
	ServerMatchId INTEGER NULL,
	Position INTEGER NULL,
	Frags INTEGER NULL
);


DROP TABLE IF EXISTS StatsServerMatchFPM;

CREATE TABLE StatsServerMatchFPM (
	StatsServerMatchFPMId INTEGER PRIMARY KEY AUTO_INCREMENT NOT NULL,
	Date DATETIME  NULL,
	Span varchar(50) NULL,
	ServerId INTEGER NULL,
	PlayerId INTEGER NULL,
	ServerMatchId INTEGER NULL,
	Position INTEGER NULL,
	FPM FLOAT NULL
);


DROP TABLE IF EXISTS StatsServerTimePlayed;

CREATE TABLE StatsServerTimePlayed (
	StatsServerTimePlayedId INTEGER PRIMARY KEY AUTO_INCREMENT NOT NULL,
	Date DATETIME  NULL,
	Span varchar(50) NULL,
	ServerId INTEGER NULL,
	PlayerId INTEGER NULL,
	Position INTEGER NULL,
	TimePlayed INTEGER NULL
);

DROP TABLE IF EXISTS StatsServerFrags;

CREATE TABLE StatsServerFrags (
	StatsServerFragsId INTEGER PRIMARY KEY AUTO_INCREMENT NOT NULL,
	Date DATETIME  NULL,
	Span varchar(50) NULL,
	ServerId INTEGER NULL,
	PlayerId INTEGER NULL,
	Position INTEGER NULL,
	Frags INTEGER NULL
);


DROP PROCEDURE IF EXISTS spProcessServerStats;

DELIMITER //

CREATE PROCEDURE spProcessServerStats (
	IN p_date DATETIME,
	IN p_serverId INTEGER)

BEGIN
	DELETE FROM StatsServerMatchFrags WHERE Date = p_date AND ServerId = p_serverId;
	DELETE FROM StatsServerMatchFPM WHERE Date = p_date AND ServerId = p_serverId;
	DELETE FROM StatsServerTimePlayed WHERE Date = p_date AND ServerId = p_serverId;
	DELETE FROM StatsServerFrags WHERE Date = p_date AND ServerId = p_serverId;
	
	-- Frags / Week
		INSERT INTO StatsServerMatchFrags (Date, Span, ServerId, PlayerId, ServerMatchId, Frags, Position)
		SELECT t.*, @rownum:=@rownum+1 FROM
			(SELECT			p_date,
						'Week' as Span,
						p_serverId,
						pm.PlayerId,
						sm.ServerMatchId,
						Frags
			FROM 			PlayerMatch as pm
			INNER JOIN		ServerMatch as sm
				ON		sm.ServerMatchId = pm.ServerMatchId
			INNER JOIN		GameServer as gs
				ON		gs.ServerId = sm.ServerId
			WHERE			pm.PlayerMatchStart >= Date_Add(p_date,INTERVAL -(DayOfWeek(p_date)-1 ) DAY)
			AND			pm.PlayerMatchStart <= p_date			
			AND			sm.ServerId = p_serverId
			ORDER BY		Frags DESC
			LIMIT			10) as t, (SELECT @rownum:=0) as r;
	
	-- Frags / Month
		INSERT INTO StatsServerMatchFrags (Date, Span, ServerId, PlayerId, ServerMatchId, Frags, Position)
		SELECT t.*, @rownum:=@rownum+1 FROM
			(SELECT			p_date,
						'Month' as Span,
						p_serverId,
						pm.PlayerId,
						sm.ServerMatchId,
						Frags
			FROM 			PlayerMatch as pm
			INNER JOIN		ServerMatch as sm
				ON		sm.ServerMatchId = pm.ServerMatchId
			INNER JOIN		GameServer as gs
				ON		gs.ServerId = sm.ServerId
			WHERE			pm.PlayerMatchStart >= Date_Add(p_date,INTERVAL -(DayOfmonth(p_date)-1 ) DAY)
			AND			pm.PlayerMatchStart <= p_date
			AND			gs.ServerId = p_serverId
			ORDER BY		Frags DESC
			LIMIT			10) as t, (SELECT @rownum:=0) as r;
	-- Frags / Year
		INSERT INTO StatsServerMatchFrags (Date, Span, ServerId, PlayerId, ServerMatchId, Frags, Position)
		SELECT t.*, @rownum:=@rownum+1 FROM
			(SELECT			p_date,
						'Year' as Span,
						p_serverId,
						pm.PlayerId,
						sm.ServerMatchId,
						Frags
			FROM 			PlayerMatch as pm
			INNER JOIN		ServerMatch as sm
				ON		sm.ServerMatchId = pm.ServerMatchId
			INNER JOIN		GameServer as gs
				ON		gs.ServerId = sm.ServerId
			WHERE			pm.PlayerMatchStart >= Date_Add(p_date,INTERVAL -(DayOfYear(p_date)-1 ) DAY)
			AND			pm.PlayerMatchStart <= p_date
			AND			gs.ServerId = p_serverId
			ORDER BY		Frags DESC
			LIMIT			10) as t, (SELECT @rownum:=0) as r;
	
	-- Frags / AllTime
		INSERT INTO StatsServerMatchFrags (Date, Span, ServerId, PlayerId, ServerMatchId, Frags, Position)
		SELECT t.*, @rownum:=@rownum+1 FROM
			(SELECT			p_date,
						'AllTime' as Span,
						p_serverId,
						pm.PlayerId,
						sm.ServerMatchId,
						Frags
			FROM 			PlayerMatch as pm
			INNER JOIN		ServerMatch as sm
				ON		sm.ServerMatchId = pm.ServerMatchId
			INNER JOIN		GameServer as gs
				ON		gs.ServerId = sm.ServerId
			WHERE			pm.PlayerMatchStart <= p_date			
			AND			gs.ServerId = p_serverId
			ORDER BY		Frags DESC
			LIMIT			10) as t, (SELECT @rownum:=0) as r;

-- -----------------------------------------------	

	-- FPM / Week
		INSERT INTO StatsServerMatchFPM (Date, Span, ServerId, PlayerId, ServerMatchId, FPM, Position)
		SELECT t.*, @rownum:=@rownum+1 FROM
			(SELECT			p_date,
						'Week' as Span,
						p_serverId,
						pm.PlayerId,
						sm.ServerMatchId,
						round(cast(pm.Frags as DECIMAL)/cast(( TIMESTAMPDIFF(SECOND, pm.PlayerMatchStart, ifnull(sm.MatchEnd,UTC_TIMESTAMP())) / 60) as DECIMAL),2)
			FROM 			PlayerMatch as pm
			INNER JOIN		ServerMatch as sm
				ON		sm.ServerMatchId = pm.ServerMatchId
			INNER JOIN		GameServer as gs
				ON		gs.ServerId = sm.ServerId
			WHERE			pm.PlayerMatchStart >= Date_Add(p_date,INTERVAL -(DayOfWeek(p_date)-1 ) DAY)
			AND			pm.PlayerMatchStart <= p_date			
			AND			gs.ServerId = p_serverId
			ORDER BY		round(cast(pm.Frags as DECIMAL)/cast(( TIMESTAMPDIFF(SECOND, pm.PlayerMatchStart, ifnull(sm.MatchEnd,UTC_TIMESTAMP())) / 60) as DECIMAL),2) DESC
			LIMIT			10) as t, (SELECT @rownum:=0) as r;

	-- FPM / Month
		INSERT INTO StatsServerMatchFPM (Date, Span, ServerId, PlayerId, ServerMatchId, FPM, Position)
		SELECT t.*, @rownum:=@rownum+1 FROM
			(SELECT			p_date,
						'Month' as Span,
						p_serverId,
						pm.PlayerId,
						sm.ServerMatchId,
						round(cast(pm.Frags as DECIMAL)/cast(( TIMESTAMPDIFF(SECOND, pm.PlayerMatchStart, ifnull(sm.MatchEnd,UTC_TIMESTAMP())) / 60) as DECIMAL),2)
			FROM 			PlayerMatch as pm
			INNER JOIN		ServerMatch as sm
				ON		sm.ServerMatchId = pm.ServerMatchId
			INNER JOIN		GameServer as gs
				ON		gs.ServerId = sm.ServerId
			WHERE			pm.PlayerMatchStart >= Date_Add(p_date,INTERVAL -(DayOfmonth(p_date)-1 ) DAY)
			AND			pm.PlayerMatchStart <= p_date			
			AND			gs.ServerId = p_serverId
			ORDER BY		round(cast(pm.Frags as DECIMAL)/cast(( TIMESTAMPDIFF(SECOND, pm.PlayerMatchStart, ifnull(sm.MatchEnd,UTC_TIMESTAMP())) / 60) as DECIMAL),2) DESC
			LIMIT			10) as t, (SELECT @rownum:=0) as r;
	-- FPM / Year
		INSERT INTO StatsServerMatchFPM (Date, Span, ServerId, PlayerId, ServerMatchId, FPM, Position)
		SELECT t.*, @rownum:=@rownum+1 FROM
			(SELECT			p_date,
						'Year' as Span,
						p_serverId,
						pm.PlayerId,
						sm.ServerMatchId,
						round(cast(pm.Frags as DECIMAL)/cast(( TIMESTAMPDIFF(SECOND, pm.PlayerMatchStart, ifnull(sm.MatchEnd,UTC_TIMESTAMP())) / 60) as DECIMAL),2)
			FROM 			PlayerMatch as pm
			INNER JOIN		ServerMatch as sm
				ON		sm.ServerMatchId = pm.ServerMatchId
			INNER JOIN		GameServer as gs
				ON		gs.ServerId = sm.ServerId
			WHERE			pm.PlayerMatchStart >= Date_Add(p_date,INTERVAL -(DayOfYear(p_date)-1 ) DAY)
			AND			pm.PlayerMatchStart <= p_date			
			AND			gs.ServerId = p_serverId
			ORDER BY		round(cast(pm.Frags as DECIMAL)/cast(( TIMESTAMPDIFF(SECOND, pm.PlayerMatchStart, ifnull(sm.MatchEnd,UTC_TIMESTAMP())) / 60) as DECIMAL),2) DESC
			LIMIT			10) as t, (SELECT @rownum:=0) as r;

	-- FPM / AllTime
		INSERT INTO StatsServerMatchFPM (Date, Span, ServerId, PlayerId, ServerMatchId, FPM, Position)
		SELECT t.*, @rownum:=@rownum+1 FROM
			(SELECT			p_date,
						'AllTime' as Span,
						p_serverId,
						pm.PlayerId,
						sm.ServerMatchId,
						round(cast(pm.Frags as DECIMAL)/cast(( TIMESTAMPDIFF(SECOND, pm.PlayerMatchStart, ifnull(sm.MatchEnd,UTC_TIMESTAMP())) / 60) as DECIMAL),2)
			FROM 			PlayerMatch as pm
			INNER JOIN		ServerMatch as sm
				ON		sm.ServerMatchId = pm.ServerMatchId
			INNER JOIN		GameServer as gs
				ON		gs.ServerId = sm.ServerId
			WHERE			pm.PlayerMatchStart <= p_date			
			AND			gs.ServerId = p_serverId
			ORDER BY		round(cast(pm.Frags as DECIMAL)/cast(( TIMESTAMPDIFF(SECOND, pm.PlayerMatchStart, ifnull(sm.MatchEnd,UTC_TIMESTAMP())) / 60) as DECIMAL),2) DESC
			LIMIT			10) as t, (SELECT @rownum:=0) as r;

-- ------------------------------------------

	-- TimeSpent / Week
		INSERT INTO StatsServerTimePlayed (Date, Span, ServerId, PlayerId, TimePlayed, Position )
		SELECT t.*, @rownum:=@rownum+1 FROM
			(SELECT			p_date,
						'Week' as Span,
						p_serverId,
						ps.PlayerId,
						SUM(TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP())))
			FROM 			PlayerSession as ps
			INNER JOIN		GameServer as gs
				ON		gs.ServerId = ps.ServerId
			WHERE			ps.SessionStart >= Date_Add(p_date,INTERVAL -(DayOfWeek(p_date)-1 ) DAY)
			AND			ps.SessionStart <= p_date
			AND			gs.ServerId = p_serverId
			GROUP BY		ps.PlayerId
			ORDER BY		SUM(TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP()))) DESC
			LIMIT			10) as t, (SELECT @rownum:=0) as r;


	-- TimeSpent / Month
		INSERT INTO StatsServerTimePlayed (Date, Span, ServerId, PlayerId, TimePlayed, Position )
		SELECT t.*, @rownum:=@rownum+1 FROM
			(SELECT			p_date,
						'Month' as Span,
						p_serverId,
						ps.PlayerId,
						SUM(TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP())))
			FROM 			PlayerSession as ps
			INNER JOIN		GameServer as gs
				ON		gs.ServerId = ps.ServerId
			WHERE			ps.SessionStart >= Date_Add(p_date,INTERVAL -(DayOfmonth(p_date)-1 ) DAY)
			AND			ps.SessionStart <= p_date
			AND			gs.ServerId = p_serverId
			GROUP BY		ps.PlayerId
			ORDER BY		SUM(TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP()))) DESC
			LIMIT			10) as t, (SELECT @rownum:=0) as r;

	-- TimeSpent / Year
		INSERT INTO StatsServerTimePlayed (Date, Span, ServerId, PlayerId, TimePlayed, Position )
		SELECT t.*, @rownum:=@rownum+1 FROM
			(SELECT			p_date,
						'Year' as Span,
						p_serverId,
						ps.PlayerId,
						SUM(TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP())))
			FROM 			PlayerSession as ps
			INNER JOIN		GameServer as gs
				ON		gs.ServerId = ps.ServerId
			WHERE			ps.SessionStart >= Date_Add(p_date,INTERVAL -(DayOfYear(p_date)-1 ) DAY)
			AND			ps.SessionStart <= p_date
			AND			gs.ServerId = p_serverId
			GROUP BY		ps.PlayerId
			ORDER BY		SUM(TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP()))) DESC
			LIMIT			10) as t, (SELECT @rownum:=0) as r;

	-- TimeSpent / AllTime
		INSERT INTO StatsServerTimePlayed (Date, Span, ServerId, PlayerId, TimePlayed, Position )
		SELECT t.*, @rownum:=@rownum+1 FROM
			(SELECT			p_date,
						'AllTime' as Span,
						p_serverId,
						ps.PlayerId,
						SUM(TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP())))
			FROM 			PlayerSession as ps
			INNER JOIN		GameServer as gs
				ON		gs.ServerId = ps.ServerId
			WHERE			ps.SessionStart <= p_date
			AND			gs.ServerId = p_serverId
			GROUP BY		ps.PlayerId
			ORDER BY		SUM(TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP()))) DESC
			LIMIT			10) as t, (SELECT @rownum:=0) as r;

-- ------------------------------------------

	-- TotalFrags / Week
		INSERT INTO StatsServerFrags (Date, Span, ServerId, PlayerId, Frags, Position )
		SELECT t.*, @rownum:=@rownum+1 FROM
			(SELECT			p_date,
						'Week' as Span,
						p_serverId,
						ps.PlayerId,
						sum(FragCount) + sum(CurrentFrags)
			FROM 			PlayerSession as ps
			INNER JOIN		GameServer as gs
				ON		gs.ServerId = ps.ServerId
			WHERE			ps.SessionStart >= Date_Add(p_date,INTERVAL -(DayOfWeek(p_date)-1 ) DAY)
			AND			ps.SessionStart <= p_date
			AND			gs.ServerId = p_serverId
			GROUP BY		ps.PlayerId
			ORDER BY		sum(FragCount) + sum(CurrentFrags) DESC
			LIMIT			10) as t, (SELECT @rownum:=0) as r;


	-- TotalFrags / Month
		INSERT INTO StatsServerFrags (Date, Span, ServerId, PlayerId, Frags, Position )
		SELECT t.*, @rownum:=@rownum+1 FROM
			(SELECT			p_date,
						'Month' as Span,
						p_serverId,
						ps.PlayerId,
						sum(FragCount) + sum(CurrentFrags)
			FROM 			PlayerSession as ps
			INNER JOIN		GameServer as gs
				ON		gs.ServerId = ps.ServerId
			WHERE			ps.SessionStart >= Date_Add(p_date,INTERVAL -(DayOfmonth(p_date)-1 ) DAY)
			AND			ps.SessionStart <= p_date
			AND			gs.ServerId = p_serverId
			GROUP BY		ps.PlayerId
			ORDER BY		sum(FragCount) + sum(CurrentFrags) DESC
			LIMIT			10) as t, (SELECT @rownum:=0) as r;

	-- TotalFrags / Year
		INSERT INTO StatsServerFrags (Date, Span, ServerId, PlayerId, Frags, Position )
		SELECT t.*, @rownum:=@rownum+1 FROM
			(SELECT			p_date,
						'Year' as Span,
						p_serverId,
						ps.PlayerId,
						sum(FragCount) + sum(CurrentFrags)
			FROM 			PlayerSession as ps
			INNER JOIN		GameServer as gs
				ON		gs.ServerId = ps.ServerId
			WHERE			ps.SessionStart >= Date_Add(p_date,INTERVAL -(DayOfYear(p_date)-1 ) DAY)
			AND			ps.SessionStart <= p_date
			AND			gs.ServerId = p_serverId
			GROUP BY		ps.PlayerId
			ORDER BY		sum(FragCount) + sum(CurrentFrags) DESC
			LIMIT			10) as t, (SELECT @rownum:=0) as r;

	-- TotalFrags / AllTime
		INSERT INTO StatsServerFrags (Date, Span, ServerId, PlayerId, Frags, Position )
		SELECT t.*, @rownum:=@rownum+1 FROM
			(SELECT			p_date,
						'AllTime' as Span,
						p_serverId,
						ps.PlayerId,
						sum(FragCount) + sum(CurrentFrags)
			FROM 			PlayerSession as ps
			INNER JOIN		GameServer as gs
				ON		gs.ServerId = ps.ServerId
			WHERE			ps.SessionStart <= p_date
			AND			gs.ServerId = p_serverId
			GROUP BY		ps.PlayerId
			ORDER BY		sum(FragCount) + sum(CurrentFrags) DESC
			LIMIT			10) as t, (SELECT @rownum:=0) as r;

END //
DELIMITER ;



DROP PROCEDURE IF EXISTS spServerStats;

DELIMITER //

CREATE PROCEDURE spServerStats (
	IN p_date DATETIME,
	IN p_serverId INTEGER)

BEGIN
	
	SET @varDate = p_Date;
	
	IF @varDate = '0000-00-00 00:00:00' THEN
		SELECT DATE INTO @varDate FROM StatsServerTimePlayed ORDER BY DATE DESC LIMIT 1;
	END IF;
	
	SELECT	gs.Serverid,smfpm.Date as Date, smfpm.Span as Span, smfpm.Position as Position,
			smfpm.FPM as MatchFPM, smfpm.ServerMatchId as MatchFPMMatchId, smfpm.PlayerId as MatchFPMPlayerId, smfpmp.Alias as MatchFPMAliasName, smfpmp.AliasBytes as MatchFPMAliasBytes,
			smf.Frags as MatchFrags, smf.ServerMatchId as MatchFragsMatchId, smf.PlayerId as MatchFragsPlayerId, smfp.Alias as MatchFragsAliasName, smfp.AliasBytes as MatchFragsAliasBytes,
			stp.TimePlayed as TimePlayed, stp.PlayerId as TimePlayedPlayerId, stpp.Alias as TimePlayedAliasName, stpp.AliasBytes as TimePlayedAliasBytes,
			sf.Frags as Frags, sf.PlayerId as FragsPlayerId, sfp.Alias as FragsAliasName, sfp.AliasBytes as FragsAliasBytes

	FROM		GameServer as gs

	INNER JOIN	StatsServerMatchFPM as smfpm
		ON	smfpm.ServerId = gs.ServerId
		AND	smfpm.Date = @varDate

	INNER JOIN 	Player as smfpmp
		ON	smfpmp.PlayerId = smfpm.PlayerId	

	INNER JOIN	StatsServerMatchFrags as smf
		ON	smf.ServerId = gs.ServerId
		AND	smf.Span = smfpm.span
		AND	smf.Position = smfpm.Position
		AND	smf.Date = smfpm.Date
		AND	smf.ServerId = gs.ServerId

	INNER JOIN 	Player as smfp
		ON	smfp.PlayerId = smf.PlayerId

	INNER JOIN	StatsServerTimePlayed as stp
		ON	stp.ServerId = gs.ServerId
		AND	stp.Span = smfpm.span
		AND	stp.Position = smfpm.Position
		AND	stp.Date = smfpm.Date
		AND	stp.ServerId = gs.ServerId

	INNER JOIN 	Player as stpp
		ON	stpp.PlayerId = stp.PlayerId

	INNER JOIN	StatsServerFrags as sf
		ON	sf.ServerId = gs.ServerId
		AND	sf.Span = smfpm.span
		AND	sf.Position = smfpm.Position
		AND	sf.Date = smfpm.Date
		AND	sf.ServerId = gs.ServerId

	INNER JOIN 	Player as sfp
		ON	sfp.PlayerId = sf.PlayerId	

	WHERE		gs.ServerId = p_serverId;

END//

DELIMITER ;

