<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMaintenance.aspx.cs" Inherits="SentinelFM.Maintenance_frmMaintenance"
    Culture="en-US" UICulture="auto" EnableTheming="true" StylesheetTheme="Default"
    Theme="Default" EnableEventValidation="false" meta:resourcekey="PageResource1" %>

<%@ Register Assembly="ISNet.WebUI.WebGrid" Namespace="ISNet.WebUI.WebGrid" TagPrefix="ISWebGrid" %>
<%@ Register Assembly="ISNet.WebUI.WebInput" Namespace="ISNet.WebUI.WebControls"
    TagPrefix="ISWebInput" %>

<%@ Register TagName="FleetVehicles" TagPrefix="fvs" Src="~/UserControl/FleetVehiclesControl.ascx" %>
<%@ Register TagName="FleetVehicles1" TagPrefix="fvs1" Src="~/UserControl/FleetVehiclesControl.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Vehicle Maintenance</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
    <link href="MaintenanceStyleSheet.css" type="text/css" rel="stylesheet"></link>

    <script type="text/javascript" language="javascript">
        function EnableEndValue(status) {
            var txt = document.getElementById("TextEndValue");
            if (txt != null) {
                if (status) txt.value = "0";
                txt.readOnly = status;
            }
            else
                alert("End Value Object is NULL!");
        }

        function VehicleMaintenaceClose(VehicleId, ServiceId) {
            var mypage = 'frmMaintenaceClose.aspx?VehicleId=' + VehicleId + '&ServiceId=' + ServiceId;
            var myname = 'Sensors';
            var w = 330;
            var h = 370;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1';
            win = window.open(mypage, myname, winprops);
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }

        function InfoWindow(NotificationId) {
            var mypage = '../Dashboard/frmNotificationInfo.aspx?NotificationId=' + NotificationId;
            var myname = '';
            var w = 450;
            var h = 300;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,';
            win = window.open(mypage, myname, winprops);
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }
    </script>

    <style type="text/css">
        .style1
        {
            width: 87px;
        }
        .style2
        {
            width: 188px;
        }
        .RegularText
        {
            margin-left: 0px;
        }
        .style3
        {
            width: 47px;
        }
    </style>

