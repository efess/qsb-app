using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using QSB.Data.ViewObject;
using System.Collections.Generic;
using Jayrock.Json;
using System.IO;
using QSB.Common;
using QSB.Data;

public partial class PlayerDetails : BasePage
{
    const string QUERY_VALUE_SUMMERY = "Summery";
    const string QUERY_VALUE_MATCHES = "Matches";
    const string QUERY_VALUE_MATCHDETAIL = "MatchDetail";

    const string QUERY_KEY_PLAYERID = "PlayerId";
    const string QUERY_KEY_MATCHID = "MatchId";

    const string QUERY_KEY_REQUEST = "Request";

    protected void Page_Load(object sender, EventArgs e)
    {
        base.PageLoad();

        string strMatchId = Request[QUERY_KEY_MATCHID];
        int matchId = 0;

        string strPlayerId = Request[QUERY_KEY_PLAYERID];
        int playerId = 0;
        
        if (!int.TryParse(strPlayerId, out playerId)
            && !int.TryParse(strMatchId, out matchId))
        {
            Response.Write("Invalid Identification");
            return;
        }

        string callback = Request[QUERY_KEY_REQUEST];
        if (callback != null)
        {
            switch (callback)
            {
                case QUERY_VALUE_MATCHES:
                    ReturnMatches(playerId);
                    break;
                case QUERY_VALUE_SUMMERY:
                    ReturnSummery(playerId);
                    break;
                case QUERY_VALUE_MATCHDETAIL:
                    ReturnMatchDetail(matchId);
                    break;
                default: Response.Write("Invalid request");
                    break;
            }
            return;
        }
    }

    private void ReturnMatchDetail(int pMatchId)
    {
        IList<MatchDetail> matchDetails = WebMain.ServerMain.Store.GetMatchDetail(pMatchId);
        StringWriter writer = new StringWriter();

        JsonTextWriter w = new JsonTextWriter(writer);

        if (matchDetails.Count > 0)
        {
            w.WriteStartObject();
            w.WriteMember("MatchId");
            w.WriteNumber(matchDetails[0].MatchId);
            w.WriteMember("HostName");
            w.WriteString(matchDetails[0].HostName + ":" + matchDetails[0].Port);
            w.WriteMember("ServerName");
            w.WriteString(matchDetails[0].ServerName);
            w.WriteMember("Map");
            w.WriteString(matchDetails[0].Map);
            w.WriteMember("Mod");
            w.WriteString(matchDetails[0].Modification);
            w.WriteMember("MatchStart");
            w.WriteString(matchDetails[0].MatchStart.Value.ToString("MM/dd/yyyy HH:mm"));
            w.WriteMember("MatchEnd");
            w.WriteString(matchDetails[0].MatchEnd.Value.ToString("MM/dd/yyyy HH:mm"));
            w.WriteMember("Players");
            w.WriteStartArray();
            foreach (MatchDetail matchDetail in matchDetails)
            {
                w.WriteStartObject();
                w.WriteMember("AliasGraphic");
                w.WriteString(WebMain.Images.GetQuakeImageUrl(new NameGraphic((Game)matchDetail.GameId, matchDetail.AliasBytes)));
                w.WriteMember("PlayerId");
                w.WriteNumber(matchDetail.PlayerId);
                w.WriteMember("PlayerMatchStart");
                w.WriteString(matchDetail.PlayerMatchStart.Value.ToString("MM/dd/yyyy HH:mm"));
                w.WriteMember("PlayerMatchEnd");
                w.WriteString(matchDetail.PlayerMatchEnd.Value.ToString("MM/dd/yyyy HH:mm"));
                w.WriteMember("Skin");
                w.WriteString(matchDetail.Skin);
                w.WriteMember("Model");
                w.WriteString(matchDetail.Model);
                w.WriteMember("Frags");
                w.WriteNumber(matchDetail.Frags);
                w.WriteMember("PantColor");
                w.WriteNumber(matchDetail.PantColor);
                w.WriteMember("ShirtColor");
                w.WriteNumber(matchDetail.ShirtColor);
                w.WriteMember("FPM");
                w.WriteNumber(matchDetail.FPM);
                w.WriteEndObject();
            }
            w.WriteEndArray();
            w.WriteEndObject();
        }

        Response.ContentType = "application/json";
        Response.Write(w.ToString());
        Response.Flush();
        Response.Close();
    }

