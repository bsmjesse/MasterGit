<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmEditFactorValues.aspx.cs"
    Inherits="SentinelFM.Configuration_Equipment_frmEditFactorValues" %>

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
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"
        Style="text-decoration: underline">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="cboFleet">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlCopy" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnSave">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAll" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnSaveCopy">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlCopy" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnAssignNew">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAll" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnCopy">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAll" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            <telerik:AjaxSetting AjaxControlID="btnYes">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAll" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <asp:Panel ID="pnlAll" runat="server">
        <table width="100%">
            <tr>
                <td align="center">
                    <table>
                        <tr>
                            <td align="center">
                                <table>
                                    <tr align="center" valign="top">
                                        <td align="left">
                                            <asp:Label ID="lblVehicle" CssClass="formtext" runat="server" Text="Vehicle:" Font-Bold="true"
                                                meta:resourcekey="lblVehicleResource1"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblVehicleNameText" CssClass="formtext" runat="server" Text="" meta:resourcekey="lblVehicleNameTextResource1"></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblBoxID" CssClass="formtext" runat="server" Text="Box ID:" Font-Bold="true"
                                                meta:resourcekey="lblBoxIDResource1"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblBoxIDName" CssClass="formtext" runat="server" Text="" meta:resourcekey="lblBoxIDNameResource1"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="left" valign="top">
                                        <td align="left">
                                            <asp:Label ID="lblEquipment" CssClass="formtext" runat="server" Text="Equipment:"
                                                Font-Bold="true" meta:resourcekey="lblEquipmentResource1"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblEquipmentName" CssClass="formtext" runat="server" Text="" meta:resourcekey="lblEquipmentNameResource1"></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblMedia" CssClass="formtext" runat="server" Text="Media:" Font-Bold="true"
                                                meta:resourcekey="lblMediaResource1"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblMediaName" CssClass="formtext" runat="server" Text="" meta:resourcekey="lblMediaNameResource1"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <table>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            <asp:Label ID="lblOld" CssClass="formtext" runat="server" Font-Bold="true" Text="Old Values"
                                                meta:resourcekey="lblOldResource1"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label1" CssClass="formtext" runat="server" Font-Bold="true" Text="New Values"
                                                meta:resourcekey="lblOldResource1"></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr align="left">
                                        <td style="border: 0px">
                                            <asp:Label ID="lblFactorName1" CssClass="formtext" runat="server" Visible="false"
                                                Font-Bold="true" meta:resourcekey="lblFactorName1Resource1"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="lblFactor1" CssClass="formtext" runat="server" Text="" Visible="false"
                                                ReadOnly="true" meta:resourcekey="lblFactor1Resource1"></asp:TextBox>
                                        </td>
                                        <td style="border: 0px">
                                            <telerik:RadNumericTextBox IncrementSettings-InterceptArrowKeys="true" IncrementSettings-InterceptMouseWheel="true"
                                                runat="server" ID="txtFactor1" Width="100px" meta:resourcekey="txtFactor1Resource1"
                                                Visible="false" Culture="en-CA">
                                                <NumberFormat AllowRounding="False" KeepNotRoundedValue="False" />
                                                <NumberFormat DecimalDigits="5"></NumberFormat>
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td align="left">
                                           <asp:Label ID="lblUnit1" runat="server"  Visible="false" meta:resourcekey="lblUnit1Resource1"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="left">
                                        <td style="border: 0px">
                                            <asp:Label ID="lblFactorName2" CssClass="formtext" runat="server" Visible="False"
                                                Font-Bold="true" meta:resourcekey="lblFactorName2Resource1"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="lblFactor2" CssClass="formtext" runat="server" Text="" Visible="false"
                                                ReadOnly="true" meta:resourcekey="lblFactor2Resource1"></asp:TextBox>
                                        </td>
                                        <td style="border: 0px">
                                            <telerik:RadNumericTextBox IncrementSettings-InterceptArrowKeys="true" IncrementSettings-InterceptMouseWheel="true"
                                                runat="server" ID="txtFactor2" Width="100px" meta:resourcekey="txtFactor2Resource1"
                                                Visible="False" Culture="en-CA">
                                                <NumberFormat AllowRounding="False" KeepNotRoundedValue="False" />
                                                <NumberFormat DecimalDigits="5"></NumberFormat>
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td align="left">
                                           <asp:Label ID="lblUnit2" runat="server" Visible="false" meta:resourcekey="lblUnit2Resource1"></asp:Label>
                                        </td>

                                    </tr>
                                    <tr align="left">
                                        <td style="border: 0px">
                                            <asp:Label ID="lblFactorName3" CssClass="formtext" runat="server" Visible="False"
                                                Font-Bold="true" meta:resourcekey="lblFactorName3Resource1"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="lblFactor3" CssClass="formtext" runat="server" Text="" Visible="false"
                                                ReadOnly="true" meta:resourcekey="lblFactor3Resource1"></asp:TextBox>
                                        </td>
                                        <td style="border: 0px">
                                            <telerik:RadNumericTextBox IncrementSettings-InterceptArrowKeys="true" IncrementSettings-InterceptMouseWheel="true"
                                                runat="server" ID="txtFactor3" Width="100px" meta:resourcekey="txtFactor3Resource1"
                                                Visible="False" Culture="en-CA">
                                                <NumberFormat AllowRounding="False" KeepNotRoundedValue="False" />
                                                <NumberFormat DecimalDigits="5"></NumberFormat>
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td align="left">
                                           <asp:Label ID="lblUnit3" runat="server" Visible="false" meta:resourcekey="lblUnit3Resource1"></asp:Label>
                                        </td>

                                    </tr>
                                    <tr align="left">
                                        <td style="border: 0px">
                                            <asp:Label ID="lblFactorName4" CssClass="formtext" runat="server" Visible="False"
                                                Font-Bold="true" meta:resourcekey="lblFactorName4Resource1"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="lblFactor4" CssClass="formtext" runat="server" Text="" Visible="false"
                                                ReadOnly="true" meta:resourcekey="lblFactor4Resource1"></asp:TextBox>
                                        </td>
                                        <td style="border: 0px">
                                            <telerik:RadNumericTextBox IncrementSettings-InterceptArrowKeys="true" IncrementSettings-InterceptMouseWheel="true"
                                                runat="server" ID="txtFactor4" Width="100px" meta:resourcekey="txtFactor4Resource1"
                                                Visible="False" Culture="en-CA">
                                                <NumberFormat AllowRounding="False" KeepNotRoundedValue="False" />
                                                <NumberFormat DecimalDigits="5"></NumberFormat>
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td align="left">
                                           <asp:Label ID="lblUnit4" Visible="false" runat="server"  meta:resourcekey="lblUnit4Resource1"></asp:Label>
                                        </td>

                                    </tr>
                                    <tr align="left">
                                        <td style="border: 0px">
                                            <asp:Label ID="lblFactorName5" CssClass="formtext" runat="server" Visible="False"
                                                Font-Bold="true" meta:resourcekey="lblFactorName5Resource1"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="lblFactor5" CssClass="formtext" runat="server" Text="" Visible="false"
                                                ReadOnly="true" meta:resourcekey="lblFactor5Resource1"></asp:TextBox>
                                        </td>
                                        <td style="border: 0px">
                                            <telerik:RadNumericTextBox IncrementSettings-InterceptArrowKeys="true" IncrementSettings-InterceptMouseWheel="true"
                                                runat="server" ID="txtFactor5" Width="100px" meta:resourcekey="txtFactor5Resource1"
                                                Visible="False" Culture="en-CA">
                                                <NumberFormat AllowRounding="False" KeepNotRoundedValue="False" />
                                                <NumberFormat DecimalDigits="5"></NumberFormat>
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td align="left">
                                           <asp:Label ID="lblUnit5" Visible="false" runat="server"  meta:resourcekey="lblUnit5Resource1"></asp:Label>
                                        </td>

                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btnSave" runat="server" Text="Update" meta:resourcekey="btnSaveResource1" CssClass="combutton"
                                                Visible="true" ValidationGroup="valGPAdd" OnClick="btnSave_Click" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btnCopy" runat="server" Text="Update To..." meta:resourcekey="btnCopyResource1" CssClass="combutton"
                                                Visible="true"  
                                                onclick="btnCopy_Click" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btnAssignNew" runat="server" Text="Assign New..." meta:resourcekey="btnAssignNewResource1" CssClass="combutton"
                                                Visible="true" 
                                                onclick="btnAssignNew_Click" />
                                        </td>

                                        <td>
                                            <asp:Button ID="BtnCancel" runat="server" Text="Cancel" meta:resourcekey="BtnCancelResource1"
                                                OnClientClick="javascript:return returnToParent();" CssClass="combutton" />
                                            <asp:HiddenField ID="hidCopy" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Panel ID="pnlCopy" runat="server" Style="visibility: hidden">
                        <fieldset>
                            <legend style="color: green" runat="server" id="legendID"></legend>
                            <table>
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="lblFleet" runat="server" CssClass="formtext" Width="70px" meta:resourcekey="lblFleetResource1"
                                            Font-Bold="true" Text="Fleet:"></asp:Label>
                                    </td>
                                    <td style="width: 312px;" align="left">
                                        <telerik:RadComboBox ID="cboFleet" runat="server" CssClass="RegularText" Width="258px"
                                            DataTextField="FleetName" DataValueField="FleetId" meta:resourcekey="cboFleetResource1"
                                            Skin="Hay" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged" AutoPostBack="true"
                                            MaxHeight="200px">
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" class="formtext" style="width: 52px">
                                        <asp:Label ID="lblVehicleName" runat="server" CssClass="formtext" meta:resourcekey="lblVehicleNameResource1"
                                            Font-Bold="true" Text="Vehicle:"></asp:Label>
                                    </td>
                                    <td style="width: 300px" align="left">
                                        <telerik:RadComboBox ID="cboVehicle" runat="server" CssClass="RegularText" Width="258px"
                                            meta:resourcekey="cboVehicleResource1" MaxHeight="200px" DataTextField="Description"
                                            DataValueField="VehicleId" EmptyMessage="Select Vehicle(s)" AllowCustomText="true"
                                            Skin="Hay">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkVehicle" Checked="false" />
                                                <asp:Label runat="server" ID="lblDescription" Text='<%# Eval("Description") %>'></asp:Label>
                                            </ItemTemplate>
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" align="center">
                                        <asp:Button ID="btnSaveCopy" runat="server" Text="Save" meta:resourcekey="btnSaveCopyResource1" CssClass="combutton"
                                            OnClientClick="if (!GetCheckValue()) return false;" OnClick="btnSaveCopy_Click" />
                                        <asp:Button ID="btnCancelCopy" runat="server" Text="Cancel" meta:resourcekey="btnCancelResource1" CssClass="combutton"
                                            OnClientClick="javascript:return ClickCancelCopy()" />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
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
                                if ($telerik.$(this).attr("disabled") == false)
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
                if ($telerik.$("#<%= hidCopy.ClientID%>").val() == "1") {
                    ClickCopy();
                }
                else {
                    ClickCancelCopy();
                }
            }
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function returnToParent() {
                var oWnd = GetRadWindow();
                oWnd.BrowserWindow.refreshGrid('Rebind');
                oWnd.close();
                return false;
            }

            function ClickCopy() {
                $telerik.$("#<%= hidCopy.ClientID%>").val("1");
                $telerik.$("#<%= pnlCopy.ClientID%>").css("visibility", "visible");
                GetRadWindow().SetHeight(480);
                return false;
            }

            function ClickCancelCopy() {
                $telerik.$("#<%= hidCopy.ClientID%>").val("");
                $telerik.$("#<%= pnlCopy.ClientID%>").css("visibility", "hidden");
                GetRadWindow().SetHeight(350);
                return false;
            }

            function GetCheckValue() {
                var checkedEle = $telerik.$("input:checkbox[name$='chkVehicle']:checked");
                if (checkedEle.length <= 0) {
                    alert("<%= selectVehicleVal %>");
                    return false;
                }
                else {
                    var vCount = 0;
                    var suffix = "i0_chkVehicle";
                    
                    //for confirm window
                    $telerik.$('#selectedVehicle option').remove();
                    //

                    for (var index = 0; index < checkedEle.length; index++) {
                        var checkID = $telerik.$(checkedEle[index]).attr("id");
                        if (checkID.substring(checkID.length - suffix.length) != suffix) {
                            vCount = vCount + 1;

                            //for confirm window
                            $telerik.$('#selectedVehicle').append('<option>' + $telerik.$(checkedEle[index]).next("span").text() + '</option>');
                            //
                        }
                    }
                    if (vCount == 0) return false;
                    else {
                        var vNames = '<%= overwriteStr2 %>';
                        if (vCount == 1) vNames = '<%= overwriteStr1 %>';
                        if ($telerik.$("#<%= legendID.ClientID%>").text() == "<%= assignTo%>")
                        {
                         if (vCount == 1) vNames = '<%= assignNewStr2 %>';
                         else vNames = '<%= assignNewStr1 %>';
                        }

                        vNames = vNames.replace('(n)', '(' + vCount + ')')
                        OpenConfirm(vNames);
                        return false;

                        var answer = confirm(vNames.replace('(n)', '(' + vCount + ')'));
                        if (answer) {
                            return true;
                        }
                    }

                }
                return false;
            }

            function ResetCheckValue() {
                $telerik.$("input:checkbox[name$='chkVehicle']:checked").attr("checked", false);
            }

            function CLoseConfirm() {
                $find('<%= RadWindowContentTemplate.ClientID%>').close();
                return false;
            }

            function YesCLoseConfirm() {
                $find('<%= RadWindowContentTemplate.ClientID%>').close();

                return true;
            }

            function OpenConfirm(vNames) {
                $telerik.$("#spanHeader").text(vNames);
                $find('<%= RadWindowContentTemplate.ClientID%>').show();
            }


        </script>
    </telerik:RadCodeBlock>

    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true"
        EnableAriaSupport="true">
        <Windows>
            <telerik:RadWindow ID="RadWindowContentTemplate" Title="Confirm Copy" VisibleStatusbar="false" VisibleTitlebar="false"
                Width="400px" Height="250px" runat="server" ReloadOnShow="true" ShowContentDuringLoad="false" Skin="Default" Modal="true"
                meta:resourcekey="RadWindowContentTemplateResource1"  >
                <ContentTemplate>
                   <table width="100%" >
                      <tr>
                         <td align="center">
                            <span id="spanHeader" class="formtext"  ></span>
                         </td>
                      </tr>
                      <tr>
                         <td align="center"  >
                             <select id="selectedVehicle" size ="10" style="width:280px" class="formtext" ></select>
                         </td>
                      </tr>
                      <tr>
                         <td align="center">
                             <asp:Button ID="btnYes" runat="server" OnClientClick="if (!YesCLoseConfirm()) return false" OnClick="btnSaveCopy_Click"  meta:resourcekey="btnYesResource1" Text="Yes" CssClass="combutton" />
                             <asp:Button ID="btnNo" runat="server"   meta:resourcekey="btnNoResource1" Text="No" OnClientClick="javascript:return CLoseConfirm();" CssClass="combutton"/>
                         </td>
                      </tr>
                   </table>
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>

    </form>
</body>
</html>
