<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmContactUs.aspx.cs" Inherits="frmContactUs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>WEXSMART</title>
     <link href="GlobalStyle.css" type="text/css" rel="stylesheet">
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    <table border=0 cellpadding=5 cellspacing=5 class=formtext    >
        <tr>
            <td><b>WEXSMART Customer Support</b></td>
        </tr>
        <tr>
            <td>Toll Free: <b> 1-866-666-5061</b></td>
        </tr>
        <tr>
            <td>Email:<a href="mailto:wexsmartservice@wrightexpress.com">wexsmartservice@wrightexpress.com</a> </td>
        </tr>
        <tr>
            <td align="right" style="height:20px" >
            </td>
        </tr>
        <tr>
            <td align="right"><asp:Button ID="cmdClose" runat="server" Text="Close" OnClientClick="window.close();"  CssClass="combutton" /></td>
        </tr>
    </table> 
     
        </div>
    </form>
</body>
</html>
