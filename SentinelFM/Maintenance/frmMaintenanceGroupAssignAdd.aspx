<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.master" AutoEventWireup="true" CodeFile="frmMaintenanceGroupAssignAdd.aspx.cs" Inherits="SentinelFM.Maintenance_frmMaintenanceGroupAssignAdd" Theme="TelerikControl" meta:resourcekey="PageResource1"  %>
<%@ MasterType VirtualPath="~/MasterPage/MasterPage.master" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<%@ Register src="ListMessageAssign.ascx" tagname="ListMessage" tagprefix="uc3" %>
<%@ Register src="ListMessageAssign_Dt.ascx" tagname="ListMessage_Dt" tagprefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1" >
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" 
        meta:resourcekey="RadAjaxManager1Resource1" 
        OnAjaxRequest="RadAjaxManager1_AjaxRequest">
        <AjaxSettings>

           <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAddServiceTypes" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        
           <telerik:AjaxSetting AjaxControlID="gvServicesSource">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAddServiceTypes" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

           <telerik:AjaxSetting AjaxControlID="gvServicesDest">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAddServiceTypes" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
  <asp:Panel ID="pnlAddServiceTypes" runat="server" 
        meta:resourcekey="pnlAddServiceTypesResource1">
  <table >
        <tr>
            <td>
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>
                            <asp:Label ID="lblMccName" runat="server" Font-Bold="True" Text="Group Name:" meta:resourcekey="lblMccNameResource"
                                CssClass="formtext"></asp:Label>
                            <asp:Label ID="lblMccNameText" runat="server" CssClass="formtext" 
                                meta:resourceKey="lblMccNameTextResource1"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Button ID="btnClose" runat="server" CssClass="combutton" Width="80px" Text="Close" 
                                meta:resourcekey="btnCloseResource1" onclick="btnClose_Click"  />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <Sentinel:Grid ID="gvServicesSource" runat="server" AllowSorting="True"  VerticalGridNum = "2"
                    Width="100%"  IsAutoResize="True" PageSize="100" AllowPaging = "True"
                    EnableTheming="True"  AutoGenerateColumns="False"
                    meta:resourcekey="gvServicesSourceResource1" 
                    ToolTip="Unassigned Services" AllowFilteringByColumn="True"
                    OnNeedDataSource="gvServicesSource_NeedDataSource" 
                    onitemdatabound="gvServicesSource_ItemDataBound" 
                     onitemcommand="gvServicesSource_ItemCommand" allText="All" 
                    ClearAllFiltersText="Clear All Filters" ExportText="Export" GridLines="None" 
                    IsShowExportIcon="True" IsShowFilterIcon="True">
                    <ClientSettings>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True"  />
                    </ClientSettings>
                    <MasterTableView ClientDataKeyNames="MaintenanceId" CommandItemDisplay="Top" 
                        DataKeyNames="MaintenanceId">
                        <CommandItemSettings ExportToPdfText="Export to Pdf" />
                        <RowIndicatorColumn>
                            <HeaderStyle Width="20px" />
                        </RowIndicatorColumn>
                        <ExpandCollapseColumn>
                            <HeaderStyle Width="20px" />
                        </ExpandCollapseColumn>
                        <Columns>
                            <telerik:GridTemplateColumn AllowFiltering="False" 
                                meta:resourceKey="GridTemplateColumnResource1" UniqueName="SelectSource">
                                <HeaderTemplate>
                                    <asp:Button ID="btnAssignAll" runat="server" CommandName="AssignAll" 
                                        CssClass="formtext" meta:resourceKey="gvServicesAssignAllResource5" 
                                        Text="Assign All" Width="100px" />
                                    <asp:Button ID="btnAssignAllTmp" runat="server" CommandName="AssignAll" 
                                        Height="0px" meta:resourceKey="btnAssignAllTmpResource1" style="display:none" 
                                        Width="0px" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Button ID="btnAssign" runat="server" CommandName="Assign" 
                                        CssClass="formtext" meta:resourceKey="gvServicesAssignResource5" Text="Assign" 
                                        Width="100px" />
                                    <asp:Button ID="btnAssignTmp" runat="server" CommandName="Assign" Height="0px" 
                                        meta:resourceKey="btnAssignTmpResource1" style="display:none" Width="0px" />
                                    <asp:HiddenField ID="hidOperationType" runat="server" 
                                        Value='<%# Eval("OperationTypeID") %>' />
                                    <asp:HiddenField ID="hidInterval" runat="server" 
                                        Value='<%# Eval("Interval") %>' />
                                    <asp:HiddenField ID="hidFixedDate" runat="server" 
                                        Value='<%# Eval("FixedDate") %>' />
                                    <asp:HiddenField ID="hidTimespanId" runat="server" 
                                        Value='<%# Eval("TimespanId") %>' />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" Width="120px" />
                                <ItemStyle HorizontalAlign="Center" Width="120px" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="Description" HeaderText="Description" 
                                meta:resourceKey="gdMCCMaintenancesDescriptionResource1" 
                                SortExpression="Description" UniqueName="Description">
                                <HeaderStyle Width="200px" />
                                <ItemStyle Width="200px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="OperationType" HeaderText="Operation Type" 
                                meta:resourceKey="gdMCCMaintenancesOperationTypeResource1" 
                                SortExpression="OperationType" UniqueName="OperationType">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="NotificationDescription" 
                                HeaderText="Notification Type" 
                                meta:resourceKey="gdMCCMaintenancesNotificationTypeResource1" 
                                SortExpression="NotificationDescription" UniqueName="NotificationType">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="FrequencyName" HeaderText="Frequency" 
                                meta:resourceKey="gdMCCMaintenancesFrequencyIDResource1" 
                                SortExpression="FrequencyName" UniqueName="FrequencyName">
                                <HeaderStyle Width="100px" />
                                <ItemStyle Width="100px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="IntervalDesc " HeaderText="Interval" 
                                meta:resourceKey="gdMCCMaintenancesIntervalResource1" 
                                SortExpression="IntervalDesc " UniqueName="Interval">
                                <HeaderStyle Width="120px" />
                                <ItemStyle Width="120px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="FixedServiceDate" HeaderText="Fixed Date" 
                                meta:resourceKey="gdMCCMaintenancesFixedFixedServiceDateResource1" 
                                SortExpression="FixedServiceDate" UniqueName="FixedDate">
                            </telerik:GridBoundColumn>
                        </Columns>
                        <ItemStyle VerticalAlign="Top" />
                        <AlternatingItemStyle VerticalAlign="Top" />
                        <HeaderStyle CssClass="RadGridtblHeader" ForeColor="White" />
                        <CommandItemTemplate>
                            <table ID="tblCustomerCommand" runat="server" width="100%">
                                <tr runat="server">
                                    <td runat="server">
                                        <b>
                                        <asp:Label ID="lblUnassignedMaintenances" runat="server" CssClass="formtext" 
                                            meta:resourceKey="lblUnassignedMaintenancesResource1" 
                                            Text='<%$ Resources:lblUnassignedMaintenancesResource %>' ></asp:Label>
                                        </b>
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
        <tr>
            <td>
                <Sentinel:Grid ID="gvServicesDest" runat="server" AllowSorting="True" VerticalGridNum = "2"
                    Width="100%"  IsAutoResize="True" PageSize="100" AllowPaging = "True"
                    EnableTheming="True"  AutoGenerateColumns="False"
                    meta:resourcekey="gvServicesDestResource1" 
                    ToolTip="Assigned Services"  AllowFilteringByColumn="True"
                    OnNeedDataSource="gvServicesDest_NeedDataSource" 
                    onitemdatabound="gvServicesDest_ItemDataBound" 
                     onitemcommand="gvServicesDest_ItemCommand" allText="All" 
                    ClearAllFiltersText="Clear All Filters" ExportText="Export" GridLines="None" 
                    IsShowExportIcon="True" IsShowFilterIcon="True">
                    <ClientSettings>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True"  />
                    </ClientSettings>
                    
                    <MasterTableView ClientDataKeyNames="MaintenanceId" CommandItemDisplay="Top" 
                        DataKeyNames="MaintenanceId">
                        <CommandItemSettings ExportToPdfText="Export to Pdf" />
                        <RowIndicatorColumn>
                            <HeaderStyle Width="20px" />
                        </RowIndicatorColumn>
                        <ExpandCollapseColumn>
                            <HeaderStyle Width="20px" />
                        </ExpandCollapseColumn>
                        <Columns>
                            <telerik:GridTemplateColumn AllowFiltering="False" 
                                meta:resourceKey="GridTemplateColumnResource2" UniqueName="SelectSource">
                                <HeaderTemplate>
                                    <asp:Button ID="btnunAssignAll" runat="server" CommandName="UnAssignAll" 
                                        CssClass="formtext" meta:resourceKey="gvServicesunAssignAllResource5" 
                                        Text="Unassign All" Width="100px" />
                                    <asp:Button ID="btnunAssignAllTmp" runat="server" CommandName="UnAssignAll" 
                                        Height="0px" meta:resourceKey="btnunAssignAllTmpResource1" style="display:none" 
                                        Width="0px" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Button ID="btnunAssign" runat="server" CommandName="UnAssign" 
                                        CssClass="formtext" meta:resourceKey="gvServicesunAssignResource5" 
                                        Text="Unassign" Width="100px" />
                                    <asp:Button ID="btnunAssignTmp" runat="server" CommandName="UnAssign" 
                                        Height="0px" meta:resourceKey="btnunAssignTmpResource1" style="display:none" 
                                        Width="0px" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" Width="120px" />
                                <ItemStyle HorizontalAlign="Center" Width="120px" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="Description" HeaderText="Description" 
                                meta:resourceKey="gdMCCMaintenancesDescriptionResource1" 
                                SortExpression="Description" UniqueName="Description">
                                <HeaderStyle Width="200px" />
                                <ItemStyle Width="200px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="OperationType" HeaderText="Operation Type" 
                                meta:resourceKey="gdMCCMaintenancesOperationTypeResource1" 
                                SortExpression="OperationType" UniqueName="OperationType">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="NotificationDescription" 
                                HeaderText="Notification Type" 
                                meta:resourceKey="gdMCCMaintenancesNotificationTypeResource1" 
                                SortExpression="NotificationDescription" UniqueName="NotificationType">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="FrequencyName" HeaderText="Frequency" 
                                meta:resourceKey="gdMCCMaintenancesFrequencyIDResource1" 
                                SortExpression="FrequencyName" UniqueName="FrequencyName">
                                <HeaderStyle Width="100px" />
                                <ItemStyle Width="100px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="IntervalDesc " HeaderText="Interval" 
                                meta:resourceKey="gdMCCMaintenancesIntervalResource1" 
                                SortExpression="IntervalDesc " UniqueName="Interval">
                                <HeaderStyle Width="120px" />
                                <ItemStyle Width="120px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="FixedServiceDate" HeaderText="Fixed Date" 
                                meta:resourceKey="gdMCCMaintenancesFixedFixedServiceDateResource1" 
                                SortExpression="FixedServiceDate" UniqueName="FixedDate">
                            </telerik:GridBoundColumn>
                        </Columns>
                        <ItemStyle VerticalAlign="Top" />
                        <AlternatingItemStyle VerticalAlign="Top" />
                        <HeaderStyle CssClass="RadGridtblHeader" ForeColor="White" />
                        <CommandItemTemplate>
                            <table ID="tblCustomerCommand" runat="server" width="100%">
                                <tr runat="server">
                                    <td runat="server">
                                        <b>
                                        <asp:Label ID="lblassignedMaintenances" runat="server" CssClass="formtext" 
                                            meta:resourceKey="lblassignedMaintenancesResource1" Text="Assigned Services"></asp:Label>
                                        </b>
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
    <asp:HiddenField id="hidTimeBaseMaintenance" runat ="server" />
  </asp:Panel>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" 
        EnableShadow="True" ShowContentDuringLoad ="False" 
        ShowOnTopWhenMaximized="False" Behavior="Default" InitialBehavior="None" 
        meta:resourcekey="RadWindowManager1Resource1">
        <Windows>
            <telerik:RadWindow ID="RadWindowContentTemplate" Title="Confirm" VisibleStatusbar="false" ShowContentDuringLoad ="false" 
                VisibleTitlebar="false" Width="380px" Height="450px" runat="server" Skin="Hay" 
                Modal="true" meta:resourcekey="RadWindowContentTemplateResource1">
                <ContentTemplate>
                    <uc3:ListMessage ID="ListMessage" runat="server" />
                </ContentTemplate>
            </telerik:RadWindow>

            <telerik:RadWindow ID="RadWindowContentTemplate_Dt" Title="Confirm" VisibleStatusbar="false" ShowContentDuringLoad ="false" 
                VisibleTitlebar="false" Width="380px" Height="450px" runat="server" Skin="Hay" 
                Modal="true" meta:resourcekey="RadWindowContentTemplate2Resource1">
                <ContentTemplate>
                    <uc4:ListMessage_Dt ID="ListMessage_Dt" runat="server" />
                </ContentTemplate>
            </telerik:RadWindow>

        </Windows>
        </telerik:RadWindowManager>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript" >
       
        function AssignAllServices(ctl) {
            var masterTable = $find("<%=gvServicesSource.ClientID%>").get_masterTableView();
            var count = masterTable.get_dataItems().length;
            var item;
            var maintenances = '';
            for (var i = 0; i < count; i++) {
                item = masterTable.get_dataItems()[i];
                var key = item.getDataKeyValue("MaintenanceId");
                if (maintenances == '') maintenances = key;
                else maintenances = maintenances + "," + key;
            }

            try {
                $find("<%= LoadingPanel1.ClientID %>").show("<%= pnlAddServiceTypes.ClientID %>");
                var postData = "{'MccId':" + <%= mccID%> + ", MaintenanceIds:'" + maintenances + "'}";
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmMaintenanceGroupAssignAdd.aspx/GetunAssignedVehicles",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnlAddServiceTypes.ClientID %>");
                        if (data.d != '-1' && data.d != "0") {
                            if (data.d == '') $telerik.$("#" + ctl).click();
                            else {
                               var dat = eval(data.d);
                               var vehicles = null;
                               if (dat != '') vehicles = eval(dat);
                               if (vehicles == null ) $telerik.$("#" + ctl).click();
                               else
                               {
                                  var isTimeBase = $telerik.$("#<%= hidTimeBaseMaintenance.ClientID %>").val();
                                  if (isTimeBase != '1')  OpenConfirm(vehicles, false, ctl, "-1");
                                  else
                                  {
                                      ListMessage_SetGroupsVehicle_Dt(vehicles, false, ctl);
                                      $find('<%= RadWindowContentTemplate_Dt.ClientID%>').show();
                                      $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("-1");
                                  }
                               }
                            }
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            alert("<%= errorLoad%>");
                        }
                    },
                    error: function (request, status, error) {
                        $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnlAddServiceTypes.ClientID %>");
                        alert("<%= errorLoad%>");
                        //alert(request.responseText);
                        return false;
                    }

                });
            }
            catch (ex) { }
            return false;           
        }

        function AssignServices(id, ctl, timeBase) {
            try {
                $find("<%= LoadingPanel1.ClientID %>").show("<%= pnlAddServiceTypes.ClientID %>");
                var postData = "{'MccId':" + <%= mccID%> + ", MaintenanceIds:'" + id + "'}";
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmMaintenanceGroupAssignAdd.aspx/GetunAssignedVehicles",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnlAddServiceTypes.ClientID %>");
                        if (data.d != '-1' && data.d != "0") {
                            if (data.d == '') $telerik.$("#" + ctl).click();
                            else {
                               var dat = eval(data.d);
                               var vehicles = null;
                               if (dat != '') vehicles = eval(dat);
                               if (vehicles == null ) $telerik.$("#" + ctl).click();
                               else 
                               {
                                   if (timeBase != '1') OpenConfirm(vehicles, false, ctl, id);
                                   else 
                                   {
                                      ListMessage_SetGroupsVehicle_Dt(vehicles, false, ctl);
                                      $find('<%= RadWindowContentTemplate_Dt.ClientID%>').show();
                                      $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest(id);
                                   }
                               }
                            }
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            alert("<%= errorLoad%>");
                        }
                    },
                    error: function (request, status, error) {
                        $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnlAddServiceTypes.ClientID %>");
                        alert("<%= errorLoad%>");
                        //alert(request.responseText);
                        return false;
                    }

                });
            }
            catch (ex) { }
            return false;       
        }

        function UnAssignAllServices(ctl) {
            var masterTable = $find("<%=gvServicesDest.ClientID%>").get_masterTableView();
            var count = masterTable.get_dataItems().length;
            var item;
            var maintenances = '';
            for (var i = 0; i < count; i++) {
                item = masterTable.get_dataItems()[i];
                var key = item.getDataKeyValue("MaintenanceId");
                if (maintenances == '') maintenances = key;
                else maintenances = maintenances + "," + key;
            }

            try {
                $find("<%= LoadingPanel1.ClientID %>").show("<%= pnlAddServiceTypes.ClientID %>");
                var postData = "{'MccId':" + <%= mccID%> + ", MaintenanceIds:'" + maintenances + "'}";
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmMaintenanceGroupAssignAdd.aspx/GetAssignedVehicles",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnlAddServiceTypes.ClientID %>");
                        if (data.d != '-1' && data.d != "0") {
                            if (data.d == '') $telerik.$("#" + ctl).click();
                            else {
                               var dat = eval(data.d);
                               var vehicles = null;
                               if (dat != '') vehicles = eval(dat);
                               if (vehicles == null ) $telerik.$("#" + ctl).click();
                               else OpenConfirm(vehicles, true, ctl, null);
                            }
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            alert("<%= errorLoad%>");
                        }
                    },
                    error: function (request, status, error) {
                        $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnlAddServiceTypes.ClientID %>");
                        alert("<%= errorLoad%>");
                        //alert(request.responseText);
                        return false;
                    }

                });
            }
            catch (ex) { }
            return false;      
        }

        function UnAssignServices(id, ctl) {
            try {
                $find("<%= LoadingPanel1.ClientID %>").show("<%= pnlAddServiceTypes.ClientID %>");
                var postData = "{'MccId':" + <%= mccID%> + ", MaintenanceIds:'" + id + "'}";
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmMaintenanceGroupAssignAdd.aspx/GetAssignedVehicles",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnlAddServiceTypes.ClientID %>");
                        if (data.d != '-1' && data.d != "0") {
                            if (data.d == '') $telerik.$("#" + ctl).click();
                            else {
                               var dat = eval(data.d);
                               var vehicles = null;
                               if (dat != '') vehicles = eval(dat);
                               if (vehicles == null ) $telerik.$("#" + ctl).click();
                               else OpenConfirm(vehicles, true, ctl, null);
                            }
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            alert("<%= errorLoad%>");
                        }
                    },
                    error: function (request, status, error) {
                        $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnlAddServiceTypes.ClientID %>");
                        alert("<%= errorLoad%>");
                        //alert(request.responseText);
                        return false;
                    }

                });
            }
            catch (ex) { }
            return false;      
        }

        function CLoseConfirm() {
            $find('<%= RadWindowContentTemplate.ClientID%>').close();
            return false;
        }

        function CLoseConfirm_Dt() {
            $find('<%= RadWindowContentTemplate_Dt.ClientID%>').close();
            return false;
        }

        function OpenConfirm(vehicles, isDelete, ctl, maintenances) {
            //ListMessage_Clean();
            ListMessage_SetGroupsVehicle(vehicles, isDelete, ctl);
            $find('<%= RadWindowContentTemplate.ClientID%>').show();
        }

    </script>
    </telerik:RadCodeBlock>
</asp:Content>

