// Globals
var modalWindow;
var timerInterval = 1000;
var lastRefresh;
var timer;
var currentServerState;
var serverExpandedArray = [];
var serverBaseURL = "http://localhost:47475/QServersSite/";
// Document Ready function
$(document).ready(function() 
{
    if (top.frames.length!=0) {
        if (window.location.href.replace)
            top.location.replace(self.location.href);
        else
            top.location.href=self.document.href; 
            }
    RefreshServers();
    lastRefresh = new Date();
    TimerTick();
});

function TimerTick(){
    var dtNow = new Date();
    var testRefresh = new Date(dtNow.getTime()-15000)
    if(lastRefresh < testRefresh){
        lastRefresh = dtNow;
        RefreshServers();
    } 
    $("#liveStatus").html(MsToMinSecs(dtNow.getTime() - lastRefresh.getTime()));
    timer = setTimeout("TimerTick()", timerInterval);
}
function CleanContent()
{   
    ConvertAllUTCTimesToLocal();
	$('#close_d').click(function(){
	    HideDialog();
	});
}

function ConvertAllUTCTimesToLocal()
{
    $(".dateTimeSmall").each(function(idx, item) {
        item.innerHTML = FormatStringDate(item.innerHTML, "dateTimeSmall");
    });
    $(".dateTimeLarge").each(function(idx, item) {
        item.innerHTML = FormatStringDate(item.innerHTML, "dateTimeLarge");
    });
    $(".dateSmall").each(function(idx, item) {
        item.innerHTML = FormatStringDate(item.innerHTML, "dateSmall");
    });
    $(".dateLarge").each(function(idx, item) {
        item.innerHTML = FormatStringDate(item.innerHTML, "dateLarge");
    });
    $(".time").each(function(idx, item) {
        item.innerHTML = FormatStringDate(item.innerHTML, "time");
    });
}

function InitState()
{
    if(modalWindow == null)
    {
        modalWindow = new TopWindow();
        
    }
}

function onFilterChange()
{
    if(currentServerState != null)
    {
        currentServerState.SetFilter($('#selFilterGame').val(), $('#selFilterRegion').val(), "All");
        $("#pnlServerList").html(currentServerState.GetTableHTML())
        CleanContent();
    }
}

function RefreshServers()
{
    $.post("http://servers.quakeone.com/php/test.php" 
        , { action: "get" } 
        ,function(data) {
            currentServerState = new ServerState(data);
            $("#pnlServerList").html(currentServerState.GetTableHTML())
            CleanContent();
        }, "json");
}

function PutServerInfo(serverInfoData)
{
}

function ShowDialog(innerHTML, width, height)
{
    InitState();
    modalWindow.setSize(width, height);
    modalWindow.setHTML(innerHTML);
    modalWindow.show();
}

function HideDialog(){if(modalWindow != null) modalWindow.hide();}

