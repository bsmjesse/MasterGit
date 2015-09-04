<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Legend.aspx.cs" Inherits="SentinelFM.Legend" meta:resourcekey="PageResource1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Legend</title>
    <style type="text/css">
        body {font-family: 	tahoma,​arial,​verdana,​sans-serif; font-size: 12px;}
        .withinlastday {background-color: #7BB273 !important;}
        .withinlast2days {background-color: #EFD700 !important;}
        .withinlast3days {background-color: #FFA64A !important;}
        .withinlast7days {background-color: #DE7973 !important;}
        .morethan7days {background-color: #637DA5 !important;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table border="0" cellspacing="10">
            <tr><td class="withinlastday" width="100">&nbsp;</td><td>
                <asp:Label ID="Label1" runat="server" 
                    Text="Within last day ( &lt; 24 hours)" meta:resourcekey="Label1Resource1"></asp:Label></td></tr>
            <tr><td class="withinlast2days" width="100">&nbsp;</td><td>
                <asp:Label ID="Label2" runat="server" 
                    Text="Within last 2 days ( &lt; 48 hours)" meta:resourcekey="Label2Resource1"></asp:Label></td></tr>
            <tr><td class="withinlast3days" width="100">&nbsp;</td><td>
                <asp:Label ID="Label3" runat="server" 
                    Text="Within last 3 days ( &lt; 72 hours)" meta:resourcekey="Label3Resource1"></asp:Label></td></tr>
            <tr><td class="withinlast7days" width="100">&nbsp;</td><td>
                <asp:Label ID="Label4" runat="server" 
                    Text="Within last 7 days ( &lt; 168 hours)" meta:resourcekey="Label4Resource1"></asp:Label></td></tr>
            <tr><td class="morethan7days" width="100">&nbsp;</td><td>
                <asp:Label ID="Label5" runat="server" Text="More than 7 days ( &gt; 168 hours)" 
                    meta:resourcekey="Label5Resource1"></asp:Label></td></tr>
        </table>
    
    </div>
    </form>
</body>
</html>
