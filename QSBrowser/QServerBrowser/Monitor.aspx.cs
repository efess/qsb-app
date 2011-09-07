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

using System.Text;
using QSB.Common;

public partial class Monitor : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        base.PageLoad();
        SessionStore.LoginLinkBack = string.Empty;

        if (!SessionStore.AdminAccess)
        {
            SessionStore.LoginLinkBack = "Monitor.aspx";
            Response.Redirect("Login.aspx");
        }

        ShowStatus();
    }

    protected void btnStart_Click(object sender, EventArgs e)
    {
        //if (WebMain.QueryScheduler == null)
        //    WebMain.QueryScheduler = new QueryThread(QSB.Data.DataBridge.GetSession());

        //WebMain.QueryScheduler.Start();
    }

    public void ShowStatus()
    {
        Status statusObject = WebMain.WebStatus;

        string statusBody = statusObject.GetStatus();
        txtStatus.Value = statusBody;

    }

    private HtmlTableCell EncapsulateHtmlBadGood(string pStr, bool pIsGood)
    {
        HtmlTableCell cell = new HtmlTableCell();
        cell.InnerText = pStr;
        cell.Attributes["align"] = "center";
        if (pIsGood)
            cell.BgColor = "#00FF00";
        else
            cell.BgColor = "#FF0000";
        return cell;
    }
    
    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        ShowStatus();
    }
}
