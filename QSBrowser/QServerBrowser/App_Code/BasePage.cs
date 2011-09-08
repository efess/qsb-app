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
using System.Collections.Generic;
using System.Text;
using QSB.Data;

/// <summary>
/// Summary description for BasePage
/// </summary>
public class BasePage : System.Web.UI.Page
{
    private const string HTTP_CONTEXT_STORE = "Store";

    protected SessionStore SessionStore;

    public BasePage()
    {
    
    }

    protected void PageLoad()
    {
        InitializeSession();
    }

    public enum JSDateTimeFormat
    {
        DateTimeSmall,
        DateTimeLarge,
        Time,
        DateSmall,
        DateLarge
    }

    public static string GetDateTimeContainer(DateTime pDateTime, JSDateTimeFormat pFormat)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<span class=\"");
        switch (pFormat)
        {
            case JSDateTimeFormat.DateLarge:
                sb.Append("dateLarge");
                break;
            case JSDateTimeFormat.DateSmall:
                sb.Append("dateSmall");
                break;
            case JSDateTimeFormat.DateTimeLarge:
                sb.Append("dateTimeLarge");
                break;
            case JSDateTimeFormat.DateTimeSmall:
                sb.Append("dateTimeSmall");
                break;
            case JSDateTimeFormat.Time:
                sb.Append("time");
                break;
        }
        sb.Append("\">" + pDateTime.ToString("MM/dd/yyyy HH:mm") + "</span>");
        return sb.ToString();
    }

    /// <summary>
    /// Should be called in all Page_Load sections - initializes SessionStore object
    /// </summary>
    protected void InitializeSession()
    {
        SessionStore = new SessionStore(Session);
        
    }

    public IDataSession Store
    {
        get
        {
            if(HttpContext.Current.Items[HTTP_CONTEXT_STORE] == null)
            {

                IDataSession sm = WebMain.DataFactory.GetDataSession();
                HttpContext.Current.Items[HTTP_CONTEXT_STORE] = sm;
                return sm;
            }
            else
                return HttpContext.Current.Items[HTTP_CONTEXT_STORE] as IDataSession;
        }
    }

    public string GetLoginString()
    {
        //if (SessionStore.SessionAccess != null)
        //{
        //    if (SessionStore.SessionAccess.UserAccess == null)
        //    {
        //        return "Unknown user specified";
                
        //    }

        //    if (!SessionStore.SessionAccess.CorrectPassword)
        //    {
        //        return "Incorrect password for user " + SessionStore.SessionAccess.UserAccess.Name;
        //    }
        //    else
        //    {
        //        return SessionStore.SessionAccess.UserAccess.Name + " logged in with " + ((AccessLevel)SessionStore.SessionAccess.UserAccess.AccessLevel).ToString() + " access";
        //    }
        //}
        return "";
    }

}
