<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddServiceTypes.ascx.cs"
    Inherits="Maintenance_AddServiceTypes" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<asp:Panel ID="pnlAddServiceTypes" runat="server">
    <table >
        <tr>
            <td>
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>
                            <asp:Label ID="lblMccName" runat="server" Text="MCC Name:" meta:resourcekey="lblMccNameResource"
                                CssClass="formtext"></asp:Label>
                            <asp:Label ID="lblMccNameText" runat="server" Width="" CssClass="formtext"></asp:Label>
                            <asp:HiddenField ID="hidMccID" runat="server" />
                        </td>
                        <td align="right">
                            <asp:Button ID="btnClose" runat="Server" CssClass="combutton" Width="80px" Text="Close"
                                meta:resourcekey="btnCloseResource1" OnClientClick="return CloseEditServicesForm()" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <Sentinel:Grid ID="gvServicesSource" runat="server" AllowSorting="false" Width="690px" 
                    EnableTheming="false" VerticalGridNum="2" Height="300px" AllowPaging="false" AutoGenerateColumns="False"
                    meta:resourcekey="gvServicesSourceResource1" ToolTip="Unassigned Maintenances"
                    OnNeedDataSource="gvServicesSource_NeedDataSource" 
                    onitemdatabound="gvServicesSource_ItemDataBound">
                    <MasterTableView DataKeyNames="MaintenanceId" ClientDataKeyNames="MaintenanceId">
                        <Columns>
                            <telerik:GridTemplateColumn  UniqueName="SelectSource">
                                <ItemTemplate>
                                    <asp:Button ID="btnAssign" runat="server" Text="Assign" meta:resourcekey="gvServicesAssignResource5" /></ItemTemplate>
                                <HeaderTemplate>
                                    <asp:Button ID="btnAssignAll" runat="server" Text="Assign All" meta:resourcekey="gvServicesAssignAllResource5"
                                        OnClientClick="return MccAddServiceTypes_All()" /></HeaderTemplate>
                                <ItemStyle Width="120px" HorizontalAlign="Center"></ItemStyle>
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn HeaderText="Description" UniqueName="Description" SortExpression="Description"
                                meta:resourcekey="gdMCCMaintenancesDescriptionResource1" AllowFiltering="true"
                                DataField="Description">
                                <ItemStyle Width="200px" />
                                <HeaderStyle Width="200px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Operation Type" UniqueName="OperationType" SortExpression="OperationType"
                                meta:resourcekey="gdMCCMaintenancesOperationTypeResource1" AllowFiltering="true"
                                DataField="OperationType">
                            </telerik:GridBoundColumn>
                            <telerik:GridHyperLinkColumn HeaderText="Notification Type" UniqueName="NotificationType"
                                DataTextField="NotificationDescription" SortExpression="NotificationDescription"
                                meta:resourcekey="gdMCCMaintenancesNotificationTypeResource1" AllowFiltering="true"
                                NavigateUrl="#">
                            </telerik:GridHyperLinkColumn>
                            <telerik:GridBoundColumn HeaderText="Frequency" UniqueName="FrequencyName" SortExpression="FrequencyName"
                                meta:resourcekey="gdMCCMaintenancesFrequencyIDResource1" AllowFiltering="true"
                                DataField="FrequencyName">
                                <ItemStyle Width="100px" />
                                <HeaderStyle Width="100px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Interval" UniqueName="Interval" SortExpression="IntervalDesc "
                                meta:resourcekey="gdMCCMaintenancesIntervalResource1" AllowFiltering="true" DataField="IntervalDesc ">
                                <ItemStyle Width="120px" />
                                <HeaderStyle Width="120px" />
                            </telerik:GridBoundColumn>
                        </Columns>
                        <ItemStyle VerticalAlign="Top" />
                        <AlternatingItemStyle VerticalAlign="Top" />
                        <CommandItemTemplate>
                            <table id="tblCustomerCommand" runat="server" width="100%">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblUnassignedMaintenances" CssClass="formtext" runat="server" Text="Unassigned Maintenances"
                                            meta:resourcekey="lblUnassignedMaintenancesResource1"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </CommandItemTemplate>
                    </MasterTableView><ClientSettings>
                        <Selecting AllowRowSelect="false" />
                        <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="true" ScrollHeight="300px" />
                    </ClientSettings>
                    <HeaderStyle CssClass="RadGridtblHeader" />
                </Sentinel:Grid>
            </td>
        </tr>
        <tr>
            <td>
                <Sentinel:Grid ID="gvServicesDest" runat="server" AllowSorting="false" Width="690px"  
                    EnableTheming="false" VerticalGridNum="2" Height="300px" AllowPaging="false" AutoGenerateColumns="False"
                    meta:resourcekey="gvServicesDestResource1" ToolTip="Assigned Maintenances" 
                    OnNeedDataSource="gvServicesDest_NeedDataSource" 
                    onitemdatabound="gvServicesDest_ItemDataBound">
                    <MasterTableView DataKeyNames="MaintenanceId" ClientDataKeyNames="MaintenanceId">
                        <Columns>
                            <telerik:GridTemplateColumn  UniqueName="SelectSource">
                                <ItemTemplate>
                                    <asp:Button ID="btnunAssign" runat="server" Text="Unassign" meta:resourcekey="gvServicesunAssignResource5" /></ItemTemplate>
                                <HeaderTemplate>
                                    <asp:Button ID="btnunAssignAll" runat="server" Text="Unassign All" meta:resourcekey="gvServicesunAssignAllResource5"
                                        OnClientClick="return MccAddServiceTypes_All()" /></HeaderTemplate>
                                <ItemStyle Width="120px" HorizontalAlign="Center"></ItemStyle>
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn HeaderText="Description" UniqueName="Description" SortExpression="Description"
                                meta:resourcekey="gdMCCMaintenancesDescriptionResource1" AllowFiltering="true"
                                DataField="Description">
                                <ItemStyle Width="200px" />
                                <HeaderStyle Width="200px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Operation Type" UniqueName="OperationType" SortExpression="OperationType"
                                meta:resourcekey="gdMCCMaintenancesOperationTypeResource1" AllowFiltering="true"
                                DataField="OperationType">
                            </telerik:GridBoundColumn>
                            <telerik:GridHyperLinkColumn HeaderText="Notification Type" UniqueName="NotificationType"
                                DataTextField="NotificationDescription" SortExpression="NotificationDescription"
                                meta:resourcekey="gdMCCMaintenancesNotificationTypeResource1" AllowFiltering="true"
                                NavigateUrl="#">
                            </telerik:GridHyperLinkColumn>
                            <telerik:GridBoundColumn HeaderText="Frequency" UniqueName="FrequencyName" SortExpression="FrequencyName"
                                meta:resourcekey="gdMCCMaintenancesFrequencyIDResource1" AllowFiltering="true"
                                DataField="FrequencyName">
                                <ItemStyle Width="100px" />
                                <HeaderStyle Width="100px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Interval" UniqueName="Interval" SortExpression="IntervalDesc "
                                meta:resourcekey="gdMCCMaintenancesIntervalResource1" AllowFiltering="true" DataField="IntervalDesc ">
                                <ItemStyle Width="120px" />
                                <HeaderStyle Width="120px" />
                            </telerik:GridBoundColumn>
                        </Columns>
                        <ItemStyle VerticalAlign="Top" />
                        <AlternatingItemStyle VerticalAlign="Top"  />
                        <CommandItemTemplate>
                            <table id="tblCustomerCommand" runat="server" width="100%">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblassignedMaintenances" CssClass="formtext" runat="server" Text="Assigned Maintenances"
                                            meta:resourcekey="lblassignedMaintenancesResource1"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </CommandItemTemplate>
                    </MasterTableView><ClientSettings>
                        <Selecting AllowRowSelect="false" />
                        <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="true" ScrollHeight="300px" />
                    </ClientSettings>
                    <HeaderStyle CssClass="RadGridtblHeader" />
                </Sentinel:Grid>
            </td>
        </tr>
    </table>
