<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmVehicleStatus.aspx.cs" Inherits="SentinelFM.Configuration_frmVehicleStatus" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Vehicle Status</title>
    <link rel="stylesheet" href="../scripts/tablesorter/themes/report/style.css" type="text/css" />
    <script src="http://maps.google.com/maps/api/js?v=3.2&sensor=false&libraries=places"></script>
</head>
<body>
    <form id="form1" runat="server">
        <table border="0" cellpadding="0" cellspacing="0" width="96%">
            <tr>
                <td valign="top">
                    <table id="tblButtons" border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <asp:Button ID="cmdInfo" runat="server" CausesValidation="False" CssClass="confbutton"
                                    OnClick="cmdInfo_Click" Text="Info" Width="104px" meta:resourcekey="cmdInfoResource1" /></td>
                            <td style="width: 105px">
                                <asp:Button ID="cmdCustomFields" runat="server" CausesValidation="False" CssClass="confbutton"
                                    OnClick="cmdCustomFields_Click" Text="Custom Fields" Width="104px" meta:resourcekey="cmdCustomFieldsResource1" /></td>
                            <td style="width: 105px">
                                <asp:Button ID="cmdVehicleStatus" runat="server" CausesValidation="False" CssClass="selectedbutton" CommandName="114"
                                    Text="Vehicle Status" Width="104px" meta:resourcekey="cmdVehicleStatusResource1" /></td>
                            <td style="width: 105px">
                                <asp:Button ID="cmdWorkingHours" runat="server" CausesValidation="False" CssClass="confbutton"
                                    Text="Working Hours" Width="104px" OnClick="cmdWorkingHours_Click" Visible="False" meta:resourcekey="cmdWorkingHoursResource1" /></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <table id="Table3" cellpadding="0" cellspacing="0" border="0" class="table" height="365px">
                        <tr>
                            <td align="center" class="configTabBackground" valign="top">
                                <table style="width: 525px" class="formtext" cellpadding="2" cellspacing="0">
                                    <tr>
                                        <td colspan="2">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="formtext" width="40%" style="text-align: right">
                                            <asp:Label ID="lblVehicleStatus" runat="server" Text="Vehicle Status:"
                                                meta:resourcekey="lblVehicleStatusResource1"></asp:Label>&nbsp;
                                        </td>
                                        <td class="formtext">
                                            <asp:DropDownList ID="cboVehicleStatus" runat="server" DataValueField="VehicleDeviceStatusID" DataTextField="VehicleDeviceStatus"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvVehicleStatus" runat="server" ControlToValidate="cboVehicleStatus"
                                                Display="Dynamic" ErrorMessage="mandatory field" ForeColor="Firebrick"
                                                meta:resourcekey="rfvVehicleStatusResource1" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formtext" width="40%" style="text-align: right">
                                            <asp:Label ID="lblDate" runat="server" Text="Date:"></asp:Label>&nbsp;</td>
                                        <td class="formtext">
                                            <asp:TextBox ID="txtDate" runat="server" CssClass="formtext" Width="100px"></asp:TextBox>
                                            <a onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtDate','cal','width=245,height=200,left=270,top=380')"
                                                href="javascript:;">
                                                <img src="../images/SmallCalendar.gif" border="0" /></a>&nbsp;
											<asp:ImageButton ID="cmdCancelExpire" runat="server" ImageUrl="../images/Cancel.gif" ToolTip="Reset Date"
                                                CausesValidation="False" meta:resourcekey="cmdCancelExpireResource1"></asp:ImageButton>
                                            <asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="txtDate"
                                                Display="Dynamic" ErrorMessage="mandatory field" ForeColor="Firebrick"
                                                meta:resourcekey="rfvDateResource1" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formtext" width="40%" style="text-align: right">
                                            <asp:Label ID="lblAddress" runat="server" Text="Address:"
                                                meta:resourcekey="lblAddressResource1"></asp:Label>&nbsp;
                                        </td>
                                        <td class="formtext">
                                            <asp:TextBox ID="txtAddress" runat="server" CssClass="formtext" Width="250px" MaxLength="250"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formtext" width="40%" style="text-align: right">
                                            <asp:Label ID="lblAuthorizationNo" runat="server" Text="Authorization No:"
                                                meta:resourcekey="lblAuthorizationNoResource1"></asp:Label>&nbsp;
                                        </td>
                                        <td class="formtext">
                                            <asp:TextBox ID="txtAuthorizationNo" runat="server" CssClass="formtext" Width="250px" MaxLength="100"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:Label ID="lblVehicleId" runat="server" Visible="False" meta:resourcekey="lblVehicleIdResource1"></asp:Label>
                                            <asp:Label ID="lblLicensePlate" runat="server" Visible="False" meta:resourcekey="lblLicensePlateResource1"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2">
                                            <asp:Button runat="server" CssClass="combutton" ID="cmdCloseWindow" OnClientClick="window.close()"
                                                Text="Close" meta:resourcekey="Button1Resource1" />
                                            <asp:Button ID="cmdUpdate" runat="server" CssClass="combutton" Text="Update" OnClick="cmdUpdate_Click" meta:resourcekey="cmdUpdateResource1"></asp:Button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formtext" colspan="2" style="color: #B22222; text-align: center">
                                            <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    <script type="text/javascript">
        //Address input autocomplete       
        var input = document.getElementById('txtAddress');
        var autocomplete = new google.maps.places.Autocomplete(input);
        
    </script>
    </form>
</body>
</html>
