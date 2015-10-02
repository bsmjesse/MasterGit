<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.master" AutoEventWireup="true" CodeFile="frmMaintenanceDTC.aspx.cs" Inherits="SentinelFM.Maintenance_frmMaintenanceDTC" Theme="TelerikControl" meta:resourcekey="PageResource1" %>
<%@ MasterType VirtualPath="~/MasterPage/MasterPage.master" %>
<%@ Register Src="../UserControl/FleetVehicleOrganizationHierarchy.ascx" TagName="FleetVehicle" TagPrefix="uc1" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<%@ Register TagName="FleetVehicles" TagPrefix="fvs" Src="~/UserControl/FleetVehiclesControl.ascx" %>
<%@ Register TagName="FleetVehicles1" TagPrefix="fvs1" Src="~/UserControl/FleetVehiclesControl.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"
        >
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="dgDTCCode">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="cboFleet">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="cboVehicle">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="cmdViewDTCCodes">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <asp:Panel ID="pnl" runat="server" meta:resourcekey="pnlResource1" >
                                                    <fieldset style="padding: 5px 5px 5px 5px;background-color:white"">
                                                        <table class="formtext" style="width: 520px; background-color:white">
                                                            <tr>
                                                                <td align="left" class="style1">
                                                                    <asp:Label ID="lblFromTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFromTitleResource1"
                                                                        Text="From:" />
                                                                </td>
                                                                <td align="left" class="style2">
                                                                    <table border="0px" cellpadding="1" cellspacing="1">
                                                                        <tr>
                                                                            <td style="height: 21px; width: 85px">
                                                                                <telerik:RadDatePicker ID="txtFrom" runat="server" Width="120px" Height="17px" 
                                                                                    meta:resourcekey="txtFromResource2" Skin="Hay" Culture="en-US" >
                                                                                    <Calendar UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False" 
                                                                                        ViewSelectorText="x">
                                                                                    </Calendar>
                                                                                    <DateInput DateFormat='<%# sn.User.DateFormat %>' DisplayDateFormat='<%# sn.User.DateFormat %>' 
                                                                                        LabelCssClass="" Width="">
                                                                                    </DateInput>
                                                                                    <DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
                                                                                </telerik:RadDatePicker>
                                                                            </td>
                                                                            <td>
                                                                                <asp:DropDownList ID="cboHoursFrom" runat="server" CssClass="RegularText" Width="76px"
                                                                                    meta:resourcekey="cboHoursFromResource1" Height="21px" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                                <td align="left" class="style3">
                                                                    <asp:Label ID="lblToTitle" runat="server" CssClass="formtext" meta:resourcekey="lblToTitleResource1"
                                                                        Text="To:" />
                                                                </td>
                                                                <td align="left">
                                                                    <table border="0px" cellpadding="1" cellspacing="1">
                                                                        <tr>
                                                                            <td style="height: 21px; width: 85px">
                                                                                <telerik:RadDatePicker ID="txtTo" runat="server" Skin="Hay" Width="120px" Height="17px" 
                                                                                    meta:resourcekey="txtToResource2" Culture="en-US">
                                                                                    <Calendar UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False" 
                                                                                        ViewSelectorText="x">
                                                                                    </Calendar>
                                                                                    <DateInput DateFormat='<%# sn.User.DateFormat %>' DisplayDateFormat='<%# sn.User.DateFormat %>'
                                                                                        LabelCssClass="" Width="">
                                                                                    </DateInput>
                                                                                    <DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
                                                                                </telerik:RadDatePicker>
                                                                            </td>
                                                                            <td style="height: 21px">
                                                                                <asp:DropDownList ID="cboHoursTo" runat="server" CssClass="RegularText" Width="75px"
                                                                                    meta:resourcekey="cboHoursToResource1" Height="21px" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" class="style1" colspan="4">
                                                                    <asp:Panel ID="pnlFleetVehicle"  runat="server" >
                                                                        <uc1:FleetVehicle ID="FleetVehicle1" runat="server" />
                                                                        </asp:Panel>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" valign="top" colspan="5">
                                                                    &nbsp;
                                                                    <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Visible="False" Height="8px"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right" colspan="4" valign="top">
                                                                    <asp:Button ID="cmdViewDTCCodes" runat="server" CssClass="combutton" Text="View Codes"
                                                                        OnClick="cmdViewDTCCodes_Click"  Width="180px" 
                                                                        meta:resourcekey="cmdViewDTCCodesResource1" />
                                                                    &nbsp;&nbsp;&nbsp;
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>
                                                    <Sentinel:Grid ID="dgDTCCode" runat="server" UseDefaultStyle="True" 
                                                        Width="980px"  IsAutoResize="True" AllowSorting="True" AutoGenerateColumns="False"
                                                        AllowFilteringByColumn="True" AllowPaging="True" PageSize="20" 
                                                        OnNeedDataSource="dgDTCCode_NeedDataSource" allText="All" 
                                                        ClearAllFiltersText="Clear All Filters" GridLines="None" 
                                                        IsShowExportIcon="True" IsShowFilterIcon="True" meta:resourcekey="dgDTCCodeResource1"
