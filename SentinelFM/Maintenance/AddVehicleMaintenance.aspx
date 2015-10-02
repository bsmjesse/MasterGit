<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddVehicleMaintenance.aspx.cs" Inherits="SentinelFM.Maintenance_AddVehicleMaintenance" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" 
        meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" 
        meta:resourcekey="RadAjaxManager1Resource1" style="text-decoration: underline">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="cboFleet">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAdd" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            
            <telerik:AjaxSetting AjaxControlID="combMccGroup">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAdd" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            <telerik:AjaxSetting AjaxControlID="btnSave">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAdd" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>

    <asp:Panel ID="pnlAdd" runat ="server" >
    <table>
        <tr valign="top">
            <td align="left" >
                <asp:Label ID="lblFleet" runat="server" CssClass="formtext" Width="70px" meta:resourcekey="lblFleetResource1"
                    Text="Fleet:"></asp:Label>
            </td>
            <td style="width: 312px;" align="left">
                <telerik:RadComboBox ID="cboFleet" runat="server" CssClass="RegularText" Width="258px"
                    DataTextField="FleetName" DataValueField="FleetId" meta:resourcekey="cboFleetResource1"
                    Skin="Hay"  onselectedindexchanged="cboFleet_SelectedIndexChanged"
                    AutoPostBack = "true" MaxHeight ="200px" 
                    >
                </telerik:RadComboBox>
            </td>
            <td class="formtext" style="width: 52px">
                <asp:Label ID="lblVehicleName" runat="server" CssClass="formtext" meta:resourcekey="lblVehicleNameResource1"
                    Text="Vehicle:"></asp:Label>
            </td>
            <td style="width: 300px" align="left">
                <telerik:RadComboBox ID="cboVehicle" runat="server" CssClass="RegularText" Width="258px"
                    meta:resourcekey="cboVehicleResource1" MaxHeight ="200px" DataTextField="Description" DataValueField="VehicleId"
                    EmptyMessage="Select Vehicle(s)" AllowCustomText ="true" 
                    Skin="Hay" >
                    <ItemTemplate>
                        <asp:CheckBox runat="server" ID="chkVehicle" Checked="false"  />
                        <asp:Label runat="server" ID="lblDescription" Text='<%# Eval("Description") %>'></asp:Label>
                    </ItemTemplate>
                </telerik:RadComboBox>
                <asp:CustomValidator ID="cvcboVehicle" runat="server" ClientValidationFunction="CustomValidateVehicle"
                    EnableClientScript="true" ValidationGroup="vgAdd" Text="Please select vehicle(s)" 
                    meta:resourcekey="cvcboVehicleResource1" 
                    OnServerValidate="ServerValidateVehicle"
                />
            </td>
        </tr>
        <tr>
            <td align="left" >
                <asp:Label ID="lblMccGroup" runat="server" CssClass="formtext" Width="70px" meta:resourcekey="lblMccGroupResource1"
                    Text="MCC Group:"></asp:Label>
            </td>
            <td  align="left" colspan="3">
                <telerik:RadComboBox ID="combMccGroup" runat="server" CssClass="RegularText" Width="258px"
                    DataTextField="MccName" DataValueField="MccId" meta:resourcekey="combMccGroupResource1"
                    Skin="Hay"  
                    AutoPostBack = "true" MaxHeight ="200px" onselectedindexchanged="combMccGroup_SelectedIndexChanged" 
                    >
                </telerik:RadComboBox>
                <asp:CustomValidator ID="cvcombMccGroup" runat="server" ClientValidationFunction="CustomValidateMccGroup"
                    EnableClientScript="true" ValidationGroup="vgAdd" Text="Please select MCC group" 
                    meta:resourcekey="cvcombMccGroupResource1" 
                ></asp:CustomValidator>

            </td>
          
        </tr>
        <tr>
           <td colspan="4">
  <telerik:RadGrid AutoGenerateColumns="False" ID="gdMCCMaintenances" 
                   AllowAutomaticDeletes="false" Visible="false"
                                                        AllowAutomaticInserts="false" 
                   AllowSorting="true" AllowAutomaticUpdates="false"
                                                        AllowPaging="True" runat="server" 
                   Skin="Simple" GridLines="None" Width="700px" 
                                                         AllowFilteringByColumn="false" 
                   FilterItemStyle-HorizontalAlign="Left" onitemdatabound="gdMCCMaintenances_ItemDataBound"
                                                        >
                                                        <GroupingSettings CaseSensitive="false" />
                                                        <MasterTableView DataKeyNames="MaintenanceId" ClientDataKeyNames="MaintenanceId"
                                                            >
                                                            <Columns>
                                                                <telerik:GridTemplateColumn DataField="Id" UniqueName="SelectSource">
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkSelectMaintenance" runat="server" onclick="CheckSelectedMaintenance(this)" />
                                                                    </ItemTemplate>
                                                                    <HeaderTemplate>
                                                                        <input type='checkbox' id="chkMccAllMaintenances" onclick="MccCheckAllMaintenances(this)" />
                                                                    </HeaderTemplate>
                                                                    <ItemStyle Width="30px" HorizontalAlign="Center"></ItemStyle>
                                                                    <HeaderStyle Width="30px" HorizontalAlign="Center" />
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
                                                                    <ItemStyle Width="150px" />
                                                                    <HeaderStyle Width="150px" />
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn HeaderText="Notification Type" UniqueName="NotificationType"
                                                                    DataField="NotificationDescription" SortExpression="NotificationDescription"
                                                                    meta:resourcekey="gdMCCMaintenancesNotificationTypeResource1" AllowFiltering="true"
                                                                    >
                                                                    <ItemStyle Width="150px" />
                                                                    <HeaderStyle Width="150px" />
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn HeaderText="Frequency ID" UniqueName="FrequencyID" SortExpression="FrequencyID"
                                                                    meta:resourcekey="gdMCCMaintenancesFrequencyIDResource1" AllowFiltering="true"
                                                                    DataField="FrequencyID" DataType="System.Int16">
                                                                    <ItemStyle Width="100px" />
                                                                    <HeaderStyle Width="100px" />
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn HeaderText="Interval" UniqueName="Interval " SortExpression="Interval "
                                                                    meta:resourcekey="gdMCCMaintenancesIntervalResource1" AllowFiltering="true" DataField="Interval "
                                                                    DataType="System.Int16">
                                                                    <ItemStyle Width="100px" />
                                                                    <HeaderStyle Width="100px" />
                                                                </telerik:GridBoundColumn>
                                                            </Columns>
                                                            <HeaderStyle CssClass="RadGridtblHeader" />
                                                        </MasterTableView>
                                                        <ClientSettings Scrolling-AllowScroll = "true" >
                                                            <ClientEvents OnGridCreated="GridCreated" />
                                                        </ClientSettings>
                                                    </telerik:RadGrid>
                                                                                <asp:CustomValidator ID="cvgdMCCMaintenances" runat="server" ClientValidationFunction="CustomValidateMCCMaintenances"
                    EnableClientScript="true" ValidationGroup="vgAdd" Text="Please select MCC maintenance" 
                    meta:resourcekey="cvgdMCCMaintenancesResource1"  OnServerValidate="ServerValidateMCCMaintenances"
                ></asp:CustomValidator>

           </td>
        </tr>
        <tr>
            <td colspan = "4" align="center">
                <asp:Button id="btnSave" runat = "server" Text = "Assign" ValidationGroup="vgAdd"
                    meta:resourcekey="btnSaveResource1" 
                    onclick="btnSave_Click" CssClass="combutton" />
                <asp:Button id="btnClose" runat = "server" Text = "Close" 
                    meta:resourcekey="btnCloseResource1" 
                    OnClientClick="javascript:return returnToMCCVehicleMaintenance();" CssClass="combutton" />

                <br />
                <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Visible="true" meta:resourcekey="lblMessageResource1"></asp:Label>
            </td>
        </tr>

    </table>
    </asp:Panel>

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
       <script type="text/javascript">
           function GridCreated() {
               var masterTable = $find("<%=gdMCCMaintenances.ClientID%>").get_masterTableView();
               $telerik.$(masterTable.get_element()).find("tr:last").find("td").css("border-bottom", "2px solid #e4e4e4");
           }

           function CustomValidateVehicle(sender, args) {
               args.IsValid = true;
               var checkedEle = $telerik.$("input:checkbox[name$='chkVehicle']:checked");
               if (checkedEle.length <= 0) args.IsValid = false;
           }

           function CustomValidateMccGroup(sender, args) {
               args.IsValid = true;
               if ($find("<%= combMccGroup.ClientID %>").get_selectedIndex() <= 0) args.IsValid = false;
           }

           function FindSelectedMaintenances() {
               var tableView = $find("<%=gdMCCMaintenances.ClientID%>").get_masterTableView();
               var count = tableView.get_dataItems().length;
               var item;
               var ServiceTypeIDs = '';
               for (var i = 0; i < count; i++) {
                   item = tableView.get_dataItems()[i];
                   var key = item.getDataKeyValue("MaintenanceId");
                   var selectCell = tableView.getCellByColumnUniqueName(item, "SelectSource");


                   if ($telerik.$(selectCell).find(":checkbox").attr("checked") == true) {
                       if (ServiceTypeIDs == '') ServiceTypeIDs = key;
                       else ServiceTypeIDs = ServiceTypeIDs + "," + key;
                   }

               }

               return ServiceTypeIDs;
           }

           function CustomValidateMCCMaintenances(sender, args) {
               args.IsValid = true;
               if ($find("<%=gdMCCMaintenances.ClientID%>") == null) return;
               if (FindSelectedMaintenances() == '') args.IsValid = false;
           }

           $telerik.$(document).ready(function () {
               SetCheckBox();
           });
           function SetCheckBox() {
               var suffix = "i0_chkVehicle";
               $telerik.$("input:checkbox[name$='chkVehicle']").click(function () {
                   var checkID = $telerik.$(this).attr("id");
                   if (checkID.substring(checkID.length - suffix.length) == suffix) {
                       if ($telerik.$(this).attr("checked")) {
                           $telerik.$("input:checkbox[name$='chkVehicle']").each(function () {
                               $telerik.$(this).attr("checked", true);
                           });
                       }
                       else {
                           $telerik.$("input:checkbox[name$='chkVehicle']").each(function () {
                               $telerik.$(this).attr("checked", false);
                           });
                       }
                   }
                   else {
                       if (!($telerik.$(this).attr("checked"))) {
                           $telerik.$("input:checkbox[id$='" + suffix + "']:checked").each(function () {
                               $telerik.$(this).attr("checked", false);
                           });
                       }
                   }
               });
           }
           function GetRadWindow() {
               var oWindow = null;
               if (window.radWindow) oWindow = window.radWindow;
               else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
               return oWindow;
           }

           function returnToMCCVehicleMaintenance() {
               var oWnd = GetRadWindow();
               oWnd.BrowserWindow.RefreshMCCVehicleMaintenance();
               oWnd.close();
               return false;
           }

           function MccCheckAllMaintenances(ctl) {
              $telerik.$("input:checkbox[id$='chkSelectMaintenance']").attr("checked", $telerik.$(ctl).attr("checked"));
           }

           function CheckSelectedMaintenance(ctl) {
               if (!($telerik.$(ctl).attr("checked"))) {
                   $telerik.$("input:checkbox[id$='chkMccAllMaintenances']:checked").attr("checked", false);
                }
           }

       </script>
       </telerik:RadCodeBlock>
    </form>
</body>
</html>