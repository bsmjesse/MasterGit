<%@ Page Language="c#" Inherits="SentinelFM.Map.frmSensorsWait" CodeFile="frmSensorsWait.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Processing...</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio 7.0">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body>
    <form method="post" runat="server">
        <table id="tblWait" style=" z-index: 103; left: 110px; width: 216px;
            color: black; position: absolute; top: 149px; height: 125px" runat="server" class=formtext >
            <tr>
                <td style="height: 15px" align="center">
                    <asp:Label ID="lblPleaseWaitMessage" runat="server" meta:resourcekey="lblPleaseWaitMessageResource1">Please wait</asp:Label></td>
            </tr>
            <tr>
                <td style="height: 15px" align="center">
                    <asp:Label ID="lblLoadingMessage" runat="server" meta:resourcekey="lblLoadingMessageResource1">Loading Sensors Info...</asp:Label></td>
            </tr>
            <tr>
                <td style="height: 22px" align="center">
                </td>
            </tr>
            <tr>
                <td style="height: 22px">
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
