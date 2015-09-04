<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.master" AutoEventWireup="true" CodeFile="Old_His.aspx.cs" Inherits="SentinelFM.Maintenance_Old_His" Theme="TelerikControl" meta:resourcekey="PageResource1" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<%@ Register Src="../UserControl/FleetVehicle.ascx" TagName="FleetVehicle" TagPrefix="uc1" %>
<%@ MasterType VirtualPath="~/MasterPage/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"
        >
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="dgHistory">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
                      <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="pnlFleetVehicle">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
                                                 <asp:Panel ID="pnl" runat="server"         meta:resourcekey="pnlResource1">
                                                   <table style="background-color:white">
        <tr>
            <td>
            <asp:Panel ID="pnlFleetVehicle"  runat="server" >
                                                    <uc1:FleetVehicle ID="FleetVehicle1" runat="server" />   
                                                    </asp:Panel>
           </td>
        </tr>
        <tr>
            <td>

                                                    <Sentinel:Grid ID="dgHistory" runat="server" 
                                                          IsAutoResize="True" 
                                                         AllowSorting="True" AutoGenerateColumns="False" AllowFilteringByColumn="True"
                                                        AllowPaging="True" AllowMultiRowSelection="True" PageSize="20"
                                                         GridLines="None" allText="All" ClearAllFiltersText="Clear All Filters" 
                                                         ExportText="Export" IsShowExportIcon="True" IsShowFilterIcon="True" 
                                                         meta:resourcekey="dgHistoryResource1" 
                                                         OnNeedDataSource="dgHistory_NeedDataSource" >
                                                        <GroupingSettings CaseSensitive="False" />
                                                        <MasterTableView CommandItemDisplay="Top">
                                                            <CommandItemSettings ExportToPdfText="Export to Pdf" />
                                                            <RowIndicatorColumn>
                                                                <HeaderStyle Width="20px" />
                                                            </RowIndicatorColumn>
                                                            <ExpandCollapseColumn>
                                                                <HeaderStyle Width="20px" />
                                                            </ExpandCollapseColumn>
                                                            <Columns>
                                                                <telerik:GridBoundColumn DataField="VehicleDescription" 
                                                                    HeaderText="<%$ Resources:dgHistory_Vehicle %>" 
                                                                    meta:resourcekey="GridBoundColumnResource1" UniqueName="VehicleDescription">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="ServiceDateTime" DataType="System.DateTime" 
                                                                    HeaderText="<%$ Resources:dgHistory_DateTime %>" 
                                                                    meta:resourcekey="GridBoundColumnResource2" UniqueName="ServiceDateTime">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="ServiceTypeDescription" 
                                                                    HeaderText="<%$ Resources:dgHistory_ServiceType %>" 
                                                                    meta:resourcekey="GridBoundColumnResource3" UniqueName="ServiceTypeDescription">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="ServiceValue" 
                                                                    HeaderText="<%$ Resources:dgHistory_ServiceValue %>" 
                                                                    meta:resourcekey="GridBoundColumnResource4" UniqueName="ServiceValue">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="CurrentClosingValue" 
                                                                    HeaderText="<%$ Resources:dgHistory_ClosingValue %>" 
                                                                    meta:resourcekey="GridBoundColumnResource5" UniqueName="CurrentClosingValue">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="ServiceDescription" 
                                                                    HeaderText="<%$ Resources:dgHistory_Description %>" 
                                                                    meta:resourcekey="GridBoundColumnResource6" UniqueName="ServiceDescription">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="Comments" HeaderText="Notes" 
                                                                    meta:resourcekey="GridBoundColumnResource7" UniqueName="Comments">
                                                                </telerik:GridBoundColumn>
                                                            </Columns>
                    <CommandItemTemplate>
                        <table id="tblCustomerCommand" runat="server" width="100%">
                            <tr>
                                <td align="left"> 
                                </td>
                            </tr>
                        </table>
                    </CommandItemTemplate>


                                                        </MasterTableView>
                <ClientSettings>
                    <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="false"  />
                    <Resizing AllowColumnResize="True" EnableRealTimeResize="True" ResizeGridOnColumnResize="false">
                    </Resizing>
                </ClientSettings>

                                                        <FilterMenu CssClass="FiltMenuCss" EnableTheming="True">
                                                        </FilterMenu>
                                                    </Sentinel:Grid>
           </td>
        </tr>
        </table>
                                                </asp:Panel>
</asp:Content>

