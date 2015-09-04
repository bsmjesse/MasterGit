<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.master" AutoEventWireup="true" CodeFile="frmMaintenanceGroupAssign.aspx.cs" Inherits="SentinelFM.Maintenance_frmMaintenanceGroupAssign" Theme="TelerikControl" meta:resourcekey="PageResource1" %>
<%@ MasterType VirtualPath="~/MasterPage/MasterPage.master" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" 
        meta:resourcekey="RadAjaxManager1Resource1" 
        OnAjaxRequest="RadAjaxManager1_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dgMCCAssignment" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="dgMCCAssignment">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dgMCCAssignment" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
             <Sentinel:Grid AutoGenerateColumns="False" ID="dgMCCAssignment" 
                  IsAutoResize="True" 
                  AllowSorting="True"
                                                        AllowPaging="True" runat="server"   
                  Width="960px" OnNeedDataSource="dgMCCAssignment_NeedDataSource"
                                                         PageSize="20" 
                  AllowFilteringByColumn="True" FilterItemStyle-HorizontalAlign="Left"
                                                        OnItemDataBound="dgMCCAssignment_ItemDataBound" 
                                                         
                  oninsertcommand="dgMCCAssignment_InsertCommand" 
                  onitemcommand="dgMCCAssignment_ItemCommand" allText="All" 
        ClearAllFiltersText="Clear All Filters" ExportText='<%$ Resources:dgMaintenance_Export %>' 
        IsShowExportIcon="True" IsShowFilterIcon="True" 
        meta:resourcekey="dgMCCAssignmentResource1">
                                                        <GroupingSettings CaseSensitive="false" />
                                                        <MasterTableView DataKeyNames="MccId" ClientDataKeyNames="MccId" CommandItemDisplay="Top"
                                                            ItemStyle-Width="300px" >
<CommandItemSettings ExportToPdfText="Export to Pdf"></CommandItemSettings>

<RowIndicatorColumn>
<HeaderStyle Width="20px"></HeaderStyle>
</RowIndicatorColumn>