function ShowPlayerDetailDialog(playerData, playerGraphic)
{ 
    var aliasCount = 0;
    var moded = 0;
    var aliasHTML = "";
    var hourlyHTML = "";
    
    aliasHTML += "<table border=\"0\" width=\"100%\">";
    aliasHTML += "<tr class=\"d1\"><th>Seen</th><th>alias</th>";
    $.each(playerData.Aliases, function(i,alias) {
        if(alias.AliasGraphic != playerGraphic
            && alias.Alias != ""
            && alias.Alias != "unconnected")
        {
            aliasHTML += "<tr><td>"
            if(alias.AliasLastSeen != "N/A")
                { aliasHTML += FormatStringDate(alias.AliasLastSeen, "dateSmall") + "</span>";}
            else
                { aliasHTML += alias.AliasLastSeen; }
            aliasHTML += "</td><td><a href=\"#\" onClick=\"ShowPlayerDetail('" + alias.AliasPlayerId + "', '" + alias.AliasGraphic + "')\"><img src=\"" + alias.AliasGraphic + "\" border=\"0\"></a></td>"; 
            aliasCount++;
        }
        return true;
    });
    aliasHTML+= "</table>";
        
    var HTML = "<table border=\"0\" width=\"100%\">";    
    HTML += "<tr><td><table border=\"0\" width=\"100%\">";
    HTML += "<tr><td id =\"playerName\"></td></td></tr>";
    HTML += "<tr><td>Details of :</td><td colspan=\"3\"><img src=\"" + playerGraphic + "\"></td></tr>";
    HTML += "<tr><td colspan=\"4\">Frag Stats:</td></tr>";
    HTML += "<tr><td></td><th>Frags</th><th>Time</th><th>FPM</th></tr>";
    HTML += "<tr class=\"d1\"><th>Today</th><td id=\"dayFrags\">" + playerData.DayFrags.toString() + "</td><td id=\"dayTime\">" + SecondsToHoursMinutes(playerData.DayTime.toString()) + "</td><td id=\"dayFPM\">" + playerData.DayFPM.toString() + "</td></tr>";
    
    HTML += "<tr class=\"d1\"><th>Week</th><td id=\"weekFrags\"";
    if(playerData.DayTime != playerData.WeekTime)
        HTML += ">" + playerData.WeekFrags.toString() + "</td><td id=\"weekTime\">" + SecondsToHoursMinutes(playerData.WeekTime.toString()) + "</td><td id=\"weekFPM\">" + playerData.WeekFPM.toString() + "</td></tr>";
    else
        HTML += " align=\"center\">-</td><td id=\"weekTime\" align=\"center\">-</td><td id=\"weekFPM\" align=\"center\">-</td></tr>";
        
    HTML += "<tr class=\"d1\"><th>" + month[lastRefresh.getMonth()].toString() + "</th><td id=\"monthFrags\"";
    if(playerData.WeekTime != playerData.MonthTime)
        HTML += ">" + playerData.MonthFrags.toString() + "</td><td id=\"monthTime\">" + SecondsToHoursMinutes(playerData.MonthTime.toString()) + "</td><td id=\"monthFPM\">" + playerData.MonthFPM.toString() + "</td></tr>";
    else
        HTML += " align=\"center\">-</td><td id=\"monthTime\" align=\"center\">-</td><td id=\"monthFPM\" align=\"center\">-</td></tr>";
        
    HTML += "<tr class=\"d1\"><th>" + lastRefresh.getFullYear().toString() +"</th><td id=\"yearFrags\"";
    if(playerData.MonthTime != playerData.YearTime)
        HTML += ">" + playerData.YearFrags.toString() + "</td><td id=\"yearTime\">" + SecondsToHoursMinutes(playerData.YearTime.toString()) + "</td><td id=\"yearFPM\">" + playerData.YearFPM.toString() + "</td></tr>";
    else
        HTML += " align=\"center\">-</td><td id=\"yearkTime\" align=\"center\">-</td><td id=\"yearFPM\" align=\"center\">-</td></tr>";
        
    HTML += "<tr class=\"d1\"><th>Total</th><td id=\"totalFrags\"";
    if(playerData.YearTime != playerData.TotalTime)        
        HTML += ">" + playerData.TotalFrags.toString() + "</td><td id=\"totalTime\">" + SecondsToHoursMinutes(playerData.TotalTime.toString()) + "</td><td id=\"totalFPM\">" + playerData.TotalFPM.toString() + "</td></tr>";
    else
        HTML += " align=\"center\">-</td><td id=\"totalTime\" align=\"center\">-</td><td id=\"totalFPM\" align=\"center\">-</td></tr>";
        
    if(aliasCount > 0)
    {
        HTML += "<tr><td>Other aliases:</td></tr>";
        HTML += "<tr><td colspan=\"4\" id=\"alias1\"><div style=\"position:relative; left:2px; top:2px; width:300px; height:110px; overflow:auto;\">";
        // Alias Div
        HTML += aliasHTML;
        HTML += "</div></td></tr>";
    }
    
    HTML += "</table></td><td valign=\"top\">";
    HTML += "<table border=\"0\" width=\"100%\">";
    HTML += "<tr><td>Last Seen</td><td>" + FormatStringDate(playerData.LastSeen, "dateTimeSmall") + "</span></td></tr>";
    HTML += "<tr><td>Last Server</td><td>" + playerData.LastServer + "</td></tr>";
    HTML += "</td></tr><tr><td colspan=\"2\">Last 10 Matches</td></tr>";
    HTML += "</td></tr><tr><td colspan=\"2\" id=\"playerMatches\"></td></tr></table>";
    
    ShowDialog(HTML, 800,330);
    CleanContent();    
    
    $.post("PlayerDetails.aspx?Request=Matches&PlayerId=" + playerData.PlayerId.toString()
        , { action: "get" } 
        ,function(playerMatches) {
        
            var matchHTML = "<table border=\"0\" width=\"100%\">";
            matchHTML += "<tr><th>Id</th><th>Server</th><th>Date</th><th>Length</th><th>Mod</th><th>Map</th></tr>"
            
            $.each(playerMatches.Matches, function(i, playerMatch){
                matchHTML += "<tr><td><a href=\"#\" onClick=\"ShowMatchDetail('" + playerMatch.MatchId.toString() + "')\">" + playerMatch.MatchId.toString() + "</a></td><td>" + playerMatch.HostName + "</td><td>" + DateTimeToDateTimeSmall(GMTToLocalDate(CSharpDateStringToJSDate(playerMatch.JoinTime))) + "</td><td>" + SecondsToHoursMinutes(playerMatch.StayDuration)+ "</td><td>" + playerMatch.Mod + "</td><td>" + playerMatch.Map + "</td></tr>";
                
            });
            
            matchHTML += "</table>";
            $("#playerMatches").html(matchHTML);
            CleanContent();
        }, "json");
}

