<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmVehicleBoxFirmwareStatus.aspx.cs" Inherits="SentinelFM.frmVehicleBoxFirmwareStatus" %>
<%@ Register Assembly="BusyBoxDotNet" Namespace="BusyBoxDotNet" TagPrefix="busyboxdotnet" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN">
<html>
<head runat="server">
    <title>Vehicle Box Firmware Status</title>
    <style id="local" type="text/css">
        
        .cmdDelimiter
        {   
            width: 10px;
            text-align: center;
            color: #BBBBBB;
        }
        .cmdEnabled
        {
            border-width: 0px;
            border-style: none;
            background-color: transparent;
            color: Black;
            font-family: Calibri, Arial, Tahoma, Verdana;
            font-size: 12px;  
            text-align: center;
            text-decoration: underline;
            }
        .cmdDisabled
        {
            border-width: 0px;
            border-style: none;
            background-color: transparent; 
            color: #BEBEBE;
            font-family: Calibri, Arial, Tahoma, Verdana;
            font-size: 12px;  
            text-align: center;
            text-decoration: underline;
            }
    </style>
</head>
<body>
    <form id="VehicleBoxFirmwareStatusForm" runat="server">
    <div>
     <table id="tblCommands" style="z-index: 101; left: 8px; position: absolute; top: 4px" cellspacing="0" cellpadding="0" border="0">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdVehicles" />
                </td>
            </tr>
            <tr><td></td></tr>
            <tr>
                <td>
                    <table id="tblBody" class="table" cellspacing="0" cellpadding="0" width="990" align="center" border="0">
                        <tr id="trSubmenu" style="vertical-align:top; height:28px; text-align:center;">
                            <td style="vertical-align:middle; width: 190px;">
                                <table id="tbSubMenu" style="z-index: 101; position: relative; top: 0px; height: 22px" cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <td>
                                            <asp:Button ID="cmdVehicleInfo" runat="server" Width="112px" CausesValidation="False"
                                                CssClass="confbutton" Text="Vehicle Info" CommandName="4" OnClick="cmdVehicleInfo_Click" 
                                                meta:resourcekey="cmdVehicleInfoResource1">
                                            </asp:Button>
                                        </td>
                                        <td>
                                            <asp:Button ID="cmdBoxFirmware" runat="server" Width="168px" CausesValidation="False"
                                                CssClass="selectedbutton" Text="Box Firmware Status" CommandName="8" 
                                                meta:resourcekey="cmdBoxFirmware">
                                            </asp:Button>
                                        </td>
                                    </tr>
                                </table>            
                            </td>
                        </tr>
                        <tr id="trSubBoard" style="height:560px;">
                            <td>
                                <table id="tbSubBoard" style="width: 960px; height: 540px; vertical-align:top; text-align:center;" class="tableDoubleBorder" border="0">
                                    <tr id="trParameters" style="height: 28px; vertical-align:top;">
                                        <td style="vertical-align: middle; text-align:left;">
                                             <table id="tbParameters">
                                                <tr>
                                                    <td style="width: 60px;">
                                                        <asp:label id="lblOrganization" runat="server" CssClass="formtext" meta:resourcekey="lblFleetResource1" Text="Organization: "></asp:label>
                                                    </td>
                                                    <td style="width: 220px;">
                                                        <asp:DropDownList ID="dlOrgamization" runat="server" AutoPostBack="true" CssClass="RegularText"
                                                            DataTextField="OrganizationName" DataValueField="OrganizationID" Width="200px"
                                                                OnSelectedIndexChanged="OrganizationSelectedIndex_Changed">
                                                        </asp:DropDownList>
                                                    </td> 
                                                    <td style="width: 40px;">
                                                        <asp:label id="lblFleet" runat="server" CssClass="formtext" meta:resourcekey="lblFleetResource1" Text="Fleet: "></asp:label>
                                                    </td>
                                                    <td style="width: 200px;">
                                                        <asp:DropDownList ID="dlFleet" runat="server" AutoPostBack="true" CssClass="RegularText"
                                                            DataTextField="FleetName" DataValueField="FleetID" Width="180px"
                                                            OnSelectedIndexChanged="FleetSelectedIndex_Changed">
                                                        </asp:DropDownList>
                                                     </td>
                                                      <td style="text-align:right; width: 500px;">
                                                        <span class="cmdDelimiter"> | </span>
                                                        <span>
                                                        <asp:Button ID="cmdExport" runat="server"  CssClass="cmdEnabled" OnClick="cmdExport_Clicked" CommandName="Export" Text="Export" />
                                                         <busyboxdotnet:BusyBox ID="BusyReport" runat="server" AnchorControl="" meta:resourcekey="BusyReportResource1"
                                                    ShowBusyBox="Custom" SlideEasing="BackBoth" Text="Preparing the Report" CompressScripts="False"
                                                    GZipCompression="False"></busyboxdotnet:BusyBox>
                                                        </span>
                                                        <span class="cmdDelimiter"> | </span>
                                                        <span>
                                                            <asp:HyperLink ID="cmdDownload" runat="server" CssClass="cmdDisabled" Text="Download" Enabled="false" Font-Underline="true" Font-Size="11px" ToolTip=""></asp:HyperLink> 
                                                        </span>
                                                        <span class="cmdDelimiter"> | </span>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="trTaskTotal" style="vertical-align:top;">
                                        <td style="text-align:left;">
                                            <asp:DataGrid ID="dgTotal" runat="server" AutoGenerateColumns="False" meta:resourcekey="dgVehiclesResource1"
                                                CellPadding="3" CellSpacing="1" GridLines="None" BackColor="White" BorderWidth="2px" BorderStyle="Ridge" BorderColor="White"
                                                Width="400px" OnUpdateCommand="TaskTotalSelectedIndex_Changed" >
                                                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                                                <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                                                <HeaderStyle CssClass="DataHeaderStyle" ></HeaderStyle>
                                                <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                <Columns>
                                                        <asp:BoundColumn DataField="No." HeaderText="No.">
                                                            <ItemStyle Width="60px" />
                                                        </asp:BoundColumn>
                                                        <asp:BoundColumn DataField="Items" HeaderText="Description">
                                                            <ItemStyle Width="260px" />
                                                        </asp:BoundColumn>
                                                        <asp:BoundColumn DataField="Number" HeaderText="Boxes">
                                                            <ItemStyle Width="80px" />
                                                        </asp:BoundColumn>
                                                        <asp:ButtonColumn Text="Select" CommandName="Update" meta:resourcekey="ButtonColumnResource1">
                                                            <ItemStyle Width="60px" />
                                                        </asp:ButtonColumn>
                                                    </Columns>
                                            </asp:DataGrid>
                                        </td>
                                    </tr>
                                    <tr id="trTaskDetail" style="vertical-align:top;">
                                        <td style="text-align:center;">
                                            <div style="height: 400px; overflow: scroll; vertical-align: top;">
                                            <asp:DataGrid ID="dgDetails" runat="server"
                                                CellSpacing="1" GridLines="None" BackColor="White"
                                                BorderWidth="2px" BorderStyle="Ridge" BorderColor="White" CellPadding="3" AutoGenerateColumns="False"
                                                DataKeyField="boxid" AllowPaging="false" Width="940px" meta:resourcekey="dgVehiclesResource1">                                                                        <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                    <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                                                    <ItemStyle CssClass="gridtext" BackColor="White" Height="26px" ></ItemStyle>
                                                    <HeaderStyle CssClass="DataHeaderStyle" Height="32px" VerticalAlign="Middle"></HeaderStyle>
                                                    <Columns>
                                                        <asp:BoundColumn DataField="OrganizationID" HeaderText='OID' Visible="false" />
                                                        <asp:BoundColumn DataField="OrganizationName" HeaderText='Organization' />
                                                        <asp:BoundColumn DataField="boxid" HeaderText='Box ID'>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle Width="50" HorizontalAlign="Center" />
                                                        </asp:BoundColumn>
                                                        <asp:BoundColumn DataField="VehicleID" HeaderText='VID' Visible="false" />
                                                        <asp:BoundColumn DataField="Vehicle" HeaderText='Vehicle' />
                                                        <asp:BoundColumn DataField="MainBoard" HeaderText='Main Board'>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle Width="80" HorizontalAlign="Center" />
                                                        </asp:BoundColumn>
                                                        <asp:BoundColumn DataField="UpperBoard" HeaderText='Upper Board'>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle Width="80" HorizontalAlign="Center" />
                                                        </asp:BoundColumn>
                                                        <asp:BoundColumn DataField="FirmwareVersion" HeaderText='Firmware Version'>
                                                            <ItemStyle Width="200" />
                                                        </asp:BoundColumn>
                                                        <asp:BoundColumn DataField="AcknowledgedAt" HeaderText='Last Update Date' DataFormatString="{0:d}">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle Width="70px" HorizontalAlign="Center" /> 
                                                        </asp:BoundColumn>
                                                        <asp:BoundColumn DataField="LastCommunicatedDateTime" HeaderText='Last Access Date' DataFormatString="{0:d}">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle Width="70px" HorizontalAlign="Center" />
                                                        </asp:BoundColumn>
                                                    </Columns>
                                                    <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6" Mode="NumericPages"></PagerStyle>
                                            </asp:DataGrid>
                                            </div>
                                        </td>
                                    </tr>
                                </table>                                        
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Message" runat="server">&nbsp;</asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
    </table>
    </div>
    </form>
</body>
</html>
