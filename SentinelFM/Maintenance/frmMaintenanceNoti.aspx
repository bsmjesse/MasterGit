<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.master" AutoEventWireup="true" CodeFile="frmMaintenanceNoti.aspx.cs" Inherits="SentinelFM.Maintenance_frmMaintenanceNoti" Theme="TelerikControl" meta:resourcekey="PageResource1"   %>
<%@ MasterType VirtualPath="~/MasterPage/MasterPage.master" %>
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
            <telerik:AjaxSetting AjaxControlID="dgNotification">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="cmdCloseNotification">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            
        </AjaxSettings>
    </telerik:RadAjaxManager>
 
                                                <asp:Panel ID="pnl" runat="server" 
        meta:resourcekey="pnlResource1">
                                                    <fieldset style="margin: 0px 0px 5px 0px; padding: 0px 0px 0px 0px;background-color:white">
                                                        <table cellpadding="1" cellspacing="1"  style="background-color:white">
                                                            <tr>
                                                                <td>
                                                                    <asp:Button CssClass="combutton" Font-Bold="True" ID="cmdCloseNotification" runat="server"
                                                                        OnClick="cmdCloseNotification_Click" 
                                                                        Text="<%$ Resources:lnkAcknowledge %>" 
                                                                        meta:resourcekey="cmdCloseNotificationResource1" />
                                                                </td>
                                                                <td>
                                                                    &nbsp;
                                                                </td>
                                                                <td>
                                                                    &nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblMessageNotifications" CssClass="formtext" ForeColor="Red" 
                                                                        runat="server" meta:resourcekey="lblMessageNotificationsResource1" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>
                                                    <asp:Label ID="LabelCustomerNote" runat="server" Font-Size="Small" ForeColor="Gray"
                                                        meta:resourcekey="LabelCustomerNoteResource1" />
                                                    <Sentinel:Grid ID="dgNotification" runat="server" IsAutoResize="True"
                                                        AllowSorting="True" AutoGenerateColumns="False" AllowFilteringByColumn="True"
                                                        AllowPaging="True" Width="980px" AllowMultiRowSelection="True" PageSize="20"
                                                        OnNeedDataSource="dgNotification_NeedDataSource" GridLines="None" 
                                                        meta:resourcekey="dgNotificationResource1" >
                                                        <GroupingSettings CaseSensitive="False" />
                                                        <ClientSettings>
                                                            <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="False"
                                                                ScrollHeight="400px" />
                                                        </ClientSettings>
                                                        <MasterTableView ClientDataKeyNames="NotificationId, TypeId" CommandItemDisplay="Top" 
                                                            DataKeyNames="NotificationId, TypeId" Width="960px">
                                                            <CommandItemSettings ExportToPdfText="Export to Pdf" />
                                                            <Columns>
                                                                <telerik:GridTemplateColumn AllowFiltering="False" 
                                                                    meta:resourcekey="GridTemplateColumnResource1" 
                                                                    UniqueName="selectNotificationCheckBox">
                                                                    <HeaderTemplate>
                                                                        <asp:CheckBox ID="chkSelectAllNotification" runat="server" onclick="return MCCMaintenancesCheckAll(this);"
                                                                            meta:resourcekey="chkSelectAllNotificationResource1" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkNotification" runat="server" onclick="return MCCMaintenancesCheck(this);"
                                                                            Checked='<%# GetCheckBox( Eval("chkBoxShow")) %>' 
                                                                            meta:resourcekey="chkNotificationResource1" />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Center" Width="30px" />
                                                                    <ItemStyle HorizontalAlign="Center" Width="30px" />
                                                                </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn  DataField="BoxId"
                            UniqueName="BoxId" HeaderText="<%$ Resources:dgNotification_BoxId %>">  
                            <ItemStyle Width="60px"  />
                            <HeaderStyle Width="60px" />
                        </telerik:GridBoundColumn>

                                                                <telerik:GridBoundColumn DataField="DateTimeCreated" DataType="System.DateTime" 
                                                                    HeaderText="<%$ Resources:dgNotification_DateTime %>" 
                                                                    UniqueName="DateTimeCreated">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="Description" 
                                                                    HeaderText="<%$ Resources:dgNotification_Vehicle %>" 
                                                                    meta:resourcekey="GridBoundColumnResource2" UniqueName="Description">
                                                                </telerik:GridBoundColumn>

                                                                <telerik:GridBoundColumn DataField="LicensePlate" 
                                                                    HeaderText="<%$ Resources:dgNotification_LicensePlate %>" 
                                                                    meta:resourcekey="GridBoundColumnResource3" UniqueName="LicensePlate">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="notificationDescription" 
                                                                    HeaderText="<%$ Resources:dgNotification_Notification %>" 
                                                                    meta:resourcekey="GridBoundColumnResource4" 
                                                                    UniqueName="notificationDescription">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="StatusPercentage" SortExpression="StatusPercentage"
                                                                   HeaderText="<%$ Resources:dgNotification_Service %>"  DataType="System.Double" 
                                                                    UniqueName="StatusPercentage ">
                                                                </telerik:GridBoundColumn>

                                                                <telerik:GridBoundColumn DataField="Data" 
                                                                    HeaderText="<%$ Resources:dgNotification_Data %>" 
                                                                    meta:resourcekey="GridBoundColumnResource5" UniqueName="Data" Visible="False">
                                                                    <HeaderStyle Width="100px" />
                                                                    <ItemStyle Width="100px" />
                                                                </telerik:GridBoundColumn>
								 <telerik:GridBoundColumn DataField="NotificationType" 
                                                                    HeaderText="Type" 

                                                                    UniqueName="NotificationType">
                                                                </telerik:GridBoundColumn>	
                                                               
                                                            </Columns>
                                                            <CommandItemTemplate>
                                                                <table ID="tblCustomerCommand" runat="server" width="100%">
                                                                    <tr id="Tr1" runat="server">
                                                                        <td id="Td1" runat="server">
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </CommandItemTemplate>
                                                        </MasterTableView>
                                                        <FilterItemStyle HorizontalAlign="Left" />
                                                    </Sentinel:Grid>
                                                </asp:Panel>
        <asp:XmlDataSource ID="XmlOperationType" runat="server" />
            <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript" >

        function MCCMaintenancesCheckAll(ctl) {
            var isChecked = $telerik.$(ctl).attr("checked");
            $telerik.$("#<%=dgNotification.ClientID %>").find("input:checkbox").attr("checked", isChecked); ;
        }

        function MCCMaintenancesCheck(ctl) {
            var isChecked = $telerik.$(ctl).attr("checked");
            if (!isChecked) $telerik.$("#<%=  dgNotification.ClientID%>").find("[ID$='chkSelectAllNotification']").attr("checked", isChecked);
        }

    </script>
    </telerik:RadCodeBlock>

</asp:Content>

