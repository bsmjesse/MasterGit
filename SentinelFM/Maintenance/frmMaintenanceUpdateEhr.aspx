<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.master" AutoEventWireup="true" CodeFile="frmMaintenanceUpdateEhr.aspx.cs" Inherits="SentinelFM.Maintenance_frmMaintenanceUpdateEhr" Theme="TelerikControl" meta:resourcekey="PageResource1" %>
<%@ MasterType VirtualPath="~/MasterPage/MasterPage.master" %>
<%@ Register Src="../UserControl/FleetVehicleOrganizationHierarchy.ascx" TagName="FleetVehicle" TagPrefix="uc1" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>

    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" 
        meta:resourcekey="RadAjaxManager1Resource1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="dgMaintenance">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dgMaintenance" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="pnlFleetVehicle">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

        </AjaxSettings>
    </telerik:RadAjaxManager>
    <asp:Panel ID="pnl" runat="server" meta:resourcekey="pnlResource1"  >
        <table>
        <tr>
            <td>
                <asp:Panel ID="pnlFleetVehicle"  runat="server" 
                    meta:resourcekey="pnlFleetVehicleResource1" >
                <uc1:FleetVehicle ID="FleetVehicle1" runat="server" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
           <td>

                <Sentinel:Grid ID="dgMaintenance" runat="server"
                AllowSorting="True" AutoGenerateColumns="False"  LoadingPanelID="LoadingPanel1"
                AllowFilteringByColumn="True" ExportText="Export" 
                AllowPaging="True"  PageSize="20"  Width="100%" Height="100%" 
                onneeddatasource="dgMaintenance_NeedDataSource" IsAutoResize="True" 
                   Skin="Simple" onitemdatabound="dgMaintenance_ItemDataBound" 
                    onitemcommand="dgMaintenance_ItemCommand" allText="All" 
                    ClearAllFiltersText="Clear All Filters" GridLines="None" 
                    IsShowExportIcon="True" IsShowFilterIcon="True" meta:resourcekey="dgMaintenanceResource1"
                >
                <GroupingSettings CaseSensitive="False" />
                <ClientSettings>
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" 
                        SaveScrollPosition="False"  />
                    <Resizing AllowColumnResize="True" EnableRealTimeResize="True">
                    </Resizing>
                </ClientSettings>
                    <MasterTableView ClientDataKeyNames="VehicleId" CommandItemDisplay="Top" 
                        DataKeyNames="VehicleId">
                        <CommandItemSettings ExportToPdfText="Export to Pdf" />
                        <RowIndicatorColumn>
                            <HeaderStyle Width="20px" />
                        </RowIndicatorColumn>
                        <ExpandCollapseColumn>
                            <HeaderStyle Width="20px" />
                        </ExpandCollapseColumn>
                        <Columns>
                            <telerik:GridBoundColumn DataField="BoxId" HeaderText="Box ID" 
                                meta:resourcekey="GridBoundColumnResource1" UniqueName="BoxId">
                                <HeaderStyle Width="100px" />
                                <ItemStyle Width="100px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Description" HeaderText="Vehicle" 
                                meta:resourcekey="GridBoundColumnResource2" UniqueName="VehicleDescription">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn HeaderText="Current Engine Hours"  DataField="CurrentEngineHours" 
                                meta:resourcekey="GridTemplateColumnResource1" 
                                SortExpression="CurrentEngineHours" UniqueName="CurrentEngineHours"> 
                                <ItemTemplate>
                                    <asp:Label ID="lblCurrentEngineHours" runat="server" 
                                        meta:resourcekey="lblCurrentEngineHoursResource1" 
                                        Text='<%# Eval("CurrentEngineHours") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="150px" />
                                <ItemStyle Width="150px" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="New  Engine Hours"  
                                meta:resourcekey="GridTemplateColumnResource2" UniqueName="CurrentVal">
                                <ItemTemplate>
                                    <telerik:RadNumericTextBox ID="txtCurrentOdoHrs" runat="server" 
                                        CssClass="formtext" Culture="en-CA" LabelCssClass="" 
                                        meta:resourcekey="txtCurrentOdoHrsResource1" Width="70px">
                                        <NumberFormat DecimalDigits="0" />
                                        <ClientEvents OnValueChanging="SaveInitalInputValueService" />
                                    </telerik:RadNumericTextBox>
                                    &nbsp;
                                </ItemTemplate>
                                <HeaderStyle Width="150px" />
                                <ItemStyle HorizontalAlign="Left" Width="150px" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn AllowFiltering="False" 
                                meta:resourcekey="GridTemplateColumnResource3" UniqueName="TemplateColumn">
                                <ItemTemplate>
                                    <asp:Button ID="btnUpdate" runat="server" CommandName="Update" 
                                        CssClass="combutton" meta:resourcekey="btnUpdateResource1" Text="Update" />
                                </ItemTemplate>

<ItemStyle Width="120px" HorizontalAlign="Center" />
                            <HeaderStyle Width="120px" HorizontalAlign="Center" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="false" >
                            <ItemTemplate>                                
                                <asp:Button ID="Button1" CssClass="combutton" runat="server" Text="Set Odometer" CommandName="SetOdometer" OnClientClick=<%# Eval("LicensePlate", "window.open(\'../Map/frmSensorMain.aspx?LicensePlate={0}\', \'\', \'height=600,width=500,top=150,left=300,location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=yes\'); return false;") %>  />                                
                            </ItemTemplate>
                          

                                <HeaderStyle HorizontalAlign="Center" Width="120px" />
                                <ItemStyle HorizontalAlign="Center" Width="120px" />
                            </telerik:GridTemplateColumn>
                        </Columns>
                        <CommandItemTemplate>
                            <table ID="tblCustomerCommand" runat="server" width="100%">
                                <tr runat="server">
                                    <td runat="server" align="left">
                                    </td>
                                </tr>
                            </table>
                        </CommandItemTemplate>
                    </MasterTableView>
                    <FilterItemStyle HorizontalAlign="Left" />
                    <FilterMenu CssClass="FiltMenuCss" EnableTheming="True">
                    </FilterMenu>
          </Sentinel:Grid>
        </td>
        </tr>
      </table>
    </asp:Panel>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript" >
            function SaveInitalInputValueService(sender, eventArgs) {
            if (eventArgs.get_newValue() < 1 ) {
                eventArgs.set_cancel(true);
            }

            if (eventArgs.get_newValue() > sender.get_maxValue()) {
                eventArgs.set_cancel(true);
            }
        }
   </script>
   </telerik:RadCodeBlock>
</asp:Content>

