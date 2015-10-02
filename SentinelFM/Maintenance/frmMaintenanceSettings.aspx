<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMaintenanceSettings.aspx.cs"
    Inherits="SentinelFM.Maintenance_frmMaintenanceSettings" Culture="en-US" meta:resourcekey="PageResource1"
    UICulture="auto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Maintenance</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body topmargin="0" leftmargin="0" rightmargin="0" bottommargin="0">
    <form id="form1" runat="server">
        <table id="Table1" border="0" cellpadding="0" cellspacing="0" style="border-right: gray 4px double;
            border-top: gray 4px double; z-index: 101; left: 8px; border-left: gray 4px double;
            width: 370px; border-bottom: gray 4px double;" width="370">
            <tr>
                <td class="formtext" style="width: 363px">
                    &nbsp;<asp:Label ID="lblVehicleNameTitle" runat="server" CssClass="formtext" meta:resourcekey="lblVehicleNameTitleResource1">Vehicle Name:</asp:Label>
                    <asp:Label ID="lblVehicleName" runat="server" CssClass="formtext" Font-Bold="True" meta:resourcekey="lblVehicleNameResource1"/>
                </td>
            </tr>
            <tr>
                <td colspan="1" height="10px" style="width: 363px"></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 363px">
                    &nbsp;
                    <asp:Label ID="lblBoxIdTitle" runat="server" CssClass="formtext" meta:resourcekey="lblBoxIdTitleResource1">Unit ID:</asp:Label>
                    <asp:Label ID="lblBoxId" runat="server" CssClass="formtext" Font-Bold="True" meta:resourcekey="lblBoxIdResource1"/>
                    <asp:Label ID="lblVehicleId" runat="server" Visible="False" meta:resourcekey="lblVehicleIdResource1"/>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="1" height="10px" style="width: 363px"></td>
            </tr>
            <tr>
                <td class="formtext" style="height: 18px; width: 363px;" align="center">
                    <asp:Button ID="cmdMaintenaceCompl" CssClass="combutton" runat="server" Text="Maintenance Completed" Width="174px" OnClick="cmdMaintenaceCompl_Click" meta:resourcekey="cmdMaintenaceComplResource1"/>
                </td>
            </tr>
            <tr>
                <td colspan="1" align="center" valign="middle" style="width: 363px">
                    <asp:RadioButtonList ID="optMaintenanceType" runat="server" RepeatDirection="Horizontal" Width="244px" CssClass="formtext" AutoPostBack="True" OnSelectedIndexChanged="optMaintenanceType_SelectedIndexChanged" Visible="False" meta:resourcekey="optMaintenanceTypeResource1">
                        <asp:ListItem Value="1" Selected="True" meta:resourcekey="ListItemResource1">Odometer</asp:ListItem>
                        <asp:ListItem Value="2" meta:resourcekey="ListItemResource2">Engine Hours</asp:ListItem>
                    </asp:RadioButtonList>
                    <table id="tblOdometerOpt" runat="server" class="formtext" width="100%">
                        <tr>
                            <td style="width: 185px; height: 26px;" align="left">
                                &nbsp;
                                <asp:Label ID="lblOdomServiceInterval" runat="server" Text="Service Interval:"  meta:resourcekey="lblOdomServiceIntervalResource1"/>
                            </td>
                            <td style="width: 100px; height: 26px;">
                                <asp:TextBox ID="txtOdomServiceInterval" runat="server" CssClass="formtext" Width="137px" meta:resourcekey="txtOdomServiceIntervalResource1">0</asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" style="width: 185px; height: 26px">
                                &nbsp;
                                <asp:Label ID="lblOdomCurOdometerValue" runat="server" Text="Current Odometer:"  meta:resourcekey="lblOdomCurOdometerValueResource1"/>
                            </td>
                            <td align="left" style="width: 100px; height: 26px">
                                <asp:Label ID="lblCurOdom" runat="server" meta:resourcekey="lblCurOdomResource1">0</asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" style="width: 185px; height: 36px">
                                &nbsp;<asp:Label ID="Label1" runat="server" Text="Next Service Odometer:" meta:resourcekey="Label1Resource1"/>
                            </td>
                            <td align="left" style="width: 100px; height: 26px">
                                <asp:Label ID="lblNextOdom" runat="server" meta:resourcekey="lblNextOdomResource1">0</asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" style="width: 185px;" valign="top">
                                &nbsp;
                                <asp:Label ID="lblOdomLastServiced" runat="server" Text="Last Serviced Odometer:" meta:resourcekey="lblOdomLastServicedResource1"/>
                            </td>
                            <td align="left" style="width: 100px;" valign="top">
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td style="width: 100px">
                                            <asp:TextBox ID="txtLastOdomServ" runat="server" CssClass="formtext" Enabled="False" meta:resourcekey="txtLastOdomServResource1">0</asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <table>
                                                <tr>
                                                    <td style="width: 100px;">
                                                        <asp:Button ID="cmdEditOdomSrv" runat="server" CssClass="combutton" Text="Edit" OnClick="cmdEditOdomSrv_Click" meta:resourcekey="cmdEditOdomSrvResource1" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top" style="height: 42px">
                                            <table>
                                                <tr>
                                                    <td style="width: 63px; height: 21px">
                                                        <asp:Button ID="cmdCancelOdomServ" runat="server" CssClass="combutton" Text="Cancel" Visible="False" Width="60px" OnClick="cmdCancelOdomServ_Click" meta:resourcekey="cmdCancelOdomServResource1" />
                                                    </td>
                                                    <td style="width: 64px; height: 21px">
                                                        <asp:Button ID="cmdSaveOdomServ" runat="server" CssClass="combutton" Text="Save" Width="60px" Visible="False" OnClick="cmdSaveOdomServ_Click" meta:resourcekey="cmdSaveOdomServResource1" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table id="tblEngineOpt" runat="server" class="formtext" width="100%">
                        <tr>
                            <td style="width: 197px;" align="left">
                                &nbsp;<asp:Label ID="lblServiceIntervalTitle" runat="server" meta:resourcekey="lblServiceIntervalTitleResource1">Service Interval:</asp:Label>
                            </td>
                            <td style="width: 100px;" align="left">
                                <asp:TextBox ID="txtEngineServiceInterval" runat="server" CssClass="formtext" meta:resourcekey="txtEngineServiceIntervalResource1">0</asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" style="width: 197px">
                                &nbsp;<asp:Label ID="lblCurrentEngineTimeTitle" runat="server" meta:resourcekey="lblCurrentEngineTimeTitleResource1">Current Engine Time:</asp:Label>
                            </td>
                            <td align="left" style="width: 100px;">
                                <asp:Label ID="lblCurEngine" runat="server" meta:resourcekey="lblCurEngineResource1">0</asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" style="width: 185px;">
                                &nbsp;<asp:Label ID="lblNextEngineServiceTimeTitle" runat="server" meta:resourcekey="lblNextEngineServiceTimeTitleResource1">Next Engine Service Time:</asp:Label><
                            /td>
                            <td align="left" style="width: 100px;">
                                <asp:Label ID="lblNextEngine" runat="server" meta:resourcekey="lblNextEngineResource1">0</asp:Label>
                             </td>
                        </tr>
                        <tr>
                            <td align="left" style="width: 197px" valign="top">
                                &nbsp;<asp:Label ID="lblLastServicedEngineTimeTitle" runat="server" meta:resourcekey="lblLastServicedEngineTimeTitleResource1">Last Serviced Engine Time:</asp:Label>
                            </td>
                            <td align="left" valign="top">
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td style="width: 100px">
                                            <asp:TextBox ID="txtEngineLastService" runat="server" CssClass="formtext" Enabled="False"  meta:resourcekey="txtEngineLastServiceResource1">0</asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="cmdEditEngineSrv" runat="server" CssClass="combutton" Text="Edit" OnClick="cmdEditEngineSrv_Click" meta:resourcekey="cmdEditEngineSrvResource1"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top" style="height: 42px">
                                            <table>
                                                <tr>
                                                    <td style="width: 63px; height: 21px">
                                                        <asp:Button ID="cmdCancelEngSrv" runat="server" CssClass="combutton" Text="Cancel" Visible="False" Width="60px" OnClick="cmdCancelEngSrv_Click" meta:resourcekey="cmdCancelEngSrvResource1" />
                                                    </td>
                                                    <td style="width: 64px; height: 21px">
                                                        <asp:Button ID="cmdSaveEngSrv" runat="server" CssClass="combutton" Text="Save" Width="60px" Visible="False" OnClick="cmdSaveEngSrv_Click" meta:resourcekey="cmdSaveEngSrvResource1" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table id="Table9" border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td class="formtext" height="5" style="width: 89px" align="left">
                                &nbsp;
                                <asp:Label ID="lblEmailTitle" runat="server" CssClass="formtext" meta:resourcekey="lblEmailTitleResource1">Email:</asp:Label>
                                <asp:RegularExpressionValidator ID="valEmail" runat="server" ControlToValidate="txtEmail"
                                    ErrorMessage="Please enter a correct email address" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                    meta:resourcekey="valEmailResource1">*</asp:RegularExpressionValidator>
                            </td>
                            <td height="5" style="width: 301px; color: #000000" align="left">
                                <asp:TextBox ID="txtEmail" runat="server" Width="168px" CssClass="formtext" meta:resourcekey="txtEmailResource1"></asp:TextBox></td>
                        </tr>
                        <tr style="color: #000000">
                            <td class="formtext" style="width: 89px; height: 14px"></td>
                            <td style="width: 301px; height: 14px;"></td>
                        </tr>
                        <tr style="color: #000000">
                            <td class="formtext" height="14" style="width: 89px; height: 14px" align="left">
                                &nbsp;
                                <asp:Label ID="lblTimeZoneTitle" runat="server" CssClass="formtext" meta:resourcekey="lblTimeZoneTitleResource1">Time Zone:</asp:Label>
                            </td>
                            <td height="14" style="width: 301px; height: 14px" align="left">
                                <asp:DropDownList ID="cboTimeZone" runat="server" CssClass="formtext" DataTextField="TimeZoneName" DataValueField="TimeZoneId" Width="171px" meta:resourcekey="cboTimeZoneResource1"/>
                            </td>
                        </tr>
                        <tr style="color: #000000">
                            <td class="formtext" height="14" style="width: 89px; height: 14px"></td>
                            <td height="4" style="width: 301px"></td>
                        </tr>
                        <tr>
                            <td style="width: 301px; height: 20px" colspan="2" align="center">
                                <asp:CheckBox ID="chkDayLight" runat="server" CssClass="formtext" Text="Automatically adjust for daylight savings time" meta:resourcekey="chkDayLightResource1" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="1" align="center" style="width: 363px">
                    <table>
                        <tr>
                            <td style="width: 100px">
                                <input id="cmdClose" class="combutton" name="cmdCancel" onclick="window.close()" type="button" value="Cancel"/>
                            </td>
                            <td style="width: 34px"></td>
                            <td style="width: 100px">
                                <asp:Button ID="cmdSave" CssClass="combutton" runat="server" Text="Save" OnClick="cmdSave_Click" meta:resourcekey="cmdSaveResource1"/>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="1" style="height: 19px; width: 363px;" align="center">
                    &nbsp;
                    <asp:Label ID="lblMessage" runat="server" CssClass="errortext" meta:resourcekey="lblMessageResource1"/>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
