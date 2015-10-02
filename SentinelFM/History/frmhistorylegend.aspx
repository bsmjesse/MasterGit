<%@ Page Language="c#" Inherits="SentinelFM.History.frmHistoryLegend" CodeFile="frmHistoryLegend.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Legends</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio 7.0"/>
    <meta name="CODE_LANGUAGE" content="C#"/>
    <meta name="vs_defaultClientScript" content="JavaScript"/>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
</head>
<body>
    <form method="post" runat="server">
        <table class="formtext" id="Table1" style="z-index: 101; left: 8px; width: 200px;
            position: absolute; top: 8px; height: 187px" cellspacing="0" cellpadding="0"
            width="200" border="0">
            <tr>
                <td class="heading" style="width: 183px">
                    <asp:Label ID="lblLegendTitle" runat="server" CssClass="heading" meta:resourcekey="lblLegendTitleResource1">Legend</asp:Label>
                </td>
                <td class="heading">
                    <asp:Label ID="lblIconTitle" runat="server" CssClass="heading" meta:resourcekey="lblIconTitleResource1">Icon</asp:Label>
                </td>
            </tr>
            <tr>
                <td class="heading" style="width: 183px"></td>
                <td class="heading"></td>
            </tr>
            <tr>
                <td style="width: 183px">
                    <asp:Label ID="lblStopLessThan15" runat="server" meta:resourcekey="lblStopLessThan15Resource1">Stops for less than 15 min.</asp:Label>
                </td>
                <td>
                    <asp:Image ID="Image1" runat="server" ImageUrl="../images/STOP_3.ico" meta:resourcekey="Image1Resource1"></asp:Image>
                </td>
            </tr>
            <tr>
                <td style="width: 183px"></td>
                <td></td>
            </tr>
            <tr>
                <td style="width: 183px">
                    <asp:Label ID="lblStop15To60" runat="server" meta:resourcekey="lblStop15To60Resource1">Stops from 15 to 60 min.</asp:Label>
                </td>
                <td>
                    <asp:Image ID="Image2" runat="server" ImageUrl="../images/STOP_15.ico" meta:resourcekey="Image2Resource1"></asp:Image>
                </td>
            </tr>
            <tr>
                <td style="width: 183px"></td>
                <td></td>
            </tr>
            <tr>
                <td style="width: 183px; height: 24px">
                    <asp:Label ID="lblStopMoreThan60" runat="server" meta:resourcekey="lblStopMoreThan60Resource1">Stops for more than 60 min.</asp:Label>
                </td>
                <td style="height: 24px">
                    <asp:Image ID="Image3" runat="server" ImageUrl="../images/STOP_60.ico" meta:resourcekey="Image3Resource1"></asp:Image>
                </td>
            </tr>
            <tr>
                <td style="width: 183px"></td>
                <td></td>
            </tr>
            <tr>
                <td style="width: 183px">
                    <asp:Label ID="lblVehicleIdle" runat="server" meta:resourcekey="lblVehicleIdleResource1">Vehicle Idle.</asp:Label>
                </td>
                <td>
                    <asp:Image ID="Image4" runat="server" ImageUrl="../images/IDLE.ico" meta:resourcekey="Image4Resource1"></asp:Image>
                </td>
            </tr>
            <tr>
                <td style="width: 183px"></td>
                <td></td>
            </tr>
            <tr>
                <td style="width: 183px">
                    <asp:Button runat="server" CssClass="combutton" ID="cmdClose" OnClientClick="window.close()" Text="Close" meta:resourcekey="cmdCloseResource1" />
                </td>
                <td></td>
            </tr>
        </table>
    </form>
</body>
</html>