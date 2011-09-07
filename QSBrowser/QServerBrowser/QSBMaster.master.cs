using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


public partial class QSBMaster : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SessionStore session = new SessionStore(Session);
        // Admin links if Admin
        if (session.AdminAccess)
        {
            adminLinks.Controls.Add(new LiteralControl("<p>Admin Links</p>"));

            adminLinks.Controls.Add(new LiteralControl("<br />"));
            adminLinks.Controls.Add(new LiteralControl("-<a href=\"monitor.aspx\">Debug Monitor</a>"));
            adminLinks.Controls.Add(new LiteralControl("<br />"));
            adminLinks.Controls.Add(new LiteralControl("-<a href=\"serverlist.aspx\">Manage Servers</a>"));
        }
        
    }
}
