<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.master" AutoEventWireup="true" CodeFile="frmMaintenances.aspx.cs" Inherits="SentinelFM.Maintenance_frmMaintenances" Theme="TelerikControl" meta:resourcekey="PageResource1" %>
<%@ MasterType VirtualPath="~/MasterPage/MasterPage.master" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<%@ Register src="ListMessage.ascx" tagname="ListMessage" tagprefix="uc3" %>
<asp:Content ID="Content3" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" 
        meta:resourcekey="RadAjaxManager1Resource1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gdMCCMaintenances" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="gdMCCMaintenances">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gdMCCMaintenances" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div style="padding-top:5px" >

      <Sentinel:Grid AutoGenerateColumns="False" ID="gdMCCMaintenances" 
            IsAutoResize="True"  ExportText="Export" IsShowExportIcon="True" AllowSorting="True"
                                                        AllowPaging="True" 
            runat="server" Skin="Hay" GridLines="None" Width="960px" OnNeedDataSource="gdMCCMaintenances_NeedDataSource"
                                                        
            OnDeleteCommand="gdMCCMaintenances_DeleteCommand" OnItemCommand="gdMCCMaintenances_ItemCommand"
                                                         AllowFilteringByColumn="True" 
            FilterItemStyle-HorizontalAlign="Left" PageSize="20" 
                                                        
            OnItemDataBound="gdMCCMaintenances_ItemDataBound" allText="All" 
            ClearAllFiltersText="Clear All Filters" IsShowFilterIcon="True" 
            meta:resourcekey="gdMCCMaintenancesResource1">
                                                        <GroupingSettings CaseSensitive="false" />
                                                        <MasterTableView DataKeyNames="MaintenanceId" ClientDataKeyNames="MaintenanceId"
                                                            CommandItemDisplay="Top">
<CommandItemSettings ExportToPdfText="Export to Pdf"></CommandItemSettings>

<RowIndicatorColumn>
<HeaderStyle Width="20px"></HeaderStyle>
</RowIndicatorColumn>

