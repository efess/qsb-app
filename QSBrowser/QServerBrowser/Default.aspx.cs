using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using System.Collections.Generic;
using QSB.Common;
using System.IO;
using Jayrock.Json;
using QSB.Common.Model;

public partial class _Default : BasePage
{
    const string QUERY_VALUE_TABLE = "table";
    const string QUERY_VALUE_JSON = "json";
    const string QUERY_KEY_CALLBACK = "e";
    const string KEYWORD_ALL = "All";

    protected void Page_Load(object sender, EventArgs e)
    {
        base.PageLoad();
        SessionStore.LoginLinkBack = string.Empty;

        
        List<ServerDetail> accessor = WebMain.ServerMain.AllServerDetail;
        if (accessor == null)
        {
            Response.Write("Server Info not available");
            return;
        }
        
        List<ServerDetail> serverDetails = new List<ServerDetail>(accessor);

        string callback = Request[QUERY_KEY_CALLBACK];
        if (callback != null)
        {
            switch (callback)
            {
                case QUERY_VALUE_TABLE:
                    //ReturnList();
                    break;
                case QUERY_VALUE_JSON:
                    ReturnJSON(serverDetails);
                    break;
                default: Response.Write("Invalid request");
                    break;
            }
            return;
        }
        return;
        // Default page load:
        //HtmlTable serverTable = GetServerList(ServerCache.Cache);
        //if (serverTable != null)
        //{
        //    pnlServerList.Controls.Add(serverTable);
        //    pnlServerList.Controls.Add(new Label() { Text = "Refreshed at: " + DateTime.UtcNow.ToLongTimeString() });
        //}
    }

    //private void ReturnList()
    //{
    //    HtmlTable serverTable = GetServerList(ServerCache.Cache);
    //    HtmlTextWriter tw = new HtmlTextWriter(new StringWriter());

    //    serverTable.RenderControl(tw);
    //    Response.ContentType = "text/plain";
    //    Response.Write(tw.InnerWriter.ToString());
    //    Response.End();
    //}

    public void ReturnJSON(List<ServerDetail> pServerDetails)
    {
        pServerDetails.Sort(new Comparison<ServerDetail>(delegate(ServerDetail act1, ServerDetail act2)
        {
            int comp = act2.Players.Count.CompareTo(act1.Players.Count);
            if (comp == 0)
                return act2.Region.CompareTo(act1.Region);
            return comp;
        }));

        JsonTextWriter w = new JsonTextWriter(new StringWriter());
        w.WriteStartObject();
        w.WriteMember("Servers");
        w.WriteStartArray();
        foreach (ServerDetail server in pServerDetails)
        {
            w.WriteStartObject();
            w.WriteMember("DNS");
            w.WriteString(server.DNS);
            w.WriteMember("ServerId");
            w.WriteNumber(server.ServerId.ToString());
            w.WriteMember("IPAddress");
            w.WriteString(server.IpAddress);
            w.WriteMember("Port");
            w.WriteNumber(server.Port);
            w.WriteMember("TimeQueried");
            w.WriteString(server.Timestamp.ToString("MM/dd/yyyy HH:mm"));
            w.WriteMember("Name");
            w.WriteString(server.ServerName);
            w.WriteMember("CurrentPlayerCount");
            w.WriteNumber(server.Players.Count.ToString());
            w.WriteMember("MaxPlayers");
            w.WriteNumber(server.MaxPlayers.ToString());
            w.WriteMember("Mod");
            w.WriteString(server.Modification);
            w.WriteMember("Map");
            w.WriteString(server.Map);
            w.WriteMember("Region");
            w.WriteString(server.Region);
            w.WriteMember("Location");
            w.WriteString(server.Location);
            w.WriteMember("Game");
            w.WriteString(server.GameId.ToString());
            w.WriteMember("Status");
            w.WriteString(server.CurrentStatus.ToString());
            w.WriteMember("Players");
            w.WriteStartArray();

            if (server.Players.Count > 0
                && server.CurrentStatus == ServerStatus.Running)
            {
                server.Players.Sort(new Comparison<PlayerDetail>(delegate(PlayerDetail p1, PlayerDetail p2)
                    {
                        return p2.CurrentFrags.CompareTo(p1.CurrentFrags);
                    }));
                foreach (PlayerDetail player in server.Players)
                {
                    TimeSpan uptime = player.UpTime;
                    string strUpTime = uptime.Hours.ToString().PadLeft(2, '0') + ":" + uptime.Minutes.ToString().PadLeft(2, '0');
                    if (uptime.TotalDays > 0)
                        strUpTime = uptime.Days.ToString() + ":" + strUpTime;
                    //string graphicURL = WebMain.Images.GetQuakeImageUrl(player.Player);
                    w.WriteStartObject();
                    w.WriteMember("PlayerId");
                    w.WriteString(player.PlayerId.ToString());
                    w.WriteMember("NameGraphic");
                    w.WriteString(WebMain.Images.GetQuakeImageUrl(new NameGraphic(server.GameId, player.NameBytes)));
                    w.WriteMember("NameText");
                    w.WriteString(player.Name);
                    w.WriteMember("Score");
                    w.WriteString(player.CurrentFrags.ToString());
                    w.WriteMember("TotalScore");
                    w.WriteString(player.TotalFrags.ToString());
                    w.WriteMember("CurrentFPM");
                    w.WriteNumber(uptime.TotalMinutes > 0 ? Math.Round(player.TotalFrags / ((Decimal)uptime.TotalMinutes), 2).ToString("##0.00") : "0.00");
                    w.WriteMember("TotalPlayTime");
                    w.WriteString(strUpTime);
                    w.WriteMember("Skin");
                    w.WriteString(player.Skin);
                    w.WriteMember("Model");
                    w.WriteString(player.Model);
                    if (server.GameId == Game.NetQuake)
                    {
                        w.WriteMember("Shirt");
                        w.WriteNumber(player.ShirtColor);
                        w.WriteMember("Pant");
                        w.WriteNumber(player.PantColor);
                    }
                    w.WriteEndObject();
                }
            }
            w.WriteEndArray();
            w.WriteEndObject();
        }
        w.WriteEndArray();
        w.WriteEndObject();
        Response.ContentType = "text/plain";
        Response.Write(w.ToString());
        Response.Write(Server.GetLastError());
        Server.ClearError();
        Response.End();
    }

