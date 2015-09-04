<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmKMLHistory.aspx.cs" Inherits="frmKMLHistory" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>KML History</title>
    <link href="GlobalStyle.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="frmMain" runat="server">
        <div style="text-align: center;">
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td class="heading" style="padding-right: 5px; padding-bottom: 2px; text-align: right;">
                        # of Messages*:</td>
                    <td style="padding-bottom: 2px; text-align: left;">
                        <asp:TextBox ID="txtMsgCount" runat="server" CssClass="RegularText" Width="100px">10</asp:TextBox></td>
                </tr>
                <tr>
                    <td class="heading" style="padding-right: 5px; padding-bottom: 2px; text-align: right;">
                        Box ID**:</td>
                    <td style="padding-bottom: 2px; text-align: left;">
                        <asp:TextBox ID="txtBoxID" runat="server" CssClass="RegularText" Width="100px"></asp:TextBox></td>
                </tr>
                <tr>
                    <td style="padding-right: 10px; padding-bottom: 10px; text-align: left;">
                        <table border="0" cellpadding="0" cellspacing="0" style="width: 204px;">
                            <tr>
                                <td class="heading">
                                    From:</td>
                                <td style="text-align: right;">
                                    <asp:DropDownList ID="ddlFrom" runat="server" CssClass="RegularText">
                                    </asp:DropDownList></td>
                            </tr>
                        </table>
                        <asp:Calendar ID="calFrom" runat="server" BorderColor="Black" BorderStyle="Solid"
                            CellSpacing="1" Font-Bold="true" Font-Size="11px" Height="162px" NextPrevFormat="ShortMonth"
                            Width="204px">
                            <TodayDayStyle ForeColor="White" BackColor="#999999"></TodayDayStyle>
                            <DayStyle BackColor="#CCCCCC"></DayStyle>
                            <NextPrevStyle Font-Size="10px" Font-Bold="True" ForeColor="white"></NextPrevStyle>
                            <DayHeaderStyle Font-Size="10px" Font-Bold="True" Height="10px" ForeColor="#333333">
                            </DayHeaderStyle>
                            <SelectedDayStyle ForeColor="white" BackColor="#009933"></SelectedDayStyle>
                            <TitleStyle Font-Size="10px" Font-Bold="True" Height="10px" ForeColor="white" BackColor="#009933">
                            </TitleStyle>
                            <OtherMonthDayStyle ForeColor="#999999"></OtherMonthDayStyle>
                        </asp:Calendar>
                    </td>
                    <td style="padding-left: 10px; padding-bottom: 10px; text-align: left;">
                        <table border="0" cellpadding="0" cellspacing="0" style="width: 204px;">
                            <tr>
                                <td class="heading">
                                    To:</td>
                                <td style="text-align: right;">
                                    <asp:DropDownList ID="ddlTo" runat="server" CssClass="RegularText">
                                    </asp:DropDownList></td>
                            </tr>
                        </table>
                        <asp:Calendar ID="calTo" runat="server" BorderColor="Black" BorderStyle="Solid" CellSpacing="1"
                            Font-Bold="true" Font-Size="11px" Height="162px" NextPrevFormat="ShortMonth"
                            Width="204px">
                            <TodayDayStyle ForeColor="White" BackColor="#999999"></TodayDayStyle>
                            <DayStyle BackColor="#CCCCCC"></DayStyle>
                            <NextPrevStyle Font-Size="10px" Font-Bold="True" ForeColor="white"></NextPrevStyle>
                            <DayHeaderStyle Font-Size="10px" Font-Bold="True" Height="10px" ForeColor="#333333">
                            </DayHeaderStyle>
                            <SelectedDayStyle ForeColor="white" BackColor="#009933"></SelectedDayStyle>
                            <TitleStyle Font-Size="10px" Font-Bold="True" Height="10px" ForeColor="white" BackColor="#009933">
                            </TitleStyle>
                            <OtherMonthDayStyle ForeColor="#999999"></OtherMonthDayStyle>
                        </asp:Calendar>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="padding-bottom: 10px;">
                        <asp:Label ID="lblStatus" runat="server" CssClass="errortext"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="2" style="padding-bottom: 5px;">
                        <asp:Button ID="btnGetKML" runat="server" CssClass="combutton" Text="Get KML" OnClick="btnGetKML_Click" /></td>
                </tr>
                <tr>
                    <td class="formtext" colspan="2">
                        *Messages with invalid GPS will be discarded.<br />
                        **Leave blank to see all boxes.</td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
