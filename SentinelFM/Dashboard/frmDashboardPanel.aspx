<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmDashboardPanel.aspx.cs" Inherits="SentinelFM.frmDashboardPanel" %>

<%@ Register Assembly="ISNet.WebUI.WebDesktop" Namespace="ISNet.WebUI.WebDesktop"
    TagPrefix="ISWebDesktop" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Untitled Page</title>
</head>
<body id="body" runat="server" leftmargin="0" topmargin="0" rightmargin="0" bottommargin="0">
    <form id="form1" runat="server">
        <div  style="vertical-align:middle; text-align:center; "  >
            <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
            <ISWebDesktop:WebPaneManager ID="WebPaneDashBoard" runat="server" Height="100%" Width="100%">
                <RootGroupPane Name="RootGroup" GroupType=HorizontalTile >
                    <Panes>
                        <ISWebDesktop:WebPane Name="Pane0" Text="Pane 0">
                        </ISWebDesktop:WebPane>
                    </Panes>
                </RootGroupPane>
                <ImagesSettings SplitterGripBottom="SplitterDown.gif" SplitterGripLeft="SplitterLeft.gif"
                    SplitterGripRight="SplitterRight.gif" SplitterGripTop="SplitterUp.gif" />
                <SplitterStyle>
                    <Active BackColor="Black" BaseStyle="Normal">
                    </Active>
                    <Over BaseStyle="Normal">
                    </Over>
                    <Normal BackColor="#E0E0E0">
                    </Normal>
                </SplitterStyle>
                <FrameStyle BackColor="#E0E0E0" BorderColor="#404040" BorderStyle="Solid" BorderWidth="1px"
                    ForeColor="White">
                    <Padding Bottom="4px" Left="4px" Right="4px" Top="4px" />
                </FrameStyle>
                <PaneSettings>
                    <HeaderSubStyle BackColor="#E0E0E0" BackColor2="White" BorderColor="Gray" BorderStyle="Solid"
                        BorderWidth="1px" Font-Names="Tahoma" Font-Size="8pt" GradientType="Vertical">
                        <Padding Bottom="4px" Left="4px" Right="4px" Top="4px" />
                    </HeaderSubStyle>
                    <InfoTextStyle Font-Names="Tahoma" Font-Size="8pt" ForeColor="White">
                    </InfoTextStyle>
                    <ContainerStyle BackColor="White" BorderColor="Gray" BorderStyle="Solid" BorderWidth="1px"
                        Font-Names="Tahoma" Font-Size="8pt" ForeColor="Black" Overflow="Auto" OverflowX="Auto"
                        OverflowY="Auto">
                        <Padding Bottom="4px" Left="4px" Right="4px" Top="4px" />
                    </ContainerStyle>
                    <HeaderMainStyle BackColor="DimGray" BackColor2="LightGray" BorderColor="Gray" BorderStyle="Solid"
                        BorderWidth="1px" Font-Bold="True" Font-Names="Arial" Font-Size="11pt" ForeColor="White"
                        GradientType="Vertical">
                        <Padding Bottom="4px" Left="4px" Right="4px" Top="4px" />
                    </HeaderMainStyle>
                </PaneSettings>
            </ISWebDesktop:WebPaneManager>
        </div>
    </form>
</body>
</html>
