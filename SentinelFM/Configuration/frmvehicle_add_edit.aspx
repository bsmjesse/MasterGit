<%@ Page Language="c#" Inherits="SentinelFM.frmVehicle_Add_Edit" CodeFile="frmVehicle_Add_Edit.aspx.cs"
    Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register Assembly="ISNet.WebUI.WebCombo" Namespace="ISNet.WebUI.WebCombo" TagPrefix="ISWebCombo" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="Head1" runat="server">
    <title>Vehicle Info</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../Styles/SentinelFM/css/ddslick.css" type="text/css" rel="stylesheet">
    <link rel="stylesheet" href="../scripts/tablesorter/themes/report/style.css" type="text/css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js"></script>
    <script type="text/javascript" src="../Scripts/jquery.ddslick.js"></script>

    <script language="javascript">
		<!--
    var pWin
    function setParent() {
        pWin = top.window.opener
    }
    function reloadParent() {
        pWin.location.href = 'frmVehicleInfo.aspx'
    }
    //-->
    </script>

    <style type="text/css">
        .formtext {
        }

        .style1 {
            width: 112px;
        }
    </style>
    <script type="text/javascript">
        var SelectIcon = "<%=SelectIcon %>";
    </script>

</head>
<body onload="setParent()" onunload="reloadParent()">
    <script type="text/javascript">
        var icondata = <%= CustomIcon %> ;

        $(document).ready(function () {
            $('#customiconDropdown').ddslick({
                data: icondata,
                width: 200,
                height: 200,
                imagePosition: "left",
                selectText: SelectIcon,
                onSelected: function (data) {
                    //alert(data.value);
                    $('#<%=CustomIconPath.ClientID %>').val(data.selectedData.value);
                }
            });
    });
    </script>
    <form id="frmVehicle_Add_EditForm" method="post" runat="server">
        <asp:HiddenField ID="CustomIconPath" runat="server" Value="" />
        <table id="tblCommands" border="0" cellpadding="0" cellspacing="0" style="z-index: 101; left: 15px; width: 390px; position: absolute; top: 11px; height: 400px"
            width="390">
            <tr>
                <td>
                    <table id="tblButtons" border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <asp:Button ID="cmdInfo" runat="server" CausesValidation="False" CssClass="selectedbutton"
                                    Text="Info" Width="104px" meta:resourcekey="cmdInfoResource1" /></td>
                            <td style="width: 105px">
                                <asp:Button ID="cmdCustomFields" runat="server" CausesValidation="False" CssClass="confbutton"
                                    OnClick="cmdCustomFields_Click" Text="Custom Fields" Width="104px" meta:resourcekey="cmdCustomFieldsResource1" />
                            </td>
                            <td style="width: 105px">
                                <asp:Button ID="cmdVehicleStatus" runat="server" CausesValidation="False" CssClass="confbutton" Width="104px" CommandName="114"
                                    Text="Vehicle Status" OnClick="cmdVehicleStatus_Click" meta:resourcekey="cmdVehicleStatusResource1" />
                            </td>
                            <td style="width: 105px">
                                <asp:Button ID="cmdWorkingHours" runat="server" CausesValidation="False" CssClass="confbutton"
                                    Text="Working Hours" Width="104px" OnClick="cmdWorkingHours_Click" Visible="False"
                                    meta:resourcekey="cmdWorkingHoursResource1" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" align="center" border="0" cellpadding="0" cellspacing="0" style="width: 413px; height: 284px"
                        width="413">
                        <tr>
                            <td>
                                <table id="tblForm" border="0" class="table" height="265px" width="421px">
                                    <tr>
                                        <td align="center" class="configTabBackground">
                                            <table id="tblVehicleInfo" height="200" cellspacing="0" cellpadding="0" border="0" runat="server" width="100%">
                                                <tr>
                                                    <td class="formtext" style="height: 20px"></td>
                                                    <td>
                                                        <asp:Label ID="lblVehicleId" runat="server" Visible="False" CssClass="formtext" meta:resourcekey="lblVehicleIdResource1"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="height: 20px">
                                                        <asp:Label ID="lblDescription" runat="server" Text="Description" meta:resourcekey="lblDescriptionResource1"></asp:Label>
                                                        &nbsp;<asp:RequiredFieldValidator ID="valDescription" runat="server" CssClass="errortext"
                                                            ControlToValidate="txtDescription" ErrorMessage="Please enter a Description."
                                                            meta:resourcekey="valDescriptionResource1" Text="*"></asp:RequiredFieldValidator>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtDescription" runat="server" CssClass="formtext" Width="168px"
                                                            meta:resourcekey="txtDescriptionResource1"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext"></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="height: 20px">
                                                        <asp:Label ID="lblVIN" runat="server" Text="VIN Number:" meta:resourcekey="lblVINResource1"></asp:Label>
                                                        <asp:RequiredFieldValidator ID="valVINNumber" runat="server" CssClass="errortext"
                                                            ControlToValidate="txtVinNum" ErrorMessage="Please enter a VIN Number." meta:resourcekey="valVINNumberResource1"
                                                            Text="*"></asp:RequiredFieldValidator></td>
                                                    <td>
                                                        <asp:TextBox ID="txtVinNum" runat="server" CssClass="formtext" Width="168px" meta:resourcekey="txtVinNumResource1"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext"></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="height: 20px">
                                                        <asp:Label ID="lblLicensePlate" runat="server" Text="License Plate:" meta:resourcekey="lblLicensePlateResource1"></asp:Label>
                                                        <asp:RequiredFieldValidator ID="valLicensePlate" runat="server" CssClass="errortext"
                                                            ControlToValidate="txtLicensePlate" ErrorMessage="Please enter a License Plate."
                                                            meta:resourcekey="valLicensePlateResource1" Text="*"></asp:RequiredFieldValidator></td>
                                                    <td>
                                                        <asp:TextBox ID="txtLicensePlate" runat="server" CssClass="formtext" Width="168px"
                                                            meta:resourcekey="txtLicensePlateResource1"></asp:TextBox>
                                                        <asp:Label ID="lblOldLicense" runat="server" Visible="False" meta:resourcekey="lblOldLicenseResource1"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext"></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="height: 20px">
                                                        <asp:Label ID="lblVehicleType" runat="server" Text="Vehicle Type:" meta:resourcekey="lblVehicleTypeResource1"></asp:Label>
                                                        <asp:RangeValidator ID="valVehicleType" runat="server" ControlToValidate="cboVehicleType"
                                                            ErrorMessage="Please select a Vehicle Type" MaximumValue="999999999999999" MinimumValue="0"
                                                            meta:resourcekey="valVehicleTypeResource1" Text="*"></asp:RangeValidator></td>
                                                    <td>
                                                        <asp:DropDownList ID="cboVehicleType" SkinID="45" runat="server"
                                                            CssClass="formtext" DataTextField="VehicleTypeName"
                                                            DataValueField="VehicleTypeId" meta:resourcekey="cboVehicleTypeResource1"
                                                            Width="168px">
                                                        </asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext"></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="height: 20px">
                                                        <asp:Label ID="lblMake" runat="server" Text="Make:" meta:resourcekey="lblMakeResource1"></asp:Label>
                                                        <asp:RangeValidator ID="valMake" runat="server" ControlToValidate="cboMake" ErrorMessage="Please select a Make."
                                                            MaximumValue="999999999999999" MinimumValue="0" meta:resourcekey="valMakeResource1"
                                                            Text="*"></asp:RangeValidator></td>
                                                    <td>
                                                        <asp:DropDownList ID="cboMake" runat="server" CssClass="formtext" DataTextField="MakeName"
                                                            DataValueField="MakeId" AutoPostBack="True" OnSelectedIndexChanged="cboMake_SelectedIndexChanged"
                                                            meta:resourcekey="cboMakeResource1" Width="168px">
                                                        </asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext"></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="height: 20px">
                                                        <asp:Label ID="lblModel" runat="server" Text="Model:" meta:resourcekey="lblModelResource1"></asp:Label>
                                                        <asp:RangeValidator ID="valMoel" runat="server" ControlToValidate="cboModel" ErrorMessage="Please select a Model."
                                                            MaximumValue="999999999999999" MinimumValue="0" meta:resourcekey="valMoelResource1"
                                                            Text="*"></asp:RangeValidator></td>
                                                    <td class="formtext" style="height: 20px">
                                                        <asp:DropDownList ID="cboModel" runat="server" CssClass="formtext" DataTextField="ModelName"
                                                            DataValueField="MakeModelId" meta:resourcekey="cboModelResource1"
                                                            Width="168px">
                                                        </asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext"></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="height: 20px">
                                                        <asp:Label ID="lblClass" runat="server" Text="Class:" meta:resourcekey="lblClassResource1"></asp:Label>
                                                    <td class="formtext" style="height: 20px">
                                                        <asp:TextBox ID="txtClass" runat="server" Width="168px" meta:resourcekey="txtClassResource1" CssClass="RegularText"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext"></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="height: 20px">
                                                        <asp:Label ID="lblYear" runat="server" Text="Year:" meta:resourcekey="lblYearResource1"></asp:Label>
                                                        <asp:RequiredFieldValidator ID="valYear" runat="server" CssClass="errortext" ControlToValidate="txtYear"
                                                            ErrorMessage="Please enter a vehicle year." meta:resourcekey="valYearResource1"
                                                            Text="*"></asp:RequiredFieldValidator>
                                                        <asp:RangeValidator ID="valYearRng" runat="server" ControlToValidate="txtYear" ErrorMessage="Please re-enter a vehicle year."
                                                            MaximumValue="2100" MinimumValue="1950" Type="Integer" meta:resourcekey="valYearRngResource1"
                                                            Text="*"></asp:RangeValidator></td>
                                                    <td class="formtext" style="height: 20px">
                                                        <asp:TextBox ID="txtYear" runat="server" CssClass="formtext" MaxLength="4" Width="168px"
                                                            meta:resourcekey="txtYearResource1"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext"></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="height: 27px">
                                                        <asp:Label ID="lblColor" runat="server" Text="Color:" meta:resourcekey="lblColorResource1"></asp:Label></td>
                                                    <td style="height: 27px">
                                                        <asp:TextBox ID="txtColor" runat="server" CssClass="formtext" Width="168px" meta:resourcekey="txtColorResource1"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext"></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="height: 20px">
                                                        <asp:Label ID="lblBox" runat="server" Text="Box ID:" meta:resourcekey="lblBoxResource1"></asp:Label>
                                                        <asp:RequiredFieldValidator ID="valBoxReq" runat="server" ControlToValidate="cboBox"
                                                            ErrorMessage="Please select a Box." meta:resourcekey="valBoxReqResource1" Text="*"></asp:RequiredFieldValidator>
                                                        <asp:RangeValidator ID="valBox" runat="server" ControlToValidate="cboBox" ErrorMessage="Please select a Box."
                                                            MaximumValue="999999999999999" MinimumValue="1" meta:resourcekey="valBoxResource1"
                                                            Text="*"></asp:RangeValidator></td>
                                                    <td>
                                                        <asp:DropDownList ID="cboBox" runat="server" CssClass="formtext" DataTextField="BoxId"
                                                            DataValueField="BoxId" meta:resourcekey="cboBoxResource1" Width="168px">
                                                        </asp:DropDownList>
                                                        <asp:Label ID="lblOldBox" runat="server" Visible="False" meta:resourcekey="lblOldBoxResource1"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext"></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="height: 20px">
                                                        <asp:Label ID="lblState" runat="server" Text="State/Province:" meta:resourcekey="lblStateResource1"></asp:Label></td>
                                                    <td>
                                                        <asp:DropDownList ID="cboProvince" runat="server" CssClass="formtext" DataTextField="StateProvince"
                                                            DataValueField="StateProvince" meta:resourcekey="cboProvinceResource1"
                                                            Width="168px">
                                                        </asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" height="5"></td>
                                                    <td height="5"></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="height: 30px" align="left">
                                                        <asp:Label ID="lblCost" runat="server" Text="Cost Per" meta:resourcekey="lblCostResource1"></asp:Label>:
                                                        <asp:Label ID="lblUnit" runat="server" meta:resourcekey="lblUnitResource1"></asp:Label>
                                                        <asp:RequiredFieldValidator ID="valCost" runat="server" CssClass="errortext" ControlToValidate="txtCost"
                                                            ErrorMessage="Please enter a cost." meta:resourcekey="valCostResource1" Text="*"></asp:RequiredFieldValidator>
                                                    </td>
                                                    <td style="height: 30px">
                                                        <asp:TextBox ID="txtCost" runat="server" CssClass="formtext" DESIGNTIMEDRAGDROP="213"
                                                            Width="168px" meta:resourcekey="txtCostResource1"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="height: 18px">
                                                        <asp:Label ID="lblIconType" runat="server" Text="Icon Type" meta:resourcekey="lblIconTypeResource1"></asp:Label></td>
                                                    <td style="height: 18px">

                                                        <asp:RadioButtonList ID="optVehicleIcons" runat="server" CssClass="formtext"
                                                            RepeatDirection="Horizontal" meta:resourcekey="optVehicleIconsResource1">
                                                            <asp:ListItem Value="1" Selected="True" meta:resourcekey="ListItemResource1" Text="Circle"></asp:ListItem>
                                                            <asp:ListItem Value="2" meta:resourcekey="ListItemResource2" Text="Square"></asp:ListItem>
                                                            <asp:ListItem Value="3" meta:resourcekey="ListItemResource3" Text="Diamond"></asp:ListItem>
                                                        </asp:RadioButtonList></td>



                                                </tr>
                                                <tr id="trCustomIcon" runat="server" visible="true">
                                                    <td class="formtext" style="height: 18px">
                                                        <asp:Label ID="lblCustomIcon" runat="server" Text="Custom Icon<br />(Supported on new map)" meta:resourcekey="lblCustomIconResource1"></asp:Label>
                                                    </td>
                                                    <td style="height: 18px">
                                                        <div id="customiconDropdown"></div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="height: 5px"></td>
                                                    <td style="height: 5px"></td>
                                                </tr>
                                                <tr id="trfuelCategory" runat="server" visible="false">
                                                    <td class="formtext" style="height: 18px">
                                                        <asp:Label ID="lblFuleCategory" runat="server" Text="GHG Category"
                                                            meta:resourcekey="lblFuleCategoryResource1" Font-Bold="True"></asp:Label>
                                                    </td>
                                                    <td style="height: 10px">
                                                        <asp:DropDownList ID="cboFuelCategory" runat="server" CssClass="formtext" DataTextField="GHGCategory"
                                                            DataValueField="FuelTypeID" meta:resourcekey="cboProvinceResource1"
                                                            Width="232px">
                                                        </asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="height: 18px"></td>
                                                    <td style="height: 18px"></td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 39px" colspan="2" align="center">
                                                        <table id="Table9" cellspacing="0" cellpadding="0" border="0" style="border-right: 3px ridge; border-top: 3px ridge; border-left: 3px ridge; border-bottom: 3px ridge;">
                                                            <tr>
                                                                <td class="formtext" height="5"></td>
                                                                <td height="10"></td>
                                                            </tr>
                                                            <tr>
                                                                <td height="5" class="formtext">&nbsp;<asp:Label ID="lblEmail" runat="server" Text="Email" meta:resourcekey="lblEmailResource1"></asp:Label>:
                                                                    <asp:RegularExpressionValidator ID="valEmail" runat="server" ErrorMessage="Please enter a correct email address"
                                                                        ControlToValidate="txtEmail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                                                        meta:resourcekey="valEmailResource1" Text="*"></asp:RegularExpressionValidator></td>
                                                                <td height="5">
                                                                    <asp:TextBox ID="txtEmail" runat="server" Width="168px" meta:resourcekey="txtEmailResource1"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td height="5" class="formtext">&nbsp;<asp:Label
                                                                    ID="lblPhone" runat="server" Text="Phone:" Visible="False"></asp:Label>
                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                                                                        ControlToValidate="txtPhone" CssClass="formtext"
                                                                        ErrorMessage="Invalid Phone Number:"
                                                                        ValidationExpression="^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$"
                                                                        Enabled="False">*</asp:RegularExpressionValidator>
                                                                </td>
                                                                <td height="5">
                                                                    <asp:TextBox ID="txtPhone" runat="server" Width="168px"
                                                                        meta:resourcekey="txtEmailResource1" Enabled="False" Visible="False"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="height: 14px" height="14" class="formtext">&nbsp;<asp:Label ID="lblTimeZone" runat="server" Text="Time Zone" meta:resourcekey="lblTimeZoneResource1"></asp:Label>:</td>
                                                                <td style="height: 14px" height="14">
                                                                    <asp:DropDownList ID="cboTimeZone" runat="server" CssClass="RegularText"
                                                                        DataValueField="TimeZoneId" DataTextField="TimeZoneName" meta:resourcekey="cboTimeZoneResource1">
                                                                    </asp:DropDownList></td>
                                                            </tr>
                                                            <tr>
                                                                <%--<td style="height: 20px">
                                                                </td>--%>
                                                                <td colspan="2" style="height: 20px">
                                                                    <asp:CheckBox ID="chkDayLight" runat="server" CssClass="formtext" Text="Automatically adjust for daylight savings time"
                                                                        meta:resourcekey="chkDayLightResource1"></asp:CheckBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="formtext" height="5"></td>
                                                                <td class="formtext" height="5"></td>
                                                            </tr>
                                                            <tr>
                                                                <%--<td class="formtext">
                                                                    &nbsp;</td>--%>
                                                                <td colspan="2" class="formtext">
                                                                    <asp:Label ID="lblNotificationDesc" runat="server" meta:resourcekey="lblNotificationDescResource1"
                                                                        Text="Email notification in case of:"></asp:Label></td>
                                                            </tr>
                                                            <tr>
                                                                <%--<td>
                                                                </td>--%>
                                                                <td colspan="2">
                                                                    <table class="formtext" id="Table10" cellspacing="0" cellpadding="0"
                                                                        border="0">
                                                                        <tr>
                                                                            <td>
                                                                                <asp:CheckBox ID="chkCritical" runat="server" Text="Critical Alarm" meta:resourcekey="chkCriticalResource1"></asp:CheckBox></td>
                                                                            <td class="style1">
                                                                                <asp:CheckBox ID="chkWarning" runat="server" Text="Warning Alarm" meta:resourcekey="chkWarningResource1"></asp:CheckBox></td>
                                                                            <td>
                                                                                <asp:CheckBox ID="chkNotify" runat="server" Text="Notify Alarm" meta:resourcekey="chkNotifyResource1"></asp:CheckBox></td>
                                                                            <td>
                                                                                <asp:CheckBox ID="chkMaintenance" runat="server" Text="Maintenance" meta:resourcekey="chkMaintenanceResource1"></asp:CheckBox></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <%--Salman Mar 05,2013--%>
                                                                            <td class="formtext" height="5">
                                                                                <asp:Label ID="lblPostedSpeed" runat="server" Text="Posted Speed: " meta:resourcekey="lblPostedSpeedResource1"></asp:Label>
                                                                            </td>
                                                                            <td class="style1" height="5" colspan="3">
                                                                                <asp:DropDownList ID="cboPostedSpeed" Width="150 px" runat="server" CssClass="RegularText"
                                                                                    DataValueField="ServiceConfigId" DataTextField="ServiceName" meta:resourcekey="cboPostedSpeedResource1">
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td style="height: 39px;" colspan="2"></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" height="10" align="left">&nbsp;&nbsp;&nbsp;<asp:Label ID="lblMessage" runat="server" Visible="False" CssClass="errortext"
                                                        Width="99%" Height="13px" meta:resourcekey="lblMessageResource1"></asp:Label>
                                                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="errortext"
                                                            Width="100%" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="10"></td>
                                                    <td height="10" colspan="2">&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td align="right">
                                                        <asp:Button runat="server" CssClass="combutton" ID="cmdClose" OnClientClick="window.close()"
                                                            Text="Close" meta:resourcekey="cmdCloseResource1" />
                                                    </td>
                                                    <td align="center">
                                                        <table id="Table2" cellspacing="0" cellpadding="0" border="0">
                                                            <tr>
                                                                <td>
                                                                    <%--<input class="combutton" id="cmdClose" onclick="window.close()" type="button"
                                                                            value="Close" name="cmdClose">--%></td>
                                                                <td>
                                                                    <asp:Button ID="cmdSave" runat="server" CssClass="combutton" Text="Save" OnClick="cmdSave_Click"
                                                                        meta:resourcekey="cmdSaveResource1"></asp:Button></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