<ExpandCollapseColumn>
<HeaderStyle Width="20px"></HeaderStyle>
</ExpandCollapseColumn>
                                                            <Columns>
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
                                                                <telerik:GridBoundColumn HeaderText="Notification Type" UniqueName="NotificationType"
                                                                    DataField ="NotificationDescription" SortExpression="NotificationDescription"
                                                                    meta:resourcekey="gdMCCMaintenancesNotificationTypeResource1" AllowFiltering="true"
                                                                    >
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn HeaderText="Frequency" UniqueName="FrequencyName" SortExpression="FrequencyName"
                                                                    meta:resourcekey="gdMCCMaintenancesFrequencyIDResource1" AllowFiltering="true"
                                                                    DataField="FrequencyName" >
                                                                    <ItemStyle Width="100px" />
                                                                    <HeaderStyle Width="100px" />
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn HeaderText="Interval" UniqueName="Interval" SortExpression="IntervalDesc" DataType="System.Int32" 
                                                                    meta:resourcekey="gdMCCMaintenancesIntervalResource1" AllowFiltering="true" DataField="IntervalDesc"
                                                                    >
                                                                    <ItemStyle Width="120px" />
                                                                    <HeaderStyle Width="120px" />
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridCheckBoxColumn HeaderText="Fixed Interval" UniqueName="FixedInterval" SortExpression="FixedInterval" DataField="FixedInterval" meta:resourcekey="gdMCCMaintenancesFixedIntervalResource1" ></telerik:GridCheckBoxColumn>
                                                                <telerik:GridBoundColumn HeaderText="Fixed Date" UniqueName="FixedDate" SortExpression="FixedServiceDate_1" DataField="FixedServiceDate_1" meta:resourcekey="gdMCCMaintenancesFixedFixedServiceDateResource1" ></telerik:GridBoundColumn>

                                                                <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn"
                                                                    meta:resourcekey="GridMccEditCommandColumnResource1" EditImageUrl="../images/edit.gif">
                                                                    <ItemStyle HorizontalAlign="Center" Width="30px" />
                                                                    <HeaderStyle HorizontalAlign="Center" Width="30px" />

                                                                </telerik:GridEditCommandColumn>
                                                                <telerik:GridButtonColumn ConfirmText="Delete this MCC maintenance?" ConfirmDialogType="Classic"
                                                                    ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                                                                    UniqueName="DeleteColumn" meta:resourcekey="GridMccMaintenanceButtonColumnDeleteResource1"
                                                                    ImageUrl="../images/delete.gif">
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                    <ItemStyle HorizontalAlign="Center" Width="30px" />
                                                                    <HeaderStyle HorizontalAlign="Center" Width="30px" />

                                                                </telerik:GridButtonColumn>
                                                                <telerik:GridTemplateColumn AllowFiltering = "false" 
                                                                    meta:resourcekey="GridTemplateColumnResource1"  >
                                                                    <ItemTemplate >
                                                                       <asp:Button id="btnDelete" Width="0px" Height="0px"  runat="server" 
                                                                            style="display:none; border:0px;" CommandName="Delete" 
                                                                            meta:resourcekey="btnDeleteResource1"  />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="0px" />
                                                                    <HeaderStyle Width="0px" />
                                                                </telerik:GridTemplateColumn>
                                                            </Columns>
                                                            <EditFormSettings ColumnNumber="2" UserControlName="AddMCCMaintenance.ascx" EditFormType="WebUserControl">
                                                                <FormTableItemStyle HorizontalAlign="Left" VerticalAlign="Top"></FormTableItemStyle>
                                                                <FormTableStyle CellSpacing="0" CellPadding="2" Font-Bold="true" HorizontalAlign="Left" />
                                                                <FormTableAlternatingItemStyle Wrap="False" HorizontalAlign="Left" VerticalAlign="Top">
                                                                </FormTableAlternatingItemStyle>
                                                                <EditColumn ButtonType="PushButton" InsertText="Save" UpdateText="Save" >
                                                                </EditColumn>
                                                                <FormMainTableStyle />
                                                            </EditFormSettings>
                                                            <HeaderStyle CssClass="RadGridtblHeader" />
                                                            <CommandItemTemplate>
                                                                <table id="tblCustomerCommand" runat="server" width="100%">
                                                                    <tr runat="server">
                                                                        <td align="left" runat="server">
                                                                            <asp:Button ID="btnAdd" Text= '<%$ Resources:btnAddNewMCCMaintenanceResource1 %>'   CommandName="InitInsert" runat="server"
                                                                                CssClass="combutton" Width="200px" />
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
                                                </div>

        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="True" 
        ShowOnTopWhenMaximized="False" Behavior="Default" InitialBehavior="None" 
        meta:resourcekey="RadWindowManager1Resource1">
        <Windows>
            <telerik:RadWindow ID="RadWindowContentTemplate" Title="Confirm" VisibleStatusbar="false" ShowContentDuringLoad ="true"
                VisibleTitlebar="false" Width="400px" Height="550px" runat="server" Skin="Hay" 
                Modal="true" meta:resourcekey="RadWindowContentTemplateResource1">
                <ContentTemplate>
                    <uc3:ListMessage ID="ListMessage" runat="server" />
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
        </telerik:RadWindowManager>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript" >
        function CLoseConfirm() {
            $find('<%= RadWindowContentTemplate.ClientID%>').close();
            return false;
        }

        function ClickEditEvent(id, ctl, validationGroup) {
            if (validationGroup != null && validationGroup != '' && !Page_ClientValidate(validationGroup)) {
                Page_BlockSubmit = false;
                return false; //not valid return false
            }
            try {
                $find("<%= LoadingPanel1.ClientID %>").show("<%= gdMCCMaintenances.ClientID %>");
                var postData = "{'MaintenanceId':'" + id + "'}";
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmMaintenances.aspx/GetVehiclesandGroup",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdMCCMaintenances.ClientID %>");
                        if (data.d != '-1' && data.d != "0") {
                            if (data.d == '') $telerik.$("#" + ctl).click();
                            else {
                                var dat = eval(data.d);
                                var groups = null;
                                var vehicles = null;
                                if (dat[0] != '') vehicles = eval(dat[0]);
                                if (dat[1] != '') groups = eval(dat[1]);
                                if (vehicles == null && groups == null) $telerik.$("#" + ctl).click();
                                else OpenConfirm(groups, vehicles, false, ctl);
                            }
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            alert("<%= Error_Load%>");
                        }
                    },
                    error: function (request, status, error) {
                        $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdMCCMaintenances.ClientID %>");
                        alert("<%= Error_Load%>");
                        //alert(request.responseText);
                        return false;
                    }

                });
            }
            catch (ex) { }
            return false;
        }

        function OpenConfirm(groups, vehicles, isDelete, ctl) {
            $find('<%= RadWindowContentTemplate.ClientID%>').show();
            ListMessage_SetGroupsVehicle(groups, vehicles, isDelete, ctl);
            var minusHeight = 0;
            if (groups == null) minusHeight = minusHeight + 90;
            if (vehicles == null) minusHeight = minusHeight + 330;
            $find('<%= RadWindowContentTemplate.ClientID%>').SetHeight(550 - minusHeight);
            $find('<%= RadWindowContentTemplate.ClientID%>').center();
        }

        function ClickDeleteEvent(id, ctl) {
            if (confirm('<%= deleteMaintenance%>')) {
                try {
                    $find("<%= LoadingPanel1.ClientID %>").show("<%= gdMCCMaintenances.ClientID %>");
                    var postData = "{'MaintenanceId':" + id + "}";
                    $telerik.$.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: "frmMaintenances.aspx/GetVehiclesandGroup",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdMCCMaintenances.ClientID %>");
                            if (data.d != '-1' && data.d != "0") {
                                if (data.d == '') $telerik.$("#" + ctl).click();
                                else {
                                    var dat = eval(data.d);
                                    var groups = null;
                                    var vehicles = null;
                                    if (dat[0] != '') vehicles = eval(dat[0]);
                                    if (dat[1] != '') groups = eval(dat[1]);
                                    if (vehicles == null && groups == null) $telerik.$("#" + ctl).click();
                                    else OpenConfirm(groups, vehicles, true, ctl);
                                }
                            }

                            if (data.d == '-1') {
                                top.document.all('TopFrame').cols = '0,*';
                                window.open('../Login.aspx', '_top')
                            }
                            if (data.d == '0') {
                                alert("<%= Error_Load%>");
                            }
                        },
                        error: function (request, status, error) {
                            $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdMCCMaintenances.ClientID %>");
                            alert("<%= Error_Load%>");
                            //alert(request.responseText);
                            return false;
                        }

                    });
                }
                catch (ex) { }
            }
            return false;
        }

    </script>
    </telerik:RadCodeBlock>
                                              
</asp:Content>