    private void ReturnMatches(int pPlayerId)
    {
        IList<PlayerMatches> matches = Store.GetLastTenPlayerMatches(pPlayerId);

        StringWriter writer = new StringWriter();

        JsonTextWriter w = new JsonTextWriter(writer);

        w.WriteStartObject();
        w.WriteMember("Matches");
        w.WriteStartArray();
        foreach (PlayerMatches match in matches)
        {
            w.WriteStartObject();
            w.WriteMember("MatchId");
            w.WriteNumber(match.MatchId.ToString());
            w.WriteMember("HostName");
            w.WriteString(match.HostName);
            w.WriteMember("Map");
            w.WriteString(match.Map);
            w.WriteMember("Mod");
            w.WriteString(match.Modification);
            w.WriteMember("JoinTime");
            w.WriteString(match.PlayerJoinTime.Value.ToString("MM/dd/yyyy HH:mm"));
            w.WriteMember("StayDuration");
            w.WriteNumber(match.PlayerStayDuration.ToString());
            w.WriteMember("MatchStart");
            w.WriteString(match.MatchStart.Value.ToString("MM/dd/yyyy HH:mm"));
            w.WriteMember("Skin");
            w.WriteString(match.Skin);
            w.WriteMember("Model");
            w.WriteString(match.Model);
            w.WriteMember("Frags");
            w.WriteNumber(match.Frags);
            w.WriteMember("PantColor");
            w.WriteNumber(match.PantColor);
            w.WriteMember("ShirtColor");
            w.WriteNumber(match.ShirtColor);
            w.WriteMember("ShirtColor");
            w.WriteNumber(match.ShirtColor);
            w.WriteEndObject();
        }
        w.WriteEndArray();
        w.WriteEndObject();

        Response.ContentType = "application/json";
        Response.Write(w.ToString());
        Response.Flush();
        Response.Close();
    }

    private void ReturnSummery(int pPlayerId)
    {
        IList<PlayerDetail> details = Store.GetPlayerDetail(pPlayerId);

        PlayerDetailSummery summery = new PlayerDetailSummery(details);

        StringWriter writer = new StringWriter();

        JsonTextWriter w = new JsonTextWriter(writer);

        w.WriteStartObject();
        w.WriteMember("Aliases");
        w.WriteStartArray();
        foreach (PlayerDetailSummery.Alias alias in summery.Aliases)
        {
            w.WriteStartObject();
            w.WriteMember("Alias");
            w.WriteString(alias.AliasName);
            w.WriteMember("AliasLastSeen");
            w.WriteString(alias.AliasSeen != null
                ? alias.AliasSeen.Value.ToString("MM/dd/yyyy HH:mm")
                : "N/A");
            w.WriteMember("AliasGraphic");
            w.WriteString(WebMain.Images.GetQuakeImageUrl(new NameGraphic((Game)summery.GameId, alias.AliasBytes)));
            w.WriteMember("AliasPlayerId");
            w.WriteNumber(alias.AliasPlayerId);
            w.WriteEndObject();
        }
        w.WriteEndArray();
        w.WriteMember("PlayerId");
        w.WriteNumber(pPlayerId);
        w.WriteMember("LastSeen");
        w.WriteString(summery.LastSeen.ToString("MM/dd/yyyy HH:mm"));
        w.WriteMember("LastServer");
        w.WriteString(summery.LastServer);
        w.WriteMember("LastMap");
        w.WriteString(summery.LastMap);
        w.WriteMember("TotalFrags");
        w.WriteNumber(summery.TotalFragsSum.ToString());
        w.WriteMember("TotalTime");
        w.WriteNumber(summery.TotalTimeSum.ToString());
        w.WriteMember("TotalFPM");
        w.WriteNumber(summery.TotalFPM.ToString());
        w.WriteMember("YearFrags");
        w.WriteNumber(summery.YearFragsSum.ToString());
        w.WriteMember("YearTime");
        w.WriteNumber(summery.YearTimeSum.ToString());
        w.WriteMember("YearFPM");
        w.WriteNumber(summery.YearFPM.ToString());
        w.WriteMember("MonthFrags");
        w.WriteNumber(summery.MonthFragsSum.ToString());
        w.WriteMember("MonthTime");
        w.WriteNumber(summery.MonthTimeSum.ToString());
        w.WriteMember("MonthFPM");
        w.WriteNumber(summery.MonthFPM.ToString());
        w.WriteMember("WeekFrags");
        w.WriteNumber(summery.WeekFragsSum.ToString());
        w.WriteMember("WeekTime");
        w.WriteNumber(summery.WeekTimeSum.ToString());
        w.WriteMember("WeekFPM");
        w.WriteNumber(summery.WeekFPM.ToString());
        w.WriteMember("DayFrags");
        w.WriteNumber(summery.DayFragsSum.ToString());
        w.WriteMember("DayTime");
        w.WriteNumber(summery.DayTimeSum.ToString());
        w.WriteMember("DayFPM");
        w.WriteNumber(summery.DayFPM.ToString());

        w.WriteEndObject();

        Response.ContentType = "application/json";
        Response.Write(w.ToString());
        Response.Flush();
        Response.Close();
    }
}
