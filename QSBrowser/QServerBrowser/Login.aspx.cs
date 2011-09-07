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


public partial class Login : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        base.PageLoad();
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {;
        UserAccess access = Store.GetAccess(txtUserName.Text);
        if(access.Password.Trim() == txtPassword.Text.Trim())
            SessionStore.SessionAccess = access;

        if (!string.IsNullOrEmpty(SessionStore.LoginLinkBack))
            Response.Redirect(SessionStore.LoginLinkBack);
        else
            Response.Redirect("Default.aspx");
    }
}
