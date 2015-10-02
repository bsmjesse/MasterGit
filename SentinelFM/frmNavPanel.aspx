<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmNavPanel.aspx.cs" Inherits="frmNavPanel" %>

<%@ Register Assembly="ISNet.WebUI.WebDesktop" Namespace="ISNet.WebUI.WebDesktop"
    TagPrefix="ISWebDesktop" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Untitled Page</title>
</head>
<body id="body" runat="server" leftmargin="0" topmargin="0" rightmargin="0" bottommargin="0">
    <form id="form1" runat="server">
        <div>
            <ISWebDesktop:WebPaneManager ID="WebPaneManager1" runat="server" Height="100%" Width="100%">
                <FrameStyle BorderStyle="Solid" ForeColor="White" BorderWidth="1px" BorderColor="#404040"
                    BackColor="#E0E0E0">
                    <Padding Top="4px" Left="4px" Bottom="4px" Right="4px"></Padding>
                </FrameStyle>
                <PaneSettings HeaderVisible="No">
                    <HeaderSubStyle BorderStyle="Solid" BackColor2="White" BorderWidth="1px"
                        BorderColor="Gray" Font-Size="8pt" Font-Names="Tahoma" BackColor="#E0E0E0" GradientType="Vertical">
                        <Padding Top="4px" Left="4px" Bottom="4px" Right="4px"></Padding>
                    </HeaderSubStyle>
                    <InfoTextStyle ForeColor="White" Font-Size="8pt" Font-Names="Tahoma">
                    </InfoTextStyle>
                    <ContainerStyle BorderStyle="Solid" ForeColor="Black" BorderWidth="1px" BorderColor="Gray"
                        Font-Size="8pt" Font-Names="Tahoma" BackColor="White" Overflow="Auto" OverflowX="Auto"
                        OverflowY="Auto">
                        <Padding Top="4px" Left="4px" Bottom="4px" Right="4px"></Padding>
                    </ContainerStyle>
                    <HeaderMainStyle BorderStyle="Solid" ForeColor="White" BackColor2="LightGray"
                        BorderWidth="1px" BorderColor="Gray" Font-Size="11pt" Font-Names="Arial" Font-Bold="True"
                        BackColor="DimGray" GradientType="Vertical">
                        <Padding Top="4px" Left="4px" Bottom="4px" Right="4px"></Padding>
                    </HeaderMainStyle>
                </PaneSettings>
                <RootGroupPane GroupType="VerticalTile" Image="/CommonLibrary/Images/WebDesktop/"
                    Name="RootGroup">
                    <Panes>
                        <ISWebDesktop:WebGroupPane GroupType="VerticalTile" Name="GroupPane0" Text="GroupPane 0">
                            <Panes>
                                <ISWebDesktop:WebPane Name="Pane0" Text="Pane 0" Width="Custom" WidthValue="30%">
                                    <ContentTemplate>
                                        <ISWebDesktop:WebNavPane ID="WebNavPane1" runat="server" Height="100%" Width="100%"
                                            GripImage BarVisible="3">
                                            <BarItemSettings DisplayMode="TextAndImage" TargetWindow="paneContent">
                                                <ItemStyle>
                                                    <Active BorderStyle="Solid" BorderWidth="1px" BorderColor="#0000C0" BaseStyle="Normal"
                                                        Font-Bold="True" BackColor="#C0C0FF">
                                                    </Active>
                                                    <Over BaseStyle="Normal" BackColor="WhiteSmoke">
                                                    </Over>
                                                    <Normal Font-Size="8.25pt" Font-Names="Tahoma" Overflow="Hidden" Cursor="Hand">
                                                        <Padding Top="3px" Left="6px" Bottom="3px" Right="6px"></Padding>
                                                    </Normal>
                                                </ItemStyle>
                                                <ItemsContainerStyle Width="100%" Font-Size="8.25pt" Font-Names="Tahoma" Height="100%"
                                                    Overflow="Auto" OverflowY="Auto">
                                                    <Padding Top="4px" Left="4px" Bottom="4px" Right="4px"></Padding>
                                                </ItemsContainerStyle>
                                            </BarItemSettings>
                                            <BarSettings >
                                                <CaptionStyle ForeColor="White" BackColor2="245, 245, 245" Font-Size="11pt" Font-Names="Arial"
                                                    Font-Bold="True" BackColor="Gray" Overflow="Hidden">
                                                    <Padding Top="4px" Left="4px" Bottom="4px" Right="4px"></Padding>
                                                </CaptionStyle>
                                                <BarStyle>
                                                    <Active BaseStyle="Normal" BackColor="LightGray">
                                                    </Active>
                                                    <Over BaseStyle="Normal" BackColor="LightSteelBlue">
                                                    </Over>
                                                    <Normal BackColor2="245, 245, 245" Font-Size="8.25pt" Font-Names="Tahoma" Font-Bold="True"
                                                        BackColor="White" GradientType="Vertical" Overflow="Hidden" Cursor="Hand">
                                                    </Normal>
                                                </BarStyle>
                                            </BarSettings>
                                            <Bars>
                                               <ISWebDesktop:WebNavBar CaptionDisplayMode="TextAndImage" Name="navbar_Mail" Text="Main">
                                                  <Items>
                                                     <ISWebDesktop:WebNavBarItem Name="itm_Home" TargetURL="Home/frmHome.aspx" Text="Home" />
                                                     <ISWebDesktop:WebNavBarItem Name="itm_Map" TargetURL="Map/frmMain.aspx" Text="Map Screen" />
                                                  </Items>
                                               </ISWebDesktop:WebNavBar>
                                               <ISWebDesktop:WebNavBar CaptionDisplayMode="TextAndImage" ContentMode="UseInlineContent"
                                                  Name="navbar_Calendar" Text="History">
                                                  <ContentTemplate>
                                                        <asp:Calendar ID="Calendar1" runat="server" BackColor="White" BorderColor="#999999"
                                                            CellPadding="4" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt"
                                                            ForeColor="Black" Height="180px" Width="100%">
                                                            <SelectedDayStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
                                                            <TodayDayStyle BackColor="#CCCCCC" ForeColor="Black" />
                                                            <SelectorStyle BackColor="#CCCCCC" />
                                                            <WeekendDayStyle BackColor="#FFFFCC" />
                                                            <OtherMonthDayStyle ForeColor="#808080" />
                                                            <NextPrevStyle VerticalAlign="Bottom" />
                                                            <DayHeaderStyle BackColor="#CCCCCC" Font-Bold="True" Font-Size="7pt" />
                                                            <TitleStyle BackColor="#999999" BorderColor="Black" Font-Bold="True" />
                                                        </asp:Calendar>
                                                  </ContentTemplate>
                                                  <ItemSettings Alignment="Center" DisplayMode="TextAndImage" ImagePosition="AboveText">
                                                  </ItemSettings>
                                                  <Items>
                                                     <ISWebDesktop:WebNavBarItem Name="itm_Calendar1" Text="Calendar 1" />
                                                     <ISWebDesktop:WebNavBarItem Name="itm_Calendar2" Text="Calendar 2" />
                                                     <ISWebDesktop:WebNavBarItem Name="itm_Calendar3" Text="Calendar 3" />
                                                  </Items>
                                               </ISWebDesktop:WebNavBar>
                                               <ISWebDesktop:WebNavBar CaptionDisplayMode="TextAndImage" Name="navbar_Contacts"
                                                  Text="Messages">
                                               </ISWebDesktop:WebNavBar>
                                               <ISWebDesktop:WebNavBar CaptionDisplayMode="TextAndImage" Name="navbar_Tasks" Text="Reports">
                                               </ISWebDesktop:WebNavBar>
                                               <ISWebDesktop:WebNavBar CaptionDisplayMode="TextAndImage" Name="navbar_Notes" Text="Configuration">
                                               </ISWebDesktop:WebNavBar>
                                                
                                            </Bars>
                                            <FrameStyle BorderWidth="1px" BorderColor="Silver" BackColor="White">
                                            </FrameStyle>
                                            <FooterSettings>
                                                <FooterStyle BackColor2="220, 220, 220" HorizontalAlign="Right" BackColor="White"
                                                    GradientType="Vertical" Overflow="Hidden"></FooterStyle>
                                            </FooterSettings>
                                            <GripStyle>
                                                <Active BackColor2="0, 0, 0" BaseStyle="Normal" BackColor="Gray">
                                                </Active>
                                                <Over BaseStyle="Normal">
                                                </Over>
                                                <Normal BackColor2="128, 128, 128" BackColor="LightGray" GradientType="Vertical"
                                                    Overflow="Hidden">
                                                </Normal>
                                            </GripStyle>
                                        </ISWebDesktop:WebNavPane>
                                    </ContentTemplate>
                                </ISWebDesktop:WebPane>
                                <ISWebDesktop:WebPane ContentMode="UseIFrame" ContentURL="Home/frmHome.aspx" Name="paneContent"
                                    Text="Pane 01">
                                </ISWebDesktop:WebPane>
                            </Panes>
                        </ISWebDesktop:WebGroupPane>
                    </Panes>
                </RootGroupPane>
                <SplitterStyle>
                    <Active BaseStyle="Normal" BackColor="Black">
                    </Active>
                    <Over BaseStyle="Normal">
                    </Over>
                    <Normal BackColor="#E0E0E0">
                    </Normal>
                </SplitterStyle>
            </ISWebDesktop:WebPaneManager>
        </div>
    </form>
</body>


</html>