    //private HtmlTable GetServerList(ServerCache pServerCache)
    //{
    //    HtmlTable tabServers = new HtmlTable();
    //    //string selectedGame = gameList.SelectedValue;
    //    //string selectedRegion = regionList.SelectedValue;
    //    //int game = -1;

    //    //if (!string.IsNullOrEmpty(selectedGame)
    //    //    && selectedGame != KEYWORD_ALL)
    //    //{
    //    //    try
    //    //    {
    //    //        game = (int)Enum.Parse(typeof(Game), selectedGame);
    //    //    }
    //    //    catch (Exception e)
    //    //    {
    //    //        WebMain.WebStatus.AddStatus("SessionId: " + Session.SessionID + " GameConvert error: " + e.Message);
    //    //        Response.Write("Issue converting Game Enum, choose All from Game list");
    //    //        return null;
    //    //    }
    //    //}

    //    //int counter = 0;

    //    //
    //    //tabServers.Width = "100%";

    //    //HtmlTableRow row = new HtmlTableRow();
    //    //HtmlTableCell newCell;

    //    //newCell = new HtmlTableCell("th");
    //    //newCell.InnerText = "DNS";
    //    //row.Cells.Add(newCell);

    //    //newCell = new HtmlTableCell("th");
    //    //newCell.InnerText = "IP Address/Port";
    //    //row.Cells.Add(newCell);

    //    //newCell = new HtmlTableCell("th");
    //    //newCell.InnerText = "Time Queried";
    //    //row.Cells.Add(newCell);

    //    //// Add header columns
    //    //newCell = new HtmlTableCell("th");
    //    //newCell.InnerText = "Name";
    //    //row.Cells.Add(newCell);

    //    //newCell = new HtmlTableCell("th");
    //    //newCell.InnerText = "Players";
    //    //row.Cells.Add(newCell);

    //    //// Add header columns
    //    //newCell = new HtmlTableCell("th");
    //    //newCell.InnerText = "Mod";
    //    //row.Cells.Add(newCell);

    //    //newCell = new HtmlTableCell("th");
    //    //newCell.InnerText = "Current Map";
    //    //row.Cells.Add(newCell);

    //    //tabServers.Rows.Add(row);

    //    //List<ServerActivity> serverList = new List<ServerActivity>();

    //    //foreach (ServerActivity server in pServerCache)
    //    //{
    //    //    serverList.Add(server);
    //    //}

    //    //serverList.Sort(new Comparison<ServerActivity>((act1, act2) =>
    //    //{
    //    //    int comp = act2.ServerSnapshot.Players.Count.CompareTo(act1.ServerSnapshot.Players.Count);
    //    //    if (comp == 0)
    //    //        return act2.DbGameServer.Region.CompareTo(act1.DbGameServer.Region);
    //    //    return comp;
    //    //}));

    //    //foreach (ServerActivity activity in serverList)
    //    //{
    //    //    if (activity.DbGameServer.LastQuerySuccess == null
    //    //        || activity.DbGameServer.LastQuerySuccess.Value < DateTime.UtcNow.AddMinutes(-15))
    //    //        continue;
    //    //    if (game > -1 && activity.DbGameServer.GameId != game)
    //    //        continue;
    //    //    if (!string.IsNullOrEmpty(selectedGame) && selectedRegion != KEYWORD_ALL && activity.DbGameServer.Region != selectedRegion)
    //    //        continue;

    //    //    row = new HtmlTableRow();
    //    //    row.Attributes["class"] = "d0";// + (counter % 2).ToString();

