<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ctlEquipmentMenu.ascx.cs"
    Inherits="Configuration_Equipment_ctlEquipmentMenu" %>
<table id="tblDriverAssgn" border="0" cellpadding="0" cellspacing="0" style="z-index: 101;
    width: 190px; position: relative; top: 0px; height: 22px">
    <tr>
        <td style="width: 250px">
            <asp:Button ID="cmpEquipment" runat="server" CausesValidation="False" PostBackUrl="~/Configuration/Equipment/frmEquipment.aspx"
                CssClass="confbutton" Text="Equipment" Width="112px" meta:resourcekey="cmpEquipmentResource1" />
        </td>
        <td style="width: 250px">
            <asp:Button ID="cmpMedia" runat="server" CausesValidation="False" PostBackUrl="~/Configuration/Equipment/frmMedia.aspx"
                CssClass="confbutton" Text="Media" Width="112px" meta:resourcekey="cmpMediaResource1" />
        </td>

    </tr>
</table>
