<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.master" AutoEventWireup="true"
    CodeFile="frmMaintenanceGrid.aspx.cs" Inherits="SentinelFM.Maintenance_frmMaintenanceGrid"  
    Theme="TelerikControl" meta:resourcekey="PageResource1" %>

<%@ MasterType VirtualPath="~/MasterPage/MasterPage.master" %>
<%@ Register Src="../UserControl/FleetVehicleOrganizationHierarchy.ascx" TagName="FleetVehicle" TagPrefix="uc1" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<%@ Register src="../UserControl/ShowInfoMessage.ascx" tagname="ShowInfoMessage" tagprefix="uc2" %>
<%@ Register src="CommentEdit.ascx" tagname="CommentEdit" tagprefix="uc3" %>
<%@ Register src="CurrentValue.ascx" tagname="CurrentValue" tagprefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>

    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" 
    meta:resourcekey="RadAjaxManager1Resource1"  
    OnAjaxRequest="RadAjaxManager1_AjaxRequest">
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
                AllowFilteringByColumn="True" ExportText='<%$ Resources:dgMaintenance_Export %>' 
                AllowPaging="True"  PageSize="20"  Width="100%" Height="100%" 
                onneeddatasource="dgMaintenance_NeedDataSource" IsAutoResize="True" 
                   Skin="Simple" onitemdatabound="dgMaintenance_ItemDataBound" allText="All" 
                   ClearAllFiltersText="Clear All Filters" GridLines="None" 
                   IsShowExportIcon="True" IsShowFilterIcon="True" meta:resourcekey="dgMaintenanceResource1"
                >
                <GroupingSettings CaseSensitive="False" />
                <ClientSettings>
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="False"  />
                    <Resizing AllowColumnResize="True" EnableRealTimeResize="True">
                    </Resizing>
                </ClientSettings>
                <mastertableview clientdatakeynames="VehicleId,MaintenanceID,MccId" 
                    commanditemdisplay="Top" datakeynames="VehicleId,MaintenanceID">
                    <commanditemsettings exporttopdftext="Export to Pdf" />
                    <rowindicatorcolumn>
                        <HeaderStyle Width="20px" />
                    </rowindicatorcolumn>
                    <expandcollapsecolumn>
                        <HeaderStyle Width="20px" />
                    </expandcollapsecolumn>
                    <Columns>
                        <telerik:GridTemplateColumn DataField="BoxId" HeaderText="Box ID" 
                            meta:resourcekey="GridTemplateColumnResource1" UniqueName="BoxId">
                            <ItemTemplate>
                                <asp:HyperLink ID="hplBoxId" runat="server" 
                                    meta:resourcekey="hplBoxIdResource1" Text='<%# Eval("BoxId") %>'></asp:HyperLink>
                            </ItemTemplate>
                            <HeaderStyle Width="60px" />
                            <ItemStyle Width="60px" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="VehicleDescription" HeaderText="Vehicle" 
                            meta:resourcekey="GridBoundColumnResource1" UniqueName="VehicleDescription">
                            <HeaderStyle Width="90px" />
                            <ItemStyle Width="90px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn HeaderText="OperationType" 
                            meta:resourcekey="GridTemplateColumnResource2" SortExpression="OperationTypeID" 
                            UniqueName="OperationType">
                            <ItemTemplate>
                                <asp:Label ID="lblOperationType" runat="server" 
                                    meta:resourcekey="lblOperationTypeResource1" 
                                    Text='<%# FindOperationType(Eval("OperationTypeID")) %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="100px" />
                            <ItemStyle Width="100px" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="Description" HeaderText="Service" 
                            meta:resourcekey="GridBoundColumnResource2" UniqueName="Description">
                            <HeaderStyle Width="90px" />
                            <ItemStyle Width="90px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Interval" HeaderText="Interval" 
                            meta:resourcekey="GridBoundColumnResource3" UniqueName="Interval">
                            <HeaderStyle Width="60px" />
                            <ItemStyle Width="60px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn HeaderText="Service Date" 
                            meta:resourcekey="GridTemplateColumnResource3" UniqueName="ServiceDate">
                            <ItemTemplate>
                                <telerik:RadDatePicker ID="calServiceDate" runat="server" Culture="en-US" 
                                    Enabled="False" MaxDate="3000-01-01" meta:resourcekey="calServiceDateResource1" 
                                    MinDate="1900-01-01" Skin="Hay" style="display: None" Width="100px">
                                    <calendar usecolumnheadersasselectors="False" userowheadersasselectors="False" 
                                        viewselectortext="x">
                                        <specialdays>
                                            <telerik:RadCalendarDay Date="" meta:resourcekey="RadCalendarDayResource1" 
                                                Repeatable="Today">
                                                <ItemStyle CssClass="rcToday" />
                                            </telerik:RadCalendarDay>
                                        </specialdays>
                                    </calendar>
                                    <dateinput dateformat="MM/dd/yyyy" displaydateformat="MM/dd/yyyy" 
                                        labelcssclass="">
                                        <clientevents onvaluechanging="SaveInitalInputCalValue_s" />
                                    </dateinput>
                                    <datepopupbutton cssclass="" hoverimageurl="" imageurl="" />
                                </telerik:RadDatePicker>
                            </ItemTemplate>
                            <HeaderStyle Width="100px" />
                            <ItemStyle Width="100px" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn DataField="CurrentVal" HeaderText="Service Odo\Hrs" 
                            meta:resourcekey="GridTemplateColumnResource4" UniqueName="CurrentVal">
                            <ItemTemplate>
                                <telerik:RadNumericTextBox ID="txtCurrentOdoHrs" runat="server" 
                                    CssClass="formtext" Culture="en-CA" ForeColor="Red" LabelCssClass="" 
                                    meta:resourcekey="txtCurrentOdoHrsResource1" Width="70px">
                                    <numberformat decimaldigits="0" />
                                    <clientevents onvaluechanging="SaveInitalInputValueService" />
                                </telerik:RadNumericTextBox>
                                <asp:Panel ID="pnlEmpty" runat="server" meta:resourcekey="pnlEmptyResource1">
                                    &nbsp;</asp:Panel>
                            </ItemTemplate>
                            <HeaderStyle Width="80px" />
                            <ItemStyle ForeColor="Red" HorizontalAlign="Left" Width="80px" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn HeaderText="PTO" 
                            meta:resourcekey="GridBoundColumnResource4" UniqueName="PTO" Visible="False">
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn DataField="ServicePerc" DataType="System.Double" 
                            HeaderText="Service%" meta:resourcekey="GridTemplateColumnResource5" 
                            SortExpression="ServicePerc" UniqueName="ServicePerc">
                            <ItemTemplate>
                                <asp:Label ID="lblServicePerc" runat="server" 
                                    meta:resourcekey="lblServicePercResource1" 
                                    Text='<%# RoundServicePerc(Eval("ServicePerc")) %>'></asp:Label>
                                &nbsp;
                            </ItemTemplate>
                            <HeaderStyle Width="70px" />
                            <ItemStyle HorizontalAlign="Left" Width="70px" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="DueValue" HeaderText="Due Value" 
                            meta:resourcekey="GridBoundColumnResource5" UniqueName="DueValue">
                            <HeaderStyle Width="80px" />
                            <ItemStyle HorizontalAlign="Left" Width="100px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Notification1" DataType="System.Int16" 
                            HeaderText="Notification 1" meta:resourcekey="GridBoundColumnResource6" 
                            UniqueName="Notification1" Visible="False">
                            <HeaderStyle Width="80px" />
                            <ItemStyle HorizontalAlign="Center" Width="80px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Notification2" DataType="System.Int16" 
                            HeaderText="Notification 2" meta:resourcekey="GridBoundColumnResource7" 
                            UniqueName="Notification2" Visible="False">
                            <HeaderStyle Width="80px" />
                            <ItemStyle HorizontalAlign="Center" Width="80px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Notification3" DataType="System.Int16" 
                            HeaderText="Notification 3" meta:resourcekey="GridBoundColumnResource8" 
                            UniqueName="Notification3" Visible="False">
                            <HeaderStyle Width="80px" />
                            <ItemStyle HorizontalAlign="Center" Width="80px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn HeaderText="Use Current Value" 
                            meta:resourcekey="GridTemplateColumnResource6" UniqueName="UseCurrentValue">
                            <ItemTemplate>
                                <nobr>
                                <asp:ImageButton ID="imgComments" runat="server" ImageUrl="~/images/Edit.gif" 
                                    meta:resourcekey="imgCommentsResource1" ToolTip="Edit Comment" />
                                <asp:ImageButton ID="lnkUseCurrentValue" runat="server" 
                                    ImageUrl="~/images/application_go.png" 
                                    meta:resourcekey="lnkUseCurrentValueResource1" ToolTip="Use Current Value" />
                                <asp:HiddenField ID="hidComments" runat="server" 
                                    Value='<%# Bind("Comments") %>' />
                                </nobr>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" Width="60px" />
                            <ItemStyle HorizontalAlign="Center" Width="60px" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Next Due Value" 
                            meta:resourcekey="GridTemplateColumnResource7" UniqueName="UseValue">
                            <ItemTemplate>
                                <telerik:RadNumericTextBox ID="txtValue" runat="server" CssClass="formtext" 
                                    Culture="en-CA" LabelCssClass="" meta:resourcekey="txtValueResource1" 
                                    Visible="False" Width="80px">
                                    <numberformat decimaldigits="0" />
                                    <clientevents onerror="onErrorInputValue" onvaluechanged="InputValueChanged" 
                                        onvaluechanging="SaveInitalInputValue" />
                                </telerik:RadNumericTextBox>
                                <telerik:RadDatePicker ID="calValue" runat="server" Culture="en-US" 
                                    MaxDate="3000-01-01" meta:resourcekey="calValueResource1" MinDate="1900-01-01" 
                                    Skin="Hay" Width="100px">
                                    <calendar usecolumnheadersasselectors="False" userowheadersasselectors="False" 
                                        viewselectortext="x">
                                        <specialdays>
                                            <telerik:RadCalendarDay Date="" meta:resourcekey="RadCalendarDayResource2" 
                                                Repeatable="Today">
                                                <ItemStyle CssClass="rcToday" />
                                            </telerik:RadCalendarDay>
                                        </specialdays>
                                    </calendar>
                                    <dateinput dateformat="MM/dd/yyyy" displaydateformat="MM/dd/yyyy" 
                                        labelcssclass="" onclientdatechanged="InputCalValueChanged">
                                        <clientevents onvaluechanged="InputCalValueChanged" 
                                            onvaluechanging="SaveInitalInputCalValue" />
                                    </dateinput>
                                    <datepopupbutton cssclass="" hoverimageurl="" imageurl="" />
                                </telerik:RadDatePicker>
                                <asp:HiddenField ID="hidOrgValue" runat="server" />
                            </ItemTemplate>
                            <HeaderStyle Width="100px" />
                            <ItemStyle Width="100px" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="False" HeaderText="Update" 
                            meta:resourcekey="GridTemplateColumnResource8" 
                            UniqueName="selectVehicleCheckBox">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkselectVehicle" runat="server" 
                                    meta:resourcekey="chkselectVehicleResource1" />
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" Width="50px" />
                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                        </telerik:GridTemplateColumn>
                    </Columns>
                    <commanditemtemplate>
                        <table ID="tblCustomerCommand" runat="server" width="100%">
                            <tr runat="server">
                                <td runat="server" align="left">
                                    <asp:Button ID="btnSave" runat="server" CssClass="combutton" 
                                        OnClientClick="return GetSelectedVehicle()" Text='<%$ Resources:btnSave %>'    />
                                    <asp:Button ID="btnCancel" runat="server" CssClass="combutton" 
                                        OnClientClick="return ClearAllCheckedVehicles()" Text='<%$ Resources:btnCancel %>'  />
                                </td>
                            </tr>
                        </table>
                    </commanditemtemplate>
                </mastertableview>
                <filteritemstyle horizontalalign="Left" />
                <filtermenu cssclass="FiltMenuCss" enabletheming="True">
                </filtermenu>
            </Sentinel:Grid>
            </td>
        </tr>
    </table>
    </asp:Panel> 
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="True" 
        ShowOnTopWhenMaximized="False" Behavior="Default" 
    InitialBehavior="None" meta:resourcekey="RadWindowManager1Resource1">
        <Windows>
            <telerik:RadWindow ID="RadWindowContentTemplate" Title="Modify" VisibleStatusbar="false" ShowContentDuringLoad ="true"
                VisibleTitlebar="false" Width="420px" Height="320px" runat="server" Skin="Hay" 
                Modal="true" meta:resourcekey="RadWindowContentTemplateResource1">
                <ContentTemplate>
                    <uc2:ShowInfoMessage ID="ShowInfoMessage1" runat="server" />
                </ContentTemplate>
            </telerik:RadWindow>
            <telerik:RadWindow ID="RadWindowContentComment" Title="Comment" VisibleStatusbar="false" ShowContentDuringLoad ="true"
                VisibleTitlebar="false" Width="490px" Height="220px" runat="server" Skin="Hay" 
                Modal="true" meta:resourcekey="RadWindowContentCommentResource1">
                <ContentTemplate>
                    <uc3:CommentEdit ID="CommentEdit" runat="server" />
                </ContentTemplate>
            </telerik:RadWindow>

            <telerik:RadWindow ID="RadWindowContentCurrent" Title="Current Value" VisibleStatusbar="false" ShowContentDuringLoad ="true"
                VisibleTitlebar="false" Width="350px" Height="150px" runat="server" Skin="Hay" 
                Modal="true" meta:resourcekey="RadWindowContentCurrentResource1">
                <ContentTemplate>
                    <uc4:CurrentValue ID="CurrentValue1" runat="server" />
                </ContentTemplate>
            </telerik:RadWindow>

        </Windows>
        </telerik:RadWindowManager>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript" >
        var isWorngInput = false;
        var vehicleID_c = "";
        var maintenanceID_c = "";
        var hidCommentID = "";
        function SaveInitalInputValue(sender, eventArgs) {
            if (eventArgs.get_newValue() > 900000) {
                alert('<%= ValueExceedAllowedValueError %>');
                eventArgs.set_cancel(true);
            }            
            else if (isWorngInput) {
                alert('<%= inputError %>');
                eventArgs.set_cancel(true);
            }
            isWorngInput = false;
        }

        function SaveInitalInputValueService(sender, eventArgs) {
            if (eventArgs.get_newValue() < 1 ) {
                eventArgs.set_cancel(true);
            }

            if (eventArgs.get_newValue() > sender.get_maxValue()) {
                alert('<%= inputError_s %>');
                eventArgs.set_cancel(true);
            }
        }

        function SaveInitalInputCalValue_s(sender, eventArgs) {
            if ($telerik.$.trim(eventArgs.get_newValue()) == '') {
                eventArgs.set_cancel(true);
                isWorngInput = false;
                return;
            }
            var newValue = eventArgs.get_newValue().split('/');
            newValue[0] = $telerik.$.trim(newValue[0]);
            newValue[1] = $telerik.$.trim(newValue[1]);
            newValue[2] = $telerik.$.trim(newValue[2]);
            if (newValue[0].length == 2 && newValue[0].substring(0, 1) == '0') newValue[0] = newValue[0].substring(1, 2);
            if (newValue[1].length == 2 && newValue[1].substring(0, 1) == '0') newValue[1] = newValue[1].substring(1, 2);
            if (newValue[2].length == 2 && newValue[2].substring(0, 1) == '0') newValue[2] = newValue[2].substring(1, 2);

            var newValueDate = new Date(parseInt(newValue[2]), parseInt(newValue[0]) - 1, parseInt(newValue[1]), 0, 0, 0, 0);

            if (newValueDate > sender.get_maxDate()) {
                alert('<%= inputCalError_s %>');
                eventArgs.set_cancel(true);
            }
            isWorngInput = false;
        }

        function SaveInitalInputCalValue(sender, eventArgs) {
            if ($telerik.$.trim(eventArgs.get_newValue()) == '') {
                eventArgs.set_cancel(true);
                isWorngInput = false;
                return;
            }
            try {
                var newValue = eventArgs.get_newValue().split('/');
                newValue[0] = $telerik.$.trim(newValue[0]);
                newValue[1] = $telerik.$.trim(newValue[1]);
                newValue[2] = $telerik.$.trim(newValue[2]);
                if (newValue[0].length == 2 && newValue[0].substring(0, 1) == '0') newValue[0] = newValue[0].substring(1, 2);
                if (newValue[1].length == 2 && newValue[1].substring(0, 1) == '0') newValue[1] = newValue[1].substring(1, 2);
                if (newValue[2].length == 2 && newValue[2].substring(0, 1) == '0') newValue[2] = newValue[2].substring(1, 2);

                var newValueDate = new Date(parseInt(newValue[2]), parseInt(newValue[0]) - 1, parseInt(newValue[1]), 0, 0, 0, 0);
                if (newValueDate < sender.get_minDate()) {
                    alert('<%= inputCalError %>');
                    eventArgs.set_cancel(true);
                }
            }
            catch (err) { }
            isWorngInput = false;
        }

        function ShowCurrentValueWin(hrs, meters) {
            $find('<%= RadWindowContentCurrent.ClientID%>').show();
            ShowCurrentOdoandEngHrs(hrs, meters);
        }

        function CloseCurrentValueWin() {
            $find('<%= RadWindowContentCurrent.ClientID%>').close();
            return false;

        }

        function ShowEditComment(rowIndex) {
            var tableView = $find("<%= dgMaintenance.ClientID %>").get_masterTableView();
            var rowItem = tableView.get_dataItems()[rowIndex];
            vehicleID_c = rowItem.getDataKeyValue("VehicleId");
            maintenanceID_c = rowItem.getDataKeyValue("MaintenanceID");
            SetCommentTextField(
               $telerik.$(rowItem.get_element()).find("[ID$='hidComments']").val());

            hidCommentID = $telerik.$(rowItem.get_element()).find("[ID$='hidComments']");
            $find('<%= RadWindowContentComment.ClientID%>').show();
            return false;
        }


        function SaveEditComment() {
        if (vehicleID_c != "" && maintenanceID_c != "") {
            $find("<%= LoadingPanel1.ClientID %>").show("<%= pnl.ClientID %>");
            var comment_c = GetCommentTextField();
            var c = "\\" + "'";
            var postData = "{'MaintenanceID':'" + maintenanceID_c +
                              "', 'VehicleId':'" + vehicleID_c +
                              "', 'Comments':'" + escape(comment_c) +
                            "'}";
            $telerik.$.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "frmMaintenanceGrid.aspx/UpdateComment",
                data: postData,
                dataType: "json",
                success: function (data) {
                    $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnl.ClientID %>");
                    if (data.d != '-1' && data.d != "0") {
                        hidCommentID.val(comment_c);
                        alert('<%= saveSucceed%>');
                        CloseEditComment()
                    }

                    if (data.d == '-1') {
                        top.document.all('TopFrame').cols = '0,*';
                        window.open('../Login.aspx', '_top')
                    }
                    if (data.d == '0') {
                        alert("<%= errorSave%>");
                    }
                },
                error: function (request, status, error) {
                    $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnl.ClientID %>");
                    alert("<%= errorSave%>");
                    //alert(request.responseText);
                    return false;
                }

            });
            }
            return false;
        }

        function CloseEditComment() {
            $find('<%= RadWindowContentComment.ClientID%>').close();
            return false;

        }

        function SetCalendarDate(ctlID, data, calServiceDate, intevalVal) {
            var calendar = $find(ctlID);
            //tobe changed
            if ($telerik.$('#' + calServiceDate).parent().css("display") == 'none') {
                var today = new Date();
                today = new Date(today.setTime(today.getTime() + (intevalVal * 24 * 60 * 60 * 1000)));

                calendar.set_selectedDate(today);
            }
            else {
                var calService_Date = $find(calServiceDate).get_selectedDate()
                calService_Date = new Date(calService_Date.setTime(calService_Date.getTime() + (intevalVal * 24 * 60 * 60 * 1000)));
                calendar.set_selectedDate(calService_Date);
            }
            return false;
       }

       function SetNumControlData(ctlID, data, currentCtl, intevalVal) {
           var dateInput = $find(ctlID);
           var currentCtl = $find(currentCtl);
           var data_1 = currentCtl.get_value() + intevalVal;
           if (data_1 != data) dateInput.set_value(data_1);
           else dateInput.set_value(data);
           return false;
       }

       function onErrorInputValue(sender, eventArgs) {
           isWorngInput = true;
           eventArgs.set_cancel(true);
       }

       function InputCalValueChanged(sender, eventArgs) {
           var orgValue = $telerik.$(sender.get_element()).parents("div[id$='calValue_wrapper']").next("input:[id$='hidOrgValue']").val();
           //$telerik.$(sender.get_element()).parents("div[id$='calValue_wrapper']").parent().find("span[id='span_star']").remove();
           $telerik.$(sender.get_element()).parents("div[id$='calValue_wrapper']").removeClass("MaintenanceGridRedFontStyle");
           if (eventArgs.get_newValue() != orgValue) {
               //$telerik.$(sender.get_element()).parents("div[id$='calValue_wrapper']").parent().append("<span id='span_star' style='color:red' >*</span>");
               $telerik.$(sender.get_element()).parents("div[id$='calValue_wrapper']").addClass("MaintenanceGridRedFontStyle");
           }
       }

       function InputValueChanged(sender, eventArgs) {
           var orgValue = $telerik.$(sender.get_element()).parents("span[id$='_wrapper']").next("input:[id$='hidOrgValue']").val();
           //$telerik.$(sender.get_element()).parents("span[id$='_wrapper']").parent().find("span[id='span_star']").remove();
           $telerik.$(sender.get_element()).parents("span[id$='_wrapper']").removeClass("MaintenanceGridRedFontStyle");
           if (eventArgs.get_newValue() != orgValue) {
               // $telerik.$(sender.get_element()).parents("span[id$='_wrapper']").parent().append("<span id='span_star' style='color:red' >*</span>");
               $telerik.$(sender.get_element()).parents("span[id$='_wrapper']").addClass("MaintenanceGridRedFontStyle");
           }
       }

       function ClearAllCheckedVehicles() {
           $telerik.$("input:checkbox").attr("checked", false); ;

           var tableView = $find("<%= dgMaintenance.ClientID %>").get_masterTableView();
           var count = tableView.get_dataItems().length;
           for (var i = 0; i < count; i++) {
               var item = tableView.get_dataItems()[i];
               var selectCell = tableView.getCellByColumnUniqueName(item, "ServiceDate")
               $telerik.$(selectCell).find("[id$='ServiceDate']").parent().css("display",'none');
           }
           return false;
       }

       function GetSelectedVehicle() {
           var selectedVehicle = new Array();
           var errorVehicleService = new Array();
           var tableView = $find("<%= dgMaintenance.ClientID %>").get_masterTableView();
           var count = tableView.get_dataItems().length;
           var index = 0;
           var index_Ser = 0;
           for (var i = 0; i < count; i++) {
               var item = tableView.get_dataItems()[i];
               var selectCell = tableView.getCellByColumnUniqueName(item, "selectVehicleCheckBox")
               var descriptionCell = tableView.getCellByColumnUniqueName(item, "Description")
               if ($telerik.$(selectCell).find("input:checkbox[id$='chkselectVehicle']").attr("checked") == true) {
                   var vehicleDescription = tableView.getCellByColumnUniqueName(item, "vehicleDescription");
                   selectedVehicle[index++] = $telerik.$(vehicleDescription).text() + " [" + $telerik.$(descriptionCell).text() + "]";
                   try {
                       var ServiceDateCtl = $telerik.$(tableView.getCellByColumnUniqueName(item, "ServiceDate")).find("[id$=_wrapper]:first");
                       var CurrentValCtl = $telerik.$(tableView.getCellByColumnUniqueName(item, "CurrentVal")).find("[id$=_wrapper]:first");
                       var UseValueCtl = $telerik.$(tableView.getCellByColumnUniqueName(item, "UseValue")).find("[id$=_wrapper]:first");
                       if (ServiceDateCtl.length == 1 && UseValueCtl.length == 1) {
                           var ServiceDateCtlID = ServiceDateCtl.attr("id");
                           var hasCurrentValCtl = false;
                           if (CurrentValCtl.length == 1) hasCurrentValCtl = true;
                           var CurrentValCtlID = '';
                           if (hasCurrentValCtl) CurrentValCtlID = CurrentValCtl.attr("id");
                           var UseValueCtlID = UseValueCtl.attr("id")
                           ServiceDateCtlID = ServiceDateCtlID.substring(0, ServiceDateCtlID.length - 8);
                           if (hasCurrentValCtl) CurrentValCtlID = CurrentValCtlID.substring(0, CurrentValCtlID.length - 8);
                           UseValueCtlID = UseValueCtlID.substring(0, UseValueCtlID.length - 8);
                           var tel_ServiceDateCtl = $find(ServiceDateCtlID);
                           var tel_CurrentValCtl = null;
                           if (hasCurrentValCtl) tel_CurrentValCtl = $find(CurrentValCtlID);
                           var tel_UseValueCtl = $find(UseValueCtlID);
                           if (UseValueCtlID.indexOf("txtValue") > 0 && hasCurrentValCtl) {
                               if (tel_UseValueCtl.get_value() <= tel_CurrentValCtl.get_value())
                                   errorVehicleService[index_Ser++] = $telerik.$(vehicleDescription).text() + " [" + $telerik.$(descriptionCell).text() + "]";
                           }
                           else {
                               if (tel_UseValueCtl.get_selectedDate() <= tel_ServiceDateCtl.get_selectedDate())
                                   errorVehicleService[index_Ser++] = $telerik.$(vehicleDescription).text() + " [" + $telerik.$(descriptionCell).text() + "]";
                           }

                       }
                   }
                   catch (err) { }
               }
           }

           if (errorVehicleService.length > 0) {
               ShowInfoMessage_SetValue('<%= updateError%>', errorVehicleService, null, ShowInfoMessage_noEvent, true, false, null, 'Close');
               $find('<%= RadWindowContentTemplate.ClientID%>').show();
               return false;
           }

           if (selectedVehicle.length == 0) {
               alert('<%= selectedError%>')
               return false;
           }

           var vtitle = '<%= closeVehicles1 %>';
           var radYesText = '<%= RadWindowContentYes %>';
           var radNoText = '<%= RadWindowContentNo %>';
           if (index > 1) vtitle = '<%= closeVehicles2 %>';
           vtitle = vtitle.replace('(n)', '(' + index + ')');
           
           ShowInfoMessage_SetValue(vtitle, selectedVehicle, ShowInfoMessage_yesEvent, ShowInfoMessage_noEvent, false, false, radYesText, radNoText) //Defined in ShowInfoMessage.ascx
           $find('<%= RadWindowContentTemplate.ClientID%>').show();

           return false;
       }
       
       function ShowInfoMessage_yesEvent() {
           $find('<%= RadWindowContentTemplate.ClientID%>').close();
           $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("SaveSelectedValues");
           return false
       }

       function ShowInfoMessage_noEvent() {
           $find('<%= RadWindowContentTemplate.ClientID%>').close();
           return false
       }

       function ShowErrorMessage(str) {
           var errorData = eval(str)
           ShowInfoMessage_SetValue('<%= closeError%>', errorData, null, ShowInfoMessage_noEvent, true, false, null, 'Close');
           $find('<%= RadWindowContentTemplate.ClientID%>').show();
       }

       function ShowErrorMessageWithTitle(str, title) {
           var errorData = eval(str)
           ShowInfoMessage_SetValue(title, errorData, null, ShowInfoMessage_noEvent, true, false, null, 'Close');
           $find('<%= RadWindowContentTemplate.ClientID%>').show();
       }

       function SetEnableCalendar(ctl, calendarCtl) {
           if ($telerik.$(ctl).attr("checked")) {
               $telerik.$('#' + calendarCtl).parent().show();
               $find(calendarCtl).set_enabled(true);
           }
           else
               $telerik.$('#' + calendarCtl).parent().hide();
       }
    </script>
    </telerik:RadCodeBlock>
</asp:Content>
