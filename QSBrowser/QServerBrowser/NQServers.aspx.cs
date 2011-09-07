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
using QSB.Data.TableObject;
using System.Collections.Generic;
using System.Text;
using QSB.Common;
using QSB.Common.Model;

public partial class NQServers : System.Web.UI.Page
{
    private const string CLIENT_PROQUAKE = "proquake";
    private const string CLIENT_QRACK = "qrack";
    private const string CLIENT_DARKPLACES = "darkplaces";

    private const string REQUEST_KEY_CLIENT = "client";
    private const string SERVER_CACHE_OBJECT = "ServerManager";

    protected void Page_Load(object sender, EventArgs e)
    {
        string queryeeName = Request[REQUEST_KEY_CLIENT];
        if(string.IsNullOrEmpty(queryeeName))
            return;

        byte[] returnByteArray = null;

        switch (queryeeName)
        {
            case CLIENT_PROQUAKE:
                returnByteArray = GetProQuakeServerByteArray(new List<ServerDetail>(WebMain.ServerMain.AllServerDetail));
                break;
            case CLIENT_QRACK:
                returnByteArray = GetQRackServerByteArray(new List<ServerDetail>(WebMain.ServerMain.AllServerDetail));
                break;
        }

        if (returnByteArray == null || returnByteArray.Length == 0)
            return;

        //char[] returnChars = Encoding.ASCII.GetChars(returnByteArray);'
        Response.ContentType = "text/plain";
        Response.BinaryWrite(returnByteArray);
        Response.Flush();
        Response.Close();
    }

    private byte[] GetQRackServerByteArray(List<ServerDetail> serverDetails)
    {
        StringBuilder sb = new StringBuilder();
        List<ServerDetail> serverList = new List<ServerDetail>();

        foreach (ServerDetail server in serverDetails)
        {
            if (server.GameId == Game.NetQuake)
               // && server.Status == ServerStatus.Running)
                serverList.Add(server);
        }

        serverList.Sort(new Comparison<ServerDetail>(delegate(ServerDetail sv1, ServerDetail sv2)
        {
            int comp = sv2.Players.Count.CompareTo(sv1.Players.Count);
            if (comp == 0)
                return sv2.Region.CompareTo(sv1.Region);
            return comp;
        }));

        bool first = true;
        foreach (ServerDetail server in serverList)
        {
            if (!first)
            {
                sb.Append(System.Environment.NewLine);
            }
            first = false;
            sb.Append(Left(server.IpAddress + ":" + server.Port, 35));
        }

        return Encoding.ASCII.GetBytes(sb.ToString());
    }

    private byte[] GetProQuakeServerByteArray(List<ServerDetail> serverInfos)
    {
        StringBuilder sb = new StringBuilder();
        List<ServerDetail> serverList = new List<ServerDetail>();

        foreach (ServerDetail server in serverInfos)
        {
            if (server.GameId == Game.NetQuake)
                //&& server.Status == ServerStatus.Running)
                serverList.Add(server);
        }

        serverList.Sort(new Comparison<ServerDetail>(delegate(ServerDetail sv1, ServerDetail sv2)
        {
            int comp = sv2.Players.Count.CompareTo(sv1.Players.Count);
            if (comp == 0)
                return sv2.Region.CompareTo(sv1.Region);
            return comp;
        }));

        bool first = true;
        foreach (ServerDetail server in serverList)
        {
            if (!first)
            {
                sb.Append(System.Environment.NewLine);
            }
            first = false;
            Region region;
            try
            {
                region = (Region)Enum.Parse(typeof(Region), server.Region);
            }
            catch
            {
                region = Region.UnitedStates;
            }

            sb.Append(server.DNS + ":" + server.Port).Append('\t');
            sb.Append(ProQuakeRegionCode(region)).Append(' ');
            sb.Append(Left(server.ServerName, 19).PadRight(19, ' ')).Append(' ');
            sb.Append(server.Players.Count.ToString().PadLeft(2,'0') + "/" + server.MaxPlayers.ToString().PadLeft(2,'0')).Append(' ');
            sb.Append(Left(server.Map, 6).PadRight(6, ' ')).Append(' ');
            sb.Append(ProQuakeModCode(server.ModificationCode)).Append(' ');
        }

        return Encoding.ASCII.GetBytes(sb.ToString());
    }

    private string ProQuakeModCode(string pMod)
    {
        // New Way: (mod code)
        return string.IsNullOrEmpty(pMod)
            ? "dm"
            : pMod.Substring(0, Math.Min(pMod.Length, 2)).ToLower();
        // Old way
        //switch (pMod.ToUpper())
        //{
        //    case "CLAN ARENA":
        //        return "ca";
        //    case "CRMOD":
        //        return "dm";
        //    default:
        //        return "dm";
        //}
    }
    private string ProQuakeRegionCode(Region pRegion)
    {
        switch (pRegion)
        {
            case Region.UnitedStates:
                return "us";
            case Region.Europe:
                return "eu";
            case Region.Brazil:
                return "br";
            default:
                return "us";
        }
    }

    private string Left(string pString, int pLength)
    {
        if (string.IsNullOrEmpty(pString))
            return string.Empty;
        return pString.Substring(0, Math.Min(pString.Length, pLength));
    }
}
