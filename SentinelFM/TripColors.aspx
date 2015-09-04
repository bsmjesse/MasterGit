<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TripColors.aspx.cs" Inherits="TripColors" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Legend</title>
    <style type="text/css">
        body {font-family: 	tahoma,​arial,​verdana,​sans-serif; font-size: 12px;}
        .plowon {background-color: #ff69b4 !important;}
        .augeron {background-color: #47260F !important;}
        .augerplowon {background-color: #0000ff !important;}
        .augerplowoff {background-color: #008000 !important;} 
        .emergencylighton {background-color: #ff0000 !important;} 
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table border="0" cellspacing="10">
            <tr><td class="plowon" width="100">&nbsp;</td><td>
                <asp:Label ID="Label1" runat="server" 
                    Text="Plowing (Wing Plow and/or Front Plow DOWN, Auger OFF)" meta:resourcekey="Label1Resource1"></asp:Label></td></tr>
            <tr><td class="augeron" width="100">&nbsp;</td><td>
                <asp:Label ID="Label2" runat="server" 
                    Text="Sanding (Wing Plow and Front Plow UP, Auger ON)" meta:resourcekey="Label2Resource1"></asp:Label></td></tr>
            <tr><td class="augerplowon" width="100">&nbsp;</td><td>
                <asp:Label ID="Label3" runat="server" 
                    Text="Plowing and Sanding (Wing Plow and Front Plow DOWN, Auger ON)" meta:resourcekey="Label3Resource1"></asp:Label></td></tr>
            <tr><td class="augerplowoff" width="100">&nbsp;</td><td>
                <asp:Label ID="Label4" runat="server" 
                    Text="Driving (Wing Plow and Front Plow UP , Auger OFF)" meta:resourcekey="Label4Resource1"></asp:Label></td></tr>
             <tr><td class="emergencylighton" width="100">&nbsp;</td><td>
                <asp:Label ID="Label5" runat="server" 
                    Text="Emergency Light ON" meta:resourcekey="Label4Resource1"></asp:Label></td></tr>
        </table>
    
    </div>
    </form>
</body>
</html>
