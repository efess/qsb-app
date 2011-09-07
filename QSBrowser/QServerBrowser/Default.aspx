<%@ Page Language="C#" MasterPageFile="QSBMaster.master" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" Culture="auto" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Main" Runat="Server">
    <table runat="server" id="gridFilter" align="left" width="1000">
        <tr>
            <td></td>
            <td>
                <div style="width:170px">Filter Game 
                <select onchange="javascript:onFilterChange()" id="selFilterGame"> 
			    <option value="All">All</option> 
			    <option value="0">Net Quake</option> 
			    <option value="1">Quake World</option> 
			    <option value="2">Quake2</option> 
			    <option value="3">Quake3</option> 
			    <option value="4">Quake4</option> 
     
		        </select></div> 
            
            </td> 
		    <td>
		        <div style="width:350px">Filter Region 
		        <select onchange="javascript:onFilterChange()" id="selFilterRegion"> 
			    <option selected="selected" value="All">All</option> 
			    <option value="UnitedStates">United States</option> 
			    <option value="Brazil">Brazil</option> 
			    <option value="Europe">Europe</option> 
		        </select></div> 
		    
            </td>
        </tr>
        <tr>
            <td>Last refresh <span id="liveStatus"></span> ago </td>
        </tr>
        <tr>
        
            <td colspan="3">
                <div id="pnlServerList">
                </div>
            </td>
        </tr>
    </table>
</asp:Content>    
	
