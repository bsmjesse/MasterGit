<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.master" AutoEventWireup="true" CodeFile="frmMaintenanceVehicleView.aspx.cs" Inherits="SentinelFM.Maintenance_frmMaintenanceVehicleView" Theme="TelerikControl" meta:resourcekey="PageResource1" %>
<%@ MasterType VirtualPath="~/MasterPage/MasterPage.master" %>
<%@ Register Src="../UserControl/FleetVehicleOrganizationHierarchy.ascx" TagName="FleetVehicle" TagPrefix="uc1" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>

    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1" 
         EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="dgMCCAssignment">
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

    <asp:Panel ID="pnl" runat="server"  >
   <table>
        <tr>
            <td>
                <asp:Panel ID="pnlFleetVehicle"  runat="server" >
                <uc1:FleetVehicle ID="FleetVehicle1" runat="server" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
           <td>
      <Sentinel:Grid AutoGenerateColumns="False" ID="dgMCCAssignment" 
                                                         AllowAutomaticDeletes="false" 
                  IsAutoResize="true" GridLines="Both" 
                                                        AllowAutomaticInserts="false" 
                  AllowSorting="true" AllowAutomaticUpdates="false"
                                                        AllowPaging="True" runat="server"   
                  Width="960px" OnNeedDataSource="dgMCCAssignment_NeedDataSource"
                                                         PageSize="20" 
                  AllowFilteringByColumn="true" FilterItemStyle-HorizontalAlign="Left"
                                                        
                   >
                                                        <GroupingSettings CaseSensitive="false" />
                                                        <MasterTableView ClientDataKeyNames="VehicleId" DataKeyNames="VehicleId" CommandItemDisplay="Top"
                                                             >
                                                            <Columns>
                                                                <telerik:GridBoundColumn HeaderText='<%$ Resources:dgMCCAssignment_BoxId %>'  DataField="BoxId"
                                                                    UniqueName="BoxId" >
                                                                                        <ItemStyle Width="60px" />
                                                                                        <HeaderStyle Width="60px" />

                                                                </telerik:GridBoundColumn>

                                                                <telerik:GridBoundColumn HeaderText='<%$ Resources:dgMCCAssignment_Vehicle %>' DataField="VehicleDescription"
                                                                    UniqueName="VehicleDescription" >
                                                                                        <ItemStyle Width="100px" />
                                                                                        <HeaderStyle Width="100px" />

                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridTemplateColumn UniqueName="Service" AllowFiltering="false">
                                                                    <ItemTemplate>
                                                                        <telerik:RadGrid ID="dgServiceAssignment" runat="server" AutoGenerateColumns="False"  OnDeleteCommand="dgServiceAssignment_ItemDeleted" OnItemDataBound="dgServiceAssignment_ItemDataBound"
                                                                            GridLines="None"  ShowHeader="true" AllowSorting="true" AllowPaging="false" AllowFilteringByColumn="false" OnNeedDataSource="dgServiceAssignment_NeedDataSource"  >
                                                                            <MasterTableView GroupLoadMode="Server"  ClientDataKeyNames="MccId, VehicleId, MaintenanceID" DataKeyNames="MccId, VehicleId, MaintenanceID" >
                                                                                <Columns>
                                                                                    <telerik:GridBoundColumn HeaderText="Group" UniqueName="MccGroupName"  SortExpression="MccName"
                                                                                        meta:resourcekey="gdMCCMaintenancesMccResource1" 
                                                                                        DataField="MccName">
                                                                                        <ItemStyle Width="80px"   />
                                                                                        <HeaderStyle Width="80px" ForeColor="White" />
                                                                                    </telerik:GridBoundColumn>

                                                                                    <telerik:GridBoundColumn HeaderText="Description" UniqueName="Description" SortExpression="Description"
                                                                                        meta:resourcekey="gdMCCMaintenancesDescriptionResource1" 
                                                                                        DataField="Description">
                                                                                        <ItemStyle Width="100px"  />
                                                                                        <HeaderStyle Width="100px" ForeColor="White" />
                                                                                    </telerik:GridBoundColumn>


                                                                                    <telerik:GridBoundColumn HeaderText="Operation Type" UniqueName="OperationType" SortExpression="OperationType"
                                                                                        meta:resourcekey="gdMCCMaintenancesOperationTypeResource1" 
                                                                                        DataField="OperationType">
                                                                                        <ItemStyle Width="80px" />
                                                                                        <HeaderStyle Width="80px" />
                                                                                    </telerik:GridBoundColumn>

                                                                                    <telerik:GridBoundColumn HeaderText="Notification Type" UniqueName="NotificationType"
                                                                                        DataField="NotificationDescription" SortExpression="NotificationDescription"
                                                                                        meta:resourcekey="gdMCCMaintenancesNotificationTypeResource1" AllowFiltering="true"
                                                                                        >
                                                                                        <ItemStyle Width="80px" />
                                                                                        <HeaderStyle Width="80px" />
                                                                                    </telerik:GridBoundColumn>
                                                                                    <telerik:GridBoundColumn HeaderText="Frequency" UniqueName="FrequencyName" SortExpression="FrequencyName"
                                                                                        meta:resourcekey="gdMCCMaintenancesFrequencyIDResource1"
                                                                                        DataField="FrequencyName" >
                                                                                        <ItemStyle Width="80px" />
                                                                                        <HeaderStyle Width="80px" />
                                                                                    </telerik:GridBoundColumn>
                                                                                    <telerik:GridBoundColumn HeaderText="Interval" UniqueName="Interval " SortExpression="IntervalDesc "
                                                                                        meta:resourcekey="gdMCCMaintenancesIntervalResource1" AllowFiltering="true" DataField="IntervalDesc "
                                                                                        DataType="System.Int16">
                                                                                        <ItemStyle Width="80px" />
                                                                                        <HeaderStyle Width="80px" />
                                                                                    </telerik:GridBoundColumn>
                                                                                    <telerik:GridButtonColumn ConfirmText="Delete this assignment?" ConfirmDialogType="Classic"
                                                                                        ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                                                                                        UniqueName="DeleteColumn" meta:resourcekey="GridMccButtonColumnDeleteResource1"
                                                                                        ImageUrl="../images/delete.gif">
                                                                                        <ItemStyle HorizontalAlign="Center" Width="30px" />
                                                                                        <HeaderStyle HorizontalAlign="Center" Width="30px" />

                                                                                    </telerik:GridButtonColumn>
                                                                                </Columns>
                                                                                <ItemStyle HorizontalAlign="Left" />
                                                                                <AlternatingItemStyle HorizontalAlign="Left" />
                                                                                <HeaderStyle HorizontalAlign="Left" Font-Bold="true"  ForeColor="White" CssClass="RadGridtblHeader" />
                                                                            </MasterTableView>
                                                                        </telerik:RadGrid>
                                                                    </ItemTemplate>
                                                                    <ItemStyle  CssClass="SubRadGridItem" />
                                                                    <HeaderStyle  />
                                                                </telerik:GridTemplateColumn>
                                                            </Columns>
                                                            <HeaderStyle CssClass="RadGridtblHeader"  />
                                                            <ItemStyle VerticalAlign="Top" />
                                                            <AlternatingItemStyle VerticalAlign="Top" />
                                                            <CommandItemTemplate>
                                                                <table id="tblCustomerCommand" runat="server" width="100%">
                                                                    <tr>
                                                                        <td>

                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </CommandItemTemplate>

                                                        </MasterTableView>
                                                        <ValidationSettings CommandsToValidate="PerformInsert,Update" ValidationGroup="valMccAdd" />
                                                        <ClientSettings>
                                                                            <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="false"  />
                    <Resizing AllowColumnResize="True" EnableRealTimeResize="True" ResizeGridOnColumnResize="false">
                    </Resizing>

                                                        </ClientSettings>

                                                    </Sentinel:Grid>
           </td>
        </tr>
    </table>
    </asp:Panel>
</asp:Content>

