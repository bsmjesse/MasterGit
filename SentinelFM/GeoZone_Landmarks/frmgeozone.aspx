<%@ Page Language="c#" Inherits="SentinelFM.frmGeoZones" CodeFile="frmGeoZone.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register Src="Components/ctlGeozoneLandmarksMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>GeoZone</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body>
    <form id="frmGeozoneForm" method="post" runat="server">
        <table id="tblCommands" style="z-index: 101; left: 8px; position: absolute; top: 4px"
            cellspacing="0" cellpadding="0" width="300" border="0">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdGeoZone" />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" cellspacing="0" cellpadding="0" width="990" align="center" border="0">
                        <tr>
                            <td>
                                <table id="tblForm" height="550px" width="990px" class="table" border="0">
                                    <tr style="vertical-align:top;">
                                        <td class="configTabBackground" style="vertical-align:top;">
                                            <table id="Table1" style="width: 720px; height: 444px" cellspacing="0" cellpadding="0"
                                                width="990" align="center" border="0">
                                                <tr>
                                                    <td class="tableheading" align="left" height="5"></td>
                                                </tr>
                                                <tr>
                                                    <td align="center" style="vertical-align:top;">
                                                        <table id="tblGeoZones" height="200" cellspacing="0" cellpadding="0" width="720"
                                                            border="0" runat="server">
                                                            <tr>
                                                                <td>
                                                                    <asp:DataGrid ID="dgGeoZone" runat="server" Width="980px" GridLines="None" CellPadding="3"
                                                                        BackColor="White" BorderWidth="2px" CellSpacing="1" BorderColor="White" PageSize="13"
                                                                        AllowPaging="True" DataKeyField="GeozoneId" AutoGenerateColumns="False" BorderStyle="Ridge"
                                                                        OnDeleteCommand="dgGeoZone_DeleteCommand" OnItemCommand="dgGeoZone_ItemCommand"
                                                                        OnItemCreated="dgGeoZone_ItemCreated" meta:resourcekey="dgGeoZoneResource1">
                                                                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                        <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                                                                        <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                                                                        <HeaderStyle CssClass="DataHeaderStyle"></HeaderStyle>
                                                                        <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                        <Columns>
                                                                            <asp:BoundColumn DataField="GeozoneName" HeaderText='<%$ Resources:dgGeozone_GeozoneName %>'>
                                                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                            <asp:BoundColumn DataField="description" HeaderText='<%$ Resources:dgGeozone_Description %>'>
                                                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                            <asp:TemplateColumn HeaderText='<%$ Resources:dgGeozone_Direction %>'>
                                                                                <ItemStyle Wrap="False"></ItemStyle>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblDirection" Text='<%# GetDirectionName(DataBinder.Eval(Container.DataItem,"DirectionName")) %>'
                                                                                        runat="server" meta:resourcekey="lblDirectionResource1"></asp:Label>
                                                                                </ItemTemplate>
                                                                                <EditItemTemplate>
                                                                                    <asp:DropDownList ID="cboGeoZoneDirection" DataSource='<%# dsGeoZoneDirections %>'
                                                                                        DataValueField="DirectionId" DataTextField="DirectionName" SelectedIndex='<%# GetGeoZoneDirection(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"Type"))) %>'
                                                                                        runat="server" meta:resourcekey="cboGeoZoneDirectionResource1">
                                                                                    </asp:DropDownList>
                                                                                </EditItemTemplate>
                                                                            </asp:TemplateColumn>
                                                                            <asp:TemplateColumn HeaderText='<%$ Resources:dgGeozone_Severity %>'>
                                                                                <ItemStyle Wrap="False"></ItemStyle>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblSeverity" Text='<%# GetSeverityName(DataBinder.Eval(Container.DataItem,"SeverityName")) %>'
                                                                                        runat="server" meta:resourcekey="lblSeverityResource1"></asp:Label>
                                                                                </ItemTemplate>
                                                                                <EditItemTemplate>
                                                                                    <asp:DropDownList ID="cboSeverity" DataSource='<%# dsSeverity %>' DataValueField="SeverityId"
                                                                                        DataTextField="SeverityName" SelectedIndex='<%# GetSeverity(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"SeverityId"))) %>'
                                                                                        runat="server" meta:resourcekey="cboSeverityResource1">
                                                                                    </asp:DropDownList>
                                                                                </EditItemTemplate>
                                                                            </asp:TemplateColumn>
                                                                            <asp:ButtonColumn Text="&lt;img src=../images/edit.gif border=0&gt;" CommandName="cmdEdit"
                                                                                meta:resourcekey="ButtonColumnResource1">
                                                                                <ItemStyle Width="40px" />
                                                                            </asp:ButtonColumn>
                                                                            <asp:ButtonColumn Text="&lt;img src=../images/delete.gif border=0&gt;" CommandName="Delete"
                                                                                meta:resourcekey="ButtonColumnResource2">
                                                                                <ItemStyle Width="40px" />
                                                                            </asp:ButtonColumn>
                                                                            <asp:HyperLinkColumn DataNavigateUrlField="GeozoneId" DataNavigateUrlFormatString="javascript:var w =GeozoneMap('{0}')"
                                                                                Text="Map It" meta:resourcekey="HyperLinkColumnResource1">
                                                                                <ItemStyle Wrap="False" ForeColor="Black" Width="50px"></ItemStyle>
                                                                            </asp:HyperLinkColumn>
                                                                            <asp:HyperLinkColumn DataNavigateUrlField="GeozoneId" DataNavigateUrlFormatString="javascript:var w =GeozoneVehicles('{0}')"
                                                                                Text="Current assignment" meta:resourcekey="HyperLinkColumnResource2">
                                                                                <ItemStyle Wrap="False" ForeColor="Black" Width="130px"></ItemStyle>
                                                                            </asp:HyperLinkColumn>
                                                                        </Columns>
                                                                        <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"
                                                                            Mode="NumericPages"></PagerStyle>
                                                                    </asp:DataGrid></td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center" height="5"></td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Button ID="cmdGeoZoneAdd" runat="server" CausesValidation="False" CssClass="combutton"
                                                                        Text="Add GeoZone" OnClick="cmdGeoZoneAdd_Click" meta:resourcekey="cmdGeoZoneAddResource1"></asp:Button></td>
                                                            </tr>
                                                        </table>
                                                        <table id="tblGeoZoneAdd" style="width: 670px" cellspacing="0" cellpadding="3" border="0"
                                                            runat="server">
                                                            <tr>
                                                                <td align="center" height="15"></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="height: 155px" align="center" height="155">
                                                                    <table id="Table8" style="height: 93px;" cellspacing="0"
                                                                        cellpadding="3" border="0">
                                                                        <tr>
                                                                            <td class="formtext">
                                                                                <asp:Label ID="lblGeoZoneId" runat="server" Visible="False" meta:resourcekey="lblGeoZoneIdResource1"></asp:Label></td>
                                                                            <td class="formtext">
                                                                                <asp:Label ID="lblOldGeoZoneName" runat="server" Visible="False" meta:resourcekey="lblOldGeoZoneNameResource1"></asp:Label></td>
                                                                            <td class="formtext"></td>
                                                                            <td class="formtext"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="formtext" style="height: 22px">
                                                                                <asp:Label ID="lblGeozoneNameTitle" runat="server" CssClass="formtext" meta:resourcekey="lblGeozoneNameTitleResource1"
                                                                                    Text="GeoZone Name:"></asp:Label>
                                                                                <asp:RequiredFieldValidator ID="valLandmark" runat="server" ControlToValidate="txtGeoZoneName"
                                                                                    ErrorMessage="Please enter a GeoZone Name" meta:resourcekey="valLandmarkResource1"
                                                                                    Text="*"></asp:RequiredFieldValidator></td>
                                                                            <td class="formtext" style="height: 22px">
                                                                                <asp:TextBox ID="txtGeoZoneName" runat="server" CssClass="formtext" Width="173px"
                                                                                    meta:resourcekey="txtGeoZoneNameResource1"></asp:TextBox></td>
                                                                            <td class="formtext" style="height: 22px">
                                                                                <asp:Label ID="lblDirectionTitle" runat="server" CssClass="formtext" meta:resourcekey="lblDirectionTitleResource1"
                                                                                    Text="Direction:"></asp:Label></td>
                                                                            <td class="formtext" style="height: 22px">
                                                                                <asp:DropDownList ID="cboDirection" runat="server" CssClass="formtext" Width="175px"
                                                                                    DataTextField="DirectionName" DataValueField="DirectionId" meta:resourcekey="cboDirectionResource1">
                                                                                </asp:DropDownList></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="formtext" style="height: 20px">
                                                                                <asp:Label ID="lblGeozoneDescriptionTitle" runat="server" CssClass="formtext" meta:resourcekey="lblGeozoneDescriptionTitleResource1"
                                                                                    Text="GeoZone Description:"></asp:Label></td>
                                                                            <td class="formtext" style="height: 20px">
                                                                                <asp:TextBox ID="txtGeoZoneDesc" runat="server" CssClass="formtext" Width="173px"
                                                                                    meta:resourcekey="txtGeoZoneDescResource1"></asp:TextBox></td>
                                                                            <td class="formtext" style="height: 20px">
                                                                                <asp:Label ID="lblDefaultSeverityTitle" runat="server" CssClass="formtext" meta:resourcekey="lblDefaultSeverityTitleResource1"
                                                                                    Text="Default Severity:"></asp:Label>
                                                                            </td>
                                                                            <td class="formtext" style="height: 20px">
                                                                                <asp:DropDownList ID="cboGeoZoneSeverity" runat="server" CssClass="formtext" Width="175px"
                                                                                    DataTextField="SeverityName" DataValueField="SeverityId" DESIGNTIMEDRAGDROP="451"
                                                                                    AutoPostBack="True" meta:resourcekey="cboGeoZoneSeverityResource1">
                                                                                </asp:DropDownList></td>
                                                                        </tr>
                                                                        <tr id="trAssignment" runat="server" visible="false">
                                                                            <td style="height: 40px;">
                                                                                <asp:Label ID="Label1" runat="server" Text="Assign to fleet:" CssClass="formtext"></asp:Label>
                                                                            </td>
                                                                            <td colspan="3" style="height: 40px;">
                                                                                <asp:DropDownList ID="cboFleet" runat="server" CssClass="RegularText"
                                                                                    DataTextField="FleetName" DataValueField="FleetId" AutoPostBack="True" meta:resourcekey="cboFleetResource6">
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <table id="Table2" cellspacing="0" cellpadding="0" border="0">
                                                                        <tr>
                                                                            <td class="formtext" align="center" height="15">
                                                                                <table id="Table4" cellspacing="0" cellpadding="0" border="0">
                                                                                    <tr>
                                                                                        <td height="5">
                                                                                            <table id="Table5" cellspacing="0" cellpadding="0" border="0">
                                                                                                <tr>
                                                                                                    <td align="center">
                                                                                                        <asp:DropDownList ID="cboLandmarks" runat="server" CssClass="formtext" Width="175px"
                                                                                                            DataTextField="LandmarkName" DataValueField="LandmarkName" AutoPostBack="True"
                                                                                                            meta:resourcekey="cboLandmarksResource1">
                                                                                                        </asp:DropDownList>
                                                                                                        <table id="tblGeoZoneMap" style="width: 275px; height: 28px" cellspacing="0" cellpadding="0"
                                                                                                            width="275" align="center" border="0" runat="server">
                                                                                                            <tr>
                                                                                                                <td align="center" style="height: 28px">
                                                                                                                    <a class="link" onclick="ShowMap()" href="#"><u><font color="#003300" size="2">
                                                                                                                        <asp:Label ID="lblDrawGeozoneTitle" runat="server" meta:resourcekey="lblDrawGeozoneTitleResource1"
                                                                                                                            Text="Draw Geozone"></asp:Label></font></u></a> &nbsp;<asp:Label
                                                                                                                                ID="lblOr" runat="server"
                                                                                                                                CssClass="formtext" Text="or" meta:resourcekey="lblOrResource1"></asp:Label>&nbsp;
                                                                                                                    <asp:LinkButton ID="lnkByCoordinates" runat="server" CausesValidation="False"
                                                                                                                        Font-Size="X-Small" OnClick="lnkByCoordinates_Click"
                                                                                                                        meta:resourcekey="lnkByCoordinatesResource1">By Coordinates</asp:LinkButton></td>
                                                                                                            </tr>
                                                                                                        </table>
                                                                                                        <table id="tblGeozone">
                                                                                                            <tr>
                                                                                                                <td colspan="4" align="center">
                                                                                                                    <asp:RadioButtonList ID="optGeozonePublicPrivate" runat="server" CssClass="formtext"
                                                                                                                        BorderWidth="0px"
                                                                                                                        RepeatDirection="Horizontal">
                                                                                                                        <asp:ListItem Value="0" Text="Private"></asp:ListItem>
                                                                                                                        <asp:ListItem Value="1" Selected="True" Text="Public">
                                                                                                                        </asp:ListItem>

                                                                                                                    </asp:RadioButtonList>
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                        </table>
                                                                                                        <table id="tblGeoZoneCoordinates" runat="server" class="formtext">
                                                                                                            <tr>
                                                                                                                <td align="center" colspan="3">
                                                                                                                    <table width="100%">
                                                                                                                        <tr>
                                                                                                                            <td style="width: 100px">
                                                                                                                                <asp:RadioButtonList ID="optGeoZoneType" runat="server" CssClass="formtext"
                                                                                                                                    RepeatDirection="Horizontal" AutoPostBack="True"
                                                                                                                                    OnSelectedIndexChanged="optGeoZoneType_SelectedIndexChanged"
                                                                                                                                    meta:resourcekey="optGeoZoneTypeResource1">
                                                                                                                                    <asp:ListItem Selected="True" Value="1" meta:resourcekey="ListItemResource1">Rectangle</asp:ListItem>
                                                                                                                                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource2">Polygon</asp:ListItem>
                                                                                                                                </asp:RadioButtonList></td>
                                                                                                                            <td align="right">
                                                                                                                                <asp:Button ID="cmdClearAllPoints" runat="server" Text="Clear All Points"
                                                                                                                                    CssClass="combutton" CausesValidation="False" OnClick="cmdClearAllPoints_Click"
                                                                                                                                    meta:resourcekey="cmdClearAllPointsResource1" /></td>
                                                                                                                        </tr>
                                                                                                                    </table>
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                            <tr>
                                                                                                                <td colspan="3" align="center">
                                                                                                                    <asp:DataGrid ID="dgGeoZonesCoordinates"
                                                                                                                        runat="server" Width="100%" AutoGenerateColumns="False" CellPadding="4"
                                                                                                                        ForeColor="#333333" GridLines="None" BorderColor="#E0E0E0" BorderStyle="Solid"
                                                                                                                        BorderWidth="1px" meta:resourcekey="dgGeoZonesCoordinatesResource1">
                                                                                                                        <PagerStyle Font-Size="11px" BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                                                                                                        <AlternatingItemStyle CssClass="gridtext" BackColor="White" ForeColor="#284775" />
                                                                                                                        <ItemStyle CssClass="gridtext" BackColor="#F7F6F3" ForeColor="#333333" />
                                                                                                                        <Columns>
                                                                                                                            <asp:BoundColumn DataField="Latitude" HeaderText="Latitude"></asp:BoundColumn>
                                                                                                                            <asp:BoundColumn DataField="Longitude" HeaderText="Longitude"></asp:BoundColumn>
                                                                                                                        </Columns>
                                                                                                                        <HeaderStyle CssClass="DataHeaderStyle"></HeaderStyle>
                                                                                                                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                                                                                                        <EditItemStyle BackColor="#999999" />
                                                                                                                        <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                                                                                                    </asp:DataGrid></td>

                                                                                                            </tr>
                                                                                                            <tr>
                                                                                                                <td>
                                                                                                                    <asp:Label ID="lblLatitude" runat="server" Text="Latitude:"
                                                                                                                        meta:resourcekey="lblLatitudeResource1"></asp:Label></td>
                                                                                                                <td>
                                                                                                                    <asp:Label ID="lblLongitude" runat="server" Text="Longitude:"
                                                                                                                        meta:resourcekey="lblLongitudeResource1"></asp:Label></td>
                                                                                                                <td></td>
                                                                                                            </tr>
                                                                                                            <tr>
                                                                                                                <td style="height: 26px">
                                                                                                                    <asp:TextBox ID="txtLatitude" runat="server"
                                                                                                                        meta:resourcekey="txtLatitudeResource1"></asp:TextBox></td>
                                                                                                                <td style="height: 26px">
                                                                                                                    <asp:TextBox ID="txtLongitude" runat="server"
                                                                                                                        meta:resourcekey="txtLongitudeResource1"></asp:TextBox></td>
                                                                                                                <td style="height: 26px">
                                                                                                                    <asp:Button ID="cmdAddGeoZonePoint" runat="server"
                                                                                                                        Text="Add" CssClass="combutton" CausesValidation="False"
                                                                                                                        OnClick="cmdAddGeoZonePoint_Click"
                                                                                                                        meta:resourcekey="cmdAddGeoZonePointResource1" /></td>
                                                                                                            </tr>
                                                                                                            <tr>
                                                                                                                <td align="left" style="height: 26px" colspan="3">
                                                                                                                    <asp:Label ID="lblMessageAddGeoZones" runat="server"
                                                                                                                        Text="Note: * Rectangle GeoZone: 2 corner points ;  * Polygon GeoZone : maximum 10 points."
                                                                                                                        meta:resourcekey="lblMessageAddGeoZonesResource1"></asp:Label></td>
                                                                                                            </tr>

                                                                                                        </table>
                                                                                                        <table id="Table3" style="border-right: 2px ridge; border-top: 2px ridge; border-left: 2px ridge; border-bottom: 2px ridge; padding-left: 10px; padding-right: 10px; padding-top: 5px; padding-bottom: 5px;"
                                                                                                            cellspacing="0" cellpadding="2" border="0">
                                                                                                            <tr>
                                                                                                                <td class="formtext" height="10">
                                                                                                                    <asp:Label ID="lblEmailTitle" runat="server" CssClass="formtext" meta:resourcekey="lblEmailTitleResource1"
                                                                                                                        Text="Email:"></asp:Label>
                                                                                                                    <asp:RegularExpressionValidator ID="valEmail" runat="server" ControlToValidate="txtEmail"
                                                                                                                        ErrorMessage="Please enter a correct email address" ValidationExpression="^(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*\s*[,;]?\s*\b)*$"
                                                                                                                        meta:resourcekey="valEmailResource1" Text="*"></asp:RegularExpressionValidator>
                                                                                                                </td>
                                                                                                                <td height="10">
                                                                                                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="formtext" Width="173px" meta:resourcekey="txtEmailResource1"></asp:TextBox>
                                                                                                                </td>
                                                                                                                <td rowspan="6" style="width: 10px;"></td>
                                                                                                                <td class="formtext" height="10">
                                                                                                                    <asp:Label ID="lblPhone" runat="server" CssClass="formtext"
                                                                                                                        Text="Phone:"></asp:Label>
                                                                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                                                                                                                        ControlToValidate="txtPhone" CssClass="formtext"
                                                                                                                        ErrorMessage="Invalid Phone Number:"
                                                                                                                        ValidationExpression="^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$">*</asp:RegularExpressionValidator>
                                                                                                                </td>
                                                                                                                <td height="10">
                                                                                                                    <asp:TextBox ID="txtPhone" runat="server" CssClass="formtext" Width="173px" Enabled="False"></asp:TextBox>
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                            <tr>
                                                                                                                <td class="formtext" height="10"></td>
                                                                                                                <td height="10"></td>
                                                                                                                <td class="formtext" height="10"></td>
                                                                                                                <td height="10"></td>
                                                                                                            </tr>
                                                                                                            <tr>
                                                                                                                <td class="formtext">
                                                                                                                    <asp:Label ID="lblTimeZoneTitle" runat="server" CssClass="formtext" meta:resourcekey="lblTimeZoneTitleResource1"
                                                                                                                        Text="Time Zone:"></asp:Label></td>
                                                                                                                <td>
                                                                                                                    <asp:DropDownList ID="cboTimeZone" runat="server" CssClass="RegularText" Width="168px"
                                                                                                                        DataTextField="TimeZoneName" DataValueField="TimeZoneId" meta:resourcekey="cboTimeZoneResource1">
                                                                                                                    </asp:DropDownList></td>
                                                                                                                <td class="formtext">&nbsp;</td>
                                                                                                                <td>&nbsp;</td>
                                                                                                            </tr>
                                                                                                            <tr>
                                                                                                                <td colspan="4" style="height: 10px;">
                                                                                                                    <asp:Label
                                                                                                                        ID="lblMultipleEmails" class="formtext" runat="server" meta:resourcekey="lblMultipleEmails" Text="*Multiple email addresses Must be Separated by semicolon or comma"></asp:Label>
                                                                                                                </td>

                                                                                                            </tr>
                                                                                                            <tr>
                                                                                                                <td colspan="4" style="height: 10px;"></td>
                                                                                                            </tr>
                                                                                                            <tr>
                                                                                                                <td colspan="2" class="formtext" style="vertical-align: top;">
                                                                                                                    <asp:Label ID="lblEmailNotificationTitle" runat="server" CssClass="formtext" meta:resourcekey="lblEmailNotificationTitleResource1"
                                                                                                                        Text="Email notification in case of:"></asp:Label><br />
                                                                                                                    <table class="formtext" id="Table9" cellspacing="0" cellpadding="0" border="0">
                                                                                                                        <tr>
                                                                                                                            <td>
                                                                                                                                <asp:CheckBox ID="chkCritical" runat="server" Text="Critical Alarm" meta:resourcekey="chkCriticalResource1"></asp:CheckBox></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td>
                                                                                                                                <asp:CheckBox ID="chkWarning" runat="server" Text="Warning Alarm" meta:resourcekey="chkWarningResource1"></asp:CheckBox></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td>
                                                                                                                                <asp:CheckBox ID="chkNotify" runat="server" Text="Notify Alarm" meta:resourcekey="chkNotifyResource1"></asp:CheckBox></td>
                                                                                                                        </tr>
                                                                                                                    </table>
                                                                                                                </td>
                                                                                                                <td colspan="2" style="vertical-align: top;">
                                                                                                                    <asp:CheckBox ID="chkDayLight" runat="server" CssClass="formtext" Text="Automatically adjust for daylight savings time"
                                                                                                                        meta:resourcekey="chkDayLightResource1"></asp:CheckBox></td>
                                                                                                            </tr>
                                                                                                        </table>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>

                                                                    <asp:Label ID="lblAddMessage" runat="server" CssClass="errortext" Width="270px" Visible="False"
                                                                        Height="8px" meta:resourcekey="lblAddMessageResource1"></asp:Label><asp:ValidationSummary
                                                                            ID="ValidationSummary1" runat="server" CssClass="errortext" Width="321px" Height="17px"
                                                                            meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 100%; height: 21px" align="center" height="21">
                                                                    <asp:TextBox ID="txtMapMessage" runat="server" BackColor="Ivory" BorderWidth="0px"
                                                                        Width="226px" CssClass="errortext" meta:resourcekey="txtMapMessageResource1"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 100%; height: 19px" align="center">
                                                                    <asp:Button ID="cmdSaveGeoZone" runat="server" CssClass="combutton" Text="Save" OnClick="cmdSaveGeoZone_Click"
                                                                        meta:resourcekey="cmdSaveGeoZoneResource1"></asp:Button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                    <asp:Button ID="cmdCancelLandmark" runat="server" CausesValidation="False" CssClass="combutton"
                                                                        Text="Cancel" OnClick="cmdCancelLandmark_Click" meta:resourcekey="cmdCancelLandmarkResource1"></asp:Button></td>
                                                            </tr>
                                                        </table>
                                                        <asp:Label ID="lblMessage" name="lblMessage" runat="server" CssClass="errortext"
                                                            Height="8px" meta:resourcekey="lblMessageResource1"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </form>


    <script language="javascript">
		<!--
    function ShowMap() {
        var mypage = '<%=(GeozoneMap==""?"../MapNew/frmDrawLandmarkGeozone.aspx?FormName=Geozone&ShowControl=True":GeozoneMap)%>';
				    var myname = '';
				    var w = 830;
				    var h = 620;
				    var winl = (screen.width - w) / 2;
				    var wint = (screen.height - h) / 2;
				    if (typeof (lblMessage) != "undefined")
				        lblMessage.innerText = "";
				    winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,'
				    win = window.open(mypage, myname, winprops)
				    if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
				}

				function GeozoneMap(geozoneId) {
				    var mypage = '<%=ViewGeozoneMap%>?geozoneId=' + geozoneId
				    var myname = 'GeozoneMap';
				    var w = 830;
				    var h = 620;
				    var winl = (screen.width - w) / 2;
				    var wint = (screen.height - h) / 2;
				    winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,resizable=no'
				    win = window.open(mypage, myname, winprops)
				    if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
				}


				function GeozoneVehicles(geozoneId) {
				    var mypage = 'frmViewVehicleGeozones.aspx?geozoneId=' + geozoneId
				    var myname = 'GeozoneMap';
				    var w = 600;
				    var h = 620;
				    var winl = (screen.width - w) / 2;
				    var wint = (screen.height - h) / 2;
				    winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=no'
				    win = window.open(mypage, myname, winprops)
				    if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
				}

				//-->
    </script>
</body>
</html>