<ExpandCollapseColumn>
<HeaderStyle Width="20px"></HeaderStyle>
</ExpandCollapseColumn>
                                                            <Columns>
                                                                <telerik:GridTemplateColumn HeaderText="Group Name" UniqueName="MccName" SortExpression="MccName"
                                                                    meta:resourcekey="GridTemplateColumnMccNameResource1" AllowFiltering="true" DataField="MccName">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblMccName" CssClass="formtext" runat="server" Text='<%# Bind("MccName") %>'
                                                                            meta:resourcekey="lblMccNameResource1"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:TextBox ID="txtMccName" runat="server" MaxLength="50" Width="300px" meta:resourcekey="txtMccNameResource1"
                                                                            Text='<%# Bind("MccName") %>'></asp:TextBox>
                                                                        <span style="color: Red">*</span><br />
                                                                        <asp:RequiredFieldValidator ID="valReqtxtMccName" runat="server" ValidationGroup="valMccAdd"
                                                                            ControlToValidate="txtMccName" meta:resourcekey="valReqtxtMccNameResource1" Text="Please enter MCC name"
                                                                            Display="Dynamic"></asp:RequiredFieldValidator>
                                                                    </EditItemTemplate>
                                                                </telerik:GridTemplateColumn>
                                                                <telerik:GridTemplateColumn UniqueName="Service" AllowFiltering="false" 
                                                                    meta:resourcekey="GridTemplateColumnResource1">
                                                                    <ItemTemplate>
                                                                        <telerik:RadGrid ID="dgServiceAssignment" runat="server" AutoGenerateColumns="False" 
                                                                            GridLines="None" Skin="Simple" ShowHeader="False" Width="750px" 
                                                                            OnItemDataBound="dgServiceAssignment_ItemDataBound" 
                                                                            meta:resourcekey="dgServiceAssignmentResource1">
                                                                            <MasterTableView>
                                                                                <CommandItemSettings ExportToPdfText="Export to Pdf" />
                                                                                <Columns>
                                                                                    <telerik:GridBoundColumn HeaderText="Description" UniqueName="Description" SortExpression="Description"
                                                                                        meta:resourcekey="gdMCCMaintenancesDescriptionResource1"
                                                                                        DataField="Description">
                                                                                        <ItemStyle Width="200px" />
                                                                                        <HeaderStyle Width="200px" />
                                                                                    </telerik:GridBoundColumn>
                                                                                    <telerik:GridBoundColumn HeaderText="Operation Type" UniqueName="OperationType" SortExpression="OperationType"
                                                                                        meta:resourcekey="gdMCCMaintenancesOperationTypeResource1"
                                                                                        DataField="OperationType">
                                                                                        <ItemStyle Width="150px" />
                                                                                        <HeaderStyle Width="150px" />
                                                                                    </telerik:GridBoundColumn>
                                                                                    <telerik:GridBoundColumn HeaderText="Notification Type" UniqueName="NotificationType"
                                                                                        DataField="NotificationDescription" SortExpression="NotificationDescription"
                                                                                        meta:resourcekey="gdMCCMaintenancesNotificationTypeResource1"
                                                                                        >
                                                                                        <ItemStyle Width="150px" />
                                                                                        <HeaderStyle Width="150px" />
                                                                                    </telerik:GridBoundColumn>
                                                                                    <telerik:GridBoundColumn HeaderText="Frequency" UniqueName="FrequencyName" SortExpression="FrequencyName"
                                                                                        meta:resourcekey="gdMCCMaintenancesFrequencyIDResource1"
                                                                                        DataField="FrequencyName" >
                                                                                        <ItemStyle Width="90px" />
                                                                                        <HeaderStyle Width="90px" />
                                                                                    </telerik:GridBoundColumn>
                                                                                    <telerik:GridBoundColumn HeaderText="Interval" UniqueName="Interval " SortExpression="IntervalDesc "
                                                                                        meta:resourcekey="gdMCCMaintenancesIntervalResource1" DataField="IntervalDesc "
                                                                                        DataType="System.Int16">
                                                                                        <ItemStyle Width="90px" />
                                                                                        <HeaderStyle Width="90px" />
                                                                                    </telerik:GridBoundColumn>
                                                                                </Columns>
                                                                                <ItemStyle HorizontalAlign="Left" />
                                                                                <AlternatingItemStyle HorizontalAlign="Left" />
                                                                                <HeaderStyle HorizontalAlign="Left" Font-Bold="True" />
                                                                            </MasterTableView>
                                                                        </telerik:RadGrid>
                                                                    </ItemTemplate>
                                                                    <HeaderTemplate>
                                                                        <norbr>
                                                                            <asp:Label ID="lblMaintenanceDescription" Width="210px" style="font:12px/16px 'segoe ui',arial,sans-serif;" ForeColor="White" runat="server" Text ="Description" meta:resourcekey="lblMaintenanceDescriptionResource1" ></asp:Label>
                                                                            <asp:Label ID="lblMaintenanceOperationType" runat="server"  Width="160px" style="font:12px/16px 'segoe ui',arial,sans-serif;" ForeColor="White" Text ="Operation Type" meta:resourcekey="lblMaintenanceOperationTypeResource1" ></asp:Label>
                                                                            <asp:Label ID="lblMaintenanceNotificationType" runat="server"  Width="160px" style="font:12px/16px 'segoe ui',arial,sans-serif;" ForeColor="White" Text ="Notification Type" meta:resourcekey="lblMaintenanceNotificationTypeResource1" ></asp:Label>
                                                                            <asp:Label ID="lblMaintenanceFrequencyID" runat="server"  Width="95px" style="font:12px/16px 'segoe ui',arial,sans-serif;" ForeColor="White" Text ="Frequency ID" meta:resourcekey="lblMaintenanceFrequencyIDResource1" ></asp:Label>
                                                                            <asp:Label ID="lblMaintenanceInterval" runat="server"  Width="80px" style="font:12px/16px 'segoe ui',arial,sans-serif;" ForeColor="White" Text ="Interval" meta:resourcekey="lblMaintenanceIntervalResource1" ></asp:Label>
                                                                        </norbr>
                                                                    </HeaderTemplate>
                                                                    <ItemStyle Width="755px" CssClass="SubRadGridItem" />
                                                                    <HeaderStyle Width="755px"  />
                                                                </telerik:GridTemplateColumn>
                                                                <telerik:GridTemplateColumn AllowFiltering="false" 
                                                                    meta:resourcekey="GridTemplateColumnResource2">
                                                                    <ItemTemplate>
                                                                        <asp:ImageButton ID="lnkModify" runat="server" ToolTip="Edit Assignment" ImageUrl="../images/edit.gif" CommandName="Modify" 
                                                                            meta:resourcekey="lnkModifyResource1" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="30px" HorizontalAlign="center" />
                                                                    <HeaderStyle Width="30px" HorizontalAlign="center" />
                                                                </telerik:GridTemplateColumn>
                                                            </Columns>
                                                            <EditFormSettings ColumnNumber="2">
                                                                <FormTableItemStyle HorizontalAlign="Left" VerticalAlign="Top"></FormTableItemStyle>
                                                                <FormTableStyle CellSpacing="0" CellPadding="2" Font-Bold="true" HorizontalAlign="Left" />
                                                                <FormTableAlternatingItemStyle Wrap="False" HorizontalAlign="Left" VerticalAlign="Top">
                                                                </FormTableAlternatingItemStyle>
                                                                <EditColumn ButtonType="PushButton" InsertText="Save" UpdateText="Save">
                                                                </EditColumn>
                                                                <FormMainTableStyle />
                                                            </EditFormSettings>
                                                            <HeaderStyle CssClass="RadGridtblHeader"  />
                                                            <ItemStyle VerticalAlign="Top" />
                                                            <AlternatingItemStyle VerticalAlign="Top" />
                                                            <CommandItemTemplate>
                                                                <table id="tblCustomerCommand" runat="server" width="100%">
                                                                    <tr runat="server">
                                                                        <td runat="server">
                                                                            <asp:Button ID="btnAdd" Text="Add New Group" CommandName="InitInsert" 
                                                                                runat="server" Visible="False"
                                                                                CssClass="combutton" Width="180px" meta:resourcekey="btnAddMccResource1" />

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

<FilterItemStyle HorizontalAlign="Left"></FilterItemStyle>

<FilterMenu CssClass="FiltMenuCss" EnableTheming="True"></FilterMenu>

                                                    </Sentinel:Grid>

</asp:Content>

