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
CustomModificationName VARCHAR(50),
ApiKey VARCHAR(50)
);

CREATE INDEX  ON GameServer(
ServerId  ASC,IDX_GAMESERVER_
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
ServerId INTEGER  NULL,
Mode VARCHAR(50) NULL
);

CREATE INDEX IDX_SERVERMATCH_ ON ServerMatch(
ServerId,
ServerMatchId,
MatchStart
);

CREATE INDEX IDX_SERVERMATCH_END ON ServerMatch(
ServerId,
ServerMatchId,
MatchEnd
);

CREATE INDEX IDX_SERVERMATCH_START_END ON ServerMatch(
ServerId,
ServerMatchId,
MatchEnd,
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
Name VARCHAR(255) NULL,
Mode VARCHAR(50) NULL
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


CREATE TABLE ServerAccess (
ServerId INTEGER  PRIMARY KEY NULL,
Key VARCHAR(50)  NULL,
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
	GameServer.ServerId,
	ServerData.Name as ServerName,
	ServerMatch.ServerMatchId as MatchId,
	ServerMatch.Map as Map,
	--COALESCE(ServerMatch.Modification, GameServer.ModificationCode) as Modification,
	GameServer.ModificationCode as Modification,
	DATE_FORMAT(ServerMatch.MatchStart, '%Y-%m-%dT%TZ') as MatchStart,
	DATE_FORMAT(ServerMatch.MatchEnd, '%Y-%m-%dT%TZ') as MatchEnd,
	TIMESTAMPDIFF(SECOND, ServerMatch.MatchStart, ifnull(ServerMatch.MatchEnd,UTC_TIMESTAMP())) as MatchDuration,
	Player.Alias as Alias,
	Player.AliasBytes as AliasBytes,
	Player.PlayerId as PlayerId,
	PlayerMatch.PlayerMatchId as PlayerMatchId,
	PlayerMatch.Frags as Frags,
	PlayerMatch.PantColor as PantColor,
	PlayerMatch.ShirtColor as ShirtColor,
	PlayerMatch.Skin as Skin,
	PlayerMatch.Model as Model,
	DATE_FORMAT(PlayerMatch.PlayerMatchStart, '%Y-%m-%dT%TZ') as PlayerMatchStart,
	DATE_FORMAT(PlayerMatch.PlayerMatchEnd, '%Y-%m-%dT%TZ') as PlayerMatchEnd,
	TIMESTAMPDIFF(SECOND, PlayerMatch.PlayerMatchStart, ifnull(PlayerMatch.PlayerMatchEnd,UTC_TIMESTAMP())) as PlayerStayDuration,
	CASE WHEN Frags > 0 THEN
	round(PlayerMatch.Frags /((TIMESTAMPDIFF(SECOND, PlayerMatch.PlayerMatchStart, PlayerMatch.PlayerMatchEnd)) / 60),2)
	ELSE 0
	END as FPM
	FROM PlayerMatch
	inner join Player on (PlayerMatch.AliasId = Player.AliasId)
	inner join ServerMatch on (PlayerMatch.ServerMatchId = ServerMatch.ServerMatchId)
	inner join GameServer on (ServerMatch.ServerId = GameServer.ServerId)
	inner join ServerData on (ServerMatch.ServerId = ServerData.ServerId)
	WHERE PlayerMatch.ServerMatchId = p_matchId
	ORDER BY PlayerMatch.Frags DESC;

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
	(select TIMESTAMPDIFF(SECOND, ifnull(SessionEnd,UTC_TIMESTAMP), UTC_TIMESTAMP())
	FROM PlayerSession

	where master_p.AliasId = LastAliasId order by SessionStart desc limit 1) as AliasLastSeenAgo,
	(select DATE_FORMAT(ifnull(SessionEnd,UTC_TIMESTAMP()), '%Y-%m-%dT%TZ')
	FROM PlayerSession

	where master_p.AliasId = LastAliasId order by SessionStart desc limit 1) as AliasLastSeen,
	(SELECT GameServer.DNS
	FROM PlayerSession
	INNER JOIN GameServer ON (GameServer.ServerId = PlayerSession.ServerId)
	WHERE PlayerId = master_p.PlayerId order by SessionStart desc limit 1) as LastServer,
	(SELECT GameServer.ServerId
	FROM PlayerSession
	INNER JOIN GameServer ON (GameServer.ServerId = PlayerSession.ServerId)
	WHERE PlayerId = master_p.PlayerId order by SessionStart desc limit 1) as LastServerId,


	(select DATE_FORMAT(ifnull(SessionEnd,UTC_TIMESTAMP()), '%Y-%m-%dT%TZ')
	FROM PlayerSession
	where PlayerId = master_p.PlayerId order by SessionStart desc limit 1) as LastSeen,
	
	(select TIMESTAMPDIFF(SECOND, ifnull(SessionEnd,UTC_TIMESTAMP), UTC_TIMESTAMP())
	FROM PlayerSession
	where PlayerId = master_p.PlayerId order by SessionStart desc limit 1) as LastSeenAgo,

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

	(select round((sum(FragCount) + sum(CurrentFrags))/(( sum( TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP()))  ) / 60)),2)
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId AND SessionStart >= Date_Add(Current_Date(),INTERVAL -(DayOfMonth(Current_Date())-1 ) DAY)) AS month_FPM      ,

	(select sum( TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP()))  )
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId AND SessionStart >= Date_Add(Current_Date(),INTERVAL -(DayOfWeek(Current_Date())-1 ) DAY)) AS week_playtime_sum,

	(select sum(FragCount) + sum(CurrentFrags)
	FROM PlayerSession
	WHERE PlayerId = master_p.PlayerId AND SessionStart >= Date_Add(Current_Date(),INTERVAL -(DayOfWeek(Current_Date())-1 ) DAY)) AS week_frags_sum,

	(select round((sum(FragCount) + sum(CurrentFrags))/(( sum( TIMESTAMPDIFF(SECOND, SessionStart, ifnull(SessionEnd,UTC_TIMESTAMP()))  ) / 60)),2)
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


DROP PROCEDURE IF EXISTS spServerMatches;

DELIMITER //
CREATE PROCEDURE spServerMatches (
IN p_serverId INTEGER,
IN p_pageRecordCount INTEGER,
IN p_pageRecordOffset INTEGER)
BEGIN
	DECLARE SQLs VARCHAR(10000);
	SET @SQLs = CONCAT('CREATE TEMPORARY TABLE ids
    SELECT ServerMatch.ServerMatchId
	FROM ServerMatch
    WHERE ServerMatch.ServerId = ', p_serverId, '
	AND TIMESTAMPDIFF(SECOND, ServerMatch.MatchStart, ifnull(ServerMatch.MatchEnd, UTC_TIMESTAMP())) > 10
	ORDER BY UNIX_TIMESTAMP(ServerMatch.MatchStart) DESC
	LIMIT ', p_pageRecordCount, ' OFFSET ', p_pageRecordOffset, ';');
    
	PREPARE query FROM @SQLs;
	EXECUTE query;
	DEALLOCATE PREPARE query;

	SELECT
	ServerMatch.ServerMatchId as ServerMatchId,
	DATE_FORMAT(ServerMatch.MatchStart, '%Y-%m-%dT%TZ') as MatchStart,
	GameServer.GameId as GameId,
	GameServer.DNS as HostName,
	GameServer.Port As Port,
	ServerData.Name as ServerName,
	GameServer.ServerId as ServerId,
	ServerMatch.Map as Map,
	ServerMatch.Modification as Modification,
	ServerMatch.Mode as Mode,
	TIMESTAMPDIFF(SECOND, ServerMatch.MatchStart, ifnull(ServerMatch.MatchEnd,UTC_TIMESTAMP())) as Duration,
	Player.PlayerId,
    Player.Alias as PlayerName,
	PlayerMatch.Frags as Frags,
	PlayerMatch.PantColor as PantColor,
	PlayerMatch.ShirtColor as ShirtColor,
	DATE_FORMAT(PlayerMatch.PlayerMatchStart, '%Y-%m-%dT%TZ') as PlayerMatchStart,
	DATE_FORMAT(PlayerMatch.PlayerMatchEnd, '%Y-%m-%dT%TZ') as PlayerMatchEnd,
	TIMESTAMPDIFF(SECOND, PlayerMatch.PlayerMatchStart, ifnull(PlayerMatch.PlayerMatchEnd,UTC_TIMESTAMP())) as PlayerStayDuration
	FROM ServerMatch
    INNER JOIN ids on (ids.ServerMatchId = ServerMatch.ServerMatchId)
	inner join GameServer on (ServerMatch.ServerId = GameServer.ServerId)
	inner join ServerData on (ServerMatch.ServerId = ServerData.ServerId)
    INNER JOIN PlayerMatch on (ids.ServerMatchId = PlayerMatch.ServerMatchId)
	INNER JOIN Player on (PlayerMatch.AliasId = Player.AliasId);
	
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS spServerMatchesCount;

DELIMITER //
CREATE PROCEDURE spServerMatchesCount (
IN p_serverId INTEGER)
BEGIN
	DECLARE SQLs VARCHAR(10000);
	
	SET @SQLs = CONCAT('SELECT Count(*) as RecordCount
	FROM ServerMatch
	inner join GameServer on (ServerMatch.ServerId = GameServer.ServerId)
	inner join ServerData on (ServerMatch.ServerId = ServerData.ServerId)
	WHERE GameServer.ServerId = ',p_serverId, ';');
	
	PREPARE query FROM @SQLs;
	EXECUTE query;
END //
DELIMITER ;


DROP PROCEDURE IF EXISTS spPlayerMatches;

DELIMITER //
CREATE PROCEDURE spPlayerMatches (
IN p_playerId INTEGER,
IN p_pageRecordCount INTEGER,
IN p_pageRecordOffset INTEGER)
BEGIN
	DECLARE SQLs VARCHAR(10000);
	
	SET @SQLs = CONCAT('SELECT
	sm.ServerMatchId as MatchId,
	DATE_FORMAT(sm.MatchStart, ''%Y-%m-%dT%TZ'')  as MatchStart,
	GameServer.GameId as GameId,
	GameServer.DNS as HostName,
	GameServer.Port As Port,
	ServerData.Name as ServerName,
	GameServer.ServerId as ServerId,
	sm.Map as Map,
	GameServer.ModificationCode as Modification,
	Player.Alias as Alias,
	Player.PlayerId as PlayerId,
	PlayerMatch.Frags as Frags,
	PlayerMatch.PantColor as PantColor,
	PlayerMatch.ShirtColor as ShirtColor,
	PlayerMatch.Skin as Skin,
	PlayerMatch.Model as Model,
	DATE_FORMAT(PlayerMatch.PlayerMatchStart, ''%Y-%m-%dT%TZ'')  as PlayerJoinTime,
	TIMESTAMPDIFF(SECOND, PlayerMatch.PlayerMatchStart, ifnull(PlayerMatch.PlayerMatchEnd,UTC_TIMESTAMP())) as PlayerStayDuration
	FROM PlayerMatch
	inner join Player on (PlayerMatch.AliasId = Player.AliasId)
	inner join ServerMatch as sm on (PlayerMatch.ServerMatchId = sm.ServerMatchId)
	inner join GameServer on (sm.ServerId = GameServer.ServerId)
	inner join ServerData on (sm.ServerId = ServerData.ServerId)
	WHERE EXISTS (SELECT * FROM PlayerMatch as pm1 WHERE pm1.ServerMatchId = sm.ServerMatchId AND pm1.Frags > 0)
	AND PlayerMatch.PlayerId = ', p_playerId, '
	AND TIMESTAMPDIFF(SECOND, PlayerMatch.PlayerMatchStart, ifnull(PlayerMatch.PlayerMatchEnd,UTC_TIMESTAMP())) > 10
	ORDER BY UNIX_TIMESTAMP(sm.MatchStart) DESC
	LIMIT ', p_pageRecordCount, ' OFFSET ', p_pageRecordOffset, ';');
	
	PREPARE query FROM @SQLs;
	EXECUTE query;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS spPlayerMatchesCount;

DELIMITER //
CREATE PROCEDURE spPlayerMatchesCount (
IN p_playerId INTEGER)
BEGIN
	DECLARE SQLs VARCHAR(10000);
	
	SET @SQLs = CONCAT('SELECT Count(*) as RecordCount
	FROM PlayerMatch
	inner join Player on (PlayerMatch.AliasId = Player.AliasId)
	inner join ServerMatch as sm on (PlayerMatch.ServerMatchId = sm.ServerMatchId)
	inner join GameServer on (sm.ServerId = GameServer.ServerId)
	inner join ServerData on (sm.ServerId = ServerData.ServerId)
	WHERE EXISTS (SELECT * FROM PlayerMatch as pm1 WHERE pm1.ServerMatchId = sm.ServerMatchId AND pm1.Frags > 0)
	AND TIMESTAMPDIFF(SECOND, PlayerMatch.PlayerMatchStart, ifnull(PlayerMatch.PlayerMatchEnd,UTC_TIMESTAMP())) > 10
	AND PlayerMatch.PlayerId = ', p_playerId, ';');
	
	PREPARE query FROM @SQLs;
	EXECUTE query;
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
,gs.DNS as DNS
,gs.Port as Port
,gs.GameId as GameId
,gs.Location as Location
,gs.LastQuerySuccess as LastQuerySuccess
,gs.Region as Region
,gs.ModificationCode as ModificationCode
,gs.Category as Category
,gs.QueryResult as CurrentStatus
,gs.CustomModificationName as CustomModificationName
,sd.Name as ServerName
,sd.Map as Map
,sd.ServerSettings as ServerSettings
,sd.Modification as Modification
,sd.Mode as Mode
,sd.PlayerData as PlayerData
,sd.Timestamp as Timestamp
,sd.MaxPlayers as MaxPlayers
,sd.IpAddress as IpAddress
,sm.MatchStart as RecentMatchStart
,sm.MatchEnd as RecentMatchEnd
,sm.Map as RecentMatchMap
,sm.ServerMatchId as RecenMatchId
,(SELECT count(*) from PlayerMatch where ServerMatchId = sm.ServerMatchId) as RecentMatchPlayers

from
GameServer gs
inner join ServerData sd
  ON gs.ServerId = sd.ServerId
left outer join ServerMatch sm
  ON sm.ServerMatchId = (SELECT  ServerMatchId 
                        from ServerMatch 
                        WHERE ServerId = sd.ServerId 
                        ORDER BY MatchStart 
                        DESC LIMIT 1)
WHERE Active = 1;


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
	WHERE HistoricalHourlyLog.ServerId = p_serverId
	AND HistoricalDate >= p_fromDate
	AND HistoricalDate <= p_toDate;

END //
DELIMITER ;


DROP PROCEDURE IF EXISTS spPlayerLookupCount;

DELIMITER //

CREATE PROCEDURE spPlayerLookupCount (
	IN p_gameId INTEGER,
	IN p_aliasPart TEXT)

BEGIN
	
	DECLARE SQLs VARCHAR(10000);
	
	SET @SQLs = CONCAT('SELECT Count(*) as RecordCount
	FROM
		(SELECT *
		FROM 	Player
		WHERE	Alias = ''', p_aliasPart, '''
		AND		GameId = ', p_gameId, '
		UNION
		SELECT 	*
		FROM 	Player
		WHERE	Alias like ''', p_aliasPart, '%''
		AND		GameId = ', p_gameId, ')
	AS sub;');
	
	PREPARE query FROM @SQLs;
	EXECUTE query;

END //
DELIMITER ;



DROP PROCEDURE IF EXISTS spPlayerLookup;

DELIMITER //

CREATE PROCEDURE spPlayerLookup (
	IN p_gameId INTEGER,
	IN p_aliasPart TEXT,
	IN p_pageRecordCount INTEGER,
	IN p_pageRecordOffset INTEGER)

BEGIN
	
	DECLARE SQLs VARCHAR(10000);
	
	SET @SQLs = CONCAT('SELECT 	master_p.PlayerId,
		master_p.AliasBytes,
		master_p.Alias,
			(select TIMESTAMPDIFF(SECOND, ifnull(SessionEnd,UTC_TIMESTAMP), UTC_TIMESTAMP())
			FROM PlayerSession
			where PlayerId = master_p.PlayerId order by SessionStart desc limit 1) as LastSeenAgo
	FROM 	Player as master_p
	WHERE	master_p.Alias = ''', p_aliasPart, '''
	AND		master_p.GameId = ', p_gameId, '
	UNION
	SELECT 	master_p.PlayerId,
		master_p.AliasBytes,
		master_p.Alias,
			(select TIMESTAMPDIFF(SECOND, ifnull(SessionEnd,UTC_TIMESTAMP), UTC_TIMESTAMP())
			FROM PlayerSession
			where PlayerId = master_p.PlayerId order by SessionStart desc limit 1) as LastSeenAgo
	
	FROM 	Player as master_p
	WHERE	master_p.Alias like ''', p_aliasPart, '%''
	AND		master_p.GameId = ', p_gameId, '
	LIMIT ', p_pageRecordCount, ' OFFSET ', p_pageRecordOffset, ';');
	
	PREPARE query FROM @SQLs;
	EXECUTE query;
	
END //
DELIMITER ;


DROP PROCEDURE IF EXISTS spPlayerAliasLookupCount;

DELIMITER //

CREATE PROCEDURE spPlayerAliasLookupCount (
	IN p_gameId INTEGER,
	IN p_playerId TEXT)

BEGIN
	
	DECLARE SQLs VARCHAR(10000);
	
	SET @SQLs = CONCAT('SELECT COUNT(*) as RecordCount
FROM 		Player as master_p
INNER JOIN 	Player as slave_p
		ON (INSTR(slave_p.IPAddress, ''.'') > 0 
			AND master_p.IPAddress like CONCAT(LEFT(slave_p.IPAddress,LENGTH(slave_p.IPAddress) - INSTR(REVERSE(slave_p.IPAddress),''.'')), ''%''))
WHERE INSTR(master_p.IPAddress,''.'') > 0
AND		master_p.PlayerId = ', p_playerId, ';');
	
	PREPARE query FROM @SQLs;
	EXECUTE query;

END //
DELIMITER ;



DROP PROCEDURE IF EXISTS spPlayerAliasLookup;

DELIMITER //

CREATE PROCEDURE spPlayerAliasLookup (
	IN p_playerId TEXT,
	IN p_pageRecordCount INTEGER,
	IN p_pageRecordOffset INTEGER)

BEGIN
	
	DECLARE SQLs VARCHAR(10000);
	
	SET @SQLs = CONCAT('SELECT 	master_p.PlayerId,
		slave_p.PlayerId as AliasPlayerId,
		slave_p.AliasBytes,
		slave_p.Alias,
			(select TIMESTAMPDIFF(SECOND, ifnull(SessionEnd,UTC_TIMESTAMP), UTC_TIMESTAMP())
			FROM PlayerSession
			where PlayerId = slave_p.PlayerId order by SessionStart desc limit 1) as LastSeenAgo
		
FROM 		Player as master_p
INNER JOIN 	Player as slave_p
		ON (INSTR(slave_p.IPAddress, ''.'') > 0 
			AND master_p.IPAddress like CONCAT(LEFT(slave_p.IPAddress,LENGTH(slave_p.IPAddress) - INSTR(REVERSE(slave_p.IPAddress),''.'')), ''%''))
WHERE INSTR(master_p.IPAddress,''.'') > 0
AND		master_p.PlayerId = ', p_playerId, '
ORDER BY (select TIMESTAMPDIFF(SECOND, ifnull(SessionEnd,UTC_TIMESTAMP), UTC_TIMESTAMP())
			FROM PlayerSession
			where PlayerId = slave_p.PlayerId order by SessionStart desc limit 1) 
	LIMIT ', p_pageRecordCount, ' OFFSET ', p_pageRecordOffset, ';');
	
	PREPARE query FROM @SQLs;
	EXECUTE query;
	
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS spServerMatches;

DELIMITER //

CREATE PROCEDURE `spServerMatches`(
IN p_serverId INTEGER,
IN p_pageRecordCount INTEGER,
IN p_pageRecordOffset INTEGER)
BEGIN
DECLARE SQLs VARCHAR(10000);
SET @SQLs = CONCAT('CREATE TEMPORARY TABLE ids
    SELECT ServerMatch.ServerMatchId
FROM ServerMatch
    WHERE ServerMatch.ServerId = ', p_serverId, '
AND TIMESTAMPDIFF(SECOND, ServerMatch.MatchStart, ifnull(ServerMatch.MatchEnd, UTC_TIMESTAMP())) > 10
AND EXISTS (SELECT * FROM PlayerMatch as pm1 WHERE pm1.ServerMatchId = ServerMatch.ServerMatchId AND pm1.Frags > 0)
ORDER BY UNIX_TIMESTAMP(ServerMatch.MatchStart) DESC
LIMIT ', p_pageRecordCount, ' OFFSET ', p_pageRecordOffset, ';');

PREPARE query FROM @SQLs;
EXECUTE query;
DEALLOCATE PREPARE query;

SELECT
ServerMatch.ServerMatchId as ServerMatchId,
DATE_FORMAT(ServerMatch.MatchStart, '%Y-%m-%dT%TZ') as MatchStart,
GameServer.GameId as GameId,
GameServer.DNS as HostName,
GameServer.Port As Port,
ServerData.Name as ServerName,
GameServer.ServerId as ServerId,
ServerMatch.Map as Map,
ServerMatch.Mod as Mod,
ServerMatch.Mode as Mode,
TIMESTAMPDIFF(SECOND, ServerMatch.MatchStart, ifnull(ServerMatch.MatchEnd,UTC_TIMESTAMP())) as Duration,
Player.PlayerId,
    Player.Alias as PlayerName,
PlayerMatch.Frags as Frags,
PlayerMatch.PantColor as PantColor,
PlayerMatch.ShirtColor as ShirtColor,
DATE_FORMAT(PlayerMatch.PlayerMatchStart, '%Y-%m-%dT%TZ') as PlayerMatchStart,
DATE_FORMAT(PlayerMatch.PlayerMatchEnd, '%Y-%m-%dT%TZ') as PlayerMatchEnd,
TIMESTAMPDIFF(SECOND, PlayerMatch.PlayerMatchStart, ifnull(PlayerMatch.PlayerMatchEnd,UTC_TIMESTAMP())) as PlayerStayDuration
FROM ServerMatch
    INNER JOIN ids on (ids.ServerMatchId = ServerMatch.ServerMatchId)
inner join GameServer on (ServerMatch.ServerId = GameServer.ServerId)
inner join ServerData on (ServerMatch.ServerId = ServerData.ServerId)
    INNER JOIN PlayerMatch on (ids.ServerMatchId = PlayerMatch.ServerMatchId)
INNER JOIN Player on (PlayerMatch.AliasId = Player.AliasId);

END //
DELIMITER ;



DROP PROCEDURE IF EXISTS spServerMatchesCount;

DELIMITER //
CREATE PROCEDURE spServerMatchesCount (
IN p_serverId INTEGER)
BEGIN
	DECLARE SQLs VARCHAR(10000);
	
	SET @SQLs = CONCAT('SELECT Count(*) as RecordCount
	FROM ServerMatch as sm
	inner join GameServer on (sm.ServerId = GameServer.ServerId)
	inner join ServerData on (sm.ServerId = ServerData.ServerId)
	WHERE EXISTS (SELECT * FROM PlayerMatch as pm1 WHERE pm1.ServerMatchId = sm.ServerMatchId AND pm1.Frags > 0)
	AND GameServer.ServerId = ',p_serverId, '
	AND TIMESTAMPDIFF(SECOND, sm.MatchStart, ifnull(sm.MatchEnd,UTC_TIMESTAMP())) > 10;');
	
	PREPARE query FROM @SQLs;
	EXECUTE query;
END //
DELIMITER ;


DROP PROCEDURE IF EXISTS spServerPlayerWeeklyRanking;

DELIMITER //

CREATE PROCEDURE spServerPlayerWeeklyRanking (
	IN p_date DATETIME,
	IN p_serverId INTEGER)

BEGIN
	SELECT sub.*, p.AliasBytes,
		(SELECT sm1.ServerMatchId 
		from 	PlayerMatch  as pm1
		INNER JOIN	ServerMatch as sm1 ON (pm1.ServerMatchId = sm1.ServerMatchId)
		where 	sub.PlayerId = pm1.PlayerId 		
		AND 	sm1.ServerId = p_serverId
		AND 	round(pm1.Frags/( TIMESTAMPDIFF(SECOND, pm1.PlayerMatchStart, ifnull(pm1.PlayerMatchEnd,UTC_TIMESTAMP)) / 60),2) = sub.FPM
		AND		TIMESTAMPDIFF(SECOND, pm1.PlayerMatchStart,ifnull(pm1.PlayerMatchEnd,UTC_TIMESTAMP)) BETWEEN 180 AND 7800
		AND			CONVERT_TZ(sm1.MatchStart, 'UTC', 'EST') BETWEEN
						Date_Add(p_date,INTERVAL -(DayOfWeek(p_date)-1 ) DAY) 
						AND Date_Add(p_date,INTERVAL (7-(DayOfWeek(p_date)-1 )) DAY)
		LIMIT 1) as MatchId
	FROM 
		(SELECT PlayerId,
			MAX(round(pm.Frags/( TIMESTAMPDIFF(SECOND, pm.PlayerMatchStart, ifnull(pm.PlayerMatchEnd,UTC_TIMESTAMP)) / 60),2)) as FPM
		FROM PlayerMatch as pm
		INNER JOIN	ServerMatch as sm ON (pm.ServerMatchId = sm.ServerMatchId)
		WHERE		TIMESTAMPDIFF(SECOND, pm.PlayerMatchStart,ifnull(pm.PlayerMatchEnd,UTC_TIMESTAMP)) BETWEEN 180 AND 7800
		AND			pm.Frags > 0
		AND			CONVERT_TZ(sm.MatchStart, 'UTC', 'EST') BETWEEN
						Date_Add(p_date,INTERVAL -(DayOfWeek(p_date)-1 ) DAY) 
						AND Date_Add(p_date,INTERVAL (7-(DayOfWeek(p_date)-1 )) DAY)
		AND 		sm.ServerId = p_serverId
		GROUP BY 	pm.PlayerId) as sub
	INNER JOIN	Player as p ON (sub.PlayerId = p.PlayerId)
	ORDER BY sub.FPM DESC
	LIMIT 100;
	
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS spServerPlayerWeeklyPlayTime;

DELIMITER //

CREATE PROCEDURE spServerPlayerWeeklyPlayTime (
	IN p_date DATETIME,
	IN p_serverId INTEGER)

BEGIN
	SELECT		sub.*, p.AliasBytes
	FROM
		(SELECT 	pm.PlayerId,
					SUM(TIMESTAMPDIFF(SECOND, pm.PlayerMatchStart,ifnull(pm.PlayerMatchEnd,UTC_TIMESTAMP))) as TimeSpent
		FROM 		PlayerMatch as pm
		INNER JOIN	ServerMatch sm on (sm.ServerMatchId = pm.ServerMatchId)
		WHERE 		sm.ServerId = p_serverId
		AND			CONVERT_TZ(sm.MatchStart, 'UTC', 'EST') BETWEEN
						Date_Add(p_date,INTERVAL -(DayOfWeek(p_date)-1 ) DAY) 
						AND Date_Add(p_date,INTERVAL (7-(DayOfWeek(p_date)-1 )) DAY)
		GROUP BY	pm.PlayerId) as sub
	INNER JOIN	Player as p ON (sub.PlayerId = p.PlayerId)
	WHERE		sub.TimeSpent > 0
	ORDER BY 	sub.TimeSpent DESC
	LIMIT 100;
	
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS spServerStatsMapPercentage;

DELIMITER //

CREATE PROCEDURE spServerStatsMapPercentage (
	IN p_date DATETIME,
	IN p_serverId INTEGER)
BEGIN

	DECLARE Total INTEGER;
	SET @Total = 
		(SELECT  SUM(TIMESTAMPDIFF(SECOND, sm.MatchStart, ifnull(sm.MatchEnd, UTC_TIMESTAMP))) 
		FROM		ServerMatch as sm
		WHERE		CONVERT_TZ(sm.MatchStart, 'UTC', 'EST') BETWEEN
					Date_Add(p_date,INTERVAL -30 DAY) 
					AND p_date
		AND 		sm.ServerId = p_serverId
		AND EXISTS (SELECT * FROM PlayerMatch as pm WHERE pm.ServerMatchId = sm.ServerMatchId AND pm.Frags > 0));
					
	SELECT   
			sub.Map as Map, 
			round(100*(TimeSpent/@Total),2) as Percentage
	FROM
		(SELECT 
			sm.Map, SUM(TIMESTAMPDIFF(SECOND, sm.MatchStart, ifnull(sm.MatchEnd, UTC_TIMESTAMP))) as TimeSpent
		FROM		ServerMatch as sm
		WHERE		CONVERT_TZ(sm.MatchStart, 'UTC', 'EST') BETWEEN
					Date_Add(p_date,INTERVAL -30 DAY) 
					AND p_date
		AND 		sm.ServerId = p_serverId
		AND EXISTS (SELECT * FROM PlayerMatch as pm WHERE pm.ServerMatchId = sm.ServerMatchId AND pm.Frags > 0)
		GROUP BY sm.Map) as sub
	ORDER BY sub.TimeSpent DESC
	LIMIT 10;
	
END//

DELIMITER ;



DROP PROCEDURE IF EXISTS spServerRecentMatches;

DELIMITER //

CREATE PROCEDURE spServerRecentMatches (
)

BEGIN
    SELECT      sm.ServerMatchId,
                gs.ServerId,
                sd.Name as ServerName,
                gs.DNS as HostName,
                gs.Port as Port,
                gs.GameId,
                sm.MatchStart as MatchStart,
                sm.MatchEnd as MatchEnd,
                TIMESTAMPDIFF(SECOND, sm.MatchStart, ifnull(sm.MatchEnd,UTC_TIMESTAMP())) as Duration,
                sm.Map as Map,
                gs.ModificationCode as Modification,
                p.PlayerId,
                p.Alias as PlayerName,
                pm.Frags as Frags,
                pm.PantColor as PantColor,
                pm.ShirtColor as ShirtColor,
                DATE_FORMAT(pm.PlayerMatchStart, '%Y-%m-%dT%TZ') as PlayerMatchStart,
                DATE_FORMAT(pm.PlayerMatchEnd, '%Y-%m-%dT%TZ') as PlayerMatchEnd,
                TIMESTAMPDIFF(SECOND, pm.PlayerMatchStart, ifnull(pm.PlayerMatchEnd,UTC_TIMESTAMP())) as PlayerStayDuration
    FROM        ServerMatch as sm
    INNER JOIN  GameServer as gs on (sm.ServerId = gs.ServerId)
    INNER JOIN  PlayerMatch as pm on (sm.ServerMatchId = pm.ServerMatchId)
    INNER JOIN  Player as p on (pm.AliasId = p.AliasId)
    INNER JOIN  ServerData as sd on (sd.ServerId = sm.ServerId)
    JOIN 
        (SELECT distinct ServerMatch.ServerMatchId
        FROM ServerMatch
        INNER JOIN 	PlayerMatch on (ServerMatch.ServerMatchId = PlayerMatch.ServerMatchId)
        WHERE
        EXISTS      (SELECT * FROM PlayerMatch WHERE PlayerMatch.Frags > 0 AND PlayerMatch.ServerMatchId = PlayerMatch.ServerMatchId)
        AND         TIMESTAMPDIFF(SECOND, ServerMatch.MatchStart, ifnull(ServerMatch.MatchEnd,UTC_TIMESTAMP())) > 60
        AND         ServerMatch.MatchStart > Date_Add(Current_Date(),INTERVAL -30 DAY)
        LIMIT 10
        ) limiter
    ON sm.ServerMatchId
    IN (limiter.ServerMatchId)
    ORDER BY        sm.MatchEnd DESC;

END //
DELIMITER ;



DROP TABLE IF EXISTS WeeklyStats;
CREATE TABLE WeeklyStats (
	WeeklyStatsId INTEGER PRIMARY KEY AUTO_INCREMENT NOT NULL,
	Date DATETIME NULL,
	ServerId INTEGER NULL,
	PlayerId INTEGER NULL,
	ServerMatchId INTEGER NULL,
	Position INTEGER NULL,
	StatValue FLOAT NULL,
	StatType VARCHAR(21)  NULL	
);

DROP PROCEDURE IF EXISTS spProcessWeeklyServerStats;

DELIMITER //

CREATE PROCEDURE spProcessWeeklyServerStats (
	IN p_date DATETIME,
	IN p_serverId INTEGER)

BEGIN
	DELETE FROM WeeklyStats WHERE Date = p_date AND ServerId = p_serverId;

	
	-- WEEKLY FPM Leaders
	
	INSERT INTO WeeklyStats (Date, ServerId, ServerMatchId,  PlayerId,  StatValue, StatType, Position)
	SELECT p_date,p_serverId,t.*, 'FPM', @rownum:=@rownum+1 FROM
		(
		SELECT 
			(SELECT sm1.ServerMatchId 
			from 	PlayerMatch  as pm1
			INNER JOIN	ServerMatch as sm1 ON (pm1.ServerMatchId = sm1.ServerMatchId)
			where 	sub.PlayerId = pm1.PlayerId 		
			AND 	sm1.ServerId = p_serverId
			AND 	round(pm1.Frags/( TIMESTAMPDIFF(SECOND, pm1.PlayerMatchStart, ifnull(pm1.PlayerMatchEnd,UTC_TIMESTAMP)) / 60),2) = sub.FPM
			AND		TIMESTAMPDIFF(SECOND, pm1.PlayerMatchStart,ifnull(pm1.PlayerMatchEnd,UTC_TIMESTAMP)) BETWEEN 180 AND 7800
			AND		CONVERT_TZ(pm1.PlayerMatchStart, 'UTC', 'EST') > Date_Add(Current_Date(),INTERVAL -(DayOfWeek(Current_Date())-1 ) DAY)	
			LIMIT 1) as ServerMatchId,
			sub.*
		FROM 
			(SELECT PlayerId,
				MAX(round(pm.Frags/( TIMESTAMPDIFF(SECOND, pm.PlayerMatchStart, ifnull(pm.PlayerMatchEnd,UTC_TIMESTAMP)) / 60),2)) as FPM
			FROM PlayerMatch as pm
			INNER JOIN	ServerMatch as sm ON (pm.ServerMatchId = sm.ServerMatchId)
			WHERE		TIMESTAMPDIFF(SECOND, pm.PlayerMatchStart,ifnull(pm.PlayerMatchEnd,UTC_TIMESTAMP)) BETWEEN 180 AND 7800
			AND			CONVERT_TZ(pm.PlayerMatchStart, 'UTC', 'EST') BETWEEN
				Date_Add(p_date,INTERVAL -(DayOfWeek(p_date)-1 ) DAY) 
				AND Date_Add(p_date,INTERVAL (7-(DayOfWeek(p_date)-1 )) DAY)
			AND			pm.Frags > 0
			AND 		sm.ServerId = p_serverId
			GROUP BY 	pm.PlayerId) as sub
		INNER JOIN	Player as p ON (sub.PlayerId = p.PlayerId)
		ORDER BY sub.FPM DESC
		LIMIT 100) as t, (SELECT @rownum:=0) as r;
		
	-- WEEKLY LongestTimePlayed Leaders
	
	INSERT INTO WeeklyStats (Date, ServerId, ServerMatchId,  PlayerId,  StatValue, StatType, Position)
	SELECT p_date,p_serverId, NULL, t.*, 'TimePlayed', @rownum:=@rownum+1 FROM
		(SELECT PlayerId, TimePlayed
		FROM
			(SELECT PlayerId,
				SUM(round( TIMESTAMPDIFF(SECOND, pm.PlayerMatchStart, ifnull(pm.PlayerMatchEnd,UTC_TIMESTAMP)) ,2)) as TimePlayed
			FROM PlayerMatch as pm
			INNER JOIN	ServerMatch as sm ON (pm.ServerMatchId = sm.ServerMatchId)
			WHERE		TIMESTAMPDIFF(SECOND, pm.PlayerMatchStart,ifnull(pm.PlayerMatchEnd,UTC_TIMESTAMP)) BETWEEN 180 AND 7800
			AND			CONVERT_TZ(pm.PlayerMatchStart, 'UTC', 'EST') BETWEEN
				Date_Add(p_date,INTERVAL -(DayOfWeek(p_date)-1 ) DAY) 
				AND Date_Add(p_date,INTERVAL (7-(DayOfWeek(p_date)-1 )) DAY)
			AND			pm.Frags > 0
			AND 		sm.ServerId = p_serverId
			GROUP BY 	pm.PlayerId) as sub
		ORDER BY sub.TimePlayed DESC
		LIMIT 100) as t, (SELECT @rownum:=0) as r;

	
END//

DELIMITER ;


-- Change delimiter so that the function body doesn't end the function declaration
DELIMITER //

CREATE FUNCTION uuid_v4()
    RETURNS CHAR(36) NO SQL
BEGIN
    -- Generate 8 2-byte strings that we will combine into a UUIDv4
    SET @h1 = LPAD(HEX(FLOOR(RAND() * 0xffff)), 4, '0');
    SET @h2 = LPAD(HEX(FLOOR(RAND() * 0xffff)), 4, '0');
    SET @h3 = LPAD(HEX(FLOOR(RAND() * 0xffff)), 4, '0');
    SET @h6 = LPAD(HEX(FLOOR(RAND() * 0xffff)), 4, '0');
    SET @h7 = LPAD(HEX(FLOOR(RAND() * 0xffff)), 4, '0');
    SET @h8 = LPAD(HEX(FLOOR(RAND() * 0xffff)), 4, '0');

    -- 4th section will start with a 4 indicating the version
    SET @h4 = CONCAT('4', LPAD(HEX(FLOOR(RAND() * 0x0fff)), 3, '0'));

    -- 5th section first half-byte can only be 8, 9 A or B
    SET @h5 = CONCAT(HEX(FLOOR(RAND() * 4 + 8)),
                LPAD(HEX(FLOOR(RAND() * 0x0fff)), 3, '0'));

    -- Build the complete UUID
    RETURN LOWER(CONCAT(
        @h1, @h2, '-', @h3, '-', @h4, '-', @h5, '-', @h6, @h7, @h8
    ));
END
//
-- Switch back the delimiter
DELIMITER ;


CREATE TABLE ServerMatchActual (
ServerMatchActualId INTEGER PRIMARY KEY AUTO_INCREMENT NOT NULL,
ServerId INTEGER,
DateAdded datetime,
Payload VARCHAR(8000)
);