</asp:Panel>
<telerik:RadAjaxLoadingPanel ID="LoadingPanel1_Grid" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
</telerik:RadAjaxLoadingPanel>
<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript">

        function MccAddServiceTypes_All() {
            var tableView = $find("<%= gvServicesSource.ClientID %>").get_masterTableView();
            var count = tableView.get_dataItems().length;
            var item;
            var ServiceTypeIDs = '';
            for (var i = 0; i < count; i++) {
                item = tableView.get_dataItems()[i];
                var key = item.getDataKeyValue("Id");
                if (ServiceTypeIDs == '') ServiceTypeIDs = key;
                else ServiceTypeIDs = ServiceTypeIDs + "," + key;
            }
            MccAddServiceTypes_Add(ServiceTypeIDs);
            return false;
        }

        function MccAddServiceTypes_Add(ServiceTypeIDs) {
            if (MCCidStr == '') return;
            if (ServiceTypeIDs == '') {
                alert('<%= selectService %>');
                return false;
            }
            else {
                $find("<%= LoadingPanel1_Grid.ClientID %>").show("<%= pnlAddServiceTypes.ClientID %>");
                var postData = "{'MccId':" + MCCidStr + ", 'MaintenanceIds':'" + ServiceTypeIDs + "'}";
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmMaintenanceGroupAssign.aspx/MCCMaintenanceAssigment_Add",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d != '-1' && data.d != "0") {
                            MccAddServiceTypesBindGrid(MCCidStr, null);
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            alert("<%= errorSave%>");
                        }
                        $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= pnlAddServiceTypes.ClientID %>");
                    },
                    error: function (request, status, error) {
                        $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= pnlAddServiceTypes.ClientID %>");
                        alert("<%= errorSave%>");
                        //alert(request.responseText);
                        return false;
                    }

                });

            }

            return false;
        }

        function MccDeleteServiceTypes_All() {
            var tableView = $find("<%= gvServicesDest.ClientID %>").get_masterTableView();

            var count = tableView.get_dataItems().length;
            var item;
            var ServiceTypeIDs = '';
            for (var i = 0; i < count; i++) {
                item = tableView.get_dataItems()[i];
                var key = item.getDataKeyValue("Id");
                if (ServiceTypeIDs == '') ServiceTypeIDs = key;
                else ServiceTypeIDs = ServiceTypeIDs + "," + key;
            }
            MccAddServiceTypes_Delete(ServiceTypeIDs);
            return false;
        }

        function MccAddServiceTypes_Delete(ServiceTypeIDs) {
            if (MCCidStr == '') return;
            if (ServiceTypeIDs == '') {
                alert('<%= selectService %>');
                return false;
            }
            else {
                $find("<%= LoadingPanel1_Grid.ClientID %>").show("<%= pnlAddServiceTypes.ClientID %>");
                var postData = "{'MccId':" + MCCidStr + ", 'MaintenanceIds':'" + ServiceTypeIDs + "'}";
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmMaintenanceGroupAssign.aspx/MCCMaintenanceAssigment_Delete",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d != '-1' && data.d != "0") {
                            MccAddServiceTypesBindGrid(MCCidStr, null)
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            alert("<%= errorDelete%>");
                        }
                        $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= pnlAddServiceTypes.ClientID %>");
                    },
                    error: function (request, status, error) {
                        $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= pnlAddServiceTypes.ClientID %>");
                        alert("<%= errorDelete%>");
                        //alert(request.responseText);
                        return false;
                    }

                });

            }

            return false;
        }
    </script>
</telerik:RadCodeBlock>
