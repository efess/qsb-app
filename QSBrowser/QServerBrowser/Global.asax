<%@ Application Language="C#" %>
<%@ Import Namespace="QSB.Common" %>
<%@ Import Namespace="QSB.Data" %>

<script runat="server">
    
    private const string SERVER_CACHE_OBJECT = "ServerManager";
    private const string SERVER_CONNECTION_STRING_TEMPLATE = @"Data Source={0};Version=3;";
    
    void Application_Start(object sender, EventArgs e)
    {
        WebMain.WebStatus = new QSB.Common.Status(1000, System.IO.Path.Combine(Server.MapPath("~"), "Debug"));
        WebMain.Images = new ImageManager(System.IO.Path.Combine(Server.MapPath("~"), @"Images"));
        
        WebMain.WebStatus.AddStatus("Starting Browser ");
        
        string connectionString = ConfigurationSettings.AppSettings["ConnectionString"] as string;
        string stringDatabaseEngine = ConfigurationSettings.AppSettings["DatabaseEngine"] as string;

        QSB.Data.DataBridge.DatabaseEngine databaseEngine = QSB.Data.DataBridge.DatabaseEngine.SQLite;

        try
        {
            databaseEngine = (QSB.Data.DataBridge.DatabaseEngine)Enum.Parse(typeof(QSB.Data.DataBridge.DatabaseEngine), stringDatabaseEngine);
        }
        catch
        {
            if (string.IsNullOrEmpty(stringDatabaseEngine))
            {
                WebMain.WebStatus.AddStatus("No DatabaseEngine key/value entry in Config.");
            }
            else
            {
                WebMain.WebStatus.AddStatus("Invalid DatabaseEngine specified in Config: " + stringDatabaseEngine);
            }
            return;
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            WebMain.WebStatus.AddStatus("No ConnectionString key/value entry in Config.");
            return;
        }

        // Set Paths
        try
        {
            DataBridge.InitializeDatabase(connectionString, databaseEngine);
        }
        catch (Exception ex)
        {
            WebMain.WebStatus.AddStatus("Error initializing database: " + ex.ToString());
            return;
        }

        WebMain.WebStatus.AddStatus("Initialized " + databaseEngine + " database");
    }
    
    void Application_Error(object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs

        if (WebMain.WebStatus != null)
        {
            // Get the exception object.
            Exception exc = Server.GetLastError();
            string errorString = "Server ApplicationError : " + exc.ToString()  + Environment.NewLine + exc.StackTrace;
            if (exc.InnerException != null)
                errorString = errorString + Environment.NewLine + "InnerException: " + exc.InnerException.Message + Environment.NewLine + exc.StackTrace;
            WebMain.WebStatus.AddStatus(errorString);

        }
        // Clear the error from the server
        Server.ClearError();
    }

    void Session_Start(object sender, EventArgs e)
    {
        if(WebMain.WebStatus != null)
            WebMain.WebStatus.AddStatus(
                "IP Connected: " + Request.ServerVariables["REMOTE_HOST"] 
                + Environment.NewLine + "Browser: " + Request.ServerVariables["HTTP_USER_AGENT"]
                + Environment.NewLine + "URL: " + Request.Url.PathAndQuery
                + Environment.NewLine + "SessionID: " + Session.SessionID);
    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
       
</script>
