<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmDashboard.aspx.cs" Inherits="SentinelFM.frmDashboard" %>

<%@ Register Assembly="ISNet.WebUI.WebDesktop" Namespace="ISNet.WebUI.WebDesktop"
    TagPrefix="ISWebDesktop" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Untitled Page</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
    
    <script language="javascript">
			<!--
			 function Test()
			 {
			    alert("ok"); 
			 }
			-->
    </script>
    
    
</head>
<body leftmargin="0" topmargin="0" rightmargin="17" bottommargin="0">
    <form id="form1" runat="server">
        <div>
          <fieldset class="formtext"  runat="server" id="MaintenanceServices" style="margin: 0px 20px 20px 0px;
                                padding: 0px 20px 20px 20px;" visible="False">
                                <legend>
                                    <asp:Label ID="lblTotals" runat="server" Text="Dashboard Summary"
                                        ></asp:Label>
                                </legend>
                                
                                <table>
                                    <tr>
                                        <td><asp:Label ID="lblAlarmsCaption" runat="server" Text="Alarms"/></td>
                                        <td><asp:Label ID="lblAlarms" runat="server" Text=""/></td>
                                        <td><asp:Label ID="lblMessagesCaption" runat="server" Text="Messages"/></td>
                                        <td><asp:Label ID="lblMessages" runat="server" Text=""/></td>
                                        <td><asp:Label ID="lblNotificationCaptions" runat="server" Text="Notification"/></td>
                                        <td><asp:Label ID="lblNotification" runat="server" Text=""/></td>
                                        <td><asp:Label ID="lblActivitySummaryCaption" runat="server" Text="Activity Summary"/></td>
                                        <td><asp:Label ID="lblActivitySummary" runat="server" Text=""/></td>
                                        <td>
                                            </asp:Label>
                                        </td>
                                    </tr>
                                </table> 
            </fieldset> 
            <table style="width: 100%" class="formtext"  >
                <tr>
                    <td valign=top  >
                        <ISWebDesktop:WebExplorerPane ID="WebPane1" runat="server" Height="100%"
                            Width="100%">
                            <PaneSettings>
                                <BarFontStyle>
                                    <Normal Font-Bold="True" Font-Names="Tahoma" Font-Size="8.25pt">
                                    </Normal>
                                    <Over ForeColor="Gray">
                                    </Over>
                                </BarFontStyle>
                                <SpecialBarFontStyle>
                                    <Normal Font-Bold="True" Font-Names="Tahoma" Font-Size="8.25pt" ForeColor="White">
                                    </Normal>
                                    <Over ForeColor="Gainsboro">
                                    </Over>
                                </SpecialBarFontStyle>
                                <BarStyle>
                                    <Normal BackColor="WhiteSmoke" BackColor2="211, 211, 211" Cursor="Hand" Font-Bold="True"
                                        Font-Names="Tahoma" Font-Size="8.25pt" Overflow="Hidden" OverflowX="Hidden" OverflowY="Hidden">
                                    </Normal>
                                    <Over ForeColor="Gray">
                                    </Over>
                                </BarStyle>
                                <SpecialBarStyle>
                                    <Normal BackColor="Gray" BackColor2="0, 0, 0" Cursor="Hand" Font-Bold="True" Font-Names="Tahoma"
                                        Font-Size="8.25pt" ForeColor="White" Overflow="Hidden" OverflowX="Hidden" OverflowY="Hidden">
                                    </Normal>
                                    <Over ForeColor="Gainsboro">
                                    </Over>
                                </SpecialBarStyle>
                                <BarFrameStyle BorderColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                                </BarFrameStyle>
                            </PaneSettings>
                            <FrameStyle BackColor="White" BackColor2="224, 224, 224" BorderColor="Silver" BorderWidth="1px"
                                GradientType="Vertical">
                                <Padding Bottom="2px" Left="2px" Right="2px" Top="2px" />
                            </FrameStyle>
                            <PaneItemSettings>
                                <ItemStyle>
                                    <Normal Cursor="Hand" Font-Names="Tahoma" Font-Size="8.25pt" Overflow="Hidden">
                                        <Padding Bottom="3px" Left="6px" Right="6px" Top="3px" />
                                    </Normal>
                                    <Over BackColor="WhiteSmoke" BaseStyle="Normal">
                                    </Over>
                                    <Active BackColor="#C0C0FF" BaseStyle="Normal" BorderColor="#0000C0" BorderStyle="Solid"
                                        BorderWidth="1px" Font-Bold="True">
                                    </Active>
                                </ItemStyle>
                                <ItemsContainerStyle BackColor="White" Font-Names="Tahoma" Font-Size="8.25pt">
                                    <Padding Bottom="4px" Left="4px" Right="4px" Top="4px" />
                                </ItemsContainerStyle>
                            </PaneItemSettings>
                        </ISWebDesktop:WebExplorerPane>
                    </td>
                </tr>
            </table>
        </div>
        
    </form>
</body>
</html>
