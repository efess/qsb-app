using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using QSB.Data;
using QSB.Common;

/// <summary>
/// Static instances 
/// </summary>
public static class WebMain
{
    private static ServerManager serverMain;
    public static ServerManager ServerMain
    {
        get
        {
            if(serverMain == null)
                serverMain = new ServerManager(DataBridge.GetSession());

            return serverMain;
        }
    }

    public static Status WebStatus;
    public static ImageManager Images;
}