    //    //    // Todo: Add "error" color, can use class "er"
    //    //    row.Cells.Add(new HtmlTableCell { InnerText = activity.DbGameServer.DNS });
    //    //    row.Cells.Add(new HtmlTableCell { InnerText = activity.ServerSnapshot.IpAddress + ":" + activity.ServerSnapshot.Port.ToString() });
    //    //    row.Cells.Add(new HtmlTableCell { InnerHtml = GetDateTimeContainer(activity.DbGameServer.LastQuery.Value, JSDateTimeFormat.Time) });
    //    //    row.Cells.Add(new HtmlTableCell { InnerText = activity.ServerSnapshot.ServerName });
    //    //    row.Cells.Add(new HtmlTableCell { InnerText = activity.ServerSnapshot.Players.Count.ToString() + "/" + activity.ServerSnapshot.MaxPlayerCount.ToString() });
    //    //    row.Cells.Add(new HtmlTableCell { InnerText = activity.DbGameServer.Mod });
    //    //    row.Cells.Add(new HtmlTableCell { InnerText = activity.ServerSnapshot.CurrentMap });

    //    //    counter++;
    //    //    tabServers.Rows.Add(row); // Server row

    //    //    if (activity.ServerSnapshot.Players.Count > 0)
    //    //    {
    //    //        activity.PlayerActivities.Sort(new Comparison<PlayerActivity>((p1,p2) => 
    //    //            {
    //    //            return p2.Player.Frags.CompareTo(p1.Player.Frags);
    //    //        }));

    //    //        HtmlTable tabPlayers = new HtmlTable();
    //    //        tabPlayers.Width = "100%";

    //    //        row = new HtmlTableRow();
                
    //    //        newCell = new HtmlTableCell("th");
    //    //        newCell.InnerText = "Name Graphic";
                
    //    //        row.Cells.Add(newCell);

    //    //        newCell = new HtmlTableCell("th");
    //    //        newCell.InnerText = "Name Text";
    //    //        row.Cells.Add(newCell);

    //    //        newCell = new HtmlTableCell("th");
    //    //        newCell.InnerText = "Frags";
    //    //        row.Cells.Add(newCell);

    //    //        newCell = new HtmlTableCell("th");
    //    //        newCell.InnerText = "Total Frags";
    //    //        row.Cells.Add(newCell);

    //    //        newCell = new HtmlTableCell("th");
    //    //        newCell.InnerText = "FPM";
    //    //        row.Cells.Add(newCell);

    //    //        newCell = new HtmlTableCell("th");
    //    //        newCell.InnerText = "Play Time";
    //    //        row.Cells.Add(newCell);

    //    //        tabPlayers.Rows.Add(row);

    //    //        foreach (PlayerActivity player in activity.PlayerActivities)
    //    //        {
    //    //            TimeSpan uptime = DateTime.UtcNow - player.Session.SessionStart.Value;
    //    //            string strUpTime = uptime.Hours.ToString().PadLeft(2,'0') + ":" + uptime.Minutes.ToString().PadLeft(2,'0');
    //    //            if (uptime.TotalDays > 0)
    //    //                strUpTime = uptime.Days.ToString() + ":" + strUpTime;
    //    //            string graphicURL = WebMain.Images.GetQuakeImageUrl(player.Player);
    //    //            row = new HtmlTableRow();
    //    //            row.Attributes["class"] = "d1";
    //    //            row.Cells.Add(new HtmlTableCell { InnerHtml = "<img src=\"" + graphicURL + "\" border=\"0\">" });
    //    //            row.Cells.Add(new HtmlTableCell { InnerHtml = "<a href=\"javascript:ShowPlayerDetail(" + player.Session.PlayerId.ToString() + ", '" + player.Player.PlayerName + "', '" + graphicURL + "')\">" + player.Player.PlayerName + "</a>"});
    //    //            row.Cells.Add(new HtmlTableCell { InnerHtml = player.Player.Frags.ToString() });
    //    //            row.Cells.Add(new HtmlTableCell { InnerText = player.TotalFrags.ToString() });
    //    //            row.Cells.Add(new HtmlTableCell { InnerText = uptime.TotalMinutes > 0 ? Math.Round(player.TotalFrags / ((Decimal)uptime.TotalMinutes), 2).ToString("##0.00") : "0.00" });
    //    //            row.Cells.Add(new HtmlTableCell { InnerText = strUpTime });
    //    //            tabPlayers.Rows.Add(row);
    //    //        }

    //    //        row = new HtmlTableRow(); // Player Table Row in Servers
    //    //        row.Cells.Add(new HtmlTableCell { InnerText = "" });

    //    //        newCell = new HtmlTableCell();
    //    //        newCell.Controls.Add(tabPlayers);
    //    //        newCell.ColSpan = 4;
    //    //        row.Cells.Add(newCell);
    //    //        tabServers.Rows.Add(row);
    //    //    }

    //    //}
    //    //if (counter == 0)
    //    //{
    //    //    row = new HtmlTableRow();

    //    //    row.Cells.Add(new HtmlTableCell { InnerText = "Nothing to display", ColSpan = 6 });

    //    //    tabServers.Rows.Add(row);
    //    //}

    //    return tabServers;        
    //}

    protected void tmrUpdateServers_Tick(object sender, EventArgs e)
    {
        // Trigger for UpdatePanel
    }
}