OnItemDataBound="dgDTCCode_ItemDataBound"
                                                       >
                                                        <GroupingSettings CaseSensitive="False" />
                                                        <ClientSettings>
                                                            <ClientEvents OnGridCreated="GridCreated" />
                                                            <Scrolling AllowScroll="True" SaveScrollPosition="False" ScrollHeight="350px" 
                                                                UseStaticHeaders="True" />
                                                        </ClientSettings>
                                                        <MasterTableView CommandItemDisplay="Top" Width="960px">
                                                            <CommandItemSettings ExportToPdfText="Export to Pdf" />
                                                            <RowIndicatorColumn>
                                                                <HeaderStyle Width="20px" />
                                                            </RowIndicatorColumn>
                                                            <ExpandCollapseColumn>
                                                                <HeaderStyle Width="20px" />
                                                            </ExpandCollapseColumn>
                                                            <Columns>
                        <telerik:GridBoundColumn HeaderText='<%$ Resources:dgDTCCode_BoxId %>'  DataField="BoxId"
                            UniqueName="BoxId" >
                            <ItemStyle Width="60px"  />
                            <HeaderStyle Width="60px" />
                        </telerik:GridBoundColumn>

                                                                <telerik:GridBoundColumn DataField="MaxDateTime" DataType="System.DateTime" 
                                                                    HeaderText="Last Updated" meta:resourcekey="GridBoundColumnResource1" 
                                                                    UniqueName="MaxDateTime">
                                                                    <HeaderStyle Width="130px" />
                                                                    <ItemStyle Width="130px" />
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="Description" HeaderText="Vehicle" 
                                                                    meta:resourcekey="GridBoundColumnResource2" UniqueName="Description">
                                                                    <HeaderStyle Width="70px" />
                                                                    <ItemStyle Width="70px" />
                                                                </telerik:GridBoundColumn>

                                                                <telerik:GridBoundColumn DataField="counter" HeaderText="Count" 
                                                                    meta:resourcekey="GridBoundColumnResource3" UniqueName="counter">
                                                                    <HeaderStyle Width="50px" />
                                                                    <ItemStyle Width="50px" />
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="Translation" 
                                                                    HeaderText="Code (DTC / MID PID FMI)" 
                                                                    meta:resourcekey="GridBoundColumnResource4" UniqueName="Translation">
                                                                    <HeaderStyle Width="200px" />
                                                                    <ItemStyle Width="200px" />
                                                                </telerik:GridBoundColumn>
                                                            </Columns>
                                                            <CommandItemTemplate>
                                                                <table ID="tblCustomerCommand" runat="server" width="100%">
                                                                    <tr runat="server">
                                                                        <td runat="server">
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </CommandItemTemplate>
                                                        </MasterTableView>
                                                        <FilterItemStyle HorizontalAlign="Left" />
                                                        <FilterMenu CssClass="FiltMenuCss" EnableTheming="True">
                                                        </FilterMenu>
                                                    </Sentinel:Grid>
        <asp:XmlDataSource ID="XmlOperationType" runat="server" />
    </asp:Panel>
</asp:Content>

