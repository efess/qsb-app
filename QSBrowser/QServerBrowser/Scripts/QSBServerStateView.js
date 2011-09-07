var serverDataObject
var FilterGame = "All";
var FilterRegion = "All";
var FilterMod = "All";

function ServerState(ServerDataObject )
{
    this.serverDataObject = ServerDataObject;
}

ServerState.prototype.SetFilter = function(pFilterGame, pFilterRegion, pFilterMod)
{
    FilterGame = pFilterGame;
    FilterRegion = pFilterRegion;
    FilterMod = pFilterMod;
}

ServerState.prototype.GetTableHTML = function()
{
    if(this.serverDataObject == null)
        return "Server data not available";
         
    var HTML = "<table>";
        HTML += "<tr><th>DNS</TH><TH>Name</TH><TH>Players</TH><TH>Mod</TH><TH>Map</TH><TH>Status</TH><TH>Time Queried</TH></TR>";
         
    var getPlayerRow = function(playerData)
    {
        var returnHTML = "";
        returnHTML += "<tr class\"darkBack\"><td style=\"width:200px\"><a href=\"#\" onClick=\"ShowPlayerDetail('" + playerData.PlayerId + "', '" + playerData.NameGraphic + "')\"><img src=\"" + playerData.NameGraphic + "\" border=\"0\"></a></td>";
        returnHTML += "<td><table border = \"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td align=\"center\"><img src=\"Images/q1color" + playerData.Shirt + ".gif\"></td></tr><tr><td align=\"center\"><img src=\"Images/q1color" + playerData.Pant + ".gif\"></td></tr></table></td>";
        returnHTML += "<td>" + playerData.Score  + "</td><td>" + playerData.TotalScore + "</td><td>" + playerData.TotalPlayTime + "</td>";
        returnHTML += "<td>" + playerData.CurrentFPM + "</td></tr>";
        return returnHTML;
    }
     
    var getServerRow = function(serverData)
    {
        var returnHTML = "";
        returnHTML += "<tr class=\"d0\"><td>" + serverData.DNS + ":" + serverData.Port + "</td><td>" + serverData.Name + "</td><td>" + serverData.CurrentPlayerCount + "/" + serverData.MaxPlayers + "</td>";
        returnHTML += "<td>" + serverData.Mod + "</td><td>" + serverData.Map + "</td><td>" + serverData.Status + "</td><td>" + FormatStringDate(serverData.TimeQueried, "time") + "</td></tr>";
        return returnHTML;
    }
    
    var getServerPlayersBlock = function(serverData)
    {
        var returnHTML = "";
        returnHTML += "<tr><td colspan=\"4\">"
        returnHTML += "<table>"
        returnHTML += "<TR><TH>Player Name</TH><TH>Color</TH><TH>Score</TH><TH>Total Score</TH><TH>Total PlayTime</TH><TH>Current FPM</TH></TR>";
        
        $.each(serverData.Players, function(j,player){
            
            returnHTML += getPlayerRow(player);
            
            return true;
        });
                    
        returnHTML += "</table>";
        returnHTML += "</td></tr>"
        return returnHTML;
    }
       
    $.each(this.serverDataObject.Servers, function(i, server){
        
        if(FilterGame == "All" || FilterGame == server.Game)
        if(FilterRegion == "All" || FilterRegion == server.Region)
        {
            HTML += getServerRow(server);
            if(server.Players.length > 0)
            {
                HTML += getServerPlayersBlock(server);
            }
            else
            { 
                //HTML += getServerRow(server);
            }
        }
        return true;
        
    });
                        
    HTML += "</table>";
    return HTML;
     
}
