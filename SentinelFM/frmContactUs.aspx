<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmContactUs.aspx.cs" Inherits="SentinelFM.frmContactUs" meta:resourcekey="PageResource1" Culture="en-US" 
    UICulture="auto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Customer Support</title>
     <link href="GlobalStyle.css" type="text/css" rel="stylesheet">
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    <table border=0 cellpadding=0 cellspacing=0 class=formtext    >
        <tr>
            <td><b> 
                <asp:Label ID="lblCustomSupport" runat="server" 
                    Text="Customer Support" meta:resourcekey="lblCustomSupportResource1"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td> <asp:Label ID="lblTollFree" runat="server" Text="Toll Free Direct Line:" 
                    meta:resourcekey="lblTollFreeResource1"></asp:Label>  <asp:Label ID="lblTollFreeNumber" runat="server" Text="<b> 1-866-578-4315</b>" ></asp:Label></td>
        </tr>
        <tr>
            <td> <b> 
               
                </b><asp:Label ID="lblEmailLabel" runat="server" Text="Email:" 
                    meta:resourcekey="lblEmailLabelResource1"></asp:Label>  <a href="mailto:customercare@bsmwireless.com">
                <asp:Label ID="lblEmail" runat="server" Text="customercare@bsmwireless.com:" 
                    meta:resourcekey="lblEmailResource1"></asp:Label></a> </td>
        </tr>
        <tr>
            <td align="right" style="height:20px" >
                &nbsp;</td>
        </tr>
        <tr>
            <td align="right"><asp:Button ID="cmdClose" runat="server" Text="Close" 
                    OnClientClick="window.close();"  CssClass="combutton" 
                    meta:resourcekey="cmdCloseResource1" /></td>
        </tr>
    </table> 
     
        </div>
    </form>
</body>
</html>
