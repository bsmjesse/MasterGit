<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmHOSFuel.aspx.cs" Inherits="HOS_frmHOSFuel"
    Theme="TelerikControl" meta:resourcekey="PageResource1" %>
<%@ Register Src="../UserControl/FleetOrganizationHierarchy.ascx" TagName="FleetVehicle" TagPrefix="uc1" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="HOSViewTabs.ascx" TagName="HosTabs" TagPrefix="uc2" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/Telerik_AddIn.js"></script>
    <link href="../Configuration/Configuration.css" type="text/css" rel="stylesheet" />
</head>
<body topmargin="5px" leftmargin="3px">
    <form id="form1" runat="server">
    <telerik:radscriptmanager runat="server" id="RadScriptManager1" asyncpostbacktimeout="300">
        <scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </scripts>
    </telerik:radscriptmanager>
    <telerik:radajaxloadingpanel id="LoadingPanel1" runat="server" skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:radajaxloadingpanel>
    <telerik:radajaxmanager runat="server" id="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"
        enableajax="true">
        <ajaxsettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="dgFuel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls> 
            </telerik:AjaxSetting>           
        </ajaxsettings>
    </telerik:radajaxmanager>
    <div style="text-align: left">
        <table id="Table1" border="0" cellpadding="0" cellspacing="0">
            <tr align="left">
                <td>
                    <asp:panel id="pnl" runat="server">
                        <table id="Table2" align="left" border="0" cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <uc2:HosTabs ID="HosTabs1" runat="server" SelectedControl="cmdFuel" />
                                </td>
                            </tr>
                            <tr valign="top">
                                <td valign="top">
                                    <table id="Table3" border="0" cellpadding="3" cellspacing="3" style="border-color: gray;
                                        border-style: outset; border-width: 2px;" height="800px">
                                        <tr>
                                            <td align="center">
                                                <table style="border-width: 1px; border-color: Gray; border-style: solid; width: 550px;"
                                                    cellspacing="0">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:label id="lblFromTitle" runat="server" cssclass="formtext" meta:resourcekey="lblFromTitleResource1"
                                                                text="From:"></asp:label>
                                                        </td>
                                                        <td align="left">
                                                            <telerik:raddatepicker id="txtFrom" runat="server" width="100px" height="17px" calendar-showrowheaders="false"
                                                                calendar-width="170px" meta:resourcekey="txtFromResource2" maxdate="9998-12-31"
                                                                mindate="1753-01-01" OnLoad="txtFrom_Load">
                                                                <calendar userowheadersasselectors="False" usecolumnheadersasselectors="False" showrowheaders="False"
                                                                    viewselectortext="x" width="170px"></calendar>
                                                                <dateinput height="17px" labelcssclass=""></dateinput>
                                                                <datepopupbutton imageurl="" hoverimageurl="" cssclass=""></datepopupbutton>
                                                            </telerik:raddatepicker>
                                                        </td>
                                                        <td align="left">
                                                            <asp:label id="lblToTitle" runat="server" cssclass="formtext" meta:resourcekey="lblToTitleResource1"
                                                                text="To:"></asp:label>
                                                        </td>
                                                        <td align="left">
                                                            <telerik:raddatepicker id="txtTo" runat="server" width="100px" height="17px" calendar-showrowheaders="false"
                                                                calendar-width="170px" meta:resourcekey="txtFromResource2" maxdate="9998-12-31"
                                                                mindate="1753-01-01">
                                                                <calendar userowheadersasselectors="False" usecolumnheadersasselectors="False" showrowheaders="False"
                                                                    viewselectortext="x" width="170px"></calendar>
                                                                <dateinput height="17px" labelcssclass=""></dateinput>
                                                                <datepopupbutton imageurl="" hoverimageurl="" cssclass=""></datepopupbutton>
                                                            </telerik:raddatepicker>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td id="FleetColumn" runat="server" class="tableheading" style="width: 52px; height: 14px" align="left">
                                                            <asp:label id="lblFleet" runat="server" cssclass="tableheading" width="33px" meta:resourcekey="lblFleetResource1"
                                                                text="Fleet:"></asp:label>
                                                        </td>
                                                        <td style="width: 312px; height: 14px" align="left">
                                                            <asp:dropdownlist id="cboFleet" runat="server" autopostback="True" cssclass="RegularText"
                                                                width="258px" datatextfield="FleetName" datavaluefield="FleetId" onselectedindexchanged="cboFleet_SelectedIndexChanged"
                                                                meta:resourcekey="cboFleetResource1">
                                                            </asp:dropdownlist>
                                                            <uc1:FleetVehicle ID="FleetVehicle1" runat="server" />
                                                        </td>
                                                        <td class="formtext" style="width: 32px">
                                                            <asp:label id="lblVehicleName" runat="server" cssclass="tableheading" width="53px"
                                                                visible="true" meta:resourcekey="lblVehicleNameResource1" text="Vehicle:"></asp:label>
                                                        </td>
                                                        <td style="width: 300px" align="left">
                                                            <asp:dropdownlist id="cboVehicle" runat="server" cssclass="RegularText" width="258px"
                                                                datatextfield="Description" datavaluefield="BoxId" visible="true" meta:resourcekey="cboVehicleResource1">
                                                            </asp:dropdownlist>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="4" align="center">
                                                            <asp:button id="cmdViewAllData" runat="server" text="View" onclick="cmdViewAllData_Click"
                                                                height="29px" style="margin-top: 2px" width="167px" meta:resourcekey="cmdViewAllDataResource1" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="4" align="center">
                                                            <asp:label id="lblMessage" runat="server" cssclass="errortext" meta:resourcekey="lblMessageResource1">
                                                            </asp:label>
                                                        </td>
                                                  </tr>
                                                  </table>
                                            </td>
                                        </tr>
                                        <tr valign="top">
                                <td align="center">
                                    <sentinel:grid id="dgFuel" runat="server" 
                                      width = "800px"
                                                    AutoGenerateColumns="False" 
                                                          EnableTheming="True" 
                                                          AllowFilteringByColumn="true"
                                                          AllowSorting="True" 
                                                          FilterItemStyle-HorizontalAlign="Left" Skin="Hay"
                                     exporttext="Export" allowpaging="True" pagesize="20"
                                         isautoresize="false" 
                                         alltext="All" clearallfilterstext="Clear All Filters" 
                                        gridlines="None" isshowexporticon="True" meta:resourcekey="dgFuelResource1" onneeddatasource="dgFuel_NeedDataSource"
                                        height="600px" isshowfiltericon="True">
                                        <groupingsettings casesensitive="False" />
                                        <groupingsettings casesensitive="False"></groupingsettings>
                                        
                                        <mastertableview commanditemdisplay="Top" >
                    <commanditemsettings exporttopdftext="Export to Pdf" />
