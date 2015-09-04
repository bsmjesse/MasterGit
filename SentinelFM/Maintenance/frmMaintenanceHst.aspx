<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.master" AutoEventWireup="true"
    CodeFile="frmMaintenanceHst.aspx.cs" Inherits="SentinelFM.Maintenance_frmMaintenanceHst" meta:resourcekey="PageResource1" Theme="TelerikControl"   %>
<%@ MasterType VirtualPath="~/MasterPage/MasterPage.master" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<%@ Register TagName="FleetVehicles" TagPrefix="fvs" Src="~/UserControl/FleetVehiclesControl.ascx" %>
<%@ Register TagName="FleetVehicles1" TagPrefix="fvs1" Src="~/UserControl/FleetVehiclesControl.ascx" %>
<%@ Register Src="../UserControl/FleetVehicleOrganizationHierarchy.ascx" TagName="FleetVehicle" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
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
            
            <telerik:AjaxSetting AjaxControlID="ddlDateTime">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="pnlFleetVehicle">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="dgMaintenance">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <asp:Panel ID="pnl" runat="server" meta:resourcekey="pnlResource1"  >
    <table style="background-color:white">
        <tr>
            <td>
                <table>
                   <tr>
                      <td>
                          <asp:Panel ID="pnlFleetVehicle"  runat="server" 
                              meta:resourcekey="pnlFleetVehicleResource1" >
                            <uc1:FleetVehicle ID="FleetVehicle1" runat="server" />
                          </asp:Panel>
                      </td>
                      <td>
                         <asp:DropDownList ID="ddlDateTime" runat = "server" class="formText" AutoPostBack="True"
                              onselectedindexchanged="ddlDateTime_SelectedIndexChanged" 
                              meta:resourcekey="ddlDateTimeResource1" >
                            <asp:ListItem Text="One Week"     Value = "1" Selected="True" 
                                 meta:resourcekey="ListItemResource4" ></asp:ListItem>
                            <asp:ListItem Text="Two Weeks"  Value = "2" 
                                 meta:resourcekey="ListItemResource5" ></asp:ListItem>
                            <asp:ListItem Text="One Month"    Value = "3" 
                                 meta:resourcekey="ListItemResource6" ></asp:ListItem>
                            <asp:ListItem Text="Two Months" Value = "4" 
                                 meta:resourcekey="ListItemResource7" ></asp:ListItem>
                            <asp:ListItem Text="Three Months"  Value = "5" 
                                 meta:resourcekey="ListItemResource8" ></asp:ListItem>
                            <asp:ListItem Text="Six Months" Value = "6" 
                                 meta:resourcekey="ListItemResource9" ></asp:ListItem>
                            <asp:ListItem Text="One Year"      Value = "7" 
                                 meta:resourcekey="ListItemResource10" ></asp:ListItem>
                            <asp:ListItem Text="Two Years"   Value = "8" 
                                 meta:resourcekey="ListItemResource11" ></asp:ListItem>
                            <asp:ListItem Text="All"           Value = "9" 
                                 meta:resourcekey="ListItemResource12" ></asp:ListItem>
                         </asp:DropDownList>
                      </td>
                   </tr>
                </table>
            </td>
        </tr>
        <tr>
           <td>
               
            <Sentinel:Grid ID="dgMaintenance" runat="server"
                AllowSorting="True" AutoGenerateColumns="False"  LoadingPanelID="LoadingPanel1"
                AllowFilteringByColumn="True" ExportText="Export" 
                AllowPaging="True"  PageSize="20"  Width="100%" Height="100%" 
                onneeddatasource="dgMaintenance_NeedDataSource" IsAutoResize="True" 
                   Skin="Simple" allText="All" ClearAllFiltersText="Clear All Filters" 
                   GridLines="None" IsShowExportIcon="True" IsShowFilterIcon="True" meta:resourcekey="dgMaintenanceResource1" 
                >
                <GroupingSettings CaseSensitive="False" />
                <ClientSettings>
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" 
                        SaveScrollPosition="False"  />
                    <Resizing AllowColumnResize="True" EnableRealTimeResize="True">
                    </Resizing>
                </ClientSettings>
                <MasterTableView CommandItemDisplay="Top">
                    <CommandItemSettings ExportToPdfText="Export to Pdf" />
                    <RowIndicatorColumn>
                        <HeaderStyle Width="20px" />
                    </RowIndicatorColumn>
                    <ExpandCollapseColumn>
                        <HeaderStyle Width="20px" />
                    </ExpandCollapseColumn>
                    <Columns>
                        <telerik:GridBoundColumn DataField="BoxId" HeaderText="Box ID" 
                            meta:resourcekey="GridBoundColumnResource2" UniqueName="BoxId">
                            <HeaderStyle Width="60px" />
                            <ItemStyle Width="60px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="VehicleDescription" HeaderText="Vehicle" 
                            meta:resourcekey="GridBoundColumnResource3" UniqueName="VehicleDescription">
                            <HeaderStyle Width="180px" />
                            <ItemStyle Width="180px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn HeaderText="OperationType" 
                            meta:resourcekey="GridTemplateColumnResource1" SortExpression="OperationTypeID" 
                            UniqueName="OperationType">
                            <ItemTemplate>
                                <asp:Label ID="lblOperationType" runat="server" 
                                    meta:resourcekey="lblOperationTypeResource1" 
                                    Text='<%# FindOperationType(Eval("OperationTypeID")) %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="Description" HeaderText="Service" 
                            meta:resourcekey="GridBoundColumnResource4" UniqueName="Description">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="MaintenanceDateTime" 
                            DataFormatString="{0:MM/dd/yyyy}" HeaderText="DateTime" 
                            meta:resourcekey="GridBoundColumnResource5" UniqueName="MaintenanceDateTime">
                            <HeaderStyle Width="150px" />
                            <ItemStyle Width="150px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="DueValue" HeaderText="Closing Value" 
                            meta:resourceKey="dgHistory_ClosingValue" UniqueName="DueValue">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Comments" HeaderText="Comments" 
                            meta:resourcekey="GridBoundColumnResource6" UniqueName="Comments">
                            <ItemStyle CssClass="WrapTextClass" Wrap="True" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="UserName" HeaderText="Closed by" 
                            meta:resourcekey="GridBoundColumnResource7" UniqueName="UserName">
                            <ItemStyle CssClass="WrapTextClass" Wrap="True" />
                        </telerik:GridBoundColumn>
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
        <asp:XmlDataSource ID="XmlOperationType" runat="server" />
</asp:Content>
