<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SecurityCode.ascx.cs" Inherits="Map_CommandsControls_SecurityCode" %>
<table id="tblSecCode" runat="server" border="0" cellpadding="0" cellspacing="0"
    class="formtext" style="width: 384px" width="384">
    <tr>
        <td class="formtext" colspan="1" style="width: 152px">
            <asp:Label ID="lblGlobalUnarmCodeTitle" runat="server" CssClass="formtext" meta:resourcekey="lblGlobalUnarmCodeTitleResource1"
                Text="Global Unarm Code:"></asp:Label></td>
        <td>
            <asp:TextBox ID="txtGlobalUnarmCode" runat="server" CssClass="formtext" meta:resourcekey="txtGlobalUnarmCodeResource1"
                TextMode="Password" Width="74px"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="formtext" style="width: 152px">
            <asp:Label ID="lblTARCode" runat="server" CssClass="formtext" meta:resourcekey="lblTARCodeResource1"
                Text="TAR Code:"></asp:Label></td>
        <td>
            <asp:TextBox ID="txtTARCode" runat="server" CssClass="formtext" meta:resourcekey="txtTARCodeResource1"
                TextMode="Password" Width="74px"></asp:TextBox></td>
    </tr>
</table>
