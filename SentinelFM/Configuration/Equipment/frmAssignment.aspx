<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmAssignment.aspx.cs" Inherits="SentinelFM.Configuration_Equipment_frmAssignment" meta:resourcekey="PageResource1" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
            <telerik:AjaxSetting AjaxControlID="btnSave">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAdd" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div>
        <table>
            <tr>
                <td>
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
                        <tr valign="top">
                           <td>
                                <asp:Label ID="lblEquipment" runat="server" CssClass="formtext" Width="70px" meta:resourcekey="lblEquipmentResource1"
                                    Text="Equipment:"></asp:Label>

                           </td>
                           <td>
                                <telerik:RadComboBox ID="cboEquipment" runat="server" CssClass="RegularText" Width="258px"  CausesValidation ="true"
                                    DataTextField="Description" DataValueField="EquipmentId" meta:resourcekey="cboEquipmentResource1"
                                    Skin="Hay"   MaxHeight ="150px"
                                    >
                                </telerik:RadComboBox>
                                <br />
                               <asp:RequiredFieldValidator ID="valReqcboEquipment" runat="server" ValidationGroup="vgAdd"
                                  ControlToValidate="cboEquipment" meta:resourcekey="valReqcboEquipmentResource1"
                                  Text="Please select a equipment"  ></asp:RequiredFieldValidator>
                                
                           </td>
                           <td>
                                <asp:Label ID="lblMedia" runat="server" CssClass="formtext" Width="33px" meta:resourcekey="lblMediaResource1"
                                    Text="Media:"></asp:Label>

                           </td>
                           <td>
                                <telerik:RadComboBox ID="cboMedia" runat="server" CssClass="RegularText" Width="258px"
                                    DataTextField="Description" DataValueField="MediaId" meta:resourcekey="cboMediaResource1"
                                    Skin="Hay"   MaxHeight ="150px" EmptyMessage="Select Media(s)" AllowCustomText ="true"
                                    >
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" ID="chkMedia" Checked="false" />
                                        <asp:Label runat="server" ID="lblMediaDescription" Text='<%# Eval("Description") %>'></asp:Label>
                                    </ItemTemplate>

                                </telerik:RadComboBox>
                                <br />
                                <asp:CustomValidator ID="cvcboMedia" runat="server" ClientValidationFunction="CustomValidateMedia"
                                    EnableClientScript="true" ValidationGroup="vgAdd" Text="Please select media(s)" 
                                    meta:resourcekey="cvcboMediaResource1" 
                                    OnServerValidate="ServerValidateMedia"
                                />

                           </td>

                        </tr>
                        <tr>
                           <td colspan = "4" align="center">
                              <asp:Button id="btnSave" runat = "server" Text = "Save" 
                                   meta:resourcekey="btnSaveResource1" ValidationGroup="vgAdd" 
                                   onclick="btnSave_Click" CssClass="combutton" />
                              <asp:Button id="btnClose" runat = "server" Text = "Close" 
                                   meta:resourcekey="btnCloseResource1" 
                                   OnClientClick="javascript:return returnToParent();" CssClass="combutton" />

                               <br />
                               <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Visible="true" meta:resourcekey="lblMessageResource1"></asp:Label>
                           </td>
                        </tr>
                    </table>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
       <script type="text/javascript">
           function CustomValidateVehicle(sender, args) {
               args.IsValid = true;
               var checkedEle = $telerik.$("input:checkbox[name$='chkVehicle']:checked");
               if (checkedEle.length <= 0) args.IsValid = false;
           }

           function CustomValidateMedia(sender, args) {
               args.IsValid = true;
               var checkedEle = $telerik.$("input:checkbox[name$='chkMedia']:checked");
               if (checkedEle.length <= 0) args.IsValid = false;
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

               var suffix_m = "i0_chkMedia";
               $telerik.$("input:checkbox[name$='chkMedia']").click(function () {
                   var checkID = $telerik.$(this).attr("id");
                   if (checkID.substring(checkID.length - suffix_m.length) == suffix_m) {
                       if ($telerik.$(this).attr("checked")) {
                           $telerik.$("input:checkbox[name$='chkMedia']").each(function () {
                               $telerik.$(this).attr("checked", true);
                           });
                       }
                       else {
                           $telerik.$("input:checkbox[name$='chkMedia']").each(function () {
                               $telerik.$(this).attr("checked", false);
                           });
                       }
                   }
                   else {
                       if (!($telerik.$(this).attr("checked"))) {
                           $telerik.$("input:checkbox[id$='" + suffix_m + "']:checked").each(function () {
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

           function returnToParent() {
               var oWnd = GetRadWindow();
               oWnd.BrowserWindow.refreshGrid('navigateToInserted');
               oWnd.close();
               return false;
           }
       </script>
    </telerik:RadCodeBlock>
    </form>
</body>
</html>
