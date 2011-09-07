<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="style.css" rel="stylesheet" type="text/css" />
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <p><p><p><p><p><p><p>
    <center>
    You are trying to access a secure page, please login
        <table width="300">
            <tr>
                <td class="style2" align="left">
                    Username</td>
                <td>
                    <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style3" align="left">
                    Password</td>
                <td class="style1">
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td>
                    <asp:Button ID="btnLogin" runat="server" Text="Login" onclick="btnLogin_Click" />
                </td>
                <% Response.Write(Server.GetLastError()); %>
            </tr>
        </table>
    </center>
    </div>
    </form>
</body>
</html>
