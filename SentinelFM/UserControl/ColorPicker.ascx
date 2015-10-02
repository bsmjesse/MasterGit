<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ColorPicker.ascx.cs" Inherits="ColorPicker" %>
<asp:Table ID="tblPicker" runat="server" CellPadding="0" CellSpacing="0" BorderColor="Black"
    BorderStyle="Solid" BorderWidth="1">
</asp:Table>
<asp:Table ID="tblShow" runat="server" CellPadding="0" CellSpacing="0">
    <asp:TableRow ID="rowShow" runat="server" BorderColor="Black" BorderStyle="Solid"
        BorderWidth="1">
        <asp:TableCell ID="cellPreview" runat="server" Text="#FFFFFF" Width="100%" ColumnSpan="5"
            HorizontalAlign="Center"></asp:TableCell>
    </asp:TableRow>
    <asp:TableRow ID="rowPalettes" runat="server" BorderColor="Black" BorderStyle="Solid"
        BorderWidth="1">
        <asp:TableCell ID="cellPal1" runat="server" Width="20%" Text="1" HorizontalAlign="Center"
            BorderColor="Black" BorderStyle="Dotted" BorderWidth="1" BackColor="White" ForeColor="Black"
            Style="cursor: hand"></asp:TableCell>
        <asp:TableCell ID="cellPal2" runat="server" Width="20%" Text="2" HorizontalAlign="Center"
            BorderColor="Black" BorderStyle="Dotted" BorderWidth="1" BackColor="White" ForeColor="Black"
            Style="cursor: hand"></asp:TableCell>
        <asp:TableCell ID="cellPal3" runat="server" Width="20%" Text="3" HorizontalAlign="Center"
            BorderColor="Black" BorderStyle="Dotted" BorderWidth="1" BackColor="White" ForeColor="Black"
            Style="cursor: hand"></asp:TableCell>
        <asp:TableCell ID="cellPal4" runat="server" Width="20%" Text="4" HorizontalAlign="Center"
            BorderColor="Black" BorderStyle="Dotted" BorderWidth="1" BackColor="White" ForeColor="Black"
            Style="cursor: hand"></asp:TableCell>
        <asp:TableCell ID="cellPal5" runat="server" Width="20%" Text="5" HorizontalAlign="Center"
            BorderColor="Black" BorderStyle="Dotted" BorderWidth="1" BackColor="White" ForeColor="Black"
            Style="cursor: hand"></asp:TableCell>
    </asp:TableRow>
</asp:Table>
