<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmvehicle_workinghours.aspx.cs"
    Inherits="Configuration_frmvehicle_workinghours" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td valign="bottom">
                        <table id="tblButtons" border="0" cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:Button ID="cmdInfo" runat="server" CausesValidation="False" CssClass="confbutton"
                                        OnClick="cmdInfo_Click" Text="Info" Width="104px" meta:resourcekey="cmdInfoResource1" /></td>
                                <td style="width: 105px">
                                    <asp:Button ID="cmdCustomFields" runat="server" CausesValidation="False" CssClass="confbutton"
                                        Text="Custom Fields" Width="104px" OnClick="cmdCustomFields_Click" meta:resourcekey="cmdCustomFieldsResource1" /></td>
                                <td>
                                </td>
                                <td style="width: 105px">
                                    <asp:Button ID="cmdWorkingHours" runat="server" CausesValidation="False" CssClass="selectedbutton"
                                        Text="Working Hours" Width="104px" meta:resourcekey="cmdWorkingHoursResource1" /></td>
                                <td>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">
                        <table id="Table3" border="0" cellpadding="0" cellspacing="0"  width="421px" height="365px" class="table">
                            <tr>
                                <td align="center" class="configTabBackground" valign="top">
                                    <table cellpadding="0" cellspacing="0" class="formtext" style="width: 323px">
                                        <tr>
                                            <td height="30" style="width: 470px">
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 470px" colspan="2">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 470px; height: 13px" colspan="2">
                                                <table id="tblOffHours" runat="server" border="0" cellpadding="0" cellspacing="0"
                                                    class="formtext" style="width: 227px">
                                                    <tr>
                                                        <td align="center" style="height: 2px; font-weight: bold;" colspan="5">
                                                            <asp:Label ID="lblWeekdayHours" runat="server" meta:resourcekey="lblWeekdayHoursResource1">Weekday Hours</asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" colspan="5" height="10" style="width: 103px">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 103px; height: 2px" align="left">
                                                            <asp:RadioButton ID="optWeekDay24" runat="server" Text="24 Hours" Width="90px" Checked="True"
                                                                AutoPostBack="True" OnCheckedChanged="optWeekDay24_CheckedChanged" meta:resourcekey="optWeekDay24Resource1" /></td>
                                                        <td style="height: 2px" colspan="4" align="left">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td height="2" style="width: 103px">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 103px; height: 10px" align="left">
                                                            <asp:RadioButton ID="optFromToWeekDay" runat="server" Text="   " AutoPostBack="True"
                                                                OnCheckedChanged="optFromToWeekDay_CheckedChanged" meta:resourcekey="optFromToWeekDayResource1" /></td>
                                                        <td style="height: 10px">
                                                            <asp:Label ID="lblFrom" runat="server" meta:resourcekey="lblFromResource1">From:</asp:Label></td>
                                                        <td style="height: 10px">
                                                            <asp:DropDownList ID="cboFromDayH" runat="server" CssClass="RegularText" Height="14px"
                                                                Width="60px" Enabled="False" meta:resourcekey="cboFromDayHResource1">
                                                            </asp:DropDownList></td>
                                                        <td style="height: 10px">
                                                            :</td>
                                                        <td style="height: 10px">
                                                            <asp:DropDownList ID="cboFromDayM" runat="server" CssClass="RegularText" Height="14px"
                                                                Width="60px" Enabled="False" meta:resourcekey="cboFromDayMResource1">
                                                                <asp:ListItem Value="00" meta:resourcekey="ListItemResource1">00</asp:ListItem>
                                                                <asp:ListItem Value="15" meta:resourcekey="ListItemResource2">15</asp:ListItem>
                                                                <asp:ListItem Value="30" meta:resourcekey="ListItemResource3">30</asp:ListItem>
                                                                <asp:ListItem Value="45" meta:resourcekey="ListItemResource4">45</asp:ListItem>
                                                            </asp:DropDownList></td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 103px; height: 18px">
                                                        </td>
                                                        <td style="height: 18px">
                                                            <asp:Label ID="lblTo" runat="server" meta:resourcekey="lblToResource1">To:</asp:Label></td>
                                                        <td style="height: 18px">
                                                            <asp:DropDownList ID="cboToDayH" runat="server" CssClass="RegularText" Height="14px"
                                                                Width="60px" Enabled="False" meta:resourcekey="cboToDayHResource1">
                                                            </asp:DropDownList></td>
                                                        <td style="height: 18px">
                                                            :</td>
                                                        <td style="height: 18px">
                                                            <asp:DropDownList ID="cboToDayM" runat="server" CssClass="RegularText" Height="14px"
                                                                Width="60px" Enabled="False" meta:resourcekey="cboToDayMResource1">
                                                                <asp:ListItem Value="00" meta:resourcekey="ListItemResource5">00</asp:ListItem>
                                                                <asp:ListItem Value="15" meta:resourcekey="ListItemResource6">15</asp:ListItem>
                                                                <asp:ListItem Value="30" meta:resourcekey="ListItemResource7">30</asp:ListItem>
                                                                <asp:ListItem Value="45" meta:resourcekey="ListItemResource8">45</asp:ListItem>
                                                            </asp:DropDownList></td>
                                                    </tr>
                                                    <tr>
                                                        <td height="2" style="width: 103px">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" style="height: 2px; font-weight: bold;" colspan="5">
                                                            <asp:Label ID="lblWeekendHours" runat="server" meta:resourcekey="lblWeekendHoursResource1">Weekend Hours</asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" colspan="5" height="10" style="font-weight: bold">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="3" style="height: 20px" align="left">
                                                            <asp:CheckBox ID="chkWeekend" runat="server" AutoPostBack="True" Text="Operated on weekends"
                                                                Width="184px" Checked="True" meta:resourcekey="chkWeekendResource1" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left" height="10" style="width: 103px">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left" style="width: 103px; height: 2px;">
                                                            <asp:RadioButton ID="optWeekEnd24" runat="server" Text="24 Hours" Width="91px" Checked="True"
                                                                AutoPostBack="True" OnCheckedChanged="optWeekEnd24_CheckedChanged" meta:resourcekey="optWeekEnd24Resource1" /></td>
                                                        <td style="height: 2px">
                                                        </td>
                                                        <td style="height: 2px">
                                                        </td>
                                                        <td style="height: 2px">
                                                        </td>
                                                        <td style="height: 2px">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td height="2" style="width: 103px">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            <asp:RadioButton ID="optFromToWeekEnd" runat="server" Text="   " TextAlign="Left"
                                                                AutoPostBack="True" OnCheckedChanged="optFromToWeekEnd_CheckedChanged" meta:resourcekey="optFromToWeekEndResource1" /></td>
                                                        <td>
                                                            <asp:Label ID="lblFrom2" runat="server" meta:resourcekey="lblFrom2Resource1">From:</asp:Label></td>
                                                        <td>
                                                            <asp:DropDownList ID="cboWeekEndFromH" runat="server" CssClass="RegularText" Height="14px"
                                                                Width="60px" Enabled="False" meta:resourcekey="cboWeekEndFromHResource1">
                                                            </asp:DropDownList></td>
                                                        <td>
                                                            :</td>
                                                        <td>
                                                            <asp:DropDownList ID="cboWeekEndFromM" runat="server" CssClass="RegularText" Height="14px"
                                                                Width="60px" Enabled="False" meta:resourcekey="cboWeekEndFromMResource1">
                                                                <asp:ListItem Value="00" meta:resourcekey="ListItemResource9">00</asp:ListItem>
                                                                <asp:ListItem Value="15" meta:resourcekey="ListItemResource10">15</asp:ListItem>
                                                                <asp:ListItem Value="30" meta:resourcekey="ListItemResource11">30</asp:ListItem>
                                                                <asp:ListItem Value="45" meta:resourcekey="ListItemResource12">45</asp:ListItem>
                                                            </asp:DropDownList></td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 103px">
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="lblTo2" runat="server" meta:resourcekey="lblTo2Resource1">To:</asp:Label></td>
                                                        <td>
                                                            <asp:DropDownList ID="cboWeekEndToH" runat="server" CssClass="RegularText" Height="14px"
                                                                Width="60px" Enabled="False" meta:resourcekey="cboWeekEndToHResource1">
                                                            </asp:DropDownList></td>
                                                        <td>
                                                            :</td>
                                                        <td>
                                                            <asp:DropDownList ID="cboWeekEndToM" runat="server" CssClass="RegularText" Height="14px"
                                                                Width="60px" Enabled="False" meta:resourcekey="cboWeekEndToMResource1">
                                                                <asp:ListItem Value="00" meta:resourcekey="ListItemResource13">00</asp:ListItem>
                                                                <asp:ListItem Value="15" meta:resourcekey="ListItemResource14">15</asp:ListItem>
                                                                <asp:ListItem Value="30" meta:resourcekey="ListItemResource15">30</asp:ListItem>
                                                                <asp:ListItem Value="45" meta:resourcekey="ListItemResource16">45</asp:ListItem>
                                                            </asp:DropDownList></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td height="10" style="width: 470px">
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 470px">
                                            </td>
                                            <td height="30">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 470px">
                                                <asp:Button runat="server" ID="Button1" CssClass="combutton" OnClientClick="window.close()"
                                                    Text="Close" meta:resourcekey="Button1Resource1" /><%--<input id="Button1" class="combutton" name="cmdClose" onclick="window.close()"
                                                        type="button" value="Close" />--%></td>
                                            <td height="30">
                                                <asp:Button ID="cmdSave" runat="server" CssClass="combutton" OnClick="cmdSave_Click"
                                                    Text="Save" meta:resourcekey="cmdSaveResource1" /></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" style="height: 32px">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
