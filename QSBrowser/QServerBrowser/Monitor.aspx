<%@ Page Language="C#" MasterPageFile="QSBMaster.master"  AutoEventWireup="true" CodeFile="Monitor.aspx.cs" Inherits="Monitor" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Main" Runat="Server">


<div>

    <table> 
	    <tr> 
		    <td runat="server" ID="sideCell" valign="top"></td> 
		    <td valign="top"><textarea id="txtStatus" runat="server" readonly="readonly" rows="2" cols="20" style="height:700px;width:500px;"/>
		    </td>
	    </tr>
	    </table>
        <asp:Panel ID="pnlMain" runat="server">
        </asp:Panel>
    
    </div>
    <asp:Button ID="btnRefresh" runat="server" onclick="btnRefresh_Click" 
        Text="Refresh" />
    <asp:Button ID="btnStart" runat="server" onclick="btnStart_Click" 
        Text="Start" />
</asp:Content>