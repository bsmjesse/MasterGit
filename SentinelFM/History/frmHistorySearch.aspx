<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmHistorySearch.aspx.cs" Inherits="SentinelFM.History_frmHistorySearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table id="tblIntersection" runat="server" class="formtext">
            <tr>
                <td style="width: 192px" valign="top">
                    <asp:Label ID="lblIntrStreet" runat="server" Text="Street:"/>
                    &nbsp;
                </td>
                <td valign="top">
                    <asp:TextBox ID="txtIntrStreet1" runat="server" CssClass="formtext" Width="173px"/>
                </td>
                <td style="width: 107px" valign="top">
                    <asp:Label ID="lblIntrState" runat="server" Text="Province/State:" Width="101px"/>
                </td>
                <td style="width: 352px" valign="top">
                    <asp:TextBox ID="txtIntrState" runat="server" CssClass="formtext" Width="173px"/>
                </td>
            </tr>
            <tr>
                <td style="width: 192px" valign="top">
                    <asp:Label ID="lblIntrCity" runat="server" Text="City:"/>
                </td>
                <td valign="top">
                    <asp:TextBox ID="txtIntrCity" runat="server" CssClass="formtext" Width="173px"/>
                </td>
                <td style="width: 107px" valign="top">
                    <asp:Label ID="lblIntrCountry" runat="server" Text="Country:"/>
                </td>
                <td style="width: 352px" valign="top">
                    <asp:DropDownList ID="cbolblIntrCountry" runat="server" CssClass="formtext" Width="175px">
                        <asp:ListItem  Selected="True" Text="Canada" Value="Canada"/>
                        <asp:ListItem  Text="USA" Value="USA"/>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="width: 192px" valign="top"></td>
                <td></td>
                <td style="width: 107px" valign="top"></td>
                <td align="left" style="width: 352px" valign="top">
                    <asp:Button ID="cmdSearch" runat="server" CssClass="combutton" OnClick="cmdSearch_Click" Text="Search"/>
                    <asp:TextBox ID="txtIntrX" runat="server" CssClass="formtext"  name="txtX" Visible="False" Width="173px"/>
                    &nbsp;&nbsp;
                    <asp:TextBox ID="txtIntrY" runat="server" CssClass="formtext" name="txtY" Visible="False" Width="173px"/>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
