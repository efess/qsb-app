using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.SessionState;
using QSB.Common;
using QSB.Data.TableObject;

/// <summary>
/// Summary description for SessionStore
/// </summary>
public class SessionStore
{
    private const string SESSION_LOGIN_LINKBACK = "LoginLinkBack";
    private const string SESSION_ACCESS = "USERACCESS";
    private HttpSessionState _session;

    public SessionStore(HttpSessionState pSessionObject)
    {
        if (pSessionObject == null)
            throw new ArgumentNullException("Constructor cannot accept Null Session object");

        _session = pSessionObject;
    }

    public UserAccess SessionAccess
    {
        get
        {
            return _session[SESSION_ACCESS] as UserAccess;
        }
        set
        {
            _session[SESSION_ACCESS] = value;
        }
    }

    public bool AdminAccess
    {
        get
        {
            return SessionAccess != null
                && SessionAccess.AccessLevel == (int)AccessLevel.Administrator;
        }
    }

    public String LoginLinkBack
    {
        get
        {
            return _session[SESSION_LOGIN_LINKBACK] as string;
        }
        set
        {
            _session[SESSION_LOGIN_LINKBACK] = value;
        }
    }
}