</head>
<body>
    <form id="frmMaintenance" runat="server">
        <table id="tblCommands" style="z-index: 101; left: 8px; position: absolute; top: 4px" cellspacing="0" cellpadding="0" width="300" border="0">
            <tr>
                <td>
                    <table id="tblButtons" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td>
                                
                                
                                 <asp:Button ID="btnMaintenance" runat="server" CausesValidation="False" CssClass="confbutton"
                                Text="Maintenance" OnClick="btnMaintenance_Click" 
                                meta:resourcekey="btnMaintenanceResource1" Width="151px" />
                                
                             </td>
                            
                            <td style="width: 7px">
                                <asp:Button ID="btnAdministration" CssClass="confbutton" CausesValidation="False"
                                runat="server" Text="Notification Policy" OnClick="btnAdministration_Click" 
                                meta:resourcekey="btnAdministrationResource1" Width="151px" />
                            </td>
                            <td>
                                <asp:Button ID="btnServices" CssClass="confbutton" CausesValidation="False" runat="server"
                                Text="Services" OnClick="btnServices_Click" 
                                meta:resourcekey="btnServicesResource1" Width="151px"/>                      
                            </td>
                            <td style="width: 7px">
                                <asp:Button ID="btnMaintenanceHistory" CssClass="confbutton" CausesValidation="False"
                                runat="server" Text="Maintenance History" OnClick="btnMaintenanceHistory_Click"
                                meta:resourcekey="btnMaintenanceHistoryResource1" Width="151px" />
                            </td>
                            
                            <td>
                                <asp:Button ID="btnNotifications" runat="server" CausesValidation="False" 
                                CssClass="selectedbutton" Text="Notifications" onclick="btnNotifications_Click" Width="151px" />
                                
                            </td>
                            
                            <td style="width: 7px">
                               <asp:Button ID="btnDTCNotifications" CssClass="confbutton" CausesValidation="False"
                                runat="server" Text="DTC Notifications" Width="151px" onclick="btnDTCNotifications_Click" />
                            </td>                              
                            <td></td>
                            <td></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <table id="tblBody" cellspacing="0" cellpadding="0" width="679" align="center" border="0">
                        <tr>
                            <td>
                                <table id="tblForm" style="height:550px" width="990px" class="table"  border="0">
                                    <tr>
                                        <td class="configTabBackground" valign="top">
                                            <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                                               
                                                <asp:View ID="ViewMaintenance" runat="server">
                                                    <div style="padding: 20px 20px 20px 20px" class="formtext" >
                                                       <div runat="server" id="ServicesButtons" visible="False" style="padding: 5px 5px 5px 5px;float: right; position: relative; right: 0px; top: 0px; border: outset thin silver;width: 200px; height: 166px" >
                                                           <asp:CheckBox runat="server" ID="CheckOdometerBased" Checked="True" Text="Odometer Based" meta:resourcekey="CheckOdometerBasedResource1" />
                                                           <br />
                                                           <asp:CheckBox runat="server" ID="CheckEngineHoursBased" Checked="True" Text="Engine Hours Based" meta:resourcekey="CheckEngineHoursBasedResource1" /> 
                                                           <br />
                                                           <asp:CheckBox runat="server" ID="CheckTimeBased" meta:resourcekey="chkTimeBased1" Checked="True" Text="Time Based"/>
                                                           <ul style="list-style: none; line-height: 20px;">
                                                               <li>
                                                                   <asp:Button runat="server" ID="btnDue" Text="Get Due Services" CssClass="combutton" Width="150px" OnClick="btnDue_Click"  meta:resourcekey="btnDueResource1"/>
                                                               </li>
                                                               <li>
                                                                   <asp:Button runat="server" ID="btnPast" Text="Get Overdue Services" CssClass="combutton" Width="150px" OnClick="btnPast_Click" meta:resourcekey="btnPastResource1"/>
                                                               </li>
                                                               <li>
                                                                   <asp:Button runat="server" ID="btnAll" Text="Get All Services" CssClass="combutton" Width="150px" OnClick="btnAll_Click" meta:resourcekey="btnGetAllServicesResource1"/>
                                                               </li>
                                                               <li>
                                                                   <asp:Button runat="server" ID="btnAddNewPlan" Text="Add New Plan" CssClass="combutton" Width="150px" OnClick="btnAddNewPlan_Click" meta:resourcekey="btnAddNewPlanResource1"/>
                                                               </li>
                                                           </ul>
                                                    </div>
                                                    <fvs:FleetVehicles runat="server" ID="FleetVehicleSelectorMaintenance" VehicleAutoPostBack="true" OnVehicleChanged="SelectVehicle" 
                                                        Width="500px" Height="50px" FleetsCssClass="RegularText"  VehiclesCssClass="RegularText" FleetsCaptionCssClass="RegularText" 
                                                        VehiclesCaptionCssClass="RegularText"/>
                                                        <table runat="server" id="TableMaintenanceVehicle" cellpadding="2" cellspacing="2" style="border: outset thin silver; width: 500px">
                                                            <tr id="Tr1" runat="server">
                                                                <td id="Td1" runat="server">
                                                                    <asp:Label runat="server" ID="lblOdometer" CssClass="formtext" Text="Current Odometer" meta:resourcekey="lblOdometerResource1"/>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox runat="server" ID="txtOdometer" CssClass="formtext" BackColor="Silver" ReadOnly="True" meta:resourcekey="txtOdometerResource1"/>
                                                                </td>
                                                                <td style="width: 115px"></td>
                                                            </tr>
                                                            <tr>
                                                                <td id="Td2" runat="server">
                                                                    <asp:Label runat="server" ID="lblEngineHours" CssClass="formtext" Text="Current Engine Hours" meta:resourcekey="lblEngineHoursResource1"/>
                                                                </td>
                                                                <td id="Td3" runat="server">
                                                                    <asp:TextBox runat="server" ID="txtEngineHours" CssClass="formtext" BackColor="Silver" ReadOnly="True" meta:resourcekey="txtEngineHoursResource1"/>
                                                                    <asp:RequiredFieldValidator runat="server" ID="EngineHoursRequiredValidator" ControlToValidate="txtEngineHours" Text="*" 
                                                                        ErrorMessage="Engine Hours is required" meta:resourcekey="EngineHoursRequiredValidatorResource1"/>
                                                                    <asp:CompareValidator runat="server" ID="EngineHoursCompareValidator" ControlToValidate="txtEngineHours" Type="Double" Operator="GreaterThanEqual" 
                                                                        ValueToCompare="0" Text="*" ErrorMessage="Engine Hours is invalid" meta:resourcekey="EngineHoursCompareValidatorResource1"/>
                                                                </td>
                                                                <td id="Td4" runat="server" style="width: 115px">
                                                                    <asp:Button runat="server" ID="ButtonMaintenanceInit" Text="Set" OnClick="ButtonMaintenanceInit_Clicked"  CssClass="combutton" 
                                                                        Visible="False" ToolTip="Set or Initialize Engine Hours" meta:resourcekey="ButtonMaintenanceInitResource1"/>
                                                                </td>
                                                            </tr>
                                                            <tr runat="server" id="tfrSaveEH" visible="False">
                                                                <td></td>
                                                                <td>
                                                                    <asp:Button runat="server" ID="ButtonMaintenanceSaveEH" Text="Save" OnClick="ButtonMaintenanceSaveEH_Clicked"  CssClass="combutton"/>
                                                                    &nbsp;
                                                                    <asp:Button runat="server" ID="ButtonMaintenanceCancel" Text="Cancel" OnClick="ButtonMaintenanceCancel_Clicked" CausesValidation="False" 
                                                                        CssClass="combutton" meta:resourcekey="ButtonMaintenanceCancelResource1"/>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <ISWebGrid:WebGrid ID="dgFleetVehiclesInfo" runat="server" Height="250px" 
                                                            UseDefaultStyle="True" Visible="False" Width="680px" 
                                                            OnInitializeDataSource="dgFleetVehiclesInfo_InitializeDataSource" 
                                                            oninitializelayout="dgFleetVehiclesInfo_InitializeLayout">
                                                            <roottable>
                                                                <Columns>
                                                                    <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgFleetVehicles_Vehicle %>" DataMember="Description" Name="Vehicle" Width="100px"/>
                                                                    <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgFleetVehicles_Odometer %>" DataMember="CurrentOdo" DataType="System.Single" Name="Odometer" Width="100px"/>
                                                                    <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgFleetVehicles_EngineHours %>" DataMember="CurrentEngHrs" DataType="System.Single" Name="EngineHours" Width="100px"/>
                                                                </Columns>
                                                            </roottable>
                                                            <layoutsettings allowexport="Yes" allowfilter="Default" autofitcolumns="True"/>
                                                        </ISWebGrid:WebGrid>
                                                        <ISWebGrid:WebGrid ID="dgFleetDueServices" runat="server" Height="250px" UseDefaultStyle="True" Visible="False" Width="680px" 
                                                            OnInitializeDataSource="dgFleetDueServices_InitializeDataSource" oninitializelayout="dgFleetDueServices_InitializeLayout">
                                                            <layoutsettings allowexport="Yes" allowfilter="Yes" allowsorting="Yes" autofitcolumns="True"/>
                                                            <roottable>
                                                                <Columns>
                                                                    <ISWebGrid:WebGridColumn Caption="Vehicle" DataMember="VehicleDescription" Name="VehicleDescription" Width="100px"/>
                                                                    <ISWebGrid:WebGridColumn Caption="Description" DataMember="ServiceDescription" Name="ServiceDescription" Width="100px"/>
                                                                    <ISWebGrid:WebGridColumn Caption="Current Odometer" DataMember="CurrentOdo" DataType="System.Int32" Name="CurrentOdo" Width="100px"/>
                                                                    <ISWebGrid:WebGridColumn Caption="Current Engine Hrs" DataMember="CurrentEngHrs" DataType="System.Int32" Name="CurrentEngHrs" Width="100px"/>
                                                                    <ISWebGrid:WebGridColumn Caption="Due Value" DataMember="DueServiceValue" DataType="System.Int32" Name="DueServiceValue" Width="100px"/>
                                                                    <ISWebGrid:WebGridColumn Caption="Operation Type" DataMember="OperationTypeDescription" Name="OperationTypeDescription" Width="100px"/>
                                                                </Columns>
                                                            </roottable>
                                                        </ISWebGrid:WebGrid>
                                                        <fieldset runat="server" id="MaintenanceServices" style="margin: 0px 20px 20px 0px; padding: 0px 20px 20px 20px;" visible="False">
                                                            <legend>
                                                                <asp:Label ID="lblMaintenanceServicesLegend" runat="server"   Text="&#160;Maintenance Services&#160;" meta:resourcekey="lblMaintenanceServicesLegendResource1"/>
                                                            </legend>
                                                            <br />
                                                            <asp:GridView runat="server" ID="gvServices" AllowPaging="True" AllowSorting="True" EnableTheming="True" DataKeyNames="VehicleId,ServiceID,StatusID" 
                                                                AutoGenerateColumns="False" OnSelectedIndexChanged="gvServices_SelectedIndexChanged" OnPageIndexChanging="gvServices_PageIndexChanging" OnRowDeleting="gvServices_RowDeleting" 
                                                                meta:resourcekey="gvServicesResource1" OnRowDataBound="gvServices_RowDataBound">
                                                                <EmptyDataTemplate>
                                                                    <asp:Label ID="lblEmptyServices" runat="server" Text="No Services" meta:resourcekey="lblEmptyServicesResource1"/>
                                                                </EmptyDataTemplate>
                                                                <Columns>
                                                                    <asp:BoundField DataField="ServiceDescription" HeaderText="Description" meta:resourcekey="BoundFieldResource1">
                                                                        <ItemStyle Width="150px" />
                                                                        <HeaderStyle CssClass="DataHeaderStyle"/>
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="DueServiceValue" HeaderText="Due Value" meta:resourcekey="BoundFieldResource2">
                                                                        <ItemStyle HorizontalAlign="Center" Width="80px"/>
                                                                        <HeaderStyle CssClass="DataHeaderStyle"/>
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="ServiceTypeID" Visible="False" meta:resourcekey="BoundFieldResource25"/>
                                                                    <asp:BoundField DataField="OperationTypeID" Visible="False" meta:resourcekey="BoundFieldResource26"/>
                                                                    <asp:BoundField DataField="OperationTypeDescription" HeaderText="Operation Type" meta:resourcekey="BoundFieldResource4">
                                                                        <ItemStyle Width="120px"/>
                                                                        <HeaderStyle CssClass="DataHeaderStyle"/>
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="NotificationID" Visible="False" meta:resourcekey="BoundFieldResource6"/>
                                                                    <asp:BoundField DataField="NotificationDescription" HeaderText="Notification" meta:resourcekey="BoundFieldResource7">
                                                                        <ItemStyle Width="150px"/>
                                                                        <HeaderStyle CssClass="DataHeaderStyle"/>
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="ServiceInterval" HeaderText="Service Interval" Visible="False" meta:resourcekey="BoundFieldResource8"/>
                                                                    <asp:BoundField DataField="EndServiceValue" HeaderText="End Value" Visible="False" meta:resourcekey="BoundFieldResource9"/>
                                                                    <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" meta:resourcekey="BoundFieldResource10">
                                                                        <ItemStyle Width="150px"/>
                                                                        <HeaderStyle CssClass="DataHeaderStyle"/>
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="FrequencyID" Visible="False" meta:resourcekey="BoundFieldResource27"/>
                                                                    <asp:BoundField DataField="Comments" Visible="False" meta:resourcekey="BoundFieldResource28"/>
                                                                    
                                                                    <asp:CommandField ButtonType="Image" HeaderStyle-CssClass="DataHeaderStyle"  ShowSelectButton="True" SelectImageUrl="~/images/Edit.gif" meta:resourcekey="CommandFieldResource1"/>
                                                                    <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="btnDeleteServicePlan" runat="server" CommandName="Delete" ToolTip="Delete" ImageUrl="~/images/delete.gif" 
                                                                                OnClientClick="javascript: return confirm('Are you sure you want to delete the service plan?')"
                                                                                meta:resourcekey="btnDeleteServicePlanResource1"/>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle CssClass="DataHeaderStyle"/>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:HyperLink ID="linkClose" ImageUrl="~/images/Closed.gif"   NavigateUrl="javascript:var w=VehicleMaintenaceClose('{0}')" runat="server" Text="Close"/>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>
                                                        </fieldset>
                                                        <fieldset id="MaintenancePlanDetails" runat="server" style="margin: 0px 20px 20px 0px; padding: 0px 20px 20px 20px;" visible="False">
                                                            <legend>
                                                                <asp:Label runat="server" ID="LabelMaintenancePlanCaption" Text="&#160;Add new maintenance plan&#160;" meta:resourcekey="LabelMaintenancePlanCaptionResource1"/>
                                                            </legend>
                                                            <div>
                                                                <div id="PanelEmail" runat="server" style="padding: 10px 10px 0px 10px; float: right; position: relative; right: 80px; top: 10px; border: outset thin Silver; width: 200px; height: 150px">
                                                                    <asp:Label ID="LabelMaintenancePlanEmail" runat="server" Text="Email" CssClass="formtext"  meta:resourcekey="LabelMaintenancePlanEmailResource1"/>
                                                                    <br />
                                                                    <asp:TextBox ID="TextMaintenancePlanEmail" runat="server" CssClass="formtext" TextMode="MultiLine" Columns="30" Rows="3" MaxLength="512" ToolTip="Please type comma delimited e-mail list to get notified" meta:resourcekey="TextMaintenancePlanEmailResource1"/>
                                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TextMaintenancePlanEmail" ID="RequiredEmailValidator" Text="*" ErrorMessage="Email is Required" meta:resourcekey="RequiredEmailValidatorResource1"/>
                                                                    <asp:RegularExpressionValidator runat="server" ID="EmailRegExValidator" ValidationExpression="(\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*(,|))*" ControlToValidate="TextMaintenancePlanEmail" Text="*" ErrorMessage="Invalid email address" meta:resourcekey="EmailRegExValidatorResource1" ToolTip=""/>
                                                                    <br />
                                                                    <asp:Label ID="LabelMaintenancePlanComments" runat="server" Text="Notes" CssClass="formtext" meta:resourcekey="LabelMaintenancePlanCommentsResource1"/>
                                                                    <br />
                                                                    <asp:TextBox ID="TextMaintenancePlanComments" runat="server" CssClass="formtext" TextMode="MultiLine" Columns="30" Rows="3" MaxLength="256" meta:resourcekey="TextMaintenancePlanCommentsResource1"/>
                                                                </div>
                                                                <div id="PanelDetails">
                                                                    <table runat="server" style="position: relative; top: 10px;" id="TableDetails">
                                                                        <tr style="height: 13px">
                                                                            <td style="width: 100px">
                                                                                <asp:Label runat="server" ID="LabelMaintenancePlanDescription" Text="Description" CssClass="formtext" meta:resourcekey="LabelMaintenancePlanDescriptionResource1"/>
                                                                            </td>
                                                                            <td style="width: 260px">
                                                                                <asp:TextBox ID="TextMaintenancePlanDescription" runat="server" CssClass="formtext" Width="200px"/>
                                                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="TextMaintenancePlanDescription" ID="ValidatorDescription" Text="*" ErrorMessage="Description is Required" meta:resourcekey="ValidatorDescriptionResource1"/>
                                                                            </td>
                                                                        </tr>
                                                                        <tr style="height: 13px">
                                                                            <td style="width: 100px">
                                                                                <asp:Label ID="lblNewPlanType" runat="server" Text="Service Type" CssClass="formtext" meta:resourcekey="lblNewPlanTypeResource1"/>
                                                                            </td>
                                                                            <td style="width: 260px;">
                                                                                <asp:DropDownList ID="ddlNewPlanType" runat="server" Width="210px" CssClass="RegularText" DataSourceID="XmlOperationType" DataTextField="description" DataValueField="id"  AutoPostBack="True" OnSelectedIndexChanged="ddlNewPlanType_SelectedIndexChanged"/>                                                    </asp:DropDownList>
                                                                                &nbsp;
                                                                            </td>
                                                                        </tr>
                                                                        <tr id="trNewPlanService" runat="server" visible="False" style="height: 13px">
                                                                            <td style="width: 100px;">
                                                                                <asp:Label runat="server" ID="lblNewPlanSvcTypeCaption" Text="Service" CssClass="formtext" meta:resourcekey="lblNewPlanSvcTypeCaptionResource1"/>
                                                                            </td>
                                                                            <td style="width: 260px;">
                                                                                <asp:DropDownList runat="server" Width="210px" ID="ddlNewPlanSvcType" CssClass="RegularText" DataTextField="ServiceTypeDescription" DataValueField="ServiceTypeID"/>
                                                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlNewPlanSvcType" ID="RequiredSvcType" Text="*" ErrorMessage="Service Type is Not Specified" meta:resourcekey="RequiredSvcTypeResource1"/>
                                                                            </td>
                                                                        </tr>
                                                                        <tr id="trNewPlanNotification" runat="server" visible="False" style="height: 13px">
                                                                            <td style="width: 100px">
                                                                                <asp:Label runat="server" ID="lblNewPlanNotification" Text="Notification" CssClass="formtext" meta:resourcekey="lblNewPlanNotificationResource1"/>
                                                                            </td>
                                                                            <td style="width: 260px">
                                                                                <asp:DropDownList runat="server" Width="210px" ID="ddlNewPlanNotification" CssClass="RegularText" DataTextField="Description" DataValueField="NotificationID"/>
                                                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlNewPlanNotification" ID="RequiredFieldNotification" Text="*" ErrorMessage="Notification is Not Specified" meta:resourcekey="RequiredFieldNotificationResource1"/>
                                                                            </td>
                                                                        </tr>
                                                                        <tr id="trNewPlanSchedule" runat="server" visible="False" style="height: 13px">
                                                                            <td style="width: 100px">
                                                                                <asp:Label runat="server" ID="lblNewPlanSchedule" Text="Schedule Type" CssClass="formtext" meta:resourcekey="lblNewPlanScheduleResource1"/>
                                                                            </td>
                                                                            <td style="width: 260px">
                                                                                <asp:DropDownList runat="server" Width="210px" ID="ddlNewPlanSchedule" CssClass="formtext" AutoPostBack="True" OnSelectedIndexChanged="ddlNewPlanSchedule_SelectedIndexChanged">
                                                                                    <asp:ListItem Value="-1" Text="Please select schedule type" Selected="True" meta:resourcekey="ListItemResource1"/>
                                                                                    <asp:ListItem Value="0" Text="Once"/>
                                                                                    <asp:ListItem Value="1" Text="Recurring"/>
                                                                                </asp:DropDownList>
                                                                                <asp:CompareValidator runat="server" ControlToValidate="ddlNewPlanSchedule" Operator="GreaterThan" Type="Integer" ValueToCompare="-1" ID="CompareScheduleType" Text="*" ErrorMessage="Schedule Type is not selected"  meta:resourcekey="CompareScheduleTypeResource1"/>
                                                                            </td>
                                                                        </tr>
                                                                        <tr id="trDueValue" runat="server" visible="False" style="height: 13px">
                                                                            <td>
                                                                                <asp:Label ID="LblDueValue" runat="server" Text="Starting Value" meta:resourcekey="LblDueValueResource1" CssClass="formtext"/>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="TextDueValue" runat="server" CssClass="formtext"/>
                                                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="TextDueValue" ID="RequiredDueValidator" Text="*" ErrorMessage="Due Value is Required" meta:resourcekey="RequiredDueValidatorResource1"/>
                                                                                <asp:CompareValidator runat="server" ControlToValidate="TextDueValue" Operator="DataTypeCheck" Type="Integer" ID="CompareDueValue" Text="*" ErrorMessage="Due Value is not valid" meta:resourcekey="CompareDueValueResource1"/>
                                                                                <asp:CompareValidator runat="server" ControlToValidate="TextDueValue" Operator="GreaterThan" ValueToCompare="0" Type="Integer" ID="CompareDueValueGT0" Text="*" ErrorMessage="Due Value must be greater than zero"  meta:resourcekey="CompareDueValueGT0Resource1"/>
                                                                            </td>
                                                                        </tr>
                                                                        <tr id="trInterval" runat="server" visible="False" style="height: 13px">
                                                                            <td>
                                                                                <asp:Label ID="LblInterval" runat="server" Text="Interval" meta:resourcekey="LblIntervalResource1" CssClass="formtext"/>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="TextInterval" runat="server" CssClass="formtext"/>
                                                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="TextInterval" ID="RequiredIntervalValidator" Text="*" ErrorMessage="Interval is Required" meta:resourcekey="RequiredIntervalValidatorResource1"/>
                                                                                <asp:CompareValidator runat="server" ControlToValidate="TextInterval" Operator="DataTypeCheck" Type="Integer" ID="CompareInterval" Text="*" ErrorMessage="Interval is not valid"  meta:resourcekey="CompareIntervalResource1"/>
                                                                            </td>
                                                                        </tr>
                                                                        <tr id="trEndValue" runat="server" visible="False" style="height: 13px">
                                                                            <td>
                                                                                <asp:Label ID="LblEndValue" runat="server" Text="End Value" meta:resourcekey="LblEndValueResource1" CssClass="formtext"/>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="TextEndValue" runat="server" CssClass="formtext"/>
                                                                                <asp:CompareValidator runat="server" ControlToValidate="TextEndValue" Operator="DataTypeCheck" Type="Integer" ID="CompareValidatorEndValue" Text="*" ErrorMessage="End Value is not valid" meta:resourcekey="CompareValidatorEndValueResource1"/>
                                                                                &nbsp;&nbsp;
                                                                                <asp:CheckBox ID="chkUnlimited" runat="server" AutoPostBack="True" OnCheckedChanged="chkUnlimited_CheckedChanged" CssClass="formtext"/>
                                                                                &nbsp;
                                                                                <asp:Label runat="server" ID="LabelUnlimited" Text="Unlimited" meta:resourcekey="LabelUnlimitedResource1" CssClass="formtext"/>
                                                                            </td>
                                                                        </tr>
                                                                        <tr id="trNewPlanDate" runat="server" visible="False" style="height: 13px">
                                                                            <td>
                                                                                <asp:Label ID="LblNewPlanDate" runat="server" Text="Date" meta:resourcekey="LblNewPlanDateResource1"/>
                                                                            </td>
                                                                            <td>
                                                                                <ISWebInput:WebInput id="txtNewPlanDate" runat="server" height="17px" width="118px" wrap="Off">
                                                                                    <HighLight IsEnabled="True" Type="Phrase"/>
                                                                                    <DateTimeEditor IsEnabled="True" AccessKey="Space"/>
                                                                                </ISWebInput:WebInput>
                                                                            </td>
                                                                        </tr>
                                                                        <tr id="trTimeBasedPlan" runat="server" visible="False" class="formtext">
                                                                           <td colspan=2>
                                                                                <table style="border-right: #ffffff 1px solid; border-top: #ffffff 1px solid; border-left: #ffffff 1px solid; border-bottom: #ffffff 1px solid;" width="300">
                                                                                    <tr>
                                                                                        <td align="left">
                                                                                            <asp:RadioButtonList ID="lstSceduledType" runat="server" AutoPostBack="True" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" CssClass="formtext" OnSelectedIndexChanged="lstSceduledType_SelectedIndexChanged" RepeatDirection="Horizontal" Width="199px">
                                                                                                <asp:ListItem  Text="Weekly" Value="2" Selected="True"/>
                                                                                                <asp:ListItem  Text="Monthly" Value="3"/>
                                                                                                <asp:ListItem  Text="Yearly" Value="4"/>
                                                                                            </asp:RadioButtonList>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                                <table id="tblWeekly" runat="server" border="0" width="300">
                                                                                    <tr>
                                                                                        <td align="left" style="width: 97px">
                                                                                            <asp:Label ID="lblWeekly" runat="server" Text="Weekly"/>
                                                                                        </td>
                                                                                        <td align="left" style="width: 204px">
                                                                                            <asp:Label ID="lblEvery" runat="server" Text="Every"/>
                                                                                            <asp:DropDownList ID="cboWeekDay" runat="server" CssClass="RegularText">
                                                                                                <asp:ListItem  Text="Mon" Value="1"/>
                                                                                                <asp:ListItem  Text="Tue" Value="2"/>
                                                                                                <asp:ListItem  Text="Wed" Value="3"/>
                                                                                                <asp:ListItem  Text="Thur" Value="4"/>
                                                                                                <asp:ListItem  Text="Fri" Value="5"/>
                                                                                                <asp:ListItem  Text="Sat" Value="6"/>
                                                                                                <asp:ListItem  Text="Sun" Value="0"/>
                                                                                            </asp:DropDownList>
                                                                                         </td>
                                                                                    </tr>
                                                                                </table>
                                                                                <table id="tblMonthly" runat="server" border="0" visible="false">
                                                                                    <tr>
                                                                                        <td align="left" style="width: 94px">
                                                                                            <asp:Label ID="lblMonthly" runat="server" Text="Monthly"/>
                                                                                        </td>
                                                                                        <td align="left">
                                                                                            <table style="width: 232px">
                                                                                                <tr>
                                                                                                    <td align="left" style="width: 100%; height: 21px;">
                                                                                                        <asp:Label ID="lblDay" runat="server"  Text="Day"></asp:Label>
                                                                                                        <asp:DropDownList ID="cboMonthlyDay" runat="server" CssClass="RegularText" meta:resourcekey="cboMonthlyDayResource1">
                                                                                                            <asp:ListItem Text="First Day" Value="1"></asp:ListItem>
                                                                                                            <asp:ListItem Text="2"/>
                                                                                                            <asp:ListItem Text="3"/>
                                                                                                            <asp:ListItem Text="4"/>
                                                                                                            <asp:ListItem Text="5"/>
                                                                                                            <asp:ListItem Text="6"/>
                                                                                                            <asp:ListItem Text="7"/>
                                                                                                            <asp:ListItem Text="8"/>
                                                                                                            <asp:ListItem Text="9"/>
                                                                                                            <asp:ListItem Text="10"/>
                                                                                                            <asp:ListItem Text="11"/>
                                                                                                            <asp:ListItem Text="12"/>
                                                                                                            <asp:ListItem Text="13"/>
                                                                                                            <asp:ListItem Text="14"/>
                                                                                                            <asp:ListItem Text="15"/>
                                                                                                            <asp:ListItem Text="16"/>
                                                                                                            <asp:ListItem Text="17"/>
                                                                                                            <asp:ListItem Text="18"/>
                                                                                                            <asp:ListItem Text="19"/>
                                                                                                            <asp:ListItem Text="20"/>
                                                                                                            <asp:ListItem Text="21"/>
                                                                                                            <asp:ListItem Text="22"/>
                                                                                                            <asp:ListItem Text="23"/>
                                                                                                            <asp:ListItem Text="24"/>
                                                                                                            <asp:ListItem Text="25"/>
                                                                                                            <asp:ListItem Text="26"/>
                                                                                                            <asp:ListItem Text="27"/>
                                                                                                            <asp:ListItem Text="28"/>
                                                                                                            <asp:ListItem Text="29"/>
                                                                                                            <asp:ListItem Text="30"/>
                                                                                                            <asp:ListItem Text="31"/>
                                                                                                            <asp:ListItem Text="Last Day" Value="32"/>
                                                                                                        </asp:DropDownList>
                                                                                                        <asp:Label ID="lblOfEveryMonth" runat="server" Text="of every month" Width="136px"/>
                                                                                                      </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                                <table id="tblYearly" runat="server" border="0" width="300" visible="false">
                                                                                    <tr>
                                                                                        <td align="left" style="width: 97px">
                                                                                            <asp:Label ID="lblYearly" runat="server" Text="Yearly"/>
                                                                                        </td>
                                                                                        <td align="left" style="width: 204px">
                                                                                            <asp:Label ID="Label2" runat="server" Text="Every"/>
                                                                                            <asp:DropDownList ID="cboYearly" runat="server" CssClass="RegularText" >
                                                                                                <asp:ListItem Value="1" Text="Jan"/>
                                                                                                <asp:ListItem Value="2" Text="Feb"/>
                                                                                                <asp:ListItem Value="3" Text="Mar"/>
                                                                                                <asp:ListItem Value="4" Text="Apr"/>
                                                                                                <asp:ListItem Value="5" Text="May"/>
                                                                                                <asp:ListItem Value="6" Text="Jun"/>
                                                                                                <asp:ListItem Value="7" Text="Jul"/>
                                                                                                <asp:ListItem Value="8" Text="Aug"/>
                                                                                                <asp:ListItem Value="9" Text="Sep"/>
                                                                                                <asp:ListItem Value="10" Text="Oct"/>
                                                                                                <asp:ListItem Value="11" Text="Nov"/>
                                                                                                <asp:ListItem Value="12" Text="Dec"/>
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                                <table>
                                                                                    <tr>
                                                                                        <td style="width: 93px">
                                                                                            <asp:Label ID="lblStartingFrom" runat="server" Text="Starting"/>
                                                                                        </td>
                                                                                        <td style="width: 100px">
                                                                                            <ISWebInput:WebInput id="txtFromDate" runat="server" height="17px" width="118px" wrap="Off">
                                                                                                <HighLight IsEnabled="True" Type="Phrase"/>
                                                                                                <DateTimeEditor IsEnabled="True" AccessKey="Space"/>
                                                                                            </ISWebInput:WebInput>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="width: 93px">
                                                                                            <asp:Label ID="lblEnding" runat="server" Text="Ending"/>
                                                                                        </td>
                                                                                        <td style="width: 100px">
                                                                                            <ISWebInput:WebInput id="txtEndDate" runat="server" height="17px" width="118px" wrap="Off">
                                                                                                <HighLight IsEnabled="True" Type="Phrase"/>
                                                                                                <DateTimeEditor IsEnabled="True" AccessKey="Space"/>
                                                                                            </ISWebInput:WebInput>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                           </td>
                                                                       </tr>
                                                                    </table>
                                                                </div>
                                                            </div>
                                                        </fieldset>
                                                        <table id="TableServiceDetailsSaveCancel" style="margin: 0px 0px 20px 20px" runat="server" visible="False">
                                                            <tr id="Tr2" runat="server">
                                                                <td id="Td5" runat="server">
                                                                    <asp:Button ID="btnSaveNewPlan" runat="server" CssClass="combutton" Text="Save" OnClick="btnSaveNewPlan_Click" meta:resourcekey="ButtonMaintenanceSaveEHResource1"/>
                                                                </td>
                                                                <td id="Td6" style="width: 20px" runat="server"></td>
                                                                <td id="Td7" runat="server">
                                                                    <asp:Button ID="btnCancelNewPlan" runat="server" CausesValidation="False" CssClass="combutton" Text="Cancel" OnClick="btnCancelNewPlan_Click" meta:resourcekey="ButtonMaintenanceCancelResource1"/>                                       
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <asp:ValidationSummary runat="server" ID="MaintenanceValidationSummary" meta:resourcekey="MaintenanceValidationSummaryResource1"/>
                                                        <asp:Label ID="LabelMaintenanceMessage" runat="server" meta:resourcekey="LabelMaintenanceMessageResource1"/>
                                                    </div>
                                                </asp:View>
                                                
                                                
                                                
                                                
                                                <asp:View ID="ViewNotification" runat="server">
                                                    <div style="padding: 20px 20px 20px 20px">
                                                    <asp:GridView ID="gdvNotifications" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="NotificationID" 
                                                        EnableTheming="True" OnRowEditing="gdvNotifications_RowEditing" OnRowCancelingEdit="gdvNotifications_RowCancelingEdit" OnRowUpdated="gdvNotifications_RowUpdated" 
                                                        OnRowUpdating="gdvNotifications_RowUpdating" OnRowCreated="gdvNotifications_RowCreated" OnPageIndexChanging="gdvNotifications_PageIndexChanging" meta:resourcekey="gdvNotificationsResource1">
                                                        <EmptyDataTemplate>
                                                            <asp:Label ID="lblEmptyData" runat="server" Text="No notifications" meta:resourcekey="lblEmptyDataResource1"/>
                                                        </EmptyDataTemplate>
                                                        <Columns>
                                                            <asp:BoundField DataField="NotificationID" HeaderText="ID" ReadOnly="True" meta:resourcekey="BoundFieldResource11">
                                                                <HeaderStyle CssClass="DataHeaderStyle"/>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="OperationTypeID" Visible="False" meta:resourcekey="BoundFieldResource12"/>
                                                            
                                                            <asp:TemplateField HeaderText="Notification Type" meta:resourcekey="TemplateFieldResource3">
                                                                <HeaderStyle CssClass="DataHeaderStyle"/>
                                                                <ItemTemplate>
                                                                    <asp:Label runat="server" ID="lblNotType" CssClass="formtext" Text='<%# Eval("OperationTypeDescription") %>' meta:resourcekey="lblNotTypeResource1"/>
                                                                </ItemTemplate>
                                                                <EditItemTemplate>
                                                                    <asp:DropDownList ID="ddlNotTypeEdit" runat="server" CssClass="formtext" DataSourceID="XmlOperationType" DataTextField="description" DataValueField="id" SelectedValue='<%# Bind("OperationTypeID") %>'  meta:resourcekey="ddlNotTypeEditResource1"/>                                               
                                                                </EditItemTemplate>
                                                                <ItemStyle Width="120px" />
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Notification1" HeaderText="Notification" meta:resourcekey="BoundFieldResource13">
                                                                <ItemStyle Width="120px" HorizontalAlign="Center"/>
                                                                <HeaderStyle CssClass="DataHeaderStyle"/>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Notification2" HeaderText="Warning" meta:resourcekey="BoundFieldResource14">
                                                                <ItemStyle Width="120px" HorizontalAlign="Center"/>
                                                                <HeaderStyle CssClass="DataHeaderStyle"/>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Notification3" HeaderText="Last Warning" meta:resourcekey="BoundFieldResource15">
                                                                <ItemStyle Width="120px" HorizontalAlign="Center"/>
                                                                <HeaderStyle CssClass="DataHeaderStyle"/>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Description" HeaderText="Description" meta:resourcekey="BoundFieldResource16">
                                                                <ItemStyle Width="200px"/>
                                                                <HeaderStyle CssClass="DataHeaderStyle"/>
                                                            </asp:BoundField>
                                                            
                                                            <asp:CommandField ButtonType="Image" HeaderStyle-CssClass="DataHeaderStyle" CancelImageUrl="~/images/Cancel.gif" EditImageUrl="~/images/Edit.gif" ShowEditButton="True" UpdateImageUrl="~/images/OK.gif" meta:resourcekey="CommandFieldResource2"/>
                                                            <asp:TemplateField meta:resourcekey="TemplateFieldResource4">
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="btnDeleteNotification" runat="server" ToolTip="Delete" OnClick="btnDeleteNotification_Clicked" ImageUrl="~/images/delete.gif" OnClientClick="javascript: return confirm('Are you sure you want to delete the notification?')"  meta:resourcekey="btnDeleteNotificationResource1"/>
                                                                </ItemTemplate>
                                                                <HeaderStyle CssClass="DataHeaderStyle"/>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                    <br />
                                                    <asp:Button ID="btnAddNotification" CssClass="combutton" runat="server" Text="Add Notification" OnClick="btnAddNotification_Click" meta:resourcekey="btnAddNotificationResource1"/>
                                                    <fieldset id="NotificationDetails" runat="server" visible="False" style="margin: 0px 20px 20px 0px; padding: 0px 20px 20px 20px;">
                                                        <legend>
                                                            <asp:Label ID="lblNotificationDetailsCaption" runat="server"  class="formtext"  Text="&#160;Notification Details&#160;" meta:resourcekey="lblNotificationDetailsCaptionResource1"/>
                                                        </legend>
                                                        <table>
                                                            <tr>
                                                                <td style="width:122px">
                                                                    <asp:Label ID="lblNotificationType" runat="server"  class="formtext"  Text="Notification Type" meta:resourcekey="lblNotificationTypeResource1"/>
                                                                </td>
                                                                <td style="width: 398px">
                                                                    <asp:DropDownList ID="ddlNotificationType" runat="server" DataSourceID="XmlOperationType" DataValueField="id" DataTextField="description" meta:resourcekey="ddlNotificationTypeResource1"/>                                                       
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 122px">
                                                                    <asp:Label ID="lblNotification1" runat="server"  class="formtext"  Text="Notification" meta:resourcekey="lblNotification1Resource1"/>
                                                                </td>
                                                                <td style="width: 398px">
                                                                    <asp:TextBox ID="txtNotification1" runat="server" ToolTip="(0 - 100)%" meta:resourcekey="txtNotification1Resource1"/>
                                                                    <asp:RequiredFieldValidator ID="reqNotif" runat="server" ControlToValidate="txtNotification1" EnableViewState="False" Text="*" meta:resourcekey="reqNotifResource1"/>
                                                                    <asp:RangeValidator ID="RangeNotif" runat="server" SetFocusOnError="True" Type="Integer" ControlToValidate="txtNotification1" MinimumValue="0" MaximumValue="100" ErrorMessage="Invalid" meta:resourcekey="RangeNotifResource1"/>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 122px">
                                                                    <asp:Label ID="lblNotification2" runat="server"  class="formtext"  Text="Warning" meta:resourcekey="lblNotification2Resource1"/>
                                                                </td>
                                                                <td style="width: 398px">
                                                                    <asp:TextBox ID="txtWarning" runat="server" ToolTip="(0 - 100)%" meta:resourcekey="txtWarningResource1"/>
                                                                    <asp:RequiredFieldValidator ID="reqWarning" runat="server" ControlToValidate="txtWarning" meta:resourcekey="reqWarningResource1" Text="*"/>
                                                                    <asp:RangeValidator ID="RangeWarning" runat="server" SetFocusOnError="True" Type="Integer" ControlToValidate="txtWarning" MinimumValue="0" MaximumValue="100" ErrorMessage="Invalid" meta:resourcekey="RangeWarningResource1"/>
                                                                    <asp:CompareValidator ID="CompareNotif" runat="server" ControlToValidate="txtWarning" ControlToCompare="txtNotification1" Type="Integer" Operator="GreaterThanEqual" ErrorMessage="Invalid" meta:resourcekey="CompareNotifResource1"/>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 122px">
                                                                    <asp:Label ID="lblNotification3" runat="server"  class="formtext"  Text="Last Warning" meta:resourcekey="lblNotification3Resource1"/>
                                                                </td>
                                                                <td style="width: 398px">
                                                                    <asp:TextBox ID="txtLastWarning" runat="server" ToolTip="(0 - 100)%" meta:resourcekey="txtLastWarningResource1"/>
                                                                    <asp:RequiredFieldValidator ID="reqLastWarning" runat="server" ControlToValidate="txtLastWarning" meta:resourcekey="reqLastWarningResource1" Text="*"/>
                                                                    <asp:RangeValidator ID="RangeLastWarning" runat="server" SetFocusOnError="True" Type="Integer" ControlToValidate="txtNotification1" MinimumValue="0" MaximumValue="100" ErrorMessage="Invalid"  meta:resourcekey="RangeLastWarningResource1"/>
                                                                    <asp:CompareValidator ID="CompareLastWarning" runat="server" ControlToValidate="txtLastWarning" ControlToCompare="txtWarning" Type="Integer" Operator="GreaterThanEqual" ErrorMessage="Invalid"  meta:resourcekey="CompareLastWarningResource1"/>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 122px">
                                                                    <asp:Label ID="lblNotificationDescription"  class="formtext"  runat="server" Text="Description" meta:resourcekey="lblNotificationDescriptionResource1"/>
                                                                </td>
                                                                <td style="width: 398px">
                                                                    <asp:TextBox ID="txtNotificationDescription" runat="server" MaxLength="50" Width="340px" meta:resourcekey="txtNotificationDescriptionResource1"/>
                                                                    <asp:RequiredFieldValidator ID="reqNotificationDescription" runat="server" ControlToValidate="txtNotificationDescription" meta:resourcekey="reqNotificationDescriptionResource1" Text="*"/>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <br />
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="btnSave" runat="server" CssClass="combutton" Text="Save" OnClick="btnSave_Click"  meta:resourcekey="btnSaveResource1"/>
                                                                </td>
                                                                <td>&nbsp;</td>
                                                                <td>
                                                                    <asp:Button ID="btnCancel" runat="server" CausesValidation="False" CssClass="combutton" Text="Cancel" OnClick="btnCancel_Click" meta:resourcekey="btnCancelResource1"/>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>
                                                    <asp:Label ID="lblNotificationMessage" runat="server" meta:resourcekey="lblNotificationMessageResource1"/>
                                                    </div>
                                                </asp:View>
                                                
                                                
                                                  <asp:View ID="ViewServices" runat="server">
                                                    <div style="padding: 20px 20px 20px 20px">
                                                        <asp:GridView ID="gvOrgServices" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="ServiceTypeID" EnableTheming="True" OnRowCancelingEdit="gvOrgServices_RowCancelingEdit" 
                                                            OnRowCreated="gvOrgServices_RowCreated" OnRowEditing="gvOrgServices_RowEditing" OnRowUpdating="gvOrgServices_RowUpdating" OnPageIndexChanging="gvOrgServices_PageIndexChanging" meta:resourcekey="gvOrgServicesResource1">
                                                            <EmptyDataTemplate>
                                                                <asp:Label ID="lblEmptyOrgServices" runat="server" Text="No Services" meta:resourcekey="lblEmptyOrgServicesResource1"/>
                                                            </EmptyDataTemplate>
                                                            <Columns>
                                                                <asp:BoundField DataField="ServiceTypeID" HeaderText="ID" ReadOnly="True" meta:resourcekey="BoundFieldResource21">
                                                                    <ItemStyle Width="50px" />
                                                                    <HeaderStyle CssClass="DataHeaderStyle"/>
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="OperationTypeID" HeaderText="Operation Type ID" Visible="False" meta:resourcekey="BoundFieldResource22"/>
                                                                
                                                                <asp:TemplateField HeaderText="Service Type" meta:resourcekey="TemplateFieldResource5">
                                                                    <ItemTemplate>
                                                                        <%# Eval("OperationTypeDescription") %>
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:DropDownList runat="server" ID="ddlServiceType" DataSourceID="XmlOperationType" DataValueField="id" DataTextField="description" SelectedValue='<%# Bind("OperationTypeID") %>' meta:resourcekey="ddlServiceTypeResource1"/>
                                                                    </EditItemTemplate>
                                                                    <ItemStyle Width="120px"/>
                                                                    <HeaderStyle CssClass="DataHeaderStyle"/>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="ServiceTypeDescription" HeaderText="Description" meta:resourcekey="BoundFieldResource23">
                                                                    <ItemStyle Width="200px"/>
                                                                    <HeaderStyle CssClass="DataHeaderStyle"/>
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="VRMSCode" Visible=false HeaderText="VRMS Code" meta:resourcekey="BoundFieldResource24">
                                                                    <ItemStyle Width="120px"/>
                                                                    <HeaderStyle CssClass="DataHeaderStyle"/>
                                                                </asp:BoundField>
                                                                
                                                                <asp:CommandField ButtonType="Image" HeaderStyle-CssClass="DataHeaderStyle" CancelImageUrl="~/images/Cancel.gif" EditImageUrl="~/images/Edit.gif" ShowEditButton="True" UpdateImageUrl="~/images/OK.gif" CausesValidation="False"  meta:resourcekey="CommandFieldResource3"/>
                                                                <asp:TemplateField meta:resourcekey="TemplateFieldResource6">
                                                                    <ItemTemplate>
                                                                        <asp:ImageButton ID="btnDeleteOrgService" runat="server" ToolTip="Delete" OnClick="btnDeleteOrgService_Clicked" ImageUrl="~/images/delete.gif" OnClientClick="javascript: return confirm('Are you sure you want to delete the service?')"  meta:resourcekey="btnDeleteOrgServiceResource1"/>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle CssClass="DataHeaderStyle"/>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                        <br />
                                                        <asp:Button ID="linkAddService" CssClass="combutton" runat="server" Text="Add Service" OnClick="btnAddService_Click" meta:resourcekey="linkAddServiceResource1"/>
                                                        <fieldset id="AddNewService" runat="server" visible="False" style="margin: 0px 20px 20px 0px;padding: 0px 20px 20px 20px;">
                                                            <legend>
                                                                <asp:Label ID="lblOrgSvcCaption" runat="server" Text="&#160;Service Details&#160;" meta:resourcekey="lblOrgSvcCaptionResource1"/>
                                                            </legend>
                                                            <table>
                                                                <tr>
                                                                    <td style="width: 122px">
                                                                        <asp:Label ID="Label3" runat="server" Text="Service Type" CssClass="formtext"  meta:resourcekey="Label3Resource1"/>
                                                                    </td>
                                                                    <td style="width: 398px">
                                                                        <asp:DropDownList ID="ddlOrgServiceType" runat="server" DataSourceID="XmlOperationType" DataValueField="id" DataTextField="description" meta:resourcekey="ddlOrgServiceTypeResource1"/>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="width: 122px">
                                                                        <asp:Label ID="lblServiceTypeDescription" runat="server" Text="Description" CssClass="formtext"  meta:resourcekey="lblServiceTypeDescriptionResource1"/>
                                                                    </td>
                                                                    <td style="width: 398px">
                                                                        <asp:TextBox ID="txtServiceTypeDescription" MaxLength="100" runat="server" Width="300px" meta:resourcekey="txtServiceTypeDescriptionResource1"/>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtServiceTypeDescription" EnableViewState="False" Text="*" meta:resourcekey="RequiredFieldValidator1Resource1"/>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="width: 122px">
                                                                        <asp:Label ID="lblVRMSCode" runat="server" Text="VRMS Code" meta:resourcekey="lblVRMSCodeResource1" Visible="False"/>
                                                                    </td>
                                                                    <td style="width: 398px">
                                                                        <asp:TextBox ID="txtVRMSCode" runat="server" MaxLength="20" meta:resourcekey="txtVRMSCodeResource1" Visible="False"/>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <br />
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Button ID="btnSaveService" runat="server" CssClass="combutton" Text="Save" OnClick="btnSaveService_Click" meta:resourcekey="btnSaveServiceResource1"/>
                                                                    </td>
                                                                    <td>&nbsp;</td>
                                                                    <td>
                                                                        <asp:Button ID="btnCancelService" runat="server" CausesValidation="False" CssClass="combutton" Text="Cancel" OnClick="btnCancelService_Click" meta:resourcekey="btnCancelServiceResource1"/>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </fieldset>
                                                        <asp:Label ID="lblServicesMessage" runat="server" meta:resourcekey="lblServicesMessageResource1"/>
                                                    </div>
                                                </asp:View>
                                                
                                                <asp:View ID="ViewHistory" runat="server">
                                                    <fvs:FleetVehicles runat="server" ID="FleetVehicleSelectorHistory" VehicleAutoPostBack="true" OnVehicleChanged="GetVehicleHistory" Width="500px" Height="50px" FleetsCssClass="RegularText" VehiclesCssClass="RegularText" FleetsCaptionCssClass="RegularText" VehiclesCaptionCssClass="RegularText"/>
                                                    <br />
                                                    <ISWebGrid:WebGrid ID="dgHistory" runat="server" Height="250px" UseDefaultStyle="True" Width="98%" OnInitializeDataSource="dgHistory_InitializeDataSource" oninitializelayout="dgHistory_InitializeLayout">
                                                        <layoutsettings allowexport="Yes" allowfilter="Yes" allowsorting="Yes" autofitcolumns="True"/>
                                                        <roottable>
                                                            <Columns>
                                                                <ISWebGrid:WebGridColumn DataMember="VehicleDescription" Caption="<%$ Resources:dgHistory_Vehicle %>" Name="VehicleDescription" Width="70px"/>
                                                                <ISWebGrid:WebGridColumn DataMember="ServiceDateTime" DataType="System.DateTime" Caption="<%$ Resources:dgHistory_DateTime %>" Name="ServiceDateTime" Width="80px"/>
                                                                <ISWebGrid:WebGridColumn DataMember="ServiceTypeDescription" Caption="<%$ Resources:dgHistory_ServiceType %>" Name="ServiceTypeDescription" Width="70px"/>
                                                                <ISWebGrid:WebGridColumn DataMember="ServiceValue" Caption="<%$ Resources:dgHistory_ServiceValue %>" Name="ServiceValue" Width="70px"/>
                                                                <ISWebGrid:WebGridColumn DataMember="CurrentClosingValue" Name="CurrentClosingValue" Caption="<%$ Resources:dgHistory_ClosingValue %>" Width="70px"/>
                                                                <ISWebGrid:WebGridColumn DataMember="ServiceDescription" Caption="<%$ Resources:dgHistory_Description %>" Name="ServiceDescription" Width="130px"/>
                                                                <ISWebGrid:WebGridColumn DataMember="Comments" Caption="Notes" Name="Comments" Width="90px"/>
                                                            </Columns>
                                                        </roottable>
                                                    </ISWebGrid:WebGrid>
                                                    <br />
                                                    <asp:Label runat="server" ID="LabelHistoryMessage" meta:resourcekey="LabelHistoryMessageResource1"/>                    
                                                </asp:View>
                                              
                                                
                                                
                                                     <asp:View ID="ViewNotifications" runat="server">
                                                    <div style="padding: 20px 20px 20px 20px;">   
                                                        <fieldset  style="margin: 0px 0px 5px 0px; padding: 0px 0px 0px 0px">
                                                        <table cellpadding="1" cellspacing="1" >
                                                            <tr>
                                                                <td>
                                                                    <asp:Button CssClass=combutton Font-Bold="true" ID="cmdCloseNotification" runat="server" 
                                                                    onclick="cmdCloseNotification_Click" Text="<%$ Resources:lnkAcknowledge %>"/>
                                                                </td>
                                                                <td>&nbsp;</td>
                                                                <td>&nbsp;</td>
                                                                <td>
                                                                    <asp:Label ID="lblMessageNotifications" CssClass="formtext" ForeColor="Red"  runat="server" Text=""/>
                                                                </td>
                                                            </tr>
                                                        </table> 
                                                        </fieldset> 
                                                        <asp:Label ID="LabelCustomerNote" runat="server" Font-Size="Small" 
                                                            ForeColor="gray" meta:resourcekey="LabelCustomerNoteResource1" />
                                                        <ISWebGrid:WebGrid ID="dgNotification" runat="server" Height="540px" 
                                                            oninitializedatasource="dgNotification_InitializeDataSource" 
                                                            oninitializelayout="dgNotification_InitializeLayout" usedefaultstyle="True" 
                                                            Width="100%">
                                                            <roottable datakeyfield="NotificationId">
                                                                <Columns>
                                                                    <ISWebGrid:WebGridColumn AllowGrouping="No" AllowSizing="No" AllowSorting="No" 
                                                                        Bound="False" Caption="chkBoxShow" ColumnType="CheckBox" 
                                                                        DataMember="chkBoxShow" EditType="NoEdit" IsRowChecker="True" Name="SelectRow" 
                                                                        ShowInSelectColumns="No" Width="25px">
                                                                    </ISWebGrid:WebGridColumn>
                                                                    <ISWebGrid:WebGridColumn Caption="DateTime" DataMember="DateTimeCreated" 
                                                                        DataType="System.DateTime" Name="DateTimeCreated" Width="100px" />
                                                                    <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgNotification_Vehicle %>" 
                                                                        DataMember="Description" Name="Description" Width="100px" />
                                                                    <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgNotification_LicensePlate %>" 
                                                                        DataMember="LicensePlate" Name="LicensePlate" Width="100px" />
                                                                    <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgNotification_Notification %>" 
                                                                        DataMember="notificationDescription" Name="notificationDescription" 
                                                                        Width="100px" />
                                                                    <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgNotification_Data %>" 
                                                                        DataMember="Data" Name="Data" Visible="false" Width="100px" />
                                                                    <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgNotification_Details %>" 
                                                                        ColumnType="Template" Name="Details" ShowInSelectColumns="No" Width="140px">
                                                                        <celltemplate>
                                                                            <asp:HyperLink ID="HyperLink_Site" runat="server" 
                                                                                NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "CustomUrl") %>' 
                                                                                Text="Details" />
                                                                        </celltemplate>
                                                                    </ISWebGrid:WebGridColumn>
                                                                </Columns>
                                                            </roottable>
                                                            <layoutsettings allowexport="Yes" allowfilter="Default" AllowSorting="Yes" 
                                                                autofitcolumns="True" />
                                                        </ISWebGrid:WebGrid>
                                                    </div>
                                                </asp:View>               
                                            
                                            
                                                
                                                <asp:View ID="ViewDTCcodes" runat="server">
                                                    <div style="padding: 20px 20px 20px 20px">
                                                        <fieldset style="padding: 5px 5px 5px 5px">
                                                            <table class="formtext" style="width: 520px" >
                                                                <tr>
                                                                    <td align="left" class="style1">
                                                                        <asp:Label ID="lblFromTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFromTitleResource1" Text="From:"/>
                                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                    </td>
                                                                    <td align="left" class="style2">
                                                                        <table  border="0px" cellpadding="1" cellspacing="1">
                                                                            <tr>
                                                                                <td style="height: 21px;Width:85px">
                                                                                    <ISWebInput:WebInput ID="txtFrom" runat="server" Width="176px"  Height="17px" Wrap="Off" MaxDate="12/31/9998 23:59:59" meta:resourcekey="txtFromResource2" MinDate="1753-01-01">
                                                                                        <CultureInfo CultureName="en-US"/>
                                                                                        <DisplayFormat>
                                                                                            <ErrorWindowInfo IsEnabled="False"/>
                                                                                        </DisplayFormat>
                                                                                        <HighLight IsEnabled="True" Type="Phrase"/>
                                                                                        <EditFormat>
                                                                                            <ErrorWindowInfo IsEnabled="False"/>
                                                                                        </EditFormat>
					                                                                    <DateTimeEditor IsEnabled="True" AccessKey="Space"/>
                                                                                     </ISWebInput:WebInput>
                                                                                </td>
                                                                                <td>
                                                                                   <asp:DropDownList ID="cboHoursFrom" runat="server" CssClass="RegularText" Width="76px" meta:resourcekey="cboHoursFromResource1" Height="21px"/>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                   <td align="left" class="style3">
                                                                        <asp:Label ID="lblToTitle" runat="server" CssClass="formtext" meta:resourcekey="lblToTitleResource1" Text="To:"/>
                                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                               
                                                                    </td>
                                                                    <td align="left">
                                                                        <table border="0px" cellpadding="1" cellspacing="1">
                                                                            <tr>
                                                                                <td style="height: 21px;Width:85px">
                                                                                    <ISWebInput:WebInput ID="txtTo" runat="server" Width="176px"  Height="17px" Wrap="Off" MaxDate="12/31/9998 23:59:59" meta:resourcekey="txtToResource2" MinDate="1753-01-01">
                                                                                        <CultureInfo CultureName="en-US"/>
                                                                                        <DisplayFormat>
                                                                                            <ErrorWindowInfo IsEnabled="False"/>
                                                                                        </DisplayFormat>
                                                                                        <HighLight IsEnabled="True" Type="Phrase"/>
                                                                                        <EditFormat>
                                                                                            <ErrorWindowInfo IsEnabled="False"/>
                                                                                        </EditFormat>
					                                                                    <DateTimeEditor IsEnabled="True" AccessKey="Space"/>
                                                                                    </ISWebInput:WebInput>
                                                                                </td>
                                                                                <td style="height: 21px" >
                                                                                    <asp:DropDownList ID="cboHoursTo" runat="server" CssClass="RegularText" Width="75px" meta:resourcekey="cboHoursToResource1" Height="21px"/>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="left" class="style1">
                                                                        <asp:Label ID="lblFleetTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFleetTitleResource1" Text="Fleet:"/>
                                                                    </td>
                                                                    <td align="left" class="style2">
                                                                        <asp:DropDownList ID="cboFleet" runat="server" AutoPostBack="True" CssClass="RegularText" DataTextField="FleetName" DataValueField="FleetId" meta:resourcekey="cboFleetResource1" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged" Width="257px"/>
                                                                    </td>
                                                                    <td align="left" class="style3">
                                                                        <asp:Label ID="lblVehicleName" runat="server" CssClass="formtext" meta:resourcekey="lblVehicleNameResource1" Text=" Vehicle:" Width="30px"/>
                                                                    </td>
                                                                    <td align="left">
                                                                        <asp:DropDownList ID="cboVehicle" runat="server" CssClass="RegularText" DataTextField="Description" DataValueField="VehicleId" meta:resourcekey="cboVehicleResource1" Width="256px"/>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="left" valign="top" colspan="4">&nbsp;</td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="right" colspan="4" valign="top">
                                                                        <asp:Button ID="cmdViewDTCCodes" runat="server" CssClass="combutton" Text="View Codes" onclick="cmdViewDTCCodes_Click"/>
                                                                        &nbsp;&nbsp;&nbsp;
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </fieldset>
                                                        <br/>
                                                        <ISWebGrid:WebGrid ID="dgDTCCode" runat="server" Height="350px" UseDefaultStyle="True" Width="100%" oninitializedatasource="dgDTCCode_InitializeDataSource">
                                                            <layoutsettings allowexport="Yes" allowfilter="Yes" allowsorting="Yes" autofitcolumns="True"/>
                                                            <roottable>
                                                                <Columns>
                                                                    <ISWebGrid:WebGridColumn DataMember="MaxDateTime" Caption="Last Updated" DataType="System.DateTime" Name="MaxDateTime" Width="60px"/>
                                                                    <ISWebGrid:WebGridColumn DataMember="Description" Caption="Vehicle" Name="Description" Width="70px"/>
                                                                    <ISWebGrid:WebGridColumn DataMember="counter" Caption="Count" Name="counter" Width="30px"/>
                                                                    <ISWebGrid:WebGridColumn DataMember="Translation" Name="Translation" Caption="Code (DTC / MID PID FMI)" Width="130px"/>
                                                                </Columns>
                                                            </roottable>
                                                        </ISWebGrid:WebGrid>
                                                    </div>
                                                </asp:View>
                                            </asp:MultiView>            
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <div>
        </div>
    <asp:XmlDataSource ID="XmlOperationType" runat="server"/>
    </form>
</body>
</html>
