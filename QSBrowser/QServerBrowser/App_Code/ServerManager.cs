using System;
using System.Collections.Generic;
using System.Configuration;
using QSB.Common.Model;
using QSB.Data.ViewObject;
using QSB.Data;

/// <summary>
/// Summary description for ServerManager
/// </summary>
public class ServerManager
{
    /// <summary>
    /// Database Storage object
    /// </summary>
    IDataSession _Store = null;
    DateTime lastQuery = DateTime.MinValue;
    private const int QUERY_SECOND_COUNT = 3;
    List<ServerDetail> _ServerDetail;

    /// <summary>
    /// Main constructor to initialize members
    /// </summary>
    /// <param name="pDatabasePath">Absolute path of SQLite database file</param>
    public ServerManager(IDataSession pDataSession)
    {
        if (pDataSession == null)
            throw new ArgumentNullException("ServerManager requires a datasession");

        _Store = pDataSession;
        _ServerDetail = new List<ServerDetail>();
	}

    public IDataSession Store
    {
        get
        {
            return _Store;
        }
    }

    private void Refresh()
    {
        _ServerDetail.Clear();
        foreach (VServerDetail server in _Store.GetServerDetail())
        {
            if(server == null)
                continue;

            try
            {
                _ServerDetail.Add(server.GetServerDetail());
            }
            catch (Exception Ex)
            {
                WebMain.WebStatus.AddStatus("Error parsing GameServer data for serverid: " + server.ServerId
                    + Environment.NewLine + "Exception: " + Ex.ToString());
            }
             
        }
    }

    public List<ServerDetail> AllServerDetail
    {
        get
        {
            lock (_ServerDetail)
            {
                if (lastQuery < DateTime.Now.AddSeconds((-1) * QUERY_SECOND_COUNT))
                {
                    Refresh();
                    lastQuery = DateTime.Now;
                }

                return _ServerDetail;
            }
        }
    }
}