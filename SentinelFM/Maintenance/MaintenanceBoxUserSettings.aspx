<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.master" AutoEventWireup="true" CodeFile="MaintenanceBoxUserSettings.aspx.cs" Inherits="SentinelFM.Maintenance_MaintenanceBoxUserSettings" Theme="TelerikControl" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<%@ MasterType VirtualPath="~/MasterPage/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
 
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>

    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"  OnAjaxRequest="RadAjaxManager1_AjaxRequest" 
         EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="dgMaintenance">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="cboFleet">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

        </AjaxSettings>
    </telerik:RadAjaxManager>
<asp:Panel ID="pnl" runat="server" meta:resourcekey="pnlResource1">
    <table >
        <tr >
           <td>
              <table>
                 <tr>
            <td align="left" >
                <asp:Label ID="lblFleetTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFleetTitleResource1"
                    Text="Fleet:" />
            </td>
            <td align="left" >
                <telerik:RadComboBox ID="cboFleet" runat="server" AutoPostBack="True" CssClass="RegularText"
                    DataTextField="FleetName" DataValueField="FleetId" meta:resourcekey="cboFleetResource1"
                    OnSelectedIndexChanged="cboFleet_SelectedIndexChanged" Width="257px" 
                    Filter="Contains" MarkFirstMatch="True" 
                    ChangeTextOnKeyBoardNavigation="False" Skin="Hay"
                    MaxHeight="400px" CausesValidation = "False"
                />
            </td>

                 </tr>
              </table>
           </td>
        </tr>
        <tr>
           <td >
  <Sentinel:Grid ID="dgMaintenance" runat="server" Visible="true" FilterItemStyle-HorizontalAlign="Left"
                AllowSorting="True" AutoGenerateColumns="False"  LoadingPanelID="LoadingPanel1"
                AllowFilteringByColumn="True" ExportText="Export" 
                AllowPaging="true"  PageSize="20"  Width="100%" Height="100%" 
                onneeddatasource="dgMaintenance_NeedDataSource" IsAutoResize="true" 
                   Skin="Simple" onitemdatabound="dgMaintenance_ItemDataBound"
                >
                <GroupingSettings CaseSensitive="false" />
                <MasterTableView  CommandItemDisplay="Top" ClientDataKeyNames="VehicleId" DataKeyNames="VehicleId" >
                    <PagerStyle Mode="NextPrevAndNumeric" />
                    <Columns>

                        <telerik:GridBoundColumn HeaderText="Box ID" DataField="BoxId"
                            UniqueName="BoxId" >
                            <ItemStyle Width="80px"  />
                            <HeaderStyle Width="80px" />
                        </telerik:GridBoundColumn>

                        <telerik:GridBoundColumn HeaderText="Vehicle" DataField="Description"
                            UniqueName="Description" >
                        </telerik:GridBoundColumn>

                        <telerik:GridBoundColumn HeaderText="Odometer" DataField="Odometer"
                            UniqueName="Odometer">
                            <ItemStyle Width="150px"  />
                            <HeaderStyle Width="150px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="EngineHrs" DataField="EngineHrs"
                            UniqueName="EngineHrs">
                            <ItemStyle Width="150px"  />
                            <HeaderStyle Width="150px" />
                        </telerik:GridBoundColumn>


                        <telerik:GridTemplateColumn UniqueName="UseCurrentValue" >
                           <ItemTemplate>
                               <nobr>
                                     <asp:ImageButton ID="imgComments" ImageUrl="~/images/Edit.gif" runat="server" ToolTip="Edit Comment" />
                               </nobr>
                           </ItemTemplate>
                            <ItemStyle Width="60px" HorizontalAlign="Center" />
                            <HeaderStyle Width="60px" HorizontalAlign="Center" />


                        </telerik:GridTemplateColumn>

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
            </Sentinel:Grid>
           </td>
        </tr>
    </table>
</asp:Panel>
</asp:Content>

