var width = 400;
var height = 300;

function TopWindow()
{
    this.topWindow = document.createElement("div");
    this.topWindow.id = "topWindow";
    this.topWindow.style.backgroundColor = "#333"
    this.topWindow.style.height = height.toString() + "px";
    this.topWindow.style.width = width.toString() + "px";
    this.topWindow.style.position = "absolute";
    this.topWindow.style.zIndex = "3"; 
    
    var bodyTable = document.createElement("table");
    bodyTable.setAttribute("cellspacing",0);
    bodyTable.setAttribute("cellpadding",3);
    bodyTable.setAttribute("border",1);
    bodyTable.setAttribute("width","100%");
    
    var mainRow = document.createElement("tr");
    var mainCell = document.createElement("td");
    mainRow.appendChild(mainCell);
    mainCell.setAttribute("valign","top");
    
    var controlRow = document.createElement("tr");
    var controlCell = document.createElement("td");
    controlRow.appendChild(controlCell);
    
    var closeLink = document.createElement("a");
    closeLink.setAttribute("onClick","HideDialog()");
    closeLink.setAttribute("href","#");
    closeLink.innerHTML = "[X] close";
    controlCell.appendChild(closeLink);
    
    
    this.setHTML = function(innerHTML)
    {
        mainCell.innerHTML = innerHTML;
        //this.topWindow.innerHTML = innerHTML;
    }
    
    this.resetWidths = function()
    {
        this.topWindow.style.height = height.toString() + "px";
        this.topWindow.style.width = width.toString() + "px";
        mainCell.style.width = width.toString() + "px";
        mainCell.style.height = (height - 30).toString() + "px";
        this.centerBox();
    }
    
    bodyTable.appendChild(mainRow);
    bodyTable.appendChild(controlRow);
    
    this.topWindow.appendChild(bodyTable);  
}

TopWindow.prototype.setSize = function(x, y)
{
    height = y;
    width = x;
    this.resetWidths();
}

TopWindow.prototype.centerBox = function()
{
    var x = 0;
    var y = 0;
    var xOffset = 0;
    var yOffset = 0;
    
    if( window.innerWidth != undefined ) {
        x = window.innerWidth;
        y = window.innerHeight;
    } else if( document.documentElement && ( document.documentElement.clientWidth || document.documentElement.clientHeight ) ) {
        x = document.documentElement.clientWidth;
        y = document.documentElement.clientHeight;
    } else if( document.body && ( document.body.clientWidth || document.body.clientHeight ) ) {
        x = document.body.clientWidth;
        y = document.body.clientHeight;
    }
    
    if(window.pageXOffset == undefined) {
        xOffset = document.documentElement.scrollLeft;
        yOffset = document.documentElement.scrollTop;
    }
    else{
        xOffset = window.pageXOffset;
        yOffset = window.pageYOffset;
    }
    
    
    x = ((x - width) / 2) + xOffset;
    y = ((y - height) / 2) +yOffset;
    
    this.topWindow.style.top = y.toString() + "px";
    this.topWindow.style.left = x.toString() + "px";
}

TopWindow.prototype.show = function()
{
    this.resetWidths();
	$('body').append(this.topWindow);
}

TopWindow.prototype.hide = function()
{
	$('#topWindow').remove()
}