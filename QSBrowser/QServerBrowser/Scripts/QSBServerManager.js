var serverDataObject

// Document Ready function
$(document).ready(function () {
    if (top.frames.length != 0) {
        if (window.location.href.replace)
            top.location.replace(self.location.href);
        else
            top.location.href = self.document.href;
    }
    $.post("ServerList.aspx?e=json",  
        , { action: "get" } 
        , function (data) {
           GetTableHTML(data);
       });
});


function GetTableHTML(serverData) {

    var HTML = "<table>";
    HTML += "<tr><th>Game</TH><TH>Name</TH><TH>DNS</TH><TH>Port</TH><TH>IP</TH><TH>Interval</TH><TH>Mod</TH><TH>Region</TH><TH>Location</TH><TH>Status</TH><TH></TH></TR>";

    var getServerRow = function (serverData) {
        var returnHTML = "";
        returnHTML += "<tr class=\"d0\"><td>" + serverData.DNS + ":" + serverData.Port + "</td><td>" + serverData.Name + "</td><td>" + serverData.CurrentPlayerCount + "/" + serverData.MaxPlayers + "</td>";
        returnHTML += "<td>" + serverData.Mod + "</td><td>" + serverData.Map + "</td><td>" + FormatStringDate(serverData.TimeQueried, "time") + "</td></tr>";
        return returnHTML;
    }

    $.each(this.serverDataObject.Servers, function (i, server) {

        if (FilterGame == "All" || FilterGame == server.Game)
            if (FilterRegion == "All" || FilterRegion == server.Region) {
                HTML += getServerRow(server);
                if (server.Players.length > 0) {
                    HTML += getServerPlayersBlock(server);
                }
                else {
                    //HTML += getServerRow(server);
                }
            }
        return true;

    });

    HTML += "</table>";
    return HTML;
}
//$.post("test.php", { name: "John", time: "2pm" },
//   function (data) {
//       alert("Data Loaded: " + data);
//   });