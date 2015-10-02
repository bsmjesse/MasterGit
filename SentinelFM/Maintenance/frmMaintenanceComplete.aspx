<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMaintenanceComplete.aspx.cs"
    Inherits="SentinelFM.Maintenance_frmMaintenanceComplete" Culture="en-US" meta:resourcekey="PageResource1"
    UICulture="auto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Maintenance Complete</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table id="Table1" border="0" cellpadding="0" cellspacing="0" style="border-right: gray 4px double;
                border-top: gray 4px double; z-index: 101; left: 8px; border-left: gray 4px double;
                width: 370px; border-bottom: gray 4px double" width="370">
                <tr>
                    <td class="formtext" style="width: 363px; height: 13px;">
                        <asp:Label ID="lblCurOdom" runat="server" Visible="False" meta:resourcekey="lblCurOdomResource1" Text="0"/>
                        <asp:Label ID="lblVehicleId" runat="server" Visible="False" meta:resourcekey="lblVehicleIdResource1"/>
                        <asp:Label ID="lblOdometer" runat="server" Visible="False" meta:resourcekey="lblOdometerResource1"/>
                        <asp:Label ID="lblLastSrvOdo" runat="server" Visible="False" meta:resourcekey="lblLastSrvOdoResource1"/>
                        <asp:Label ID="lblEngineHours" runat="server" Text="0" Visible="False"/>
                        <asp:Label ID="lblLastEngineHours" runat="server" Text="0" Visible="False"/>
                    </td>
                </tr>
                <tr>
                    <td colspan="1" style="width: 363px; height: 10px;" align="center">
                        <table>
                            <tr>
                                <td align="left" class="formtext" style="font-weight: bold; width: 100px; text-decoration: underline">
                                    <asp:Label ID="lblThisServiceTitle" runat="server" CssClass="formtext" meta:resourcekey="lblThisServiceTitleResource1" Text="Service Info:"/>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100px">
                                    <table>
                                        <tr>
                                            <td style="width: 100px">
                                                <asp:DropDownList ID="cboServiceThis" runat="server" CssClass="formtext" Width="205px" meta:resourcekey="cboServiceThisResource1"/>
                                            </td>
                                            <td style="width: 100px">
                                                <asp:Button ID="cmdAddThisService" runat="server" CssClass="combutton" OnClick="cmdAddThisService_Click" Text="Add" Width="67px" meta:resourcekey="cmdAddThisServiceResource1" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100px; height: 120px;">
                                    <asp:TextBox ID="txtThisService" runat="server" Height="182px" TextMode="MultiLine" Width="278px" meta:resourcekey="txtThisServiceResource1"/>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100px"></td>
                            </tr>
                            <tr>
                                <td style="width: 100px">
                                    <asp:Label ID="lblNextServiceTitle" runat="server" CssClass="formtext" meta:resourcekey="lblNextServiceTitleResource1" Text="Next Service" Visible="False"></asp:Label><asp:DropDownList ID="cboServiceNext" runat="server" CssClass="formtext" Width="205px" meta:resourcekey="cboServiceNextResource1" Visible="False"/>
                                    <asp:Button ID="cmdAddNextService" runat="server" CssClass="combutton" OnClick="cmdAddNextService_Click" Text="Add" Width="67px" meta:resourcekey="cmdAddNextServiceResource1" Visible="False" />
                                    <asp:TextBox ID="txtNextService" runat="server" Height="5px" TextMode="MultiLine" Width="278px" meta:resourcekey="txtNextServiceResource1" Visible="False"/>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" class="formtext" style="width: 363px; height: 18px"></td>
                </tr>
                <tr>
                    <td align="center" colspan="1" style="width: 363px" valign="middle">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="1" style="width: 363px">
                        <table>
                            <tr>
                                <td style="width: 100px">
                                    <asp:Button ID="cmdClose" runat="server" CssClass="combutton" OnClientClick="window.close()" Text="Cancel" meta:resourcekey="cmdCloseResource1"/>
                                 </td>
                                <td style="width: 34px">
                                </td>
                                <td style="width: 100px">
                                    <asp:Button ID="cmdSave" runat="server" CssClass="combutton" OnClick="cmdSave_Click" Text="Save" meta:resourcekey="cmdSaveResource1"/>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="1" style="width: 363px; height: 19px">
                        &nbsp;
                        <asp:Label ID="lblMessage" runat="server" CssClass="errortext" meta:resourcekey="lblMessageResource1"/>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