function ShowPlayerDetail(playerId, playerGraphic) {

    $.post("PlayerDetails.aspx?Request=Summery&PlayerId=" + playerId.toString()
        , { action: "get" }
        , function (data) {
            ShowPlayerDetailDialog(data, playerGraphic);
        }, "json");

    
//    $.getJSON("PlayerDetails.aspx?Request=Summery&PlayerId=" + playerId.toString(),
//        function(data) { ShowPlayerDetailDialog(data, playerGraphic);}
//    );    
}

function ShowMatchDetailDialog(matchData)
{
    var matchHTML = "";
    
    matchHTML += "<table border=\"0\" width=\"100%\">";
    matchHTML += "<tr><td>";
    matchHTML += "<table border=\"0\" width=\"100%\">";
    matchHTML += "<tr><td>Match Id:</td><td>" + matchData.MatchId.toString() + "</td><td>Address:</td><td>" + matchData.HostName + "</td></tr>";
    matchHTML += "<tr><td>Server Name:</td><td>" + matchData.ServerName + "</td><td>Map:</td><td>" + matchData.Map + "</td></tr>";
    matchHTML += "<tr><td>Match Start:</td><td>" + DateTimeToDateTimeSmall(GMTToLocalDate(CSharpDateStringToJSDate(matchData.MatchStart))) + "</td><td>Mod:</td><td>" + matchData.Mod + "</td></tr>";
    matchHTML += "<tr><td>Match End:</td><td>" + DateTimeToDateTimeSmall(GMTToLocalDate(CSharpDateStringToJSDate(matchData.MatchEnd))) + "</td><td>Players:</td><td></td></tr>";  
    matchHTML += "</table>";
    matchHTML += "</td></tr><tr><td>";
    matchHTML += "<table border=\"0\" width=\"100%\">";
    matchHTML += "<th>Alias</th><th>Color</th><th>Score</th><th>Join</th><th>Ended</th><th>FPM</th>";
    
    $.each(matchData.Players, function(i, player){
    
        matchHTML += "<tr class\"darkBack\"><td style=\"width:200px\"><a href=\"#\" onClick=\"ShowPlayerDetail('" + player.PlayerId + "', '" + player.AliasGraphic + "')\"><img src=\"" + player.AliasGraphic + "\" border=\"0\"></a></td>";
        matchHTML += "<td><table border = \"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td align=\"center\"><img src=\"Images\\q1color" + player.ShirtColor + ".gif\"></td></tr><tr><td align=\"center\"><img src=\"Images\\q1color" + player.PantColor + ".gif\"></td></tr></table></td>";
        matchHTML += "<td>" + player.Frags  + "</td><td>" + player.PlayerMatchStart + "</td><td>" + player.PlayerMatchEnd + "</td>";
        matchHTML += "<td>" + player.FPM + "</td></tr>";
        
    });
    matchHTML += "</table>";
    matchHTML += "</td></tr></table>";
    
    ShowDialog(matchHTML, 700,330);
    CleanContent();    
}

function ShowMatchDetail(matchId)
{
    $.post("PlayerDetails.aspx?Request=MatchDetail&MatchId=" + matchId.toString()
        , { action: "get" } 
        ,function(data) {
             ShowMatchDetailDialog(data);
        }, "json");
//    $.getJSON("PlayerDetails.aspx?Request=MatchDetail&MatchId=" + matchId.toString(),
//        function(data) { 
//        ShowMatchDetailDialog(data);
//    });
}