<CommandItemSettings ExportToPdfText="Export to Pdf"></CommandItemSettings>

                    <rowindicatorcolumn>
                        <HeaderStyle Width="20px" />
                    </rowindicatorcolumn>
                    <expandcollapsecolumn>
                        <HeaderStyle Width="20px" />
                    </expandcollapsecolumn>
                    <Columns>
                        <telerik:GridBoundColumn DataField="BoxId" HeaderText="Box ID" 
                            meta:resourcekey="BoxIdResource1" UniqueName="BoxId">
                            <HeaderStyle Width="60px" />
                            <ItemStyle Width="60px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="EquipID" HeaderText="Equipment No" 
                            meta:resourcekey="EquipIDResource1" UniqueName="EquipID">
                            <HeaderStyle Width="70px" />
                            <ItemStyle Width="70px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="Date Time"  DataField="FuelTime"
                            meta:resourcekey="FuelTimeResource2" SortExpression="FuelTime" 
                            UniqueName="FuelTime">
                            <HeaderStyle Width="100px" />
                            <ItemStyle Width="100px" />
                        </telerik:GridBoundColumn>

                        <telerik:GridBoundColumn HeaderText="Odometer"  DataField="Odometer"
                            meta:resourcekey="OdometerResource2" SortExpression="Odometer" 
                            UniqueName="Odometer">
                            <HeaderStyle Width="70px" />
                            <ItemStyle Width="70px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="DriverName" HeaderText="Driver" 
                            meta:resourcekey="DriverNameResource2" UniqueName="DriverName">
                            <HeaderStyle Width="80px" />
                            <ItemStyle Width="80px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Address" HeaderText="Address" 
                            meta:resourcekey="AddressResource3" UniqueName="Address">
                            <HeaderStyle Width="80px" />
                            <ItemStyle Width="80px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Provider" HeaderText="Provider" 
                            meta:resourcekey="ProviderResource3" UniqueName="Provider">
                            <HeaderStyle Width="90px" />
                            <ItemStyle Width="90px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Quantity" HeaderText="Quantity" 
                            meta:resourcekey="QuantityResource3" UniqueName="Quantity">
                            <HeaderStyle Width="70px" />
                            <ItemStyle Width="70px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Unit" HeaderText="Unit" 
                            meta:resourcekey="UnitResource3" UniqueName="Unit">
                            <HeaderStyle Width="50px" />
                            <ItemStyle Width="50px" />
                        </telerik:GridBoundColumn>

                    </Columns>
                    <commanditemtemplate>
                        <table ID="tblCustomerCommand" runat="server" width="100%">
                            <tr id="Tr1" runat="server">
                                <td id="Td1" runat="server" align="left">
                                </td>
                            </tr>
                        </table>
                    </commanditemtemplate>
                </mastertableview>
                                        <filteritemstyle horizontalalign="Left" />
                                        <filteritemstyle horizontalalign="Left"></filteritemstyle>
                                        <filtermenu cssclass="FiltMenuCss" enabletheming="True">
                </filtermenu>
                                    </sentinel:grid>
                                </td>
                            </tr>
                                    </table>
                               </td>
                          </tr>
                        </table>
                    </asp:panel>
                </td>
            </tr>
        </table>
    </div>
    <telerik:radcodeblock id="RadCodeBlock1" runat="server">
        <script type="text/javascript">
        </script>
    </telerik:radcodeblock>
    </form>
</body>
</html>
