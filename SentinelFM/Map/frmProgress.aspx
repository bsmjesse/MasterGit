<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmProgress.aspx.cs" Inherits="Map_frmProgress" %>
<%@ Register Assembly="BusyBoxDotNet" Namespace="BusyBoxDotNet" TagPrefix="busyboxdotnet" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Progress</title>
    <base target="_self" />
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
    <meta http-equiv = "refresh" content = "1" />
    <script type="text/javascript">
    </script>
</head>
<body>
    <form id="formProgress" method="post" runat="server">
    <div>
        <br />
        <asp:Label ID="lblStatus" runat="server" CssClass="RegularText" Text="Processing... Please wait..."></asp:Label>
        <br />
        <asp:Image ID="imgWait" runat="server" Width="105px" ImageUrl="../images/prgBar.gif" Height="10px"></asp:Image>
    </div>
    </form>
</body>
</html>
